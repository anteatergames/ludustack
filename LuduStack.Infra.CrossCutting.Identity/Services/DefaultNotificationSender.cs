using LuduStack.Infra.CrossCutting.Abstractions;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Identity.Services
{
    public class DefaultNotificationSender : INotificationSender
    {
        public Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SendEmailAsync(string email, string templateId, object templateData)
        {
            return Task.FromResult(true);
        }

        public Task<bool> SendTeamNotificationAsync(string message)
        {
            return Task.FromResult(true);
        }
    }
}