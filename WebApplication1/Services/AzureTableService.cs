using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using FileManagerAPI.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI.Services
{
    public class AzureTableService : IAzureTableService
    {
        AZTableStorage _aztablestorage;

        public AzureTableService(IOptions<AZTableStorage> storageoptions)
        {
            this._aztablestorage = storageoptions.Value;
        }

        public async Task<List<AzureQuestion>> GetTableData()
        {
            List<AzureQuestion> azureQuestions = new List<AzureQuestion>();

            // New instance of the TableClient class
            TableServiceClient tableServiceClient = new TableServiceClient(
                 new Uri("https://aadhiraistorage.table.core.windows.net/"),
    new TableSharedKeyCredential("aadhiraistorage", "mmXihU7Zw8EXUPnZ6w+LfiIuoUzasdcgtT/GjIDUFBVPOwrwAkCuFwbTsgIj4Js/V/dt9+4dd5Qn+AStVMR4fQ==")
    );

            TableClient table = tableServiceClient.GetTableClient("SecretQuestion");

            //var products = table.GetEntityAsync<AzureQuestion>(rowKey: "1234", partitionKey: "Anand");

            // Pageable<TableEntity> questions =  table.Query<TableEntity>(filter: $"PartitionKey eq 'Anand'");
            AsyncPageable<TableEntity> questions = table.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'Anand'");

            await foreach (TableEntity question in questions)
            {

                azureQuestions.Add(new AzureQuestion()
                {
                    ETag = question.ETag,
                    PartitionKey = question.PartitionKey,
                    Question = question.GetString("Question")
                });
                //Console.WriteLine($"{qEntity.GetString("Product")}: {qEntity.GetDouble("Price")}");
            }


            return azureQuestions;

        }

        public async Task<bool> CheckAnswers(List<QA> QAs)
        {
            bool isAllAnswersCorrect = true;
            List<AzureQuestion> azureQuestions = new List<AzureQuestion>();
            TableServiceClient tableServiceClient = new TableServiceClient(
                 new Uri("https://aadhiraistorage.table.core.windows.net/"),
                 new TableSharedKeyCredential("aadhiraistorage", "mmXihU7Zw8EXUPnZ6w+LfiIuoUzasdcgtT/GjIDUFBVPOwrwAkCuFwbTsgIj4Js/V/dt9+4dd5Qn+AStVMR4fQ==")
                );

            TableClient table = tableServiceClient.GetTableClient("SecretQuestion");
            AsyncPageable<TableEntity> questions = table.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'Anand'");

            await foreach (TableEntity question in questions)
            {

                azureQuestions.Add(new AzureQuestion()
                {
                    ETag = question.ETag,
                    PartitionKey = question.PartitionKey,
                    Question = question.GetString("Question"),
                    Answer = question.GetString("Answer")
                });
            }

            QAs.ForEach((QA ques) => {
                   if(azureQuestions.Find((AzureQuestion azq) => azq.Question == ques.Question).Answer != ques.Answer)
                   {
                    isAllAnswersCorrect = false;
                   }
            }
            
            );

            return isAllAnswersCorrect;
        }


    }
}
