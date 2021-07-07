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

        [UiInfo(Display = "Gameplay")]
        [Display(Name = "Gameplay")]
        Gameplay = 5,

        [UiInfo(Display = "Level Design")]
        [Display(Name = "Level Design")]
        LevelDesign = 6,

        [UiInfo(Display = "Story")]
        [Display(Name = "Story")]
        Story = 7,

        [UiInfo(Display = "Fun")]
        [Display(Name = "Fun")]
        Fun = 8,

        [UiInfo(Display = "Controls")]
        [Display(Name = "Controls")]
        Controls = 9,

        [UiInfo(Display = "Mechanics")]
        [Display(Name = "Mechanics")]
        Mechanics = 10,

        [UiInfo(Display = "Innovation")]
        [Display(Name = "Innovation")]
        Innovation = 11,
    }
}