using LuduStack.Domain.Core.Enums;
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

        public Task<GiveawayBasicInfo> GetBasicGiveawayById(Guid id)
        {
            var obj = DbSet.AsQueryable().Where(x => x.Id == id).Select(x => new GiveawayBasicInfo
            {
                Id = x.Id,
                UserId = x.UserId,
                CreateDate = x.CreateDate,
                Status = x.Status,
                Name = x.Name,
                Description = x.Description,
                FeaturedImage = x.FeaturedImage,
                ImageList = x.ImageList,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                TimeZone = x.TimeZone,
                MembersOnly = x.MembersOnly,
                WinnerAmount = x.WinnerAmount,
                PrizePriceInDolar = x.PrizePriceInDolar,
                TermsAndConditions = x.TermsAndConditions,
                SponsorName = x.SponsorName,
                SponsorWebsite = x.SponsorWebsite
            });

            return Task.FromResult(obj.FirstOrDefault());
        }

        public List<GiveawayListItemVo> GetGiveawayListByUserId(Guid userId)
        {
            IQueryable<GiveawayListItemVo> obj = DbSet.AsQueryable().Where(x => x.UserId == userId).Select(x => new GiveawayListItemVo
            {
                Id = x.Id,
                Name = x.Name,
                ImageList = x.ImageList,
                ParticipantCount = x.Participants.Count,
                CreateDate = x.CreateDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status
            });

            return obj.ToList();
        }

        public void UpdateGiveawayStatus(Guid giveawayId, GiveawayStatus newStatus)
        {
            FilterDefinition<Giveaway> filter = Builders<Giveaway>.Filter.Where(x => x.Id == giveawayId);
            UpdateDefinition<Giveaway> add = Builders<Giveaway>.Update.Set(x => x.Status, newStatus);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
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
                .Set(c => c.Participants[-1].WantNotifications, participant.WantNotifications)
                .Set(c => c.Participants[-1].ShortUrl, participant.ShortUrl)
                .Set(c => c.Participants[-1].IsWinner, participant.IsWinner)
                .Set(c => c.Participants[-1].Entries, participant.Entries);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));
        }

        public void RemoveParticipant(Guid giveawayId, Guid participantId)
        {
            FilterDefinition<Giveaway> filter = Builders<Giveaway>.Filter.Where(x => x.Id == giveawayId);
            UpdateDefinition<Giveaway> remove = Builders<Giveaway>.Update.PullFilter(c => c.Participants, m => m.Id == participantId);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, remove));
        }

        public void ClearParticipants(Guid giveawayId)
        {
            FilterDefinition<Giveaway> filter = Builders<Giveaway>.Filter.Where(x => x.Id == giveawayId);
            UpdateDefinition<Giveaway> remove = Builders<Giveaway>.Update.PullFilter(c => c.Participants, m => true);

            Context.AddCommand(() => DbSet.UpdateOneAsync(filter, remove));
        }

        public GiveawayParticipant GetParticipantByEmail(Guid giveawayId, string email)
        {
            GiveawayParticipant model = DbSet.AsQueryable().Where(x => x.Id == giveawayId).SelectMany(x => x.Participants).FirstOrDefault(x => x.Email.Equals(email));

            return model;
        }

        public Guid CheckParticipantByEmail(Guid giveawayId, string email)
        {
            var guid = DbSet.AsQueryable().Where(x => x.Id == giveawayId).SelectMany(x => x.Participants).Where(x => x.Email.Equals(email)).Select(x => x.Id).FirstOrDefault();

            return guid;
        }

        public GiveawayParticipant GetParticipantByReferralCode(Guid giveawayId, string referralCode)
        {
            GiveawayParticipant model = DbSet.AsQueryable().Where(x => x.Id == giveawayId).SelectMany(x => x.Participants).FirstOrDefault(x => x.ReferralCode.Equals(referralCode));

            return model;
        }

        public GiveawayParticipant GetParticipantById(Guid giveawayId, Guid participantId)
        {
            GiveawayParticipant model = DbSet.AsQueryable().Where(x => x.Id == giveawayId).SelectMany(x => x.Participants).FirstOrDefault(x => x.Id == participantId);

            return model;
        }
    }
}