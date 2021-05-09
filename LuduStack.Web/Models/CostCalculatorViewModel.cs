using LuduStack.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Web.Models
{
    public class CostCalculatorViewModel
    {
        [Display(Name = "Dimensions", Description = "How many dimensions your game has?")]
        public Dimensions VisualDimensions { get; set; }

        [Display(Name = "Art Style", Description = "How do you want the game to look?")]
        public ArtStyle ArtStyle { get; set; }

        [Display(Name = "Sound Style", Description = "How do you want the game to sound?")]
        public SoundStyle SoundStyle { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Concept Art Pieces", Description = "How many concept arts you need for the project?")]
        public int ConceptArtCount { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "2D Characters", Description = "How many 2D characters the game has?")]
        public int CharacterCount2d { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "2D Assets", Description = "How many 2D assets the game has?")]
        public int AssetCount2d { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more.")]
        [Display(Name = "2D Levels/Maps/Tracks", Description = "How many 2D levels/maps/tracks the game has?")]
        public int LevelCount2d { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "3D Characters", Description = "How many 3D characters the game has?")]
        public int CharacterCount3d { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "3D Assets", Description = "How many 3D assets the game has?")]
        public int AssetCount3d { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more.")]
        [Display(Name = "3D Levels/Maps/Tracks", Description = "How many 3D levels/maps/tracks the game has?")]
        public int LevelCount3d { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Sound FX", Description = "How many sound effects you need in your game?")]
        public int SoundEffectCount { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Music Tracks", Description = "How many music tracks you need in your game?")]
        public int MusicTrackCount { get; set; }
    }
}