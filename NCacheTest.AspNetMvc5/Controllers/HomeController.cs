namespace NCacheTest.AspNetMvc5.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.SessionState;

    using Alachisoft.NCache.Caching;
    using Alachisoft.NCache.Runtime.Caching;
    using Alachisoft.NCache.Web.Caching;

    using NCacheTest.AspNetMvc5.Models;

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
            const byte SESSION_ITEMS = 1;
            const byte SESSION_STATIC_ITEMS = 2;

            var cache = NCache.Caches["AspNetCache"];
            var ht = cache.GetByTag(new Tag("NC_ASP.net_session_data"));
            var dctNcache = ht.Cast<DictionaryEntry>().ToDictionary(c => (string)c.Key, c => c.Value);

            byte[] array = (byte[])dctNcache.First().Value;
            using (var ms = new MemoryStream(array))
            {
                var bf = new BinaryFormatter();
                var obj = bf.Deserialize(ms) as Hashtable;
                Debug.WriteLine($"{nameof(obj)} is {obj.GetType()}");

                if (obj["SD"] is byte[] sd)
                {
                    SessionStateItemCollection itemCollection = null;
                    HttpStaticObjectsCollection staticItemCollection = null;
                    int timeout = 0;
                    using (var ms1 = new MemoryStream(sd))
                    {
                        BinaryReader reader = new BinaryReader(ms1);
                        byte sessionFlag = reader.ReadByte();
                        if ((byte)(sessionFlag & SESSION_ITEMS) == SESSION_ITEMS)
                        {
                            itemCollection = SessionStateItemCollection.Deserialize(reader);
                        }
                        if ((byte)(sessionFlag & SESSION_STATIC_ITEMS) == SESSION_STATIC_ITEMS)
                        {
                            staticItemCollection = HttpStaticObjectsCollection.Deserialize(reader);
                        }
                        timeout = reader.ReadInt32();
                    }
                    var sssd = new SessionStateStoreData(itemCollection, staticItemCollection, timeout);
                }
            }

            var mcs = Session.Keys.Cast<string>().Where(c => c.StartsWith(csp))
                .ToDictionary(c => c.Substring(csp.Length), c => $"{Session[c]}");
            var mes = new List<ApplicationEvent>();
            mes.AddRange(Session[nameof(ApplicationEvent)] as List<ApplicationEvent> ?? new List<ApplicationEvent>());
            mes.AddRange(MvcApplication.AppStartEvents);
            mes.AddRange(MvcApplication.AppEndEvents);

            var viewModel = new HomeIndexViewModel { CustomSession = mcs, DctNcache = dctNcache, EventSession = mes };
            return View(viewModel);
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