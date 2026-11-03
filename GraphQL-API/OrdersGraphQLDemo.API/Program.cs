using HotChocolate;
using OrdersGraphQLDemo.API.Queries;
using OrdersGraphQLDemo.API.Schema.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer() // Add the GraphQL server
     .AddQueryType<Query>() // Register the Query class as the root query type
       .AddProjections(); // Add projection support for the Order entity

var app = builder.Build();

app.MapGraphQL();

app.Run();
