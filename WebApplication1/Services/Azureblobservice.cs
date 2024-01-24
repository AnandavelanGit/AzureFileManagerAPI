using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileManagerAPI;
using Microsoft.Extensions.Configuration;
using FileManagerAPI.Model;
using Microsoft.Extensions.Options;

namespace AzureBlob.API.Service
{
    public class AzureBlobService : IAzureBlobService
    {
        BlobClient _blobClient;
        BlobServiceClient _blobServiceClient;
        BlobContainerClient _containerClient;
        IConfiguration configuration;
        AzureStorage _azstore;
        //string azureConnectionString; //= "DefaultEndpointsProtocol=https;AccountName=aadhiraistorage;AccountKey=mmXihU7Zw8EXUPnZ6w+LfiIuoUzasdcgtT/GjIDUFBVPOwrwAkCuFwbTsgIj4Js/V/dt9+4dd5Qn+AStVMR4fQ==;EndpointSuffix=core.windows.net";
        //string azurecontainer; // = "aadhidocuments";
        
        public AzureBlobService(IOptions<AzureStorage> options)
        {
            this._azstore = options.Value;
            this.getAzureConnections();
        }

        private void getAzureConnections()
        {
           // configuration.Bind("AzureStorage", this._azstore);
            _blobServiceClient = new BlobServiceClient($"DefaultEndpointsProtocol={this._azstore.DefaultEndpointsProtocol};" +
                $"AccountName={this._azstore.AccountName};AccountKey={this._azstore.AccountKey};EndpointSuffix={this._azstore.EndpointSuffix};");
            _containerClient = _blobServiceClient.GetBlobContainerClient(this._azstore.Container);

        }

        public async Task<List<Azure.Response<BlobContentInfo>>> UploadFiles(List<IFormFile> files)
        {

            var azureResponse = new List<Azure.Response<BlobContentInfo>>();
            foreach (var file in files)
            {
                string fileName = file.FileName;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    var client = await _containerClient.UploadBlobAsync(fileName, memoryStream, default);
                    azureResponse.Add(client);
                }
            };

            return azureResponse;
        }

        public async Task<List<FileProperties>> GetUploadedBlobs()
        {
            //var items = new List<BlobItem>();
            var filepropslist = new List<FileProperties>();
            var uploadedFiles = _containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None);
            await foreach (BlobItem file in uploadedFiles)
            {

                filepropslist.Add(
                    new FileProperties() { Name = file.Name, UploadedDate = file.Properties.CreatedOn, UploadedBy = file.Properties.CopyId, Size = file.Properties.ContentLength / 1000 }
                    );
            }

            return filepropslist;
        }

        public async Task<byte[]> DownloadFileFromBlob(string blobname)
        {
            //var items = new List<BlobItem>();
            //this._blobClient = new BlobClient()
            this._blobClient = this._containerClient.GetBlobClient(blobname);
            BlobDownloadResult downloadResult = await this._blobClient.DownloadContentAsync();

            //return file as binary
            return downloadResult.Content.ToArray();
          
        }


    }
}
