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
using Azure.Security.KeyVault.Secrets;
using System;
using Azure.Identity;
using Azure.Core;
using Azure;
using System.Collections;
using System.Reflection.Metadata;

namespace AzureBlob.API.Service
{
    public class AzureBlobService : IAzureBlobService
    {
        BlobClient _blobClient;
        BlobServiceClient _blobServiceClient;
        BlobContainerClient _containerClient;
        //IConfiguration configuration;
        AzureStorage _azstore;
        AzureKeyVault _azkeyvault;
        private string _container;
        public string Container
        {
            get { return _azstore.Container; }

            set
            {
                _container = value;
                _azstore.Container = _container;
            }
        }
        //string azureConnectionString; //= "DefaultEndpointsProtocol=https;AccountName=aadhiraistorage;AccountKey=mmXihU7Zw8EXUPnZ6w+LfiIuoUzasdcgtT/GjIDUFBVPOwrwAkCuFwbTsgIj4Js/V/dt9+4dd5Qn+AStVMR4fQ==;EndpointSuffix=core.windows.net";
        //string azurecontainer; // = "aadhidocuments";

        public AzureBlobService(IOptions<AzureStorage> storageoptions, IOptions<AzureKeyVault> keyvaultoptions)
        {
            this._azstore = storageoptions.Value;
            this._azkeyvault = keyvaultoptions.Value;
            this._azstore.AccountKey = this.getStorageKeyFromKeyvault();
            this.getAzureConnections();
        }

        private void getAzureConnections()
        {


            _blobServiceClient = new BlobServiceClient($"DefaultEndpointsProtocol={this._azstore.DefaultEndpointsProtocol};" +
                $"AccountName={this._azstore.AccountName};AccountKey={this._azstore.AccountKey};EndpointSuffix={this._azstore.EndpointSuffix};");
            // _containerClient = _blobServiceClient.GetBlobContainerClient(this.Container);

        }

        private string getStorageKeyFromKeyvault()
        {

            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
            };
            var client = new SecretClient(new Uri(_azkeyvault.Url),
                 new DefaultAzureCredential(), options);

            // new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId= "4a65243b-ac45-4d96-a9fb-6fb09623eb65" }), options);

            KeyVaultSecret secret = client.GetSecret(_azkeyvault.KeyName);

            string secretValue = secret.Value;

            return secretValue;
        }

        public async Task<List<Azure.Response<BlobContentInfo>>> UploadFiles(List<IFormFile> files)
        {
            _containerClient = _blobServiceClient.GetBlobContainerClient(this.Container);

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


        public async Task<Response> DeleteBlobAsync(string filename)
        {
            Response resp;
            _containerClient = _blobServiceClient.GetBlobContainerClient(this.Container);
            resp = await _containerClient.DeleteBlobAsync(filename, DeleteSnapshotsOption.IncludeSnapshots);
            return resp;
        }

        public async Task<List<FileProperties>> GetUploadedBlobs()
        {
            //var items = new List<BlobItem>();
            _containerClient = _blobServiceClient.GetBlobContainerClient(this.Container);
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
            _containerClient = _blobServiceClient.GetBlobContainerClient(this.Container);
            this._blobClient = this._containerClient.GetBlobClient(blobname);
            BlobDownloadResult downloadResult = await this._blobClient.DownloadContentAsync();

            //return file as binary
            return downloadResult.Content.ToArray();

        }

        public async Task<List<string>> ListContainers()
        {
            try
            {

                List<string> containers = new List<string>();
                // Call the listing operation and enumerate the result segment.
                var resultSegment = this._blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata);

                //this._blobServiceClient.GetBlobContainers()



                //this._blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, prefix, default)
                //.AsPages(default, segmentSize);

                await foreach (BlobContainerItem containerItem in resultSegment)
                {
                    containers.Add(containerItem.Name);
                    //Console.WriteLine("Container name: {0}", containerItem.Name);
                    //Console.WriteLine();
                }

                return containers;
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }


    }
}
