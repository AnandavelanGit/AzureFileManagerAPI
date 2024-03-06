using FileManagerAPI.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileManagerAPI.Services
{
    public interface IAzureTableService
    {
        //Task<Azure.Response<AzureQuestion>> GetTableData();

        public Task<List<AzureQuestion>> GetTableData();

        public Task<bool> CheckAnswers(List<QA> QAs);

    }
}