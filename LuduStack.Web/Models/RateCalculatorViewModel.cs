using System;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Web.Models
{
    public class RateCalculatorViewModel
    {
        [Required]
        [Display(Name = "Work days per year", Description = "The count of working days per year minus holidays, vacation days and sick days.")]
        [Range(1, 366)]
        public int YearWorkDays { get; set; }

        [Required]
        [Display(Name = "Montly Expenses", Description = "The amount of money needed to survive only paying the bills and buying food.")]
        [Range(1, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double MontlyExpenses { get; set; }

        [Required]
        [Display(Name = "Annual Cost", Description = "Montly Expenses x 12")]
        [Range(1, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double AnnualCost { get; set; }

        [Required]
        [Display(Name = "Savings %", Description = "A percentage of money to save for the future. We recommend 15%-25%")]
        [Range(1, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double SavingsPercentage { get; set; }

        [Display(Name = "per month", Description = "The amount of money needed to save each month")]
        public double SavingsValueMonthly { get; set; }

        [Display(Name = "per year", Description = "The amount of money needed to save each year")]
        public double SavingsValueYearly { get; set; }

        [Required]
        [Display(Name = "Extra %", Description = "A percentage of money for games, movies, books, etc. Try to stick between 25%-50%")]
        [Range(0, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double YourExtraPercentage { get; set; }

        [Display(Name = "per month", Description = "The amount of money for games, movies, books, etc. Each month.")]
        public double YourExtraValueMontly { get; set; }

        [Display(Name = "per year", Description = "The amount of money for games, movies, books, etc. Each year.")]
        public double YourExtraValueYearly { get; set; }

        [Required]
        [Display(Name = "Taxes %", Description = "The percentage of money you must pay as taxes to your country/state/city. Choose a value between 25%-40% according to where you live.")]
        [Range(0, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double TaxesPercentage { get; set; }

        [Display(Name = "per month", Description = "The amount of money for taxes each month.")]
        public double TaxesValueMontly { get; set; }

        [Display(Name = "per year", Description = "The amount of money for taxes each year.")]
        public double TaxesValueYearly { get; set; }

        [Required]
        [Display(Name = "Revision Factor", Description = "The percentage to add to a revision job. Revisions tend to be more detailed and demand more effort from you and from the client itself. We recommend a minimum of 25%.")]
        [Range(0, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double RevisionFactor { get; set; }

        [Required]
        [Display(Name = "Rush Factor", Description = "The percentage to add to a rushed job. If the client want a fast delivery, it must understand the overrate. We recommend a number between 50%-100%.")]
        [Range(0, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double RushFactor { get; set; }

        [Required]
        [Display(Name = "Asshole Factor", Description = "The percentage to add to a job you don't want to get. We recommend a starting at 100%.")]
        [Range(0, double.MaxValue, ErrorMessage = "Need a number here!")]
        public double AssholeFactor { get; set; }

        public RateCalculatorViewModel()
        {
            YearWorkDays = 231;
            SavingsPercentage = 15;
            YourExtraPercentage = 25;
            TaxesPercentage = 25;
            RevisionFactor = 25;
            RushFactor = 50;
            AssholeFactor = 100;
        }
    }
}