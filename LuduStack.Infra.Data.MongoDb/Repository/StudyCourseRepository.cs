using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.Data.MongoDb.Extensions;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class StudyCourseRepository : BaseRepository<StudyCourse>, IStudyCourseRepository
    {
        public StudyCourseRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<List<StudyCourseListItemVo>> GetCourses()
        {
            IQueryable<StudyCourseListItemVo> obj = DbSet.AsQueryable().Select(x => new StudyCourseListItemVo
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                FeaturedImage = x.FeaturedImage,
                OpenForApplication = x.OpenForApplication,
                StudentCount = x.Members.Count
            });

            return await obj.ToMongoListAsync();
        }

        public async Task<List<StudyCourseListItemVo>> GetCoursesByUserId(Guid userId)
        {
            IQueryable<StudyCourseListItemVo> obj = DbSet.AsQueryable().Where(x => x.UserId == userId).Select(x => new StudyCourseListItemVo
            {
                Id = x.Id,
                UserId = x.UserId,
                Name = x.Name,
                FeaturedImage = x.FeaturedImage,
                OpenForApplication = x.OpenForApplication,
                StudentCount = x.Members.Count
            });

            return await obj.ToMongoListAsync();
        }

        public async Task<List<StudyPlan>> GetPlans(Guid courseId)
        {
            IQueryable<StudyPlan> objs = DbSet.AsQueryable().Where(x => x.Id == courseId).SelectMany(x => x.Plans);

            return await objs.ToMongoListAsync();
        }

        public async Task<bool> AddPlan(Guid courseId, StudyPlan plan)
        {
            plan.Id = Guid.NewGuid();

            FilterDefinition<StudyCourse> filter = Builders<StudyCourse>.Filter.Where(x => x.Id == courseId);
            UpdateDefinition<StudyCourse> add = Builders<StudyCourse>.Update.AddToSet(c => c.Plans, plan);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, add);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public async Task<bool> UpdatePlan(Guid courseId, StudyPlan plan)
        {
            plan.LastUpdateDate = DateTime.Now;

            FilterDefinition<StudyCourse> filter = Builders<StudyCourse>.Filter.And(
                Builders<StudyCourse>.Filter.Eq(x => x.Id, courseId),
                Builders<StudyCourse>.Filter.ElemMatch(x => x.Plans, x => x.Id == plan.Id));

            UpdateDefinition<StudyCourse> update = Builders<StudyCourse>.Update
                .Set(c => c.Plans[-1].Name, plan.Name)
                .Set(c => c.Plans[-1].Description, plan.Description)
                .Set(c => c.Plans[-1].ScoreToPass, plan.ScoreToPass)
                .Set(c => c.Plans[-1].Order, plan.Order)
                .Set(c => c.Plans[-1].Activities, plan.Activities);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, update);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> RemovePlan(Guid courseId, Guid planId)
        {
            FilterDefinition<StudyCourse> filter = Builders<StudyCourse>.Filter.Where(x => x.Id == courseId);
            UpdateDefinition<StudyCourse> remove = Builders<StudyCourse>.Update.PullFilter(c => c.Plans, m => m.Id == planId);

            UpdateResult result = await DbSet.UpdateOneAsync(filter, remove);

            return result.IsAcknowledged && result.MatchedCount > 0;
        }

        public bool CheckStudentEnrolled(Guid courseId, Guid userId)
        {
            return DbSet.AsQueryable().FirstOrDefault(x => x.Id == courseId && x.Members.Any(y => y.UserId == userId)) != null;
        }

        public async Task AddStudent(Guid courseId, CourseMember student)
        {
            student.Id = Guid.NewGuid();

            FilterDefinition<StudyCourse> filter = Builders<StudyCourse>.Filter.Where(x => x.Id == courseId);
            UpdateDefinition<StudyCourse> add = Builders<StudyCourse>.Update.AddToSet(c => c.Members, student);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, add));
        }

        public async Task UpdateStudent(Guid courseId, CourseMember student)
        {
            student.LastUpdateDate = DateTime.Now;

            FilterDefinition<StudyCourse> filter = Builders<StudyCourse>.Filter.And(
                Builders<StudyCourse>.Filter.Eq(x => x.Id, courseId),
                Builders<StudyCourse>.Filter.ElemMatch(x => x.Members, x => x.UserId == student.Id));

            UpdateDefinition<StudyCourse> update = Builders<StudyCourse>.Update
                .Set(c => c.Members[-1].Accepted, student.Accepted)
                .Set(c => c.Members[-1].PlanId, student.PlanId)
                .Set(c => c.Members[-1].Passed, student.Passed)
                .Set(c => c.Members[-1].FinalScore, student.FinalScore);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, update));
        }

        public async Task RemoveStudent(Guid courseId, Guid userId)
        {
            FilterDefinition<StudyCourse> filter = Builders<StudyCourse>.Filter.Where(x => x.Id == courseId);
            UpdateDefinition<StudyCourse> remove = Builders<StudyCourse>.Update.PullFilter(c => c.Members, m => m.UserId == userId);

            await Context.AddCommand(() => DbSet.UpdateOneAsync(filter, remove));
        }
    }
}