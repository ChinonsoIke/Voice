using Voice.DTOs;

namespace Voice.Application.Interfaces
{
    public interface IVoiceService
    {
        Task<string> TranscribeAndTranslate(TranscribeRequest request);
        IAsyncEnumerable<string> TranscribeAndTranslateStream(TranscribeRequest request);
    }
}
