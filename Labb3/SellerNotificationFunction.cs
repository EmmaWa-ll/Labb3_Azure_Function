using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MinApi.Models;
using System.Net;
using System.Net.Mail;

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
        Connection = "CosmosDBConnection",
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

            _logger.LogInformation("FUNCTION TRIGGERED");
            await SendEmailAsync(customer);
            _logger.LogInformation($"Email sent to {customer.Seller.Email}");
        }
    }

    private async Task SendEmailAsync(Customer customer)
    {
        var host = Environment.GetEnvironmentVariable("Mailtrap:Host");
        var port = int.Parse(Environment.GetEnvironmentVariable("Mailtrap:Port")!);
        var user = Environment.GetEnvironmentVariable("Mailtrap:User");
        var pass = Environment.GetEnvironmentVariable("Mailtrap:Pass");

        var subject = $"new customer: {customer.Name}";

        var body =
            $"Hey: {customer.Seller.Name}!\n\n" +
            $"You are the responsible seller for the customer: \n\n" +
            $"Title: {customer.Title}\n" +
            $"Name: {customer.Name}\n" +
            $"Phone: {customer.Phone}\n" +
            $"Email: {customer.Email}\n" +
            $"Address: {customer.Address}\n\n" +
            $"Best regards \n THE SYSTEM";


        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };
        var mail = new MailMessage("crm@system.com", customer.Seller.Email, subject, body);

        await client.SendMailAsync(mail);

    }
}

