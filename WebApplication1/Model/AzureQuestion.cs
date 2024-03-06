using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI.Model
{
    public class AzureQuestion : ITableEntity
    {
        public string RowKey { get; set; }

        public string PartitionKey { get; set; }
        public int? QuestionID { get; set; }
        // public string Name;
        public string Question { get; set; }

        public string Answer { get; set; }

        public ETag ETag { get; set; } = default!;

        public DateTimeOffset? Timestamp { get; set; } = default!;
    }


    // public class AnswerList
    // {        
    //     public List<QA> QAs { get; set; }
    // }

    

        public struct QA
        {

            public string Question { get; set; }

            public string Answer { get; set; }
        }

}
