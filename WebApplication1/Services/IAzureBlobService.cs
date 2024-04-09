using Azure;
using Azure.Storage.Blobs.Models;
using FileManagerAPI;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureBlob.API.Service
{
    public interface IAzureBlobService
    {
        public string Container { get; set; }
        Task<List<FileProperties>> GetUploadedBlobs();
        Task<List<Response<BlobContentInfo>>> UploadFiles(List<IFormFile> files);

        Task<byte[]> DownloadFileFromBlob(string blobname);

         Task<List<string>> ListContainers();

        Task<Response> DeleteBlobAsync(string blobname);
    }

}