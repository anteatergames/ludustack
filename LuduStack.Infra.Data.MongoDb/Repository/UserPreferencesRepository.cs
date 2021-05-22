using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.Data.MongoDb.Interfaces;
using LuduStack.Infra.Data.MongoDb.Repository.Base;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Infra.Data.MongoDb.Repository
{
    public class UserPreferencesRepository : BaseRepository<UserPreferences>, IUserPreferencesRepository
    {
        public UserPreferencesRepository(IMongoContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SupportedLanguage>> GetUserLanguagesByUserId(Guid userId)
        {
            IEnumerable<SupportedLanguage> profile = await DbSet.Find(x => x.UserId == userId).Project(x => FormatList(x.ContentLanguages)).FirstOrDefaultAsync();

            return profile;
        }

        private IEnumerable<SupportedLanguage> FormatList(string contentLanguages)
        {
            string[] languages = (contentLanguages ?? string.Empty)
                .Split(new char[] { '|' });

            IEnumerable<SupportedLanguage> languagesConverted = languages.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => (SupportedLanguage)Enum.Parse(typeof(SupportedLanguage), x));

            return languagesConverted.ToList();
        }
    }
}