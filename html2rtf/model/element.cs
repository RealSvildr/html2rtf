using System.Collections.Generic;

namespace html2rtf
{
    public class Element
    {
        public ElementType Type { get; set; }
        public string Value { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public Dictionary<string, string> Styles { get; set; }

        public List<Element> Children { get; set; }
    }
}