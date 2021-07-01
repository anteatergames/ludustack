using LuduStack.Domain.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Domain.Core.Enums
{
    public enum GameJamVoters
    {
        [UiInfo(Display = "Judges Only")]
        [Display(Name = "Judges Only")]
        JudgesOnly = 1,

        [UiInfo(Display = "Judges (if any) and Submitters")]
        [Display(Name = "Judges (if any) and Submitters")]
        JudgesAndSubmitters = 2,

        [UiInfo(Display = "Judges (if any) and Team Members")]
        [Display(Name = "Judges (if any) and Team Members")]
        JudgesAndTeamMembers = 3,

        [UiInfo(Display = "Judges (if any) and Non-Participants")]
        [Display(Name = "Judges (if any) and Non-Participants")]
        JudgesAndNonParticipants = 4,

        [UiInfo(Display = "Judges (if any) and the whole community")]
        [Display(Name = "Judges (if any) and the whole community")]
        JudgesAndTheWholeCommunity = 5,
    }
}