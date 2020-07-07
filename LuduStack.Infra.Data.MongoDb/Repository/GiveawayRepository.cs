using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class GiveawayRepository : BaseRepository<Giveaway>, IGiveawayRepository
    {
        public GiveawayRepository(IMongoContext context) : base(context)
        {
        }

        public List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId)
        {
            IQueryable<GiveawayListItemVo> obj = DbSet.AsQueryable().Where(x => x.UserId == userId).Select(x => new GiveawayListItemVo
            {
                Id = x.Id,
                Name = x.Name,
                FeaturedImage = x.FeaturedImage,
                ParticipantCount = x.Participants.Count,
                CreateDate = x.CreateDate,
                Status = x.Status
            });

            return obj.ToList();
        }
    }
}