using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkerRole1
{
    public class sumQueue : TableEntity
    {
        public sumQueue(string value, int a)
        {
            this.PartitionKey = value;
            this.RowKey = Guid.NewGuid().ToString();
            this.sum = a;
        }

        public sumQueue() { }

        public int sum { get; set; }


    }
}