using LuduStack.Application.ViewModels.FeaturedContent;
using LuduStack.Application.ViewModels.Home;
using System.Collections.Generic;

namespace LuduStack.Application
{
    public static class FakeData
    {
        public static CarouselViewModel FakeCarousel()
        {
            CarouselViewModel carousel = new CarouselViewModel
            {
                Items = new List<FeaturedContentViewModel>()
            };

            FeaturedContentViewModel item1 = new FeaturedContentViewModel
            {
                Url = "#",
                FeaturedImage = "/images/featured/fez.jpg",
                Title = "Fez",
                Introduction = "Fez made history, admit it."
            };
            carousel.Items.Add(item1);

            FeaturedContentViewModel item2 = new FeaturedContentViewModel
            {
                Url = "#",
                FeaturedImage = "/images/featured/indies.jpg",
                Title = "Indie love",
                Introduction = "Why people love to play independent games?"
            };
            carousel.Items.Add(item2);

            FeaturedContentViewModel item3 = new FeaturedContentViewModel
            {
                Url = "#",
                FeaturedImage = "/images/featured/indiedev.jpg",
                Title = "Wildlife channel presents",
                Introduction = "Independent developers. Where they live? What they eat."
            };
            carousel.Items.Add(item3);

            FeaturedContentViewModel item4 = new FeaturedContentViewModel
            {
                Url = "#",
                FeaturedImage = "/images/featured/bioshock-collection.jpg",
                Title = "BioIndie",
                Introduction = "Bioshock is becoming indie"
            };
            carousel.Items.Add(item4);

            return carousel;
        }
    }
}