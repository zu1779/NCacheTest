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

    using Alachisoft.NCache.Web.Caching;
    using Newtonsoft.Json;

    public class MvcApplication : HttpApplication
    {
        public const string AppEventKey = nameof(ApplicationEvent);
        private static readonly Cache cache;

        static MvcApplication()
        {
            cache = NCache.InitializeCache("AspNetDataCache");
        }
        
        private void addAppEvent(ApplicationEvent applicationEvent)
        {
            var list = cache.Get(AppEventKey) as List<ApplicationEvent> ?? new List<ApplicationEvent>();
            list.Add(applicationEvent);
            cache.Insert(AppEventKey, list);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string message = $"Started";
            var applicationEvent = new ApplicationEvent { Name = nameof(Application_Start), Description = message, TimeStamp = DateTime.Now };
            addAppEvent(applicationEvent);
        }

        public override void Init()
        {
            base.Init();

            AcquireRequestState += MvcApplication_AcquireRequestState;
        }

        private void MvcApplication_AcquireRequestState(object sender, EventArgs e)
        {
            string key = "Last_AcquireRequestState";
            if (Session[key] == null) Session.Add(key, DateTimeOffset.Now);
            else Session[key] = DateTimeOffset.Now;
        }

        protected void Application_End()
        {
            string message = $"Ended";
            var applicationEvent = new ApplicationEvent { Name = nameof(Application_End), Description = message, TimeStamp = DateTime.Now };
            addAppEvent(applicationEvent);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            string message = $"Started (SessionId: {Session.SessionID})";
            var applicationEvent = new ApplicationEvent { Name = nameof(Session_Start), Description = message, TimeStamp = DateTime.Now };
            addAppEvent(applicationEvent);
        }

        protected void Session_End()
        {
            string message = $"Ended (SessionId: {Session.SessionID})";
            var applicationEvent = new ApplicationEvent { Name = nameof(Session_End), Description = message, TimeStamp = DateTime.Now };
            addAppEvent(applicationEvent);
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
