using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Enums;
using LuduStack.Web.Exceptions;
using LuduStack.Web.Extensions;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    public class HomeController : SecureBaseController
    {
        private readonly IFeaturedContentAppService featuredContentAppService;
        private readonly IGameAppService gameAppService;

        public HomeController(IFeaturedContentAppService featuredContentAppService, IGameAppService gameAppService) 
        {
            this.featuredContentAppService = featuredContentAppService;
            this.gameAppService = gameAppService;
        }

        public async Task<IActionResult> Index()
        {
            await SetFeaturedCarousel();

            await SetDonateButton();

            SetLanguage();

            Dictionary<GameGenre, UiInfoAttribute> genreDict = Enum.GetValues(typeof(GameGenre)).Cast<GameGenre>().ToUiInfoDictionary(true);
            ViewData["Genres"] = genreDict;

            IEnumerable<SelectListItemVo> games = await gameAppService.GetByUser(CurrentUserId);
            List<SelectListItem> gamesDropDown = games.ToSelectList();
            ViewBag.UserGames = gamesDropDown;

            SetEmailConfirmed();

            ViewBag.BuildNumber = Environment.GetEnvironmentVariable("LUDUSTACK_BUILD_NUMBER") ?? "0.0.0";

            return View();
        }

        public IActionResult Articles()
        {
            return View();
        }

        [Route("/termsandconditions")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Counters()
        {
            return ViewComponent("Counters", new { qtd = 3 });
        }

        public IActionResult GameIdea()
        {
            return ViewComponent("GameIdea");
        }

        public IActionResult Notifications()
        {
            return ViewComponent("Notification", new { qtd = 5 });
        }

        [Route("/timeline")]
        public IActionResult Timeline()
        {
            TimeLineViewModel model = GenerateTimeline();

            return View(model);
        }

        [Route("/errortest")]
        public IActionResult ErrorTest()
        {
            throw new CustomApplicationException("meh");
        }

        private async Task SetFeaturedCarousel()
        {
            OperationResultVo<Application.ViewModels.PlatformSetting.PlatformSettingViewModel> showFeatureCarouselResult = await PlatformSettingAppService.GetByElement(CurrentUserId, PlatformSettingElement.ShowFeatureCarousel);
            if (showFeatureCarouselResult.Success)
            {
                bool showFeatureCarousel = showFeatureCarouselResult.Value.Value.Equals("1");
                ViewBag.ShowFeatureCarousel = showFeatureCarousel;

                if (showFeatureCarousel)
                {
                    CarouselViewModel featured = await featuredContentAppService.GetFeaturedNow();
                    ViewBag.Carousel = featured;
                }
            }
        }

        private async Task SetDonateButton()
        {
            OperationResultVo<Application.ViewModels.PlatformSetting.PlatformSettingViewModel> showDonateButtonResult = await PlatformSettingAppService.GetByElement(CurrentUserId, PlatformSettingElement.ShowDonateButton);
            if (showDonateButtonResult.Success)
            {
                bool showDonateButton = showDonateButtonResult.Value.Value.Equals("1");
                ViewBag.ShowDonateButton = showDonateButton;
            }
        }

        private void SetLanguage()
        {
            PostFromHomeViewModel postModel = new PostFromHomeViewModel();

            string postLanguageFromCookie = GetCookieValue(SessionValues.PostLanguage);
            if (postLanguageFromCookie != null)
            {
                List<SupportedLanguage> allLanguages = Enum.GetValues(typeof(SupportedLanguage)).Cast<SupportedLanguage>().ToList();
                SupportedLanguage langEnum = allLanguages.FirstOrDefault(x => x.ToUiInfo().Locale.Equals(postLanguageFromCookie));
                postModel.DefaultLanguage = langEnum;
            }
            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    SetCookieValue(SessionValues.PostLanguage, base.CurrentLocale, 7);
                }

                postModel.DefaultLanguage = base.SetLanguageFromCulture(base.CurrentLocale);
            }

            ViewBag.PostFromHome = postModel;
        }

        private static TimeLineItemViewModel GenerateTimeLineStart(DateTime date, string icon, string color, string title, string subtitle, string description)
        {
            return GenerateTimeLineItem(true, date, icon, color, title, subtitle, description);
        }

        private static TimeLineItemViewModel GenerateTimeLineItem(DateTime date, string icon, string color, string title, string subtitle, string description)
        {
            return GenerateTimeLineItem(false, date, icon, color, title, subtitle, description);
        }

        private static TimeLineItemViewModel GenerateTimeLineItem(bool start, DateTime date, string icon, string color, string title, string subtitle, string description)
        {
            return new TimeLineItemViewModel
            {
                Start = start,
                Date = date,
                Icon = icon,
                Color = color,
                Title = title,
                Subtitle = subtitle,
                Description = description
            };
        }

        private static TimeLineViewModel GenerateTimeline()
        {
            DateTime startDate = new DateTime(2018, 08, 27);
            TimeLineViewModel model = new TimeLineViewModel();

            model.Items.Add(GenerateTimeLineStart(startDate, "fas fa-asterisk", "success", "The Idea", startDate.ToShortDateString(), "This is where the whole idea began. We wrote a Google Document to sketch the idea and see the big picture forming."));

            model.Items.Add(GenerateTimeLineItem(new DateTime(2018, 09, 13), "fas fa-play", "success", "The First Commit", "Every journey starts with a first step.", "A simple README file added to start the repository."));

            TimeLineItemViewModel september2018 = GenerateTimeLineItem(new DateTime(2018, 09, 14), "fas fa-cloud", "warning", "September 2018", "A month full of tasks", "September 2018 was a busy month for us. We setup our CI/CD pipeline and also:");
            september2018.Items = new List<string> {
                "Archtecture defined",
                    "Menu defined",
                    "Basic Register and Login working",
                    "Profile page prototyped",
                    "Tag Cloud"
            };
            model.Items.Add(september2018);

            model.Items.Add(GenerateTimeLineItem(new DateTime(2018, 10, 01), "fas fa-shield-alt", "info", "October 2018", "Security first", "October was a month to work on the security system. Several improvements were made on the Register and Login workflows."));

            TimeLineItemViewModel november2018 = GenerateTimeLineItem(new DateTime(2018, 11, 01), "fas fa-cloud", "danger", "November 2018", "A really busy month!", "November was awesome for LUDUSTACK. We manage to implement several core systems that are used across the whole platform.");
            november2018.Items = new List<string> {
                    "Forgot password, password reset, email verification",
                    "Front page improvements",
                    "Facebook, Google and Microsoft Authentication",
                    "Game page prototyped",
                    "Terms of Use and Privacy Policy added",
                    "Image Upload",
                    "Use user image when registering with Facebook",
                    "AJAX front page",
                    "Latest games component",
                    "Counters component",
                    "Favicons and Manifest",
                    "Basic localization system implemented with Portuguese (by Daniel Gomes)  and Russian by Denis Mokhin)",
                    "Dynamic taglist on the front page",
                    "Basic staff roles defined",
                    "Translation project integration",
                    "OpenGraph implementated",
                    "Like/Unlike system",
                    "Comment system",
                    "Progressive Web App implemented"
            };
            model.Items.Add(november2018);

            TimeLineItemViewModel december2018 = GenerateTimeLineItem(new DateTime(2018, 12, 01), "fas fa-cloud", "success", "December 2018", "To close the year.", "In December we implemented a few things needed for the launch day.");
            december2018.Items = new List<string> {
                "Language selection",
                "Game activity feed",
                "Content view and edit",
                "Image cropping",
                "GDPR cookies warning",
                "Facebook sharing",
                "Structured data from schema.org"
            };
            model.Items.Add(december2018);

            TimeLineItemViewModel january2019 = GenerateTimeLineItem(new DateTime(2019, 01, 01), "fas fa-globe", "primary", "January 2019", "Happy new year! Let's work!", "We started the year by optimizing the whole platform.");
            january2019.Items = new List<string> {
                "Page speed improvements",
                "Search Engine Optimization (title, description, sitemap, etc",
                "Added Bosnian, Croatian and Serbian languages added by Kamal Tufekčić",
                "German language started by Thorben",
                "Featured article system implemented (staff only)",
                "Username validation",
                "Cache management"
            };
            model.Items.Add(january2019);

            TimeLineItemViewModel february2019 = GenerateTimeLineItem(new DateTime(2019, 02, 01), "fas fa-globe", "success", "February 2019", "Some tweaks", "LuduStack is for everyone!");
            february2019.Items = new List<string> {
                "Image size descriptions",
                "Accessibility improvements",
                "Images on CDN"
            };
            model.Items.Add(february2019);

            TimeLineItemViewModel march2019 = GenerateTimeLineItem(new DateTime(2019, 03, 01), "fas fa-vote-yea", "primary", "March 2019", "Democracy", "The voting system! Users can now:");
            march2019.Items = new List<string> {
                "Suggest ideas",
                "Vote on other people's ideas",
                "Comment on ideas",
                "Create your own brainstorm sessions"
            };
            model.Items.Add(march2019);

            TimeLineItemViewModel april2019 = GenerateTimeLineItem(new DateTime(2019, 04, 01), "fas fa-comment", "primary", "April 2019", "Fast posting", "Like a good social network you can now:");
            april2019.Items = new List<string> {
                "Post directly from the front page",
                "Add a image to post",
                "Post from within your game",
                "Game like"
            };
            model.Items.Add(april2019);

            TimeLineItemViewModel may2019 = GenerateTimeLineItem(new DateTime(2019, 05, 01), "fas fa-trophy", "success", "May 2019", "Game On!", "Climb the LuduStack Ranks to the glory");
            may2019.Items = new List<string> {
                "Ranking System",
                "Experience Points",
                "Badges"
            };
            model.Items.Add(may2019);

            TimeLineItemViewModel june2019 = GenerateTimeLineItem(new DateTime(2019, 06, 01), "fas fa-connectdevelop", "info", "June 2019", "Social Interaction", "Follow games and users and connect to users to increase your social network!");
            june2019.Items = new List<string> {
                "Notifications!",
                "Game Follow",
                "User Follow",
                "User Connection System"
            };
            model.Items.Add(june2019);

            TimeLineItemViewModel july2019 = GenerateTimeLineItem(new DateTime(2019, 07, 01), "fas fa-poll", "success", "July 2019", "This or that?", "Polls, preferences and more!");
            july2019.Items = new List<string> {
                "Basic Polls - Get opinions from your fellow devs!",
                "Better preferences - Now you can change your email, set your phone and more!",
                "Two Factor Authentication - Be more safe with this security feature",
                "Content post language selection",
                "Ranking Page",
                "Post new suggestions right from the sidebar",
                "Basic search results",
                "QR code generation"
            };
            model.Items.Add(july2019);

            model.Items.Add(GenerateTimeLineItem(new DateTime(2019, 08, 01), "fas fa-question", "primary", "August 2019", "...", "Nothing to see here, keep scrolling!"));

            TimeLineItemViewModel september2019 = GenerateTimeLineItem(new DateTime(2019, 09, 01), "fas fa-trash-alt", "danger", "September 2019", "You got the power!", "Must have features!");
            september2019.Items = new List<string> {
                "You can now delete your own posts",
                "Share your game!",
                "Rank Levels page"
            };
            model.Items.Add(september2019);

            TimeLineItemViewModel october2019 = GenerateTimeLineItem(new DateTime(2019, 10, 01), "fas fa-users", "info", "October 2019", "Team up!", "Join forces to make games.");
            october2019.Items = new List<string> {
                "Team Management",
                "Points Earned notification",
                "Brainstorm Ideas status control",
                "External Links working on profiles and games",
                "Teams can be linked to games",
                "Recruitin Teams!",
                "#hashtagging"
            };
            model.Items.Add(october2019);

            model.Items.Add(GenerateTimeLineItem(new DateTime(2019, 12, 01), "fas fa-bolt", "danger", "December 2019", "Optimizations!", "Now the whole web rendering is blazing fast!"));

            TimeLineItemViewModel january2020 = GenerateTimeLineItem(new DateTime(2020, 01, 01), "fas fa-briefcase", "warning", "January 2020", "It is time to work! Seriously!", "The job management is here!");
            january2020.Items = new List<string> {
                "Create job position",
                "List existing positions",
                "Apply to positions",
                "See who applied to your posted job positions"
            };

            TimeLineItemViewModel march2020 = GenerateTimeLineItem(new DateTime(2020, 03, 01), "fas fa-heart", "danger", "March 2020", "Thank you all!", "We reached the mark of 300 users and  100 games. Thank you all for your love!");
            march2020.Items = new List<string> {
                "A Special Thanks page"
            };
            model.Items.Add(march2020);

            TimeLineItemViewModel april2020 = GenerateTimeLineItem(new DateTime(2020, 04, 01), "fas fa-language", "primary", "April 2020", "Localize your games!", "The localization tool has arrived!");
            april2020.Items = new List<string> {
                "Ask for translation from the community",
                "Help others, translating terms to your own language",
                "Import and export files",
                "Review translations",
                "Post game content directly from home",
                "Set Game Characteristics"
            };
            model.Items.Add(april2020);

            model.Items.Add(GenerateTimeLineItem(new DateTime(2020, 06, 01), "fas fa-rocket", "secondary", "June 2020", "New name!", "Now we are called LUDUSTACK! The one stop place for game developers."));

            TimeLineItemViewModel july2020 = GenerateTimeLineItem(new DateTime(2020, 07, 01), "fas fa-fw fa-ticket-alt", "success", "July 2020", "It is time for sweepstakes!", "You can now run your own giveaways!");
            july2020.Items = new List<string> {
                "GDPR compliant",
                "Email confirmation",
                "Tracked link sharing for more chances to win",
                "Daily entries"
            };
            model.Items.Add(july2020);

            model.Items.Add(GenerateTimeLineItem(new DateTime(2020, 08, 01), "fas fa-dice", "primary", "August 2020", "New Ideas!", "Random Game Idea added to the front page to overcome your creative block!"));

            model.Items.Add(GenerateTimeLineItem(new DateTime(2020, 09, 01), "fas fa-dollar-sign", "success", "September 2020", "Calculations!", "The Rate Calculator will help you charge correctly for jobs with your skillset!"));
            TimeLineItemViewModel march2021 = GenerateTimeLineItem(new DateTime(2021, 03, 01), "fas fa-cogs", "info", "March 2021", "Back to basics", "We decided to take a step back and redesign some architectural aspects of our software.");
            march2021.Items = new List<string> {
                "CQRS migration started"
            };
            model.Items.Add(march2021);

            TimeLineItemViewModel april2021 = GenerateTimeLineItem(new DateTime(2021, 04, 01), "fas fa-calculator", "success", "April 2021", "How much?", "Now that most of the platform migration to CQRS is done, we are releasing a new tool and several improvements to the platform.");
            april2021.Items = new List<string> {
                "Cost Calculator",
                "Bill Rate management",
                "Users handlers",
                "Speed improvement on several pages"
            };
            model.Items.Add(april2021);

            TimeLineItemViewModel june2021 = GenerateTimeLineItem(new DateTime(2021, 06, 01), "fas fa-comments", "info", "June 2021", "A must have!", "We finally have a forum system!");
            june2021.Items = new List<string> {
                "Organized in groups, categories and topics",
                "Forum Icons and Open Graph Images",
                "Rich Text posts",
                "Multi-language",
                "Voting System"
            };
            model.Items.Add(june2021);

            TimeLineItemViewModel july2021 = GenerateTimeLineItem(new DateTime(2021, 07, 01), "fas fa-shapes", "danger", "July 2021", "C'mon and slam!", "And welcome to the Jam!");
            july2021.Items = new List<string> {
                "Game Jam system with many options",
                "With or without Judges",
                "Community vote"
            };
            model.Items.Add(july2021);

            // Future

            model.Items.Add(GenerateTimeLineItem(new DateTime(2098, 11, 01), "fas fa-bug", "danger", "Soon", "Open Beta", "At this point, we hope to have a consistent beta tester base so we can polish the platform and fix every possible bug that shows up."));

            model.Items.Add(GenerateTimeLineItem(new DateTime(2099, 01, 01), "fas fa-star", "success", "Near Future", "Launch day!", "This is the launch day. On this day, all the core features will be implemented."));

            model.Items = model.Items.OrderBy(x => x.Date).ToList();
            return model;
        }
    }
}