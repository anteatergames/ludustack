using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public IQueryable<GiveawayParticipant> GetParticipants(Guid giveawayId)
        {
            IQueryable<GiveawayParticipant> participants = DbSet.AsQueryable().Where(x => x.Id == giveawayId).SelectMany(x => x.Participants);

            return participants;
        }

        public void AddParticipant(Guid giveawayId, GiveawayParticipant participant)
        {
            participant.Id = Guid.NewGuid();

            FilterDefinition<Giveaway> filter = Builders<Giveaway>.Filter.Where(x => x.Id == giveawayId);
            UpdateDefinition<Giveaway> add = Builders<Giveaway>.Update.AddToSet(c => c.Participants, participant);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
        }

        public void UpdateParticipant(Guid giveawayId, GiveawayParticipant participant)
        {
            FilterDefinition<Giveaway> filter = Builders<Giveaway>.Filter.And(
                Builders<Giveaway>.Filter.Eq(x => x.Id, giveawayId),
                Builders<Giveaway>.Filter.ElemMatch(x => x.Participants, x => x.Id == participant.Id));

            UpdateDefinition<Giveaway> update = Builders<Giveaway>.Update
                .Set(c => c.Participants[-1].Email, participant.Email)
                .Set(c => c.Participants[-1].GdprConsent, participant.GdprConsent)
                .Set(c => c.Participants[-1].WantNotifications, participant.WantNotifications);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));
        }

        public void RemoveParticipant(Guid giveawayId, Guid participantId)
        {
            FilterDefinition<Giveaway> filter = Builders<Giveaway>.Filter.Where(x => x.Id == giveawayId);
            UpdateDefinition<Giveaway> remove = Builders<Giveaway>.Update.PullFilter(c => c.Participants, m => m.Id == participantId);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, remove));
        }
    }
}