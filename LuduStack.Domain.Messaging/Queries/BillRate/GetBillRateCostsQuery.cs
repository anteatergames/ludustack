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

            SetVisualElementRates(vo, rates);

            SetAudioElementRates(vo, rates);

            SetCodeElementRates(vo, rates);

            SetTextElementRates(vo, rates);

            return vo;
        }

        private static void SetVisualElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            IEnumerable<ArtStyle> styles = Enum.GetValues(typeof(ArtStyle)).Cast<ArtStyle>();

            foreach (ArtStyle style in styles)
            {
                VisualRateVo conceptArtVo = CreateVisualRateVo(rates, GameElement.ConceptArt, style);
                vo.VisualRates.Add(conceptArtVo);

                VisualRateVo character2dVo = CreateVisualRateVo(rates, GameElement.Character2d, style);
                vo.VisualRates.Add(character2dVo);

                VisualRateVo level2dVo = CreateVisualRateVo(rates, GameElement.Level2d, style);
                vo.VisualRates.Add(level2dVo);

                VisualRateVo asset2dVo = CreateVisualRateVo(rates, GameElement.Asset2d, style);
                vo.VisualRates.Add(asset2dVo);

                VisualRateVo character3dVo = CreateVisualRateVo(rates, GameElement.Character3d, style);
                vo.VisualRates.Add(character3dVo);

                VisualRateVo level3dVo = CreateVisualRateVo(rates, GameElement.Level3d, style);
                vo.VisualRates.Add(level3dVo);

                VisualRateVo asset3dVo = CreateVisualRateVo(rates, GameElement.Asset3d, style);
                vo.VisualRates.Add(asset3dVo);
            }
        }

        private static void SetAudioElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            IEnumerable<SoundStyle> styles = Enum.GetValues(typeof(SoundStyle)).Cast<SoundStyle>();

            foreach (SoundStyle style in styles)
            {
                AudioRateVo chiptuneFxVo = CreateAudioRateVo(rates, GameElement.SoundFx, style);
                vo.AudioRates.Add(chiptuneFxVo);

                AudioRateVo chiptuneMusicTrackVo = CreateAudioRateVo(rates, GameElement.MusicTrack, style);
                vo.AudioRates.Add(chiptuneMusicTrackVo);
            }
        }

        private static void SetCodeElementRates(CostCalculatorVo vo, IQueryable<Models.BillRate> rates)
        {
            CodeRateVo gameplay2dCodeVo = CreateCodeRateVo(rates, GameElement.GameplayCode2d);
            vo.CodeRates.Add(gameplay2dCodeVo);

            CodeRateVo gameplay3dCodeVo = CreateCodeRateVo(rates, GameElement.GameplayCode3d);
            vo.CodeRates.Add(gameplay3dCodeVo);
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