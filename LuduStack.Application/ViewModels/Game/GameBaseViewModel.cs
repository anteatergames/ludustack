using LuduStack.Domain.Interfaces.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Game
{
    public class GameBaseViewModel : BaseViewModel, IGameCardBasic
    {
        [Required(ErrorMessage = "Which game are you seeking help for?")]
        [Display(Name = "Game")]
        public new Guid Id { get; set; }

        public string Title { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ThumbnailResponsive { get; set; }

        public string ThumbnailLquip { get; set; }

        public string DeveloperImageUrl { get; set; }

        public string DeveloperName { get; set; }

        public GameBaseViewModel()
        {
        }
    }
}