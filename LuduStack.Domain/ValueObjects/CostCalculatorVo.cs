using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Domain.ValueObjects
{
    public class CostCalculatorVo
    {
        public List<VisualRateVo> VisualRates { get; set; }

        public List<AudioRateVo> AudioRates { get; set; }

        public List<CodeRateVo> CodeRates { get; set; }

        public List<TextRateVo> TextRates { get; set; }

        public CostCalculatorVo()
        {
            VisualRates = new List<VisualRateVo>();
            AudioRates = new List<AudioRateVo>();
            CodeRates = new List<CodeRateVo>();
            TextRates = new List<TextRateVo>();
        }
    }

    public abstract class RateBaseVo
    {
        public GameElement GameElement { get; set; }

        public decimal Minimum { get; set; }

        public decimal Average { get; set; }

        public decimal Maximum { get; set; }
    }

    public class VisualRateVo : RateBaseVo
    {
        public ArtStyle ArtStyle { get; set; }
    }

    public class AudioRateVo : RateBaseVo
    {
        public SoundStyle SoundStyle { get; set; }
    }

    public class CodeRateVo : RateBaseVo
    {
    }

    public class TextRateVo : RateBaseVo
    {
    }
}
