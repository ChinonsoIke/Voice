using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Audio;
using Voice.Application.Interfaces;
using Voice.DTOs;

namespace Voice.Application.Implementations
{
    public class VoiceService : IVoiceService
    {
        readonly AudioClient _audioClient;
        readonly IChatClient _chatClient;

        public VoiceService(IConfiguration config)
        {
            _audioClient = new OpenAIClient(config["OpenAI:ApiKey"]).GetAudioClient(config["OpenAI:AudioModel"]);
            _chatClient = new OpenAIClient(config["OpenAI:ApiKey"]).GetChatClient(config["OpenAI:ChatModel"]).AsIChatClient();
        }

        public async Task<string> TranscribeAndTranslate(TranscribeRequest request)
        {
            using var stream = request.Audio.OpenReadStream();
            var res = await _audioClient.TranscribeAudioAsync(stream, request.Audio.FileName, options: new AudioTranscriptionOptions { Language = request.SourceLanguage });
            string text = res.Value.Text;

            var trans = await _chatClient.GetResponseAsync($"Translate this text to {request.TranslationLanguage}. Don't return any other text but the translation: {text}");
            return trans.Text;
        }

        public async IAsyncEnumerable<string> TranscribeAndTranslateStream(TranscribeRequest request)
        {
            using var stream = request.Audio.OpenReadStream();
            var res = await _audioClient.TranscribeAudioAsync(stream, request.Audio.FileName, options: new AudioTranscriptionOptions { Language = request.SourceLanguage });
            string text = res.Value.Text;

            await foreach(var update in _chatClient.GetStreamingResponseAsync(new ChatMessage(ChatRole.User, 
                $"Translate this text to {request.TranslationLanguage}. Don't return any other text but the translation: {text}")))
            {
                yield return update.Text;
            }
        }
    }
}
