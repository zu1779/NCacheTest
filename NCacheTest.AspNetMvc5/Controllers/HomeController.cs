namespace NCacheTest.AspNetMvc5.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public const string csp = MvcApplication.CustomSessionPrefix;

        [HttpPost]
        public ActionResult Restart()
        {
            HttpRuntime.UnloadAppDomain();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult EndSession()
        {
            Session.Abandon();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Index()
        {
            var modelCustomSession = Session.Keys.Cast<string>().Where(c => c.StartsWith(csp))
                .ToDictionary(c => c.Substring(csp.Length), c => $"{Session[c]}");
            var modelEventSession = new List<ApplicationEvent>();
            modelEventSession.AddRange(Session[nameof(ApplicationEvent)] as List<ApplicationEvent> ?? new List<ApplicationEvent>());
            modelEventSession.AddRange(MvcApplication.AppStartEvents);
            return View((modelCustomSession, modelEventSession));
        }
        [HttpPost]
        public ActionResult Index(string key, string value)
        {
            Session.Add(MvcApplication.CustomSessionPrefix + key, value);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public ActionResult Delete(string key)
        {
            Session.Remove(MvcApplication.CustomSessionPrefix + key);
            return RedirectToAction(nameof(Index));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}