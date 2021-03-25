using LuduStack.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Web.Models
{
    public class CostCalculatorViewModel
    {
        [Display(Name = "Art Style", Description = "How you want the game to look?")]
        public ArtStyle ArtStyle { get; set; }

        [Display(Name = "Sound Style", Description = "How you want the game to sound?")]
        public SoundStyle SoundStyle { get; set; }

        [Display(Name = "Concept Art Pieces", Description = "How many concept arts you need for the project?")]
        public int ConceptArtPieces { get; set; }

        [Display(Name = "Characters", Description = "How many characters the game has?")]
        public int Characters { get; set; }

        [Display(Name = "Levels/Maps/Tracks", Description = "How many levels/maps/tracks the game has?")]
        public int Levels { get; set; }

        [Display(Name = "Sound FX", Description = "How many sound effects you need in your game?")]
        public int SoundEffects { get; set; }

        [Display(Name = "Music Tracks", Description = "How many music tracks you need in your game?")]
        public int MusicTracks { get; set; }
    }
}