namespace LuduStack.Application.ViewModels.ShortUrl
{
    public class ShortUrlViewModel : BaseViewModel
    {
        public string OriginalUrl { get; set; }

        public string Token { get; set; }

        public string NewUrl { get; set; }

        public int AccessCount { get; set; }
    }
}