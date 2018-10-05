using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Invoices;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.ApiClients.Test
{
    public class PowerofficeClientTester : BaseTester
    {
        public PowerofficeClientTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            Client = Task.Run(() => PowerofficeClient.Create(
                TestTypedEnvironment.PowerofficeApiSettings,
                TestConfigurations.PowerofficeConfiguration.PowerofficeClientKey)).Result;
        }

        private PowerofficeClient Client { get; }
        private const long CompanyIdForTestingCrudOperationsOnContactPersons = 3495003;

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCreateUpdateDeleteInvoice()
        {
            var newInvoice = new NewOutgoingInvoice
            {
                CustomerCode = 1522391,
                Status = OutgoingInvoiceStatus.Approved
            };
            newInvoice.OutgoingInvoiceLines.Add(new OutgoingInvoiceLine
            {
                Description = "Created by automated test"
            });

            var createdInvoice = await Client.CreateInvoice(newInvoice);
            createdInvoice.Status.Should().Be(newInvoice.Status);
            createdInvoice.OutgoingInvoiceLines.Should().HaveCount(1);
            createdInvoice.OutgoingInvoiceLines[0].Description.Should().Be(newInvoice.OutgoingInvoiceLines[0].Description);

            createdInvoice.Status = OutgoingInvoiceStatus.Draft;
            var updatedInvoice = await Client.UpdateInvoice(createdInvoice);
            updatedInvoice.Id.Should().Be(createdInvoice.Id);
            updatedInvoice.Status.Should().Be(createdInvoice.Status);

            await Client.DeleteInvoice(createdInvoice.Id);
            await Assert.ThrowsAsync<ApplicationException>(async () => await Client.GetInvoice(createdInvoice.Id));
        }

        [Theory]
        [Trait(Traits.Execution, Traits.Manual)]
        [InlineData(CompanyIdForTestingCrudOperationsOnContactPersons)]
        public async Task TestCreateReadUpdateDeleteContactPerson(long customerId)
        {
            var newContactPerson = new NewContactPerson
            {
                FirstName = "Created by an automated test",
                LastName = "(Should have been deleted)"
            };

            var createdContactPerson = await Client.CreateContactPerson(customerId, newContactPerson);
            createdContactPerson.FirstName.Should().Be(newContactPerson.FirstName);
            createdContactPerson.LastName.Should().Be(newContactPerson.LastName);

            var readContactPerson = await Client.GetContactPerson(customerId, createdContactPerson.Id);
            readContactPerson.Id.Should().Be(createdContactPerson.Id);
            readContactPerson.FirstName.Should().Be(createdContactPerson.FirstName);
            readContactPerson.LastName.Should().Be(createdContactPerson.LastName);

            createdContactPerson.FirstName = "Created and updated by an automated test";
            var updatedContactPerson = await Client.UpdateContactPerson(customerId, createdContactPerson);
            updatedContactPerson.Id.Should().Be(createdContactPerson.Id);
            updatedContactPerson.FirstName.Should().Be(createdContactPerson.FirstName);
            updatedContactPerson.LastName.Should().Be(createdContactPerson.LastName);

            await Client.DeleteContactPerson(customerId, updatedContactPerson.Id);
            var deletedContactPerson = await Client.GetContactPerson(customerId, updatedContactPerson.Id);
            deletedContactPerson.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCreateReadUpdateDeleteCustomer()
        {
            var newCustomer = new NewCustomer
            {
                Name = "Created by an automated test (should also have been deleted)"
            };

            var createdCustomer = await Client.CreateCustomer(newCustomer);
            createdCustomer.Name.Should().Be(newCustomer.Name);

            var readCustomer = await Client.GetCustomer(createdCustomer.Id);
            readCustomer.Name.Should().Be(createdCustomer.Name);

            createdCustomer.Name = "Created and updated by an automated test (should also have been deleted)";
            var updatedCustomer = await Client.UpdateCustomer(createdCustomer);
            updatedCustomer.Name.Should().Be(createdCustomer.Name);

            await Client.DeleteCustomer(updatedCustomer.Id);
            var deletedCustomer = await Client.GetCustomer(updatedCustomer.Id);
            deletedCustomer.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetProductByCode()
        {
            const string productCode = "8";
            var product = await Client.GetProductByCode(productCode);

            product.Should().NotBeNull();
            product.Id.Should().Be(2059019);
            product.Name.Should().Be("Cola");
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedInvoices()
        {
            var longAgo = DateTime.UtcNow.Subtract(new TimeSpan(1000, 0, 0, 0));
            var upsertedInvoices = await Client.GetUpsertedInvoices(longAgo);

            upsertedInvoices.Should().HaveCountGreaterOrEqualTo(2);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedOrganisations()
        {
            var longAgo = DateTime.UtcNow.Subtract(new TimeSpan(1000, 0, 0, 0));
            var customers = await Client.GetUpsertedOrganisations(longAgo);

            customers.Should().HaveCountGreaterOrEqualTo(2);

            foreach (var customer in customers)
            {
                customer.Id.Should().BeGreaterOrEqualTo(1);
                customer.Name.Should().NotBeNullOrWhiteSpace();
                customer.Name.Length.Should().BeGreaterOrEqualTo(1);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedProducts()
        {
            var longAgo = DateTime.UtcNow.Subtract(new TimeSpan(1000, 0, 0, 0));
            var products = await Client.GetUpsertedProducts(longAgo);

            products.Should().HaveCountGreaterOrEqualTo(2);

            foreach (var product in products)
            {
                product.Id.Should().BeGreaterOrEqualTo(1);
                product.Name.Should().NotBeNullOrWhiteSpace();
                product.Name.Length.Should().BeGreaterOrEqualTo(1);
                product.Code.Should().NotBeNullOrWhiteSpace();
                product.Code.Length.Should().BeGreaterOrEqualTo(1);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetVatCodeBySalesAccount()
        {
            long salesAccount = 3000;
            var vatCodes = await Client.GetVatCodesBySalesAccount(salesAccount);

            Assert.NotNull(vatCodes);
            Assert.True(vatCodes.Count >= 1);
            var firstCode = vatCodes.First();
            Assert.Equal(25, firstCode.Rate);
        }
    }
}