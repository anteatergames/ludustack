﻿using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace LuduStack.Application.ViewModels.CostCalculator
{
    public class CostsViewModel
    {
        public List<VisualRateCostsViewModel> VisualRates { get; set; }

        public List<AudioRateCostsViewModel> AudioRates { get; set; }

        public List<CodeRateCostsViewModel> CodeRates { get; set; }

        public List<TextRateCostsViewModel> TextRates { get; set; }

        public CostsViewModel()
        {
            VisualRates = new List<VisualRateCostsViewModel>();
            AudioRates = new List<AudioRateCostsViewModel>();
            CodeRates = new List<CodeRateCostsViewModel>();
            TextRates = new List<TextRateCostsViewModel>();
        }
    }

    public abstract class RateBaseViewModel
    {
        public GameElement GameElement { get; set; }

        public decimal Minimum { get; set; }

        public decimal Average { get; set; }

        public decimal Maximum { get; set; }
    }

    public class VisualRateCostsViewModel : RateBaseViewModel
    {
        public ArtStyle ArtStyle { get; set; }
    }

    public class AudioRateCostsViewModel : RateBaseViewModel
    {
        public SoundStyle SoundStyle { get; set; }
    }

    public class CodeRateCostsViewModel : RateBaseViewModel
    {
    }

    public class TextRateCostsViewModel : RateBaseViewModel
    {
    }
}
