using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
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
            var vo = new CostCalculatorVo();

            var rates = (await repository.GetAll()).AsQueryable();

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
            VisualRateVo pixelArtConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.PixelArt);
            vo.VisualRates.Add(pixelArtConceptArtVo);

            VisualRateVo pixelArtCharacterVo = CreateVisualRateVo(rates, GameElement.Character, ArtStyle.PixelArt);
            vo.VisualRates.Add(pixelArtCharacterVo);

            VisualRateVo pixelArtLevelVo = CreateVisualRateVo(rates, GameElement.Level, ArtStyle.PixelArt);
            vo.VisualRates.Add(pixelArtLevelVo);
        }

        private static void SetVectorElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo vectorConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Vector);
            vo.VisualRates.Add(vectorConceptArtVo);

            VisualRateVo vectorCharacterVo = CreateVisualRateVo(rates, GameElement.Character, ArtStyle.Vector);
            vo.VisualRates.Add(vectorCharacterVo);

            VisualRateVo vectorLevelVo = CreateVisualRateVo(rates, GameElement.Level, ArtStyle.Vector);
            vo.VisualRates.Add(vectorLevelVo);
        }

        private static void SetArtisticElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo artisticConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Artistic);
            vo.VisualRates.Add(artisticConceptArtVo);

            VisualRateVo artisticCharacterVo = CreateVisualRateVo(rates, GameElement.Character, ArtStyle.Artistic);
            vo.VisualRates.Add(artisticCharacterVo);

            VisualRateVo artisticLevelVo = CreateVisualRateVo(rates, GameElement.Level, ArtStyle.Artistic);
            vo.VisualRates.Add(artisticLevelVo);
        }

        private static void SetRealisticElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            VisualRateVo realisticConceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, ArtStyle.Realistic);
            vo.VisualRates.Add(realisticConceptArtVo);

            VisualRateVo realisticCharacterVo = CreateVisualRateVo(rates, GameElement.Character, ArtStyle.Realistic);
            vo.VisualRates.Add(realisticCharacterVo);

            VisualRateVo realisticLevelVo = CreateVisualRateVo(rates, GameElement.Level, ArtStyle.Realistic);
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
            CodeRateVo gameplayCodeVo = CreateCodeRateVo(rates, GameElement.GameplayCode);
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
            var typeQuery = rates.Where(x => x.BillRateType == BillRateType.Visual);
            var styleQuery = typeQuery.Where(x => x.ArtStyle == style);
            var elementQuery = styleQuery.Where(x => x.GameElement == element);

            return new VisualRateVo
            {
                GameElement = element,
                ArtStyle = style,
                Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
            };
        }

        private static AudioRateVo CreateAudioRateVo(IQueryable<Models.BillRate> rates, GameElement element, SoundStyle style)
        {
            var typeQuery = rates.Where(x => x.BillRateType == BillRateType.Audio);
            var styleQuery = typeQuery.Where(x => x.SoundStyle == style);
            var elementQuery = styleQuery.Where(x => x.GameElement == element);

            return new AudioRateVo
            {
                GameElement = element,
                SoundStyle = style,
                Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
            };
        }

        private static CodeRateVo CreateCodeRateVo(IQueryable<Models.BillRate> rates, GameElement element)
        {
            var typeQuery = rates.Where(x => x.BillRateType == BillRateType.Code);
            var elementQuery = typeQuery.Where(x => x.GameElement == element);

            return new CodeRateVo
            {
                GameElement = element,
                Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
            };
        }

        private static TextRateVo CreateTextRateVo(IQueryable<Models.BillRate> rates, GameElement element)
        {
            var typeQuery = rates.Where(x => x.BillRateType == BillRateType.Text);
            var elementQuery = typeQuery.Where(x => x.GameElement == element);

            return new TextRateVo
            {
                GameElement = element,
                Minimum = elementQuery.Any() ? elementQuery.Min(x => x.HourPrice) : 0,
                Average = elementQuery.Any() ? elementQuery.Median(x => x.HourPrice) : 0,
                Maximum = elementQuery.Any() ? elementQuery.Max(x => x.HourPrice) : 0
            };
        }
    }
}