using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging.Queries.BillRate
{
    public class GetBillRateCostsQuery : Query<CostCalculatorVo>
    {
        public GetBillRateCostsQuery()
        {
        }
    }

    public class GetBillRateCostsQueryHandler : QueryHandler, IRequestHandler<GetBillRateCostsQuery, CostCalculatorVo>
    {
        protected readonly IBillRateRepository repository;

        public GetBillRateCostsQueryHandler(IBillRateRepository repository)
        {
            this.repository = repository;
        }

        public async Task<CostCalculatorVo> Handle(GetBillRateCostsQuery request, CancellationToken cancellationToken)
        {
            CostCalculatorVo vo = new CostCalculatorVo();

            IQueryable<Models.BillRate> rates = (await repository.GetAll()).AsQueryable();

            SetPixelArtElementRates(vo, rates);
            SetVectorElementRates(vo, rates);
            SetArtisticElementRates(vo, rates);
            SetRealisticElementRates(vo, rates);

            SetChiptuneElementRates(vo, rates);
            SetSnesEraElementRates(vo, rates);
            SetOrchestralElementRates(vo, rates);

            SetCodeElementRates(vo, rates);

            SetTextElementRates(vo, rates);

            return vo;
        }

        private static void SetPixelArtElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo pixelArtConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Pixel);
            vo.VisualRates.Add(pixelArtConceptArtVo);

            VisualRateVo pixelArtCharacterVo = CreateVisualRateVo(rates, GameElement.Character2d, ArtStyle.Pixel);
            vo.VisualRates.Add(pixelArtCharacterVo);

            VisualRateVo pixelArtLevelVo = CreateVisualRateVo(rates, GameElement.Level2d, ArtStyle.Pixel);
            vo.VisualRates.Add(pixelArtLevelVo);
        }

        private static void SetVectorElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo vectorConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Flat);
            vo.VisualRates.Add(vectorConceptArtVo);

            VisualRateVo vectorCharacterVo = CreateVisualRateVo(rates, GameElement.Character2d, ArtStyle.Flat);
            vo.VisualRates.Add(vectorCharacterVo);

            VisualRateVo vectorLevelVo = CreateVisualRateVo(rates, GameElement.Level2d, ArtStyle.Flat);
            vo.VisualRates.Add(vectorLevelVo);
        }

        private static void SetArtisticElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo artisticConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Artistic);
            vo.VisualRates.Add(artisticConceptArtVo);

            VisualRateVo artisticCharacterVo = CreateVisualRateVo(rates, GameElement.Character2d, ArtStyle.Artistic);
            vo.VisualRates.Add(artisticCharacterVo);

            VisualRateVo artisticLevelVo = CreateVisualRateVo(rates, GameElement.Level2d, ArtStyle.Artistic);
            vo.VisualRates.Add(artisticLevelVo);
        }

        private static void SetRealisticElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo realisticConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Realistic);
            vo.VisualRates.Add(realisticConceptArtVo);

            VisualRateVo realisticCharacterVo = CreateVisualRateVo(rates, GameElement.Character2d, ArtStyle.Realistic);
            vo.VisualRates.Add(realisticCharacterVo);

            VisualRateVo realisticLevelVo = CreateVisualRateVo(rates, GameElement.Level2d, ArtStyle.Realistic);
            vo.VisualRates.Add(realisticLevelVo);
        }

        private static void SetChiptuneElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            AudioRateVo chiptuneFxVo = CreateAudioRateVo(rates, GameElement.SoundFx, SoundStyle.Chiptune);
            vo.AudioRates.Add(chiptuneFxVo);

            AudioRateVo chiptuneMusicTrackVo = CreateAudioRateVo(rates, GameElement.MusicTrack, SoundStyle.Chiptune);
            vo.AudioRates.Add(chiptuneMusicTrackVo);
        }

        private static void SetSnesEraElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            AudioRateVo snesEraFxVo = CreateAudioRateVo(rates, GameElement.SoundFx, SoundStyle.SnesEra);
            vo.AudioRates.Add(snesEraFxVo);

            AudioRateVo snesEraMusicTrackVo = CreateAudioRateVo(rates, GameElement.MusicTrack, SoundStyle.SnesEra);
            vo.AudioRates.Add(snesEraMusicTrackVo);
        }

        private static void SetOrchestralElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            AudioRateVo orchestralFxVo = CreateAudioRateVo(rates, GameElement.SoundFx, SoundStyle.Orchestral);
            vo.AudioRates.Add(orchestralFxVo);

            AudioRateVo orchestralMusicTrackVo = CreateAudioRateVo(rates, GameElement.MusicTrack, SoundStyle.Orchestral);
            vo.AudioRates.Add(orchestralMusicTrackVo);
        }

        private static void SetCodeElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            CodeRateVo gameplayCodeVo = CreateCodeRateVo(rates, GameElement.GameplayCode2d);
            vo.CodeRates.Add(gameplayCodeVo);

            CodeRateVo uiCodeVo = CreateCodeRateVo(rates, GameElement.UiCode);
            vo.CodeRates.Add(uiCodeVo);
        }

        private static void SetTextElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            TextRateVo storyVo = CreateTextRateVo(rates, GameElement.Story);
            vo.TextRates.Add(storyVo);

            TextRateVo gddVo = CreateTextRateVo(rates, GameElement.GameDesignDocument);
            vo.TextRates.Add(gddVo);
        }

        private static VisualRateVo CreateVisualRateVo(IQueryable<Models.BillRate> rates, GameElement element, ArtStyle style)
        {
            IQueryable<Models.BillRate> typeQuery = rates.Where(x => x.BillRateType == BillRateType.Visual);
            IQueryable<Models.BillRate> styleQuery = typeQuery.Where(x => x.ArtStyle == style);
            IQueryable<Models.BillRate> elementQuery = styleQuery.Where(x => x.GameElement == element);

            return new VisualRateVo
            {
                GameElement = element,
                ArtStyle = style,
                Price = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
                },
                Time = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourQuantity) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => (decimal)x.HourQuantity) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourQuantity) : 0
                }
            };
        }

        private static AudioRateVo CreateAudioRateVo(IQueryable<Models.BillRate> rates, GameElement element, SoundStyle style)
        {
            IQueryable<Models.BillRate> typeQuery = rates.Where(x => x.BillRateType == BillRateType.Audio);
            IQueryable<Models.BillRate> styleQuery = typeQuery.Where(x => x.SoundStyle == style);
            IQueryable<Models.BillRate> elementQuery = styleQuery.Where(x => x.GameElement == element);

            return new AudioRateVo
            {
                GameElement = element,
                SoundStyle = style,
                Price = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
                },
                Time = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourQuantity) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => (decimal)x.HourQuantity) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourQuantity) : 0
                }
            };
        }

        private static CodeRateVo CreateCodeRateVo(IQueryable<Models.BillRate> rates, GameElement element)
        {
            IQueryable<Models.BillRate> typeQuery = rates.Where(x => x.BillRateType == BillRateType.Code);
            IQueryable<Models.BillRate> elementQuery = typeQuery.Where(x => x.GameElement == element);

            return new CodeRateVo
            {
                GameElement = element,
                Price = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
                },
                Time = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourQuantity) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => (decimal)x.HourQuantity) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourQuantity) : 0
                }
            };
        }

        private static TextRateVo CreateTextRateVo(IQueryable<Models.BillRate> rates, GameElement element)
        {
            IQueryable<Models.BillRate> typeQuery = rates.Where(x => x.BillRateType == BillRateType.Text);
            IQueryable<Models.BillRate> elementQuery = typeQuery.Where(x => x.GameElement == element);

            return new TextRateVo
            {
                GameElement = element,
                Price = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
                },
                Time = new ResultRateVo
                {
                    Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourQuantity) : 0,
                    Average = elementQuery.Any() ? elementQuery.Median(x => (decimal)x.HourQuantity) : 0,
                    Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourQuantity) : 0
                }
            };
        }
    }
}