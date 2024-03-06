using AzureBlob.API.Service;
using FileManagerAPI.Model;
using FileManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI.Controllers
{

    [ApiController]
    // [Authorize]
    public class FileuploadToBlobController : ControllerBase
    {

        IAzureBlobService _service;
        IAzureSecretQuestions _secretQuestionsService;
        IAzureTableService _azureTableService;

        
        public FileuploadToBlobController(IAzureBlobService service, IAzureSecretQuestions secretQuestionsService, IAzureTableService azureTableService)
        {
            _service = service;
            _secretQuestionsService = secretQuestionsService;
            _azureTableService = azureTableService;

        }

        [HttpPost]
        [Route("/UploadFiles")]

        // [Authorize(Roles = "FileManager.ReadWrite")]
        public async Task<IActionResult> UploadBlobs(List<IFormFile> files)
        {
            var response = await _service.UploadFiles(files);
            return Ok(response);
        }

        [HttpPost]
        [Route("/UploadAFile")]
        //[Authorize(Roles = "FileManager.ReadWrite")]
        public async Task<IActionResult> UploadBlob(IFormFile file, [FromForm] string container)
        {
            _service.Container = container;
            var fileList = new List<IFormFile>();
            fileList.Add(file);
            var response = await _service.UploadFiles(fileList);
            return Ok(response);
        }

        [HttpGet]
        [Route("/GetFiles")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        // [Authorize(Roles = "FileManager.Read,FileManager.ReadWrite")]
        public async Task<IActionResult> GetAllBlobs()
        {
            var response = await _service.GetUploadedBlobs();
            return Ok(response);
        }

        [HttpGet]
        [Route("/GetFiles/{container}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        //[Authorize(Roles = "FileManager.Read,FileManager.ReadWrite")]
        public async Task<IActionResult> GetAllBlobs(string container)
        {
            _service.Container = container;
            var response = await _service.GetUploadedBlobs();
            return Ok(response);
        }

        [HttpGet]
        [Route("/DownloadBlob/{blobname}")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        //[Authorize(Roles = "FileManager.ReadWrite")]
        public async Task<IActionResult> DownloadBlob(string blobname)
        {
            var blobbyte = await _service.DownloadFileFromBlob(blobname);
            return File(blobbyte, "application/octet-stream", blobname);
        }

        [HttpGet]
        [Route("/GetContainers")]
        [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        // [Authorize(Roles = "FileManager.Read,FileManager.ReadWrite")]
        public async Task<IActionResult> GetAllContainers()
        {
            var response = await _service.ListContainers();
            return Ok(response);
        }

        [HttpGet]
        [Route("/GetSecretQuestions")]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        // [Authorize(Roles = "FileManager.Read,FileManager.ReadWrite")]
        public IActionResult GetAllQuestion()
        {
            var response = _secretQuestionsService.GetQuestions();
            return Ok(response);
        }

        [HttpGet]
        [Route("/GetSecretQuestions2")]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        // [Authorize(Roles = "FileManager.Read,FileManager.ReadWrite")]
        public async Task<IActionResult> GetAllQuestion2()
        {
            var response = await _azureTableService.GetTableData();
            return Ok(response);
        }

        [HttpPost]
        [Route("/CheckAnswers")]
        //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
        // [Authorize(Roles = "FileManager.Read,FileManager.ReadWrite")]
        public async Task<IActionResult> CheckAnswers([FromBody] List<QA> QAs)
        {
            var response = await _azureTableService.CheckAnswers(QAs);
            return Ok(response);
        }





    }
}
