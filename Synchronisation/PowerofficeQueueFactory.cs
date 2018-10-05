using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Webcrm.ErpIntegrations.Synchronisation
{
    public class PowerofficeQueueFactory
    {
        public PowerofficeQueueFactory(
            ILogger logger,
            string storageAccountConnectionString)
        {
            Logger = logger;
            StorageAccountConnectionString = storageAccountConnectionString;
        }

        private ILogger Logger { get; }
        private string StorageAccountConnectionString { get; }

        public async Task<PowerofficeQueue> Create()
        {
            var powerofficeQueue = await PowerofficeQueue.Create(Logger, StorageAccountConnectionString);
            return powerofficeQueue;
        }
    }
}