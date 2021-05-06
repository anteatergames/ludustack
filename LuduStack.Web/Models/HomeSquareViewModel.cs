namespace LuduStack.Web.Models
{
    public class HomeSquareViewModel
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public object RouteParameters { get; set; }

        public string ColorClass { get; set; }

        public string Icon { get; set; }

        public string Text { get; set; }

        public HomeSquareViewModel(string controller, string action, string colorClass, string icon, string text)
        {
            Controller = controller;
            Action = action;
            RouteParameters = null;
            ColorClass = colorClass;
            Icon = icon;
            Text = text;
        }

        public HomeSquareViewModel(string controller, string action, object routeParameters, string colorClass, string icon, string text)
        {
            Controller = controller;
            Action = action;
            RouteParameters = routeParameters;
            ColorClass = colorClass;
            Icon = icon;
            Text = text;
        }
    }
}