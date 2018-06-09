namespace NCacheTest.AspNetMvc5.Models
{
    using System.Collections.Generic;

    public class HomeIndexViewModel
    {
        public Dictionary<string, string> CurrentSession { get; set; }
        public Dictionary<string, Dictionary<string, object>> AllSession { get; set; }
        public List<ApplicationEvent> EventSession { get; set; }
    }
}