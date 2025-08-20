namespace Voice.DTOs
{
    public class TranscribeRequest
    {
        public IFormFile Audio { get; set; }
        public string SourceLanguage { get; set; }
        public string TranslationLanguage { get; set; }
    }
}
