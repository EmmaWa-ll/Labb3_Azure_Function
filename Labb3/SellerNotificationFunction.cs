using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MinApi.Models;

namespace Labb3;

public class SellerNotificationFunction
{
    private readonly ILogger<SellerNotificationFunction> _logger;

    public SellerNotificationFunction(ILogger<SellerNotificationFunction> logger)
    {
        _logger = logger;
    }

    [Function("SellerNotificationFunction")]
    public async Task Run([CosmosDBTrigger(
        databaseName: "CosmoDBLabb3",
        containerName: "Customers",
        Connection = "ConnectionStrings:CosmosDBConnection",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)]
        IReadOnlyList<Customer> customers)
    {
        if (customers == null || customers.Count == 0)
            return;

        foreach (var customer in customers)
        {
            if (customer.Seller == null || string.IsNullOrWhiteSpace(customer.Seller.Email))
                continue;

            await SendEmailAsync(customer);

            _logger.LogInformation($"Email sent to {customer.Seller.Email}");
        }
    }

    private async Task SendEmailAsync(Customer customer)
    {

    }
}

