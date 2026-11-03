using OrdersGraphQLDemo.API.Data;
using OrdersGraphQLDemo.API.Queries;
using OrdersGraphQLDemo.API.Mutations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryStore>();

builder.Services
    .AddGraphQLServer() // Add the GraphQL server
    .AddQueryType<Query>() // Register the root query type
    .AddProjections() // Register projection support
    .AddMutationType<Mutation>(); // Register the root mutation type

var app = builder.Build();

app.MapGraphQL();

app.Run();
