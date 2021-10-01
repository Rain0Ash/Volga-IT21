using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volga_IT.Database;
using Volga_IT.Extractor;
using Volga_IT.Extractor.Interfaces;
using Volga_IT.Models;

namespace Volga_IT.Controllers
{
    [ApiController]
    [Route("analyze")]
    public class HtmlAnalyzeController : ControllerBase
    {
        private ApplicationHtmlContext Context { get; }
        private IWebHostEnvironment Environment { get; }
        private IWordCounterRecordSorter Sorter { get; }
        private DatabaseHtmlExtractHandler ExtractHandler { get; }

        private const Int64 MaximumFileSize = 1 << 20; // 1 MB

        public HtmlAnalyzeController(ApplicationHtmlContext context, IWebHostEnvironment environment, IWordCounterRecordSorter sorter)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Sorter = sorter ?? throw new ArgumentNullException(nameof(sorter));

            ExtractHandler = new DatabaseHtmlExtractHandler(context);

            Directory.CreateDirectory(Path.Join(Environment.WebRootPath, "Files"));
        }

        [HttpGet("{hash:long}")]
        public IActionResult Get(Int64 hash)
        {
            IEnumerable<WordCounterRecord>? result = ExtractHandler.Extract(hash);

            if (result is null)
            {
                return Ok(Array.Empty<WordCounterRecord>());
            }

            result = Sorter.Sort(result).ToArray();
            
            return Ok(result);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormFile? upload)
        {
            if (upload is null)
            {
                return NotFound();
            }
            
            if (Path.GetExtension(upload.FileName) != ".html")
            {
                return StatusCode(415);
            }

            if (upload.Length > MaximumFileSize)
            {
                return StatusCode(413);
            }

            String path = Path.Join("Files", upload.FileName);

            await using FileStream stream = new FileStream(Path.Join(Environment.WebRootPath, path), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
            await upload.CopyToAsync(stream);

            IEnumerable<WordCounterRecord> extract = ExtractHandler.ExtractAndSaveToDatabase(stream, new HtmlTextExtractor(stream), out Int64 hash);

            WordCounterRecord[] result = Sorter.Sort(extract).ToArray();
            
            return Ok(new { Hash = hash.ToString(), Records = result }); //JSON не поддерживает полноценные 64 битовые числа.
        }

        [HttpDelete]
        public IActionResult Delete(Int64 hash)
        {
            Boolean result = ExtractHandler.DeleteFromDatabase(hash);
            return Ok(result);
        }
    }
}