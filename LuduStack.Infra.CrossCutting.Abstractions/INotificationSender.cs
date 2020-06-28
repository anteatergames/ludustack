using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Abstractions
{
    public interface INotificationSender
    {
        Task<bool> SendEmailAsync(string email, string subject, string message);

        Task<bool> SendEmailAsync(string email, string templateId, object templateData);

        Task<bool> SendTeamNotificationAsync(string message);
    }
}