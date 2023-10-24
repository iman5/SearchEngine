using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SearchEngine.API.Interfaces;
using SearchEngine.API.Models;

namespace SearchEngine.API.Controllers;

/// <summary>
/// Adding the API Controller
/// 
/// Token based routing is created by using both controller and action tokens
/// <code>
/// [Route("/api/v1/[controller]/[action]")]
/// </code>
/// 
/// API Versioning is added with the first major release of v1
/// 
/// </summary>
[ApiController]
[Route("/api/v1/[controller]/[action]")]
public class ContentController : ControllerBase
{
    /// <summary>
    /// luceneSearchEngineService is injected to the ContentController via DI
    /// logger has been used to log the information
    /// </summary>
    private readonly ILuceneSearchEngineService luceneSearchEngineService;
    private readonly ILogger<ContentController> logger;

    /// <summary>
    /// In the ctor, logger and luceneSearchEngineService has been initialized
    /// </summary>
    /// <param name="luceneSearchEngineService"></param>
    /// <param name="logger"></param>
    public ContentController(ILuceneSearchEngineService luceneSearchEngineService, ILogger<ContentController> logger)
    {
        this.luceneSearchEngineService = luceneSearchEngineService;
        this.logger = logger;
    }

    /// <summary>
    /// This method will get the IEnumerable of ContentModels from the body as JSON and will post it to the IndexContent method of luceneSearchEngineService.
    /// If the indexing is done successfully, it will result in a Ok response otherwise it will be a Problem response.
    /// </summary>
    /// <param name="contentModels"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult IndexContent([FromBody] IEnumerable<ContentModel> contentModels)
    {
        try
        {
            //Return BadRequest if contents are null
            if (contentModels is null) { return BadRequest(); }

            //Indexes the content via the luceneSearchEngineService
            var result = luceneSearchEngineService.IndexContent(contentModels);

            //Responding to the request via appropriate ActionResult
            if (result)
            {
                ResponseModel responseModel = new ResponseModel { ResponseMessage = "Success" };
                return Ok(responseModel);
            }
            else {
                ResponseModel responseModel = new ResponseModel { ResponseMessage = "Failure" };
                return Problem(responseModel.ResponseMessage);
            }
        }
        catch (Exception ex)
        {
            //Log exceptions and respond accordingly to the request
            logger?.LogError(ex.ToString());
            ResponseModel responseModel = new ResponseModel { ResponseMessage = "Failure" };
            return Problem(responseModel.ResponseMessage);
        }
    }


    [HttpGet("{searchKey}/{createdDate}")]
    public IActionResult SearchContent(string searchKey, DateTime createdDate)
    {
        try
        {
            //Return BadRequest if searchKey or createdDate are not provided
            if (string.IsNullOrEmpty(searchKey) && DateTime.MinValue==createdDate ) { return BadRequest(); }

            //Search the content via the luceneSearchEngineService and return the results
            var results = luceneSearchEngineService.SearchContent(searchKey, createdDate);

            //Responding to the request via appropriate ActionResult with the fetched contents
            if (results.Any())
            {
                ResponseModel responseModel = new ResponseModel { ResponseMessage = "Success" };
                return Ok(results);
            }
            else
            {
                ResponseModel responseModel = new ResponseModel { ResponseMessage = "Not Found" };
                return NotFound(responseModel);
            }
        }
        catch (Exception ex)
        {
            //Log exceptions and respond accordingly to the request
            logger?.LogError(ex.ToString());
            ResponseModel responseModel = new ResponseModel { ResponseMessage = "Failure" };
            return Problem(responseModel.ResponseMessage);
        }
    }

}

