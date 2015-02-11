using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace WebRole1
{
    /// <summary>
    /// Summary description for Admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Admin : System.Web.Services.WebService
    {

        [WebMethod]
        public void WorkerRoleCalculateSum(int a, int b, int c)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudQueueClient queueClient =
storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("mysum");
            queue.CreateIfNotExists();

            CloudQueueMessage message = new CloudQueueMessage("" + a + "," + b + "," + c);
            queue.AddMessage(message);

            CloudQueueMessage message4 =
            queue.GetMessage(TimeSpan.FromMinutes(1));
            queue.DeleteMessage(message4);

        }


        [WebMethod]
        public int ReadSumFromTableStorage()
        {
            int value = -1;
            CloudStorageAccount storageAccount =
            CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
            CloudTableClient tableClient =
storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("sumQueue");
            table.CreateIfNotExists();

            TableQuery<sumQueue> rangeQuery = new TableQuery<sumQueue>().Where(TableQuery.CombineFilters(
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThan,
"a"),
TableOperators.And,
TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan,
"zjkxljkl")));

            foreach (sumQueue entity in table.ExecuteQuery(rangeQuery))
            {
                if (entity.sum != 0)
                {
                    value = entity.sum;
                }
            }


            return value;
        }

    }
}
