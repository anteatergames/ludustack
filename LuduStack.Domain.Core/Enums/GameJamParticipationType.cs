using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamParticipationType
    {
        [Display(Name = "Individuals Only")]
        [UiInfo(Display = "Individuals Only", Description = "Only individuals can participate on the Jam and must produce the game all alone.")]
        IndividualsOnly = 1,

        [Display(Name = "Teams Only")]
        [UiInfo(Display = "Teams Only", Description = "Only teams can participate on the Jam. No individuals allowed.")]
        TeamsOnly = 2,

        [Display(Name = "Individuals and Teams")]
        [UiInfo(Display = "Individuals and Teams", Description = "Anyone can join this Jam either as individuals or as teams.")]
        IndividualsAndTeams = 3,
    }
}