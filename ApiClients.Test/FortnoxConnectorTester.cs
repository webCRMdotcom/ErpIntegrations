using FluentAssertions;
using System;
using System.Collections.Generic;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.ApiClients.Test
{
    public class FortnoxConnectorTester : BaseTester
    {
        public FortnoxConnectorTester(ITestOutputHelper output) : base(output)
        {
            Client = new FortnoxClient(TestConfigurations.FortnoxApiKeys);
        }

        private FortnoxClient Client { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async void TestCreateCustomer()
        {
            var customer = new Customer
            {
                Name = $"API Test {DateTime.Now.Ticks}",
                Email = $"email@{DateTime.Now.Ticks}.com"
            };

            var fortnoxCustomer = await Client.CreateCustomer(customer);
            fortnoxCustomer.CustomerNumber.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async void TestCreateInvoice()
        {
            var invoice = new NewFortnoxInvoice
            {
                CustomerNumber = "9",
                DeliveryDate = DateTime.Today.AddDays(30).ToString("yyyy-MM-dd"),
                OurReference = "12345",
                InvoiceRows = new List<InvoiceRow>
                {
                    new InvoiceRow
                    {
                        ArticleNumber = "1",
                        DeliveredQuantity = "2",
                        Description = "A nice product",
                        Discount = "0.50",
                        Price = "2.40",
                        VAT = "25"
                    }
                }
            };

            var fortnoxInvoice = await Client.CreateInvoice(invoice);
            fortnoxInvoice.DocumentNumber.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async void TestGetCustomer()
        {
            var customer = await Client.GetCustomer("1");
            customer.Name.Should().Be("Kund 1");
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async void TestGetActiveCustomers()
        {
            var customers = await Client.GetActiveCustomers();
            customers.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async void TestGetRecentlyUpsertedCustomers()
        {
            var dateSinceUpserted = new DateTime(2018, 1, 1);
            var customers = await Client.GetRecentlyUpsertedCustomers(dateSinceUpserted);
            customers.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async void TestGetRecentlyUpsertedInvoices()
        {
            var dateSinceUpserted = new DateTime(2018, 1, 1);
            var invoices = await Client.GetRecentlyUpsertedInvoices(dateSinceUpserted);
            invoices.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async void TestUpdateCustomer()
        {
            var customer = await Client.GetCustomer("1");
            customer.Phone1 = DateTime.Now.Ticks.ToString().Substring(0, 6);

            await Client.UpdateCustomer(customer);
        }
    }
}