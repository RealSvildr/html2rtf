using System.Collections.Generic;

namespace html2rtf
{
    public class Page
    {
        public List<Style> Style { get; set; }
        public List<Element> Header { get; set; }
        public List<Element> Footer { get; set; }
        public List<Element> Body { get; set; }
    }
}