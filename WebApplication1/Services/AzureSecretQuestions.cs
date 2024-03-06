using FileManagerAPI.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Data;
//using System.Data.SqlClient;
using FileManagerAPI.Utilities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AzureBlob.API.Service
{
    public class AzureSecretQuestions : IAzureSecretQuestions
    {

        private SQLConnections _sqlcon;
       public AzureSecretQuestions(IOptions<SQLConnections> sqlconnectionoptions)
        {
            this._sqlcon = sqlconnectionoptions.Value;
        }

        public AzureSecretQuestions()
        {
        }

        public List<AzureQuestion> GetQuestions()
        {
            SqlCommand cmd = new SqlCommand(SQLConstants.getQuestionsQuery, new SqlConnection(this._sqlcon.AzureConString));

            cmd.CommandType = CommandType.Text;
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
             da.Fill(ds);

            var questions = new List<AzureQuestion>();
            for(int i= 0; i< ds.Tables[0].Rows.Count;i++)
            {
                var row = ds.Tables[0].Rows[i];
                questions.Add(new AzureQuestion() { QuestionID = (int)row["QuestionID"], Question = row["question"].ToString() });

            }            
            return questions;
        }

        public async Task<bool> CheckAnswers(List<AzureQuestion> answers)
        {

            return true;

        }





    }
}
