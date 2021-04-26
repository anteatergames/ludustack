using LuduStack.Domain.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.BillRate
{
    public class BillRateViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "The type is required!")]
        [Display(Name = "Type", Description = "Here you must set what you are billing for.")]
        public BillRateType BillRateType { get; set; }

        [Display(Name = "Art Style", Description = "What kind of visual art? If you can't find an exact match, choose the closest one.")]
        public ArtStyle? ArtStyle { get; set; }

        [Display(Name = "Sound Style", Description = "How would you describe the sound style? If you can't find an exact match, choose the closest one.")]
        public SoundStyle? SoundStyle { get; set; }

        [Required(ErrorMessage = "The Game Element is required!")]
        [Display(Name = "Game Element", Description = "What would be the outcome of this service?")]
        public GameElement GameElement { get; set; }

        [Required(ErrorMessage = "The Element Price is required!")]
        [Display(Name = "Element Price (USD)", Description = "How much you bill (in US dolars) for one game element of this type?")]
        public decimal ElementPrice { get; set; }

        [Required(ErrorMessage = "The Hour Price is required!")]
        [Display(Name = "Hour Price (USD)", Description = "How much you bill (in US dolars) for one hour working on this?")]
        public decimal HourPrice { get; set; }

        [Required(ErrorMessage = "The Hour Quantity is required!")]
        [Display(Name = "Hour Quantity", Description = "How many hours it takes to accomplish one game element of this?")]
        public int HourQuantity { get; set; }

        public string Icon { get; set; }
    }
}