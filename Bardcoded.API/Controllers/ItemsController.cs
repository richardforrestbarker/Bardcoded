﻿using Bardcoded.API.Data;
using Bardcoded.API.Data.Requests;
using Bardcoded.Data.Responses;
using Bardcoded.Data.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Bardcoded.API.Controllers
{
    [Route("/item")]
    [ApiController]
    [Produces("application/json")]
    public class ItemsController : ControllerBase
    {
        public IBarcodeDataContext Context { get; }
        public BarcodeFetcher Fetcher { get; }
        internal IOMapper Mapper { get; }

        public ItemsController(IBarcodeDataContext dataContext, BarcodeFetcher fetcher)
        {
            Context = dataContext;
            Fetcher = fetcher;
            Mapper = new IOMapper();
        }

        /// <summary>
        /// Gets all known barcodes.
        /// </summary>
        /// <returns>A List of known barcodes and their product images</returns>
        /// <remarks>
        /// Example response: 
        /// []
        /// </remarks>
        /// <response code="200">The list.</response>
        [HttpGet("/item/all")]
        [ProducesResponseType(typeof(BarcodeView), 200)]
        public async Task<IResult> GetAllItems()
        {
            var results = await Context.GetAll();
            return Results.Ok(results.Select(Mapper.Map).ToList());
        }

        /// <summary>
        /// Gets a single item by its barcode.
        /// </summary>
        /// <param name="bard">The code of the item to get.</param>
        /// <returns>The item and an image.</returns>
        /// <response code="200">The item.</response>
        /// <response code="400">If the bard is null or empty string.</response>
        /// <response code="404">If the bard is not found.</response>
        [HttpGet()]
        [ProducesResponseType(typeof(BarcodeView), 200)]
        [ProducesResponseType(typeof(BarcodeView), 400)]
        [ProducesResponseType(typeof(BarcodeView), 404)]
        public async Task<IResult> Get([FromQuery]string bard)
        {
            if (bard == null || bard.Equals(string.Empty))
            {
                return Results.BadRequest(new ProblemDetails() { Detail = "Please provide a bard.", Status = (int)HttpStatusCode.BadRequest, Title = "No Bard Given." });
            }
            var result = await Fetcher.FindItem(bard);
            if (result == null)
            {
                return Results.NotFound(new ProblemDetails() { Detail = "That bard wasn't found.", Status = (int)HttpStatusCode.NotFound, Title = "Unknown Bard." });
            }
            return Results.Ok(result);
        }

        /// <summary>
        /// Creates an item by it's barcode. If that item is known already then returns a 409. If the barcode fails validation, returns a 400.
        /// </summary>
        /// <param name="bard">The code of the item to get.</param>
        /// <returns>The item and an image.</returns>
        /// <response code="201">The item.</response>
        /// <response code="400">If the bard is null or empty string.</response>
        /// <response code="409">If the bard exists.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BarcodeView), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        public async Task<IResult> Post([FromBody] BardcodeInjestRequest request)
        {
            try
            {
                var mapper = new IOMapper();
                var create = mapper.Map(request);
                var id = await Context.InsertBarcode(create);
                Console.WriteLine($"barcode {id}:{create.Bard} was created");
                return Results.Created($"/item?bard={create.Bard}", mapper.Map(create));
            }
            catch (InvalidOperationException inval)
            {
                return Results.Conflict(new ProblemDetails() { Detail = $"Multiple entries exist in the database for that bard. This is an application error state.", Title = "Uh Oh." });
            }
        }

        /// <summary>
        /// Updates an item by it's barcode. If that item is not found then returns a 404. If the barcode fails validation, returns a 400.
        /// </summary>
        /// <param name="bard">The code of the item to get.</param>
        /// <returns>The item and an image.</returns>
        /// <response code="200">The item.</response>
        /// <response code="400">If the bard is null or empty string.</response>
        /// <response code="404">If the bard doesn't exist.</response>
        [HttpPut()]
        [ProducesResponseType(typeof(BarcodeView), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IResult> Put([FromBody] BardcodeUpdateRequest request)
        {
            try
            {
                var code = await Context.GetBarcode(request.Bard);
                if (code == null)
                {
                    return Results.NotFound(new ProblemDetails() { Detail = $"That bard doesn't exist in the database. Use the post verb to create it.", Title = "Barcode Doesn't Exist." });
                }
                var mapper = new IOMapper();
                return Results.Ok(await Context.UpdateBarcode(mapper.Map(request)));
            }
            catch (InvalidOperationException inval)
            {
                return Results.Conflict(new ProblemDetails() { Detail = $"Multiple entries exist in the database for that bard. This is an application error state.", Title = "Uh Oh." });
            }
        }

        /// <summary>
        /// Does nothing. Will delete later
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType( 404)]
        public Task<IResult> Delete(int id)
        {
            return Task.FromResult(Results.NotFound());
        }
    }
}
