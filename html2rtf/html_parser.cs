
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace html2rtf
{
    public class HtmlParser
    {
        private List<Style> _styles;

        public Page Parse(string header, string body, string footer)
        {
            _styles = new List<Style>();

            var _page = new Page()
            {
                Header = GetElements(header),
                Body = GetElements(body),
                Footer = GetElements(footer),
            };

            _page.Style = _styles;

            return _page;
        }

        private List<Element> GetElements(string html)
        {
            html = html.Replace("\t", "").Replace("\r\n", "");

            var ret = new List<Element>();
            var reg = new Regex(@"<(.*?)+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match;

            while ((match = reg.Match(html)).Success)
            {
                var element = GetElement(match, html, out string new_html);
                html = new_html;
                if (element != null)
                    ret.Add(element);
            }


            return ret;
        }

        private Element GetElement(Match match, string html, out string new_html)
        {
            var elTypes = new List<ElementType>() { ElementType.hr, ElementType.br }; // Sigle
            var type = Static_ElementType.GetElementType(match.Value.Trim('<', '>', '/').ToLower().Split(' ')[0]);

            if (match.Value.StartsWith("</"))
            {
                if (!elTypes.Contains(type))
                {
                    throw new Exception("Html incorrectly formatted");
                }
            }

            //var ret = new List<Element>();
            var reg = new Regex(@"<(?:""[^""]*""['""]*|'[^']*'['""]*|[^'"" >])+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var el = new Element();
            el.Children = new List<Element>();


            if (match.Success)
            {
                if (match.Index > 0)
                {
                    el.Children.Add(GetPlainText(html.Substring(0, match.Index)));
                }

                string innerHTML = html.Substring(match.Index + match.Value.Length); // html.Substring(reg.Index);
                var value = match.Value.Trim('<', '>').ToLower();
                var attr = value.Split(' ').ToList();

                if (attr.Count > 1)
                {
                    value = attr[0];
                    attr.RemoveAt(0);
                    attr = string.Join(' ', attr).Split('\'', '"').Where(e => !string.IsNullOrEmpty(e)).ToList();

                    el.Attributes = new Dictionary<string, string>();

                    for (var i = 0; i < attr.Count; i++)
                    {
                        var attrName = attr[i].ToLower().Trim();
                        var attrValue = "";

                        if (attrName.EndsWith('='))
                        {
                            attrName = attrName.TrimEnd('=');
                            i++;

                            if (attrName == "style")
                            {
                                el.Styles = GetCss(attr[i]);
                                continue;
                            }
                            else
                            {
                                attrValue = attr[i];
                            }
                        }
                        else
                        {
                            attrValue = attrName;
                        }


                        el.Attributes.Add(attrName, attrValue);
                    }
                }

                el.Type = type;

                if (elTypes.Contains(el.Type))
                {
                    innerHTML = "";
                    new_html = html.Substring(match.Index + match.Value.Length);
                }
                else
                {
                    var regEnd = new Regex(@"</" + value + ">", RegexOptions.IgnoreCase);
                    var end = regEnd.Match(innerHTML);

                    if (end.Success)
                    {
                        innerHTML = innerHTML.Substring(0, end.Index);
                        new_html = html.Substring(match.Index + match.Value.Length + end.Index + end.Value.Length);
                    }
                    else
                    {
                        throw new Exception("Html incorrectly formatted");
                    }
                }


                if (reg.Match(innerHTML).Success)
                {
                    el.Children = new List<Element>();
                    el.Children.AddRange(GetElements(innerHTML));
                }
                else
                {
                    if (el.Type == ElementType.style)
                    {
                        GetStyles(innerHTML);
                        return null;
                    }
                    else
                    {
                        el.Value = innerHTML;
                    }
                }
            }
            else
            {
                new_html = "";
                el.Type = ElementType.plain_text;
                el.Value = html;
            }

            if (el.Children.Count == 0)
                el.Children = null;

            return el;
        }

        private Element GetPlainText(string text)
        {
            var _el = new Element();
            _el.Type = ElementType.plain_text;
            _el.Value = text;
            return _el;
        }

        private void GetStyles(string styleHtml)
        {
            foreach (var style in styleHtml.Trim().Split("}"))
            {
                if (!string.IsNullOrEmpty(style))
                {
                    var tmp = style.Split('{');
                    var multiKey = tmp[0].Trim().Split(',');
                    var css = GetCss(tmp[1]);

                    foreach (var key in multiKey)
                    {
                        _styles.Add(new Style() { Element = key, Css = css });
                    }
                }
            }
        }

        private Dictionary<string, string> GetCss(string css)
        {
            var dic = new Dictionary<string, string>();
            foreach (var item in css.Trim().Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var tmp = item.Split(':');
                    dic.Add(tmp[0].Trim(), tmp[1].Trim());
                }
            }

            return dic;
        }
    }
}
