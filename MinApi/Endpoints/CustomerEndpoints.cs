using MinApi.Models;
using MinApi.Services;

namespace MinApi.Endpoints;

public static class CustomerEndpoints
{
    public static void MapCustomerEndpoints(this WebApplication app)
    {
        app.MapPost("/customers", async (Customer customer, CosmoService db) =>
        {
            if (customer.Seller == null)
                return Results.BadRequest("Customer must have a seller.");

            var created = await db.CreateCustomerAsync(customer);
            return Results.Created($"/customers/{created.id}", created);
        })
            .WithSummary("Add a new customer");



        app.MapGet("/customers", async (CosmoService db) =>
        {
            var customers = await db.GetAllCustomersAsync();
            return Results.Ok(customers);
        })
            .WithSummary("Get all customers");




        app.MapGet("/customers/{id}", async (string id, CosmoService db) =>
        {
            var customer = await db.GetCustomerAsync(id);

            return customer is null
                  ? Results.NotFound()
                  : Results.Ok(customer);
        })
           .WithSummary("GetCustomerById");



        app.MapGet("/customers/search", async (string search, CosmoService db) =>
        {
            var customers = await db.SearchAsync(search);
            return Results.Ok(customers);
        })
            .WithSummary("Serach customer by customers name or sellers name");



        app.MapPut("/customers/{id}", async (string id, Customer customer, CosmoService db) =>
        {
            var updated = await db.UpdateCustomerAsync(id, customer);
            return updated is null ? Results.NotFound() : Results.Ok(updated);

        })
            .WithSummary("Update a customer");




        app.MapDelete("/customers/{id}", async (string id, CosmoService db) =>
        {
            var deleted = await db.DeleteCustomerAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
            .WithSummary("Delete a customer");






    }



}
