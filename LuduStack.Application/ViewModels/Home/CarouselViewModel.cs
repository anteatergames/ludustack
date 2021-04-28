using LuduStack.Application.ViewModels.FeaturedContent;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Home
{
    public class CarouselViewModel
    {
        public List<FeaturedContentViewModel> Items { get; set; }

        public CarouselViewModel()
        {
            Items = new List<FeaturedContentViewModel>();
        }
    }
}