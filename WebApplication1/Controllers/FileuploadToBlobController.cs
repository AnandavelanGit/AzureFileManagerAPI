using AzureBlob.API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI.Controllers
{
   
    [ApiController]
    public class FileuploadToBlobController : ControllerBase
    {

        IAzureBlobService _service;
        public FileuploadToBlobController(IAzureBlobService service)
        {
            _service = service;
        }

        [HttpPost]        
        [Route("/UploadFiles")]
        public async Task<IActionResult> UploadBlobs(List<IFormFile> files)
        {
            var response = await _service.UploadFiles(files);
            return Ok(response);
        }

        [HttpPost]
        [Route("/UploadAFile")]
        public async Task<IActionResult> UploadBlob(IFormFile file)
        {
            var fileList = new List<IFormFile>();
            fileList.Add(file);
            var response = await _service.UploadFiles(fileList);
            return Ok(response);
        }

        [HttpGet]
        [Route("/GetFiles")]
        public async Task<IActionResult> GetAllBlobs()
        {
            var response = await _service.GetUploadedBlobs();
            return Ok(response);
        }

        [HttpGet]
        [Route("/DownloadBlob/{blobname}")]
        public async Task<IActionResult> DownloadBlob(string blobname)
        {
            var blobbyte = await _service.DownloadFileFromBlob(blobname);
            return File(blobbyte, "application/octet-stream", blobname);
        }

    }
}
