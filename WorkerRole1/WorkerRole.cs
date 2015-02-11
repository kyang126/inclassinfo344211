using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private int sum = 0;
        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {

                while (true)
                {

                    CloudStorageAccount storageAccount =

CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                    CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                    CloudQueue queue = queueClient.GetQueueReference("mysum");
                    queue.CreateIfNotExists();



                    // Retrieve the first visible message in the queue.
                    CloudQueueMessage retrievedMessage = queue.GetMessage();



                    if (retrievedMessage != null)
                    {
                        String s = retrievedMessage.AsString;
                        string[] values = s.Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            sum += Int32.Parse(values[i]);
                        }
                        // Process the message in less than 30 seconds, and then delete the message.
                        queue.DeleteMessage(retrievedMessage);
                    }
                    Thread.Sleep(10000);

                    if (retrievedMessage == null)
                    {
                        CloudStorageAccount storageAccount1 =
                        CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                        CloudTableClient tableClient = storageAccount1.CreateCloudTableClient();
                        CloudTable table = tableClient.GetTableReference("sumQueue");
                        table.CreateIfNotExists();

                        sumQueue sq = new sumQueue("first",sum);


                        TableOperation insertOperation =
                        TableOperation.Insert(sq);
                        table.Execute(insertOperation);
                    }

                }


                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
