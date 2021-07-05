using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamCriteriaType
    {
        [UiInfo(Display = "Overall")]
        [Display(Name = "Overall")]
        Overall = 1,

        [UiInfo(Display = "Graphics")]
        [Display(Name = "Graphics")]
        Graphics = 2,

        [UiInfo(Display = "Sound")]
        [Display(Name = "Sound")]
        Sound = 3,

        [UiInfo(Display = "Theme")]
        [Display(Name = "Theme")]
        Theme = 4,

        [UiInfo(Display = "Fun")]
        [Display(Name = "Fun")]
        Fun = 5,

        [UiInfo(Display = "Gameplay")]
        [Display(Name = "Gameplay")]
        Gameplay = 6,

        [UiInfo(Display = "Level Design")]
        [Display(Name = "Level Design")]
        LevelDesign = 7,
    }
}