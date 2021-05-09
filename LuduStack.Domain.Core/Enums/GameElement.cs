using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameElement
    {
        [UiInfo(Display = "Concept Art", Type = (int)BillRateType.Visual)]
        [Display(Name = "Concept Art")]
        ConceptArt = 1,

        [UiInfo(Display = "2D Character", Type = (int)BillRateType.Visual, SubType = (int)Dimensions.TwoDee)]
        [Display(Name = "2D Character")]
        Character2d = 2,

        [UiInfo(Display = "2D Asset", Type = (int)BillRateType.Visual, SubType = (int)Dimensions.TwoDee)]
        [Display(Name = "2D Asset")]
        Asset2d = 3,

        [UiInfo(Display = "2D Level Design", Type = (int)BillRateType.Visual, SubType = (int)Dimensions.TwoDee)]
        [Display(Name = "2D Level Design")]
        Level2d = 4,

        [UiInfo(Display = "3D Character", Type = (int)BillRateType.Visual, SubType = (int)Dimensions.ThreeDee)]
        [Display(Name = "3D Character")]
        Character3d = 5,

        [UiInfo(Display = "3D Asset", Type = (int)BillRateType.Visual, SubType = (int)Dimensions.ThreeDee)]
        [Display(Name = "3D Asset")]
        Asset3d = 6,

        [UiInfo(Display = "3D Level Design", Type = (int)BillRateType.Visual, SubType = (int)Dimensions.ThreeDee)]
        [Display(Name = "3D Level Design")]
        Level3d = 7,

        [UiInfo(Display = "Sound FX", Type = (int)BillRateType.Audio)]
        [Display(Name = "Sound FX")]
        SoundFx = 8,

        [UiInfo(Display = "Music Track", Type = (int)BillRateType.Audio)]
        [Display(Name = "Music Track")]
        MusicTrack = 9,

        [UiInfo(Display = "Voice Over", Type = (int)BillRateType.Audio)]
        [Display(Name = "Voice Over")]
        VoiceOver = 10,

        [UiInfo(Display = "2D Gameplay Code", Type = (int)BillRateType.Code)]
        [Display(Name = "2D Gameplay Code")]
        GameplayCode2d = 11,

        [UiInfo(Display = "3D Gameplay Code", Type = (int)BillRateType.Code)]
        [Display(Name = "3D Gameplay Code")]
        GameplayCode3d = 12,

        [UiInfo(Display = "Story", Type = (int)BillRateType.Text)]
        [Display(Name = "Story")]
        Story = 13,

        [UiInfo(Display = "Game Design Document", Type = (int)BillRateType.Text)]
        [Display(Name = "GDD")]
        GameDesignDocument = 14
    }
}