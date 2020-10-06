using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleApp.Services
{
    public class ServiceA : BackgroundService
    {
        public Dictionary<string, object> Opertation = new Dictionary<string, object>();
        public Dictionary<string, bool> OpertationDone = new Dictionary<string, bool>();
        public Dictionary<string, DateTime> OpertationExpire = new Dictionary<string, DateTime>();
        public Dictionary<string, Task> OpertationTask=new Dictionary<string, Task>();
        Timer cleanerTimer;
        public ServiceA(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<ServiceA>();
            cleanerTimer = new Timer(RemoveExpired, null, 200000, 200000);

        }

        private void RemoveExpired(object obj)
        {
            List<string> removeList = new List<string>();
            foreach (var item in OpertationExpire)
            {
                if (item.Value < DateTime.Now)
                    removeList.Add(item.Key);
                    
            }


            foreach (var item in removeList)
            {
                Opertation.Remove(item);
                OpertationExpire.Remove(item);
                OpertationDone.Remove(item);
                
                OpertationTask[item].Dispose();
                OpertationTask.Remove(item);
            }
        }
       
        public KeyValuePair<bool,object> Do(string id)
        {
            KeyValuePair<bool, object> retVal = new KeyValuePair<bool, object>(false,null);
           
            if(Opertation.ContainsKey(id))
            {
                if (OpertationDone[id])
                    retVal=new KeyValuePair<bool, object>(true,Opertation[id]);
            }
            else
            {
                Opertation.Add(id, null);
                OpertationDone.Add(id,false);
                OpertationExpire.Add(id, DateTime.Now.AddSeconds(500000));
                
            }

            //todo call operation

             OpertationTask[id] = Task.Factory.StartNew(() => {
                 simulateA(id);
            });


            return retVal;
        }

        private void simulateA(string id)
        {
            Thread.Sleep(10000);
            Opertation[id] = "return = "+ id;
            OpertationDone[id] = true;
        }
        public ILogger Logger { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation("ServiceA is starting.");

            stoppingToken.Register(() => Logger.LogInformation("ServiceA is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("ServiceA is doing background work.");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Logger.LogInformation("ServiceA has stopped.");
        }
    }
}
