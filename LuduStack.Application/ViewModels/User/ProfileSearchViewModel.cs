namespace LuduStack.Application.ViewModels.User
{
    public class ProfileSearchViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string ProfileImageUrl { get; set; }

        public string CoverImageUrl { get; set; }

        public bool HasCoverImage { get; set; }
    }
}