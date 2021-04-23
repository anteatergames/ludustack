using LuduStack.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LuduStack.Web.Models
{
    public class CostCalculatorViewModel
    {
        [Display(Name = "Art Style", Description = "How you want the game to look?")]
        public ArtStyle ArtStyle { get; set; }

        [Display(Name = "Sound Style", Description = "How you want the game to sound?")]
        public SoundStyle SoundStyle { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Concept Art Pieces", Description = "How many concept arts you need for the project?")]
        public int ConceptArtCount { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Characters", Description = "How many characters the game has?")]
        public int CharacterCount { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more.")]
        [Display(Name = "Levels/Maps/Tracks", Description = "How many levels/maps/tracks the game has?")]
        public int LevelCount { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Sound FX", Description = "How many sound effects you need in your game?")]
        public int SoundEffectCount { get; set; }

        [Range(0, 9999, ErrorMessage = "Zero or more")]
        [Display(Name = "Music Tracks", Description = "How many music tracks you need in your game?")]
        public int MusicTrackCount { get; set; }
    }
}