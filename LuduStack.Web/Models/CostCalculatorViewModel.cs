using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Models
{
    public class CostCalculatorViewModel
    {
        [Display(Name = "Art Style", Description = "How you want the game to look?")]
        public ArtStyle2D ArtStyle { get; set; }

        [Display(Name = "Characters", Description = "How many characters the game has?")]
        public int Characters { get; set; }

        [Display(Name = "Levels/Maps/Tracks", Description = "How many levels/maps/tracks the game has?")]
        public int Levels { get; set; }

        [Display(Name = "Concept Arts", Description = "How many concept art pieces the game has?")]
        public int ConceptArts { get; set; }
    }
}
