namespace NCacheTest.AspNetMvc5
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Newtonsoft.Json;

    public class MvcApplication : HttpApplication
    {
        public const string CustomSessionPrefix = "csp-";
        public static readonly ConcurrentBag<ApplicationEvent> AppStartEvents = new ConcurrentBag<ApplicationEvent>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string message = $"Started";
            AppStartEvents.Add(new ApplicationEvent { Name = nameof(Application_Start), Description = message, TimeStamp = DateTime.Now });
        }

        protected void Application_End()
        {
            string message = $"Ended (SessionId: {Session.SessionID})";
            var list = Session[nameof(ApplicationEvent)] as List<ApplicationEvent> ?? new List<ApplicationEvent>();
            list.Add(new ApplicationEvent { Name = nameof(Application_End), Description = message, TimeStamp = DateTime.Now });
            Session[nameof(ApplicationEvent)] = list;
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            string message = $"Started (SessionId: {Session.SessionID})";
            var list = Session[nameof(ApplicationEvent)] as List<ApplicationEvent> ?? new List<ApplicationEvent>();
            list.Add(new ApplicationEvent { Name = nameof(Session_Start), Description = message, TimeStamp = DateTime.Now });
            Session[nameof(ApplicationEvent)] = list;
        }

        protected void Session_End()
        {
            string message = $"Ended (SessionId: {Session.SessionID})";
            var list = Session[nameof(ApplicationEvent)] as List<ApplicationEvent> ?? new List<ApplicationEvent>();
            list.Add(new ApplicationEvent { Name = nameof(Session_End), Description = message, TimeStamp = DateTime.Now });
            Session[nameof(ApplicationEvent)] = list;
        }
    }

    [Serializable]
    public class ApplicationEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime TimeStamp { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
