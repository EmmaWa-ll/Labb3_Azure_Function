using Microsoft.Azure.Cosmos;
using MinApi.Models;

namespace MinApi.Services
{
    public class CosmoService
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public CosmoService(IConfiguration config)
        {
            var connectionString = config["CosmosDb:ConnectionString"];
            var databaseName = config["CosmosDb:Databasename"];
            var containerName = config["CosmosDb:ContainerName"];

            var client = new CosmosClient(connectionString);
            _container = client.GetContainer(databaseName, containerName);

        }


        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            customer.id = Guid.NewGuid().ToString();
            var response = await _container.CreateItemAsync(customer, new PartitionKey(customer.id));

            return response.Resource;
        }



        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var query = _container.GetItemQueryIterator<Customer>("SELECT * FROM c");

            var customers = new List<Customer>();
            while (query.HasMoreResults)
                customers.AddRange(await query.ReadNextAsync());

            return customers;
        }

        public async Task<Customer?> GetCustomerAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Customer>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }


        public async Task<List<Customer>> SearchAsync(string search)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE CONTAINS(LOWER(c.Name), LOWER(@search)) OR CONTAINS(LOWER(c.Seller.Name), LOWER(@search))"
            )
            .WithParameter("@search", search);

            var queryResult = _container.GetItemQueryIterator<Customer>(query);
            var customers = new List<Customer>();
            while (queryResult.HasMoreResults)
                customers.AddRange(await queryResult.ReadNextAsync());

            return customers;
        }


        public async Task<Customer?> UpdateCustomerAsync(string id, Customer updatedCustomer)
        {
            var existing = await GetCustomerAsync(id);

            if (existing == null)
                return null;

            updatedCustomer.id = id;

            var response = await _container.ReplaceItemAsync(updatedCustomer, id, new PartitionKey(id));
            return response.Resource;

        }


        public async Task<bool> DeleteCustomerAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex)
                      when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

    }
}
