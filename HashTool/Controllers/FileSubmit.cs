using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public ActionResult Put([FromBody] string sasToken)
        {
            if (string.IsNullOrWhiteSpace(sasToken))
            {
                _logger.LogError("Empty token received");
                return BadRequest("SAS Token required");
            }
            return Ok();
        }
    }
}
