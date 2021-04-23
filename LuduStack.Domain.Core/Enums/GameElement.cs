using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameElement
    {
        [UiInfo(Display = "Concept Art", Type = (int)BillRateType.Visual)]
        [Display(Name = "Concept Art")]
        ConceptArt = 1,

        [UiInfo(Display = "Character", Type = (int)BillRateType.Visual)]
        [Display(Name = "Character")]
        Character = 2,

        [UiInfo(Display = "Level", Type = (int)BillRateType.Visual)]
        [Display(Name = "Level")]
        Level = 3,

        [UiInfo(Display = "Sound FX", Type = (int)BillRateType.Audio)]
        [Display(Name = "Sound FX")]
        SoundFx = 4,

        [UiInfo(Display = "Music Track", Type = (int)BillRateType.Audio)]
        [Display(Name = "Music Track")]
        MusicTrack = 5,

        [UiInfo(Display = "Gameplay Code", Type = (int)BillRateType.Code)]
        [Display(Name = "Gameplay Code")]
        GameplayCode = 6,

        [UiInfo(Display = "UI Code", Type = (int)BillRateType.Code)]
        [Display(Name = "UI Code")]
        UiCode = 7,

        [UiInfo(Display = "Story", Type = (int)BillRateType.Text)]
        [Display(Name = "Story")]
        Story = 8,

        [UiInfo(Display = "Game Design Document", Type = (int)BillRateType.Text)]
        [Display(Name = "GDD")]
        GameDesignDocument = 9
    }
}