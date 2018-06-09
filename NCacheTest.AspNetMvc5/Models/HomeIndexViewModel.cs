namespace NCacheTest.AspNetMvc5.Models
{
    using System.Collections.Generic;

    public class HomeIndexViewModel
    {
        public Dictionary<string, string> CustomSession { get; set; }
        public Dictionary<string, object> DctNcache { get; set; }
        public List<ApplicationEvent> EventSession { get; set; }
    }
}