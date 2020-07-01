using LuduStack.Application.ViewModels;

namespace LuduStack.Web.Extensions.ViewModelExtensions
{
    public static class UserGeneratedBaseViewModelExtensions
    {
        public static void SetShareUrl(this UserGeneratedBaseViewModel vm, string url)
        {
            vm.ShareUrl = url;
        }

        public static void SetShareText(this UserGeneratedBaseViewModel vm, string text)
        {
            vm.ShareText = text;
        }
    }
}