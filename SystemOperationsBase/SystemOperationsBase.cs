using BrokerBazePodataka;
using System;
using System.Collections.Generic;
using System.Text;

namespace SO
{
    public abstract class SystemOperationsBase
    {
        public GenericBroker broker;
        public SystemOperationsBase()
        {
            broker = new GenericBroker();
        }
        public void Execute()
        {
            try
            {
                ExecuteConcreteOperation();
                broker.Commit();
            }
            catch(Exception x)
            {
                broker.Rollback();
                Console.WriteLine(x.Message);
            }
            finally
            {
                broker.Close();

            }
        }
        protected abstract void ExecuteConcreteOperation();
    }
}
