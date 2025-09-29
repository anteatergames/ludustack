using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Helper;
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
            UserPreferences userPrefs = await DbSet.Find(x => x.UserId == userId).FirstOrDefaultAsync();
            
            if (userPrefs == null)
                return Enumerable.Empty<SupportedLanguage>();

            return LanguageDomainHelper.FormatList(userPrefs.ContentLanguages);
        }
    }
}