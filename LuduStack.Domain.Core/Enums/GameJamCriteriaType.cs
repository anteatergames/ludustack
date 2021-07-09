using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamCriteriaType
    {
        [UiInfo(Display = "Overall", Class = "globe")]
        [Display(Name = "Overall")]
        Overall = 1,

        [UiInfo(Display = "Graphics", Class = "eye")]
        [Display(Name = "Graphics")]
        Graphics = 2,

        [UiInfo(Display = "Sound", Class = "volume-up")]
        [Display(Name = "Sound")]
        Sound = 3,

        [UiInfo(Display = "Gameplay", Class = "dice")]
        [Display(Name = "Gameplay")]
        Gameplay = 4,

        [UiInfo(Display = "Story", Class = "scroll")]
        [Display(Name = "Story")]
        Story = 5,

        [UiInfo(Display = "Levels", Class = "book")]
        [Display(Name = "Levels")]
        LevelDesign = 6,

        [UiInfo(Display = "Theme", Class = "dungeon")]
        [Display(Name = "Theme")]
        Theme = 7,

        [UiInfo(Display = "Fun", Class = "laugh")]
        [Display(Name = "Fun")]
        Fun = 8,

        [UiInfo(Display = "Controls", Class = "gamepad")]
        [Display(Name = "Controls")]
        Controls = 9,

        [UiInfo(Display = "Mechanics", Class = "puzzle-piece")]
        [Display(Name = "Mechanics")]
        Mechanics = 10,

        [UiInfo(Display = "Innovation", Class = "hat-wizard")]
        [Display(Name = "Innovation")]
        Innovation = 11,
    }
}