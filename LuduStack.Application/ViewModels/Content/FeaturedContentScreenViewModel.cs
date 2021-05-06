using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Content
{
    public class FeaturedContentScreenViewModel
    {
        public List<UserContentToBeFeaturedViewModel> Featured { get; set; }
        public List<UserContentToBeFeaturedViewModel> NotFeatured { get; set; }

        public FeaturedContentScreenViewModel()
        {
            Featured = new List<UserContentToBeFeaturedViewModel>();
            NotFeatured = new List<UserContentToBeFeaturedViewModel>();
        }
    }
}