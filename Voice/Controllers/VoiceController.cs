using Microsoft.AspNetCore.Mvc;
using Voice.Application.Interfaces;
using Voice.DTOs;

namespace Voice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoiceController : ControllerBase
    {
        private readonly IVoiceService _voiceService;

        public VoiceController(IVoiceService voiceService)
        {
            _voiceService = voiceService;
        }

        [HttpPost("translate")]
        public async Task<IActionResult> TranscribeAndTranslate([FromForm] TranscribeRequest request)
        {
            if (request == null || request.Audio == null || request.Audio.Length == 0)
                return BadRequest("No audio uploaded");

            return Ok(new {text = await _voiceService.TranscribeAndTranslate(request) });
        }

        [HttpPost("translate/stream")]
        public async IAsyncEnumerable<string> TranscribeAndTranslateStream([FromForm] TranscribeRequest request)
        {
            if (request == null || request.Audio == null || request.Audio.Length == 0)
                yield break;

            await foreach(var update in _voiceService.TranscribeAndTranslateStream(request))
            {
                yield return update;
            }
        }
    }
}
