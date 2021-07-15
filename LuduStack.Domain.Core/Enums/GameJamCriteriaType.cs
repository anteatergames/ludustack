using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamCriteriaType
    {
        [UiInfo(Display = "Overall", Description = "Overall feeling about the game.", Class = "globe")]
        [Display(Name = "Overall", Description = "Overall feeling about the game.")]
        Overall = 1,

        [UiInfo(Display = "Graphics", Description = "Does the submitted game look beautiful?", Class = "eye")]
        [Display(Name = "Graphics", Description = "Does the submitted game look beautiful?")]
        Graphics = 2,

        [UiInfo(Display = "Sound", Description = "Does the submitted game sound beautiful?", Class = "volume-up")]
        [Display(Name = "Sound", Description = "Does the submitted game sound beautiful?")]
        Sound = 3,

        [UiInfo(Display = "Gameplay", Description = "Is the submitted game easy to play?", Class = "dice")]
        [Display(Name = "Gameplay", Description = "Is the submitted game easy to play?")]
        Gameplay = 4,

        [UiInfo(Display = "Story", Description = "Does the submitted game have a good story?", Class = "scroll")]
        [Display(Name = "Story", Description = "Does the submitted game have a good story?")]
        Story = 5,

        [UiInfo(Display = "Levels", Description = "Do the levels of the submitted game have a good design?", Class = "book")]
        [Display(Name = "Levels", Description = "Do the levels of the submitted game have a good design?")]
        LevelDesign = 6,

        [UiInfo(Display = "Theme", Description = "Did the submitted game use the Game Jam theme well?", Class = "dungeon")]
        [Display(Name = "Theme", Description = "Did the submitted game use the Game Jam theme well?")]
        Theme = 7,

        [UiInfo(Display = "Fun", Description = "Is the submitted game fun to play?", Class = "laugh")]
        [Display(Name = "Fun", Description = "Is the submitted game fun to play?")]
        Fun = 8,

        [UiInfo(Display = "Controls", Description = "Are the controls of the submitted game easy to learn and use?", Class = "gamepad")]
        [Display(Name = "Controls", Description = "Are the controls of the submitted game easy to learn and use?")]
        Controls = 9,

        [UiInfo(Display = "Mechanics", Description = "Does the submitted game make use of good and clever mechanics?", Class = "puzzle-piece")]
        [Display(Name = "Mechanics", Description = "Does the submitted game make use of good and clever mechanics?")]
        Mechanics = 10,

        [UiInfo(Display = "Innovation", Description = "Does the submitted game bring some innovation to the game industry?", Class = "hat-wizard")]
        [Display(Name = "Innovation", Description = "Does the submitted game bring some innovation to the game industry?")]
        Innovation = 11,
    }
}