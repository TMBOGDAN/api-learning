"""Run with `python -m unittest discover -s tests -v` after dotnet build.

Uses only Python's standard library and starts an isolated API process with
fresh sample data on an available local port. It never calls your running API.
"""

import json
from datetime import datetime
import os
from pathlib import Path
import socket
import subprocess
import tempfile
import time
import unittest
from urllib.error import HTTPError, URLError
from urllib.request import Request, urlopen


class GraphQLCrudTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        root = Path(__file__).resolve().parents[1]
        api = root / "OrdersGraphQLDemo.API"
        assembly = api / "bin/Debug/net11.0/OrdersGraphQLDemo.API.dll"
        if not assembly.exists():
            raise RuntimeError("Run dotnet build OrdersGraphQLDemo.slnx first.")

        with socket.socket() as listener:
            listener.bind(("127.0.0.1", 0))
            port = listener.getsockname()[1]
        cls.endpoint = f"http://127.0.0.1:{port}/graphql"
        cls.log = tempfile.TemporaryFile(mode="w+b")
        cls.server = None
        cls.addClassCleanup(cls.stop_server)
        cls.server = subprocess.Popen(
            ["dotnet", str(assembly), "--urls", f"http://127.0.0.1:{port}"],
            cwd=api,
            stdout=cls.log,
            stderr=subprocess.STDOUT,
            creationflags=subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0,
        )
        deadline = time.monotonic() + 30
        while time.monotonic() < deadline and cls.server.poll() is None:
            try:
                if cls.graphql("{ hello }")["hello"] == "Hello World!":
                    return
            except (URLError, TimeoutError, AssertionError):
                time.sleep(0.1)
        cls.log.seek(0)
        raise RuntimeError("API did not start:\n" + cls.log.read().decode(errors="replace"))

    @classmethod
    def stop_server(cls):
        if cls.server is not None and cls.server.poll() is None:
            cls.server.terminate()
            try:
                cls.server.wait(timeout=5)
            except subprocess.TimeoutExpired:
                cls.server.kill()
                cls.server.wait(timeout=5)
        cls.log.close()

    @classmethod
    def graphql(cls, query, variables=None, expect_error=False):
        request = Request(
            cls.endpoint,
            data=json.dumps({"query": query, "variables": variables or {}}).encode(),
            headers={"Content-Type": "application/json"},
        )
        try:
            with urlopen(request, timeout=5) as response:
                result = json.load(response)
        except HTTPError as error:
            result = json.loads(error.read())
        if expect_error:
            if not result.get("errors"):
                raise AssertionError(f"Expected GraphQL errors, received {result}")
            return result["errors"]
        if result.get("errors"):
            raise AssertionError(result["errors"])
        return result["data"]

    def save(self, field, value, selection, expect_error=False):
        types = {"Product": "ProductDtoInput", "Order": "OrderDtoInput", "OrderLine": "OrderLineDtoInput"}
        input_type = next(type_name for suffix, type_name in types.items() if field.endswith(suffix))
        result = self.graphql(
            f"mutation($input: {input_type}!) {{ {field}(input: $input) {{ {selection} }} }}",
            {"input": value},
            expect_error=expect_error,
        )
        return result if expect_error else result[field]

    @staticmethod
    def order_input(order_id, **changes):
        return {
            "id": order_id,
            "customerId": "TEST-CUSTOMER",
            "name": "Test customer",
            "addresssLine1": "10 Test Street",
            "addresssLine2": "",
            "country": "Romania",
            "postalCode": "010001",
            "date": "2026-11-03T10:00:00Z",
            **changes,
        }

    def counts(self):
        data = self.graphql("{ products { skuId } orders { id } orderLines { lineId } }")
        return tuple(len(data[field]) for field in ("products", "orders", "orderLines"))

    def test_crud_and_relationship_cleanup(self):
        baseline = self.counts()
        product = {"skuId": "TEST-001", "name": "Test product", "price": 12.5, "description": "Original"}
        self.assertEqual(self.save("addProduct", product, "skuId name price description"), product)
        other_product = {**product, "skuId": "TEST-002"}
        self.save("addProduct", other_product, "skuId")

        # A separate HTTP request must see the new product.
        found = self.graphql('{ productBySkuId(skuId: "TEST-001") { skuId name price description } }')
        self.assertEqual(found["productBySkuId"], product)
        updated_product = {**product, "name": "Updated product", "price": 0, "description": ""}
        self.assertEqual(self.save("updateProduct", updated_product, "skuId name price description"), updated_product)
        self.assertEqual(
            self.graphql('{ productBySkuId(skuId: "TEST-001") { name price description } }')["productBySkuId"],
            {"name": "Updated product", "price": 0, "description": ""},
        )

        self.save("addOrder", self.order_input(1001), "id")
        self.save("addOrder", self.order_input(1002), "id")
        updated_order = self.order_input(1001, name="Updated customer", addresssLine1="20 New Street")
        fields = "id customerId name addresssLine1 addresssLine2 country postalCode date"
        result = self.save("updateOrder", updated_order, fields)
        for field in updated_order.keys() - {"date"}:
            self.assertEqual(result[field], updated_order[field])
        self.assertEqual(
            datetime.fromisoformat(result["date"].replace("Z", "+00:00")),
            datetime.fromisoformat(updated_order["date"].replace("Z", "+00:00")),
        )
        result = self.graphql('{ ordersByCustomerId(customerId: "TEST-CUSTOMER") { id } orderById(id: 1001) { name } }')
        self.assertEqual({o["id"] for o in result["ordersByCustomerId"]}, {1001, 1002})
        self.assertEqual(result["orderById"]["name"], "Updated customer")

        line = {"lineId": 2001, "orderId": 1001, "skuId": "TEST-001", "quantity": 2}
        self.assertEqual(self.save("addOrderLine", line, "lineId orderId skuId quantity"), line)
        result = self.graphql('''{
          orderLineById(lineId: 2001) { quantity order { id } product { skuId } }
          orderById(id: 1001) { orderLines { lineId } }
          productBySkuId(skuId: "TEST-001") { orderItems { lineId } }
        }''')
        self.assertEqual(result["orderLineById"], {"quantity": 2, "order": {"id": 1001}, "product": {"skuId": "TEST-001"}})
        self.assertEqual(result["orderById"]["orderLines"], [{"lineId": 2001}])
        self.assertEqual(result["productBySkuId"]["orderItems"], [{"lineId": 2001}])

        # Updating in place must not duplicate either navigation collection.
        self.save("updateOrderLine", {**line, "quantity": 3}, "lineId")
        result = self.graphql('{ orderById(id: 1001) { orderLines { quantity } } }')
        self.assertEqual(result["orderById"]["orderLines"], [{"quantity": 3}])

        moved = {**line, "orderId": 1002, "skuId": "TEST-002", "quantity": 4}
        self.assertEqual(self.save("updateOrderLine", moved, "lineId orderId skuId quantity"), moved)
        result = self.graphql('''{
          oldOrder: orderById(id: 1001) { orderLines { lineId } }
          oldProduct: productBySkuId(skuId: "TEST-001") { orderItems { lineId } }
          newOrder: orderById(id: 1002) { orderLines { lineId quantity } }
          newProduct: productBySkuId(skuId: "TEST-002") { orderItems { lineId } }
          orderLinesByOrderId(orderId: 1002) { lineId }
          orderLinesBySkuId(skuId: "TEST-002") { lineId }
        }''')
        self.assertEqual(result["oldOrder"]["orderLines"], [])
        self.assertEqual(result["oldProduct"]["orderItems"], [])
        self.assertEqual(result["newOrder"]["orderLines"], [{"lineId": 2001, "quantity": 4}])
        self.assertEqual(result["newProduct"]["orderItems"], [{"lineId": 2001}])
        self.assertEqual(result["orderLinesByOrderId"], [{"lineId": 2001}])
        self.assertEqual(result["orderLinesBySkuId"], [{"lineId": 2001}])

        self.graphql('mutation { deleteProduct(skuId: "TEST-002") }', expect_error=True)
        self.assertTrue(self.graphql('mutation { deleteProduct(skuId: "TEST-001") }')["deleteProduct"])
        self.assertTrue(self.graphql("mutation { deleteOrderLine(lineId: 2001) }")["deleteOrderLine"])
        result = self.graphql('''{
          orderLineById(lineId: 2001) { lineId }
          orderById(id: 1002) { orderLines { lineId } }
          productBySkuId(skuId: "TEST-002") { orderItems { lineId } }
        }''')
        self.assertIsNone(result["orderLineById"])
        self.assertEqual(result["orderById"]["orderLines"], [])
        self.assertEqual(result["productBySkuId"]["orderItems"], [])

        # Deleting an order must also remove its lines from the store and product.
        self.save("addOrderLine", {**moved, "lineId": 2002}, "lineId")
        self.assertTrue(self.graphql("mutation { deleteOrder(id: 1002) }")["deleteOrder"])
        result = self.graphql('''{
          orderById(id: 1002) { id }
          orderLineById(lineId: 2002) { lineId }
          orderLinesByOrderId(orderId: 1002) { lineId }
          productBySkuId(skuId: "TEST-002") { orderItems { lineId } }
        }''')
        self.assertIsNone(result["orderById"])
        self.assertIsNone(result["orderLineById"])
        self.assertEqual(result["orderLinesByOrderId"], [])
        self.assertEqual(result["productBySkuId"]["orderItems"], [])
        self.graphql('mutation { deleteOrder(id: 1001) deleteProduct(skuId: "TEST-002") }')
        self.assertEqual(self.counts(), baseline)

    def test_invalid_input_and_missing_resources(self):
        baseline = self.counts()
        product = {"skuId": "BOOK-001", "name": "Test", "price": 1, "description": ""}
        line = {"lineId": 1, "orderId": 1, "skuId": "BOOK-001", "quantity": 1}
        for field, value in (
            ("addProduct", product),
            ("addOrder", self.order_input(1)),
            ("addOrderLine", line),
            ("addProduct", {**product, "skuId": "INVALID", "price": -1}),
            ("updateProduct", {**product, "name": " "}),
            ("updateProduct", {**product, "description": "x" * 501}),
            ("addProduct", {**product, "skuId": "x" * 51}),
            ("addOrder", self.order_input(0)),
            ("updateOrder", self.order_input(1, country="")),
            ("updateOrder", self.order_input(1, postalCode="x" * 51)),
            ("addOrderLine", {**line, "lineId": 3001, "orderId": 99999}),
            ("addOrderLine", {**line, "lineId": 3001, "skuId": "MISSING"}),
            ("addOrderLine", {**line, "lineId": 0}),
            ("addOrderLine", {**line, "lineId": 3001, "orderId": 0}),
            ("updateOrderLine", {**line, "quantity": 0}),
            ("updateOrderLine", {**line, "quantity": -1}),
            ("updateOrderLine", {**line, "orderId": 2, "skuId": "MISSING"}),
            ("updateOrderLine", {**line, "orderId": 99999}),
        ):
            with self.subTest(field=field, value=value):
                self.save(field, value, "__typename", expect_error=True)

        # Failed writes must preserve both values and relationships.
        result = self.graphql('''{
          productBySkuId(skuId: "BOOK-001") { price }
          orderLineById(lineId: 1) { lineId orderId skuId quantity order { id } product { skuId } }
        }''')
        self.assertEqual(result["productBySkuId"]["price"], 29.99)
        self.assertEqual(result["orderLineById"], {**line, "order": {"id": 1}, "product": {"skuId": "BOOK-001"}})

        self.assertIsNone(self.save("updateProduct", {**product, "skuId": "MISSING"}, "skuId"))
        self.assertIsNone(self.save("updateOrder", self.order_input(99999), "id"))
        self.assertIsNone(self.save("updateOrderLine", {**line, "lineId": 99999}, "lineId"))
        missing = self.graphql('''{
          productBySkuId(skuId: "MISSING") { skuId }
          orderById(id: 99999) { id }
          orderLineById(lineId: 99999) { lineId }
          ordersByCustomerId(customerId: "MISSING") { id }
          orderLinesByOrderId(orderId: 99999) { lineId }
          orderLinesBySkuId(skuId: "MISSING") { lineId }
        }''')
        self.assertEqual(missing, {"productBySkuId": None, "orderById": None, "orderLineById": None,
                                   "ordersByCustomerId": [], "orderLinesByOrderId": [], "orderLinesBySkuId": []})
        deleted = self.graphql('''mutation {
          deleteProduct(skuId: "MISSING") deleteOrder(id: 99999) deleteOrderLine(lineId: 99999)
        }''')
        self.assertEqual(deleted, {"deleteProduct": False, "deleteOrder": False, "deleteOrderLine": False})
        self.assertEqual(self.counts(), baseline)


if __name__ == "__main__":
    unittest.main(verbosity=2)
