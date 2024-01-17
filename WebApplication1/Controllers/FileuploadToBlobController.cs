using AzureBlob.API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileuploadToBlobController : ControllerBase
    {

        IAzureBlobService _service;
        public FileuploadToBlobController(IAzureBlobService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlobs(List<IFormFile> files)
        {
            var response = await _service.UploadFiles(files);
            return Ok(response);
        }

        [HttpGet]
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
