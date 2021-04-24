using LuduStack.Domain.Core.Enums;
using System.Collections.Generic;

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

        public ResultRateVo Price { get; set; }

        public ResultRateVo Time { get; set; }
    }

    public class ResultRateVo
    {
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