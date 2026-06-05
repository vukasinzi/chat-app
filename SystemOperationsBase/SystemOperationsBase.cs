using BrokerBazePodataka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SO
{
    public abstract class SystemOperationsBase
    {
        public GenericBroker broker;
        public SystemOperationsBase()
        {
            broker = new GenericBroker();
        }
        public async Task ExecuteAsync(CancellationToken token = default)
        {
            try
            {
                await ExecuteConcreteOperationAsync(token);
                await broker.CommitAsync(token);
            }
            catch
            {
                await broker.RollbackAsync();
                throw;
            }
            finally
            {
                await broker.CloseAsync();

            }
        }
        protected abstract Task ExecuteConcreteOperationAsync(CancellationToken token = default);
    }
}
