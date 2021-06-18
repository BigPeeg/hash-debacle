using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace HashTool.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSubmit : ControllerBase
    {
        private readonly ILogger<FileSubmit> _logger;

        public FileSubmit(ILogger<FileSubmit> logger)
        {
            _logger = logger;
        }

        [HttpPut]
        public async System.Threading.Tasks.Task<ActionResult> PutAsync([FromBody] string sasUriString)
        {
            if (string.IsNullOrWhiteSpace(sasUriString))
            {
                _logger.LogError("Empty token received");
                return BadRequest("SAS Token required");
            }
            try
            {
                var sasUri = new Uri(sasUriString);

                BlobClient blobClient = new BlobClient(sasUri, null);

                // Download blob contents to a stream and read the stream.
                BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();
                using (StreamReader reader = new StreamReader(blobDownloadInfo.Content, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

                _logger.LogInformation("Read operation succeeded for SAS {0}", sasUri);

                return Ok();
            }
            catch (UriFormatException)
            {
                _logger.LogError("Invalid SAS URI Provided.");
                return BadRequest("Invalid SAS URI Provided.");
            }
            catch (RequestFailedException e)
            {
                // Check for a 403 (Forbidden) error. If the SAS is invalid, 
                // Azure Storage returns this error.
                if (e.Status == 403)
                {
                    _logger.LogError("Read operation failed for SAS {0}", sasUriString);
                    _logger.LogError("Additional error information: " + e.Message);
                }
                else
                {
                    _logger.LogError(e.Message);
                }
                return BadRequest(e.Message);
            }
        }
    }
}
