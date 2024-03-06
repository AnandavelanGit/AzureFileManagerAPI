using FileManagerAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlob.API.Service
{
    public interface IAzureSecretQuestions
    {
        public List<AzureQuestion> GetQuestions();

        public  Task<bool> CheckAnswers(List<AzureQuestion> answers);
    }
}
