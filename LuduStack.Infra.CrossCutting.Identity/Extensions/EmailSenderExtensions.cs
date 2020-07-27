using LuduStack.Infra.CrossCutting.Abstractions;
using System.Threading.Tasks;

namespace LuduStack.Infra.CrossCutting.Identity.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this INotificationSender notificationSender, string email, string link)
        {
            EmailSendRequest request = new EmailSendRequest
            {
                Subject = "LUDUSTACK - Email Verification",
                ActionUrl = link,
                ActionText = "Confirm your email",
                Greeting = "Hi there,",
                TextBeforeAction = "You just registered yourself at LUDUSTACK community. Now you need to confirm your email.",
                TextAfterAction = "After confirmation you will be able to enjoy interacting with other fellow game developers.",
                ByeText = "And welcome to YOUR community."
            };

            return notificationSender.SendEmailAsync(email, "d-e9b748bc725c48b18f00b3d1cc88c790", request);
        }

        public static Task SendEmailPasswordResetAsync(this INotificationSender notificationSender, string email, string link)
        {
            EmailSendRequest request = new EmailSendRequest
            {
                Subject = "LUDUSTACK - Password Reset",
                ActionUrl = link,
                ActionText = "Reset your password",
                TextBeforeAction = "You requested for a password reset. Click the button below and choose a new password.",
                TextAfterAction = "Do not share your password with anyone."
            };

            return notificationSender.SendEmailAsync(email, "d-e9b748bc725c48b18f00b3d1cc88c790", request);
        }

        public static Task SendEmailApplicationAsync(this INotificationSender notificationSender, string email, string emailApplicant, string link)
        {
            EmailSendRequest request = new EmailSendRequest
            {
                Subject = "LUDUSTACK - New Job Applicant",
                ActionUrl = link,
                ActionText = "Go to the job position",
                Greeting = "Hi there",
                TextBeforeAction = string.Format("We have great news! Recently you posted a job position on the LUDUSTACK Jobs and now someone applied to the job position you posted. The applicant's email is {0}", emailApplicant),
                TextAfterAction = "Log in to the LUDUSTACK platform to evaluate the applicants.",
                ByeText = "Thank you for helping the game development industry. We hope you find a good collaborator so we all can grow together."
            };

            return notificationSender.SendEmailAsync(email, "d-e9b748bc725c48b18f00b3d1cc88c790", request);
        }

        public static Task SendGiveawayEmailConfirmationAsync(this INotificationSender notificationSender, string email, string link, string giveawayName)
        {
            EmailSendRequest request = new EmailSendRequest
            {
                Subject = string.Format("{0} - Email Verification", giveawayName),
                ActionUrl = link,
                ActionText = "Confirm your email",
                Greeting = "Hi there,",
                TextBeforeAction = string.Format("You just entered a giveaway ({0}). Now you need to confirm your email.", giveawayName),
                TextAfterAction = "After confirmation you receive an extra entry.",
                ByeText = "See you there!."
            };

            return notificationSender.SendEmailAsync(email, "d-e9b748bc725c48b18f00b3d1cc88c790", request);
        }

        public class EmailSendRequest
        {
            public string Subject { get; set; }
            public string ActionUrl { get; set; }
            public string ActionText { get; set; }
            public string Greeting { get; set; }
            public string TextBeforeAction { get; set; }
            public string TextAfterAction { get; set; }
            public string ByeText { get; set; }
        }
    }
}