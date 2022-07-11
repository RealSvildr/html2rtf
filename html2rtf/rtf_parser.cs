using System;
using System.Collections.Generic;
using System.Text;

namespace html2rtf
{
    public class RtfParser
    {
        private Rtf_Data rtf;
        public string Parse(Page page, Dictionary<string, string> args)
        {
            rtf = new Rtf_Data(args);

            var str_builder = new StringBuilder();
            str_builder.AppendLine(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033");
            str_builder.AppendLine(@"{\fonttbl");
            str_builder.AppendLine(@$"{{\f0\fswiss\fcharset0 {rtf.FontFamily}}}");
            str_builder.AppendLine(@$"{{\f1\fswiss\fcharset0 {rtf.FontFixed}}}");
            str_builder.AppendLine(@$"{{\f2\fswiss\fcharset0 {rtf.FontBarcode}}}");
            str_builder.AppendLine(@"{\f3\fnil\fcharset2 Symbol;}");
            str_builder.AppendLine("}");

            str_builder.AppendLine(@"{\info }"); // title/base/meta (different lines)

            str_builder.AppendLine(@"{\colortbl;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue0;\red255\green0\blue0;\red255\green255\blue255}");

            str_builder.AppendLine(@"{\stylesheet");
            str_builder.AppendLine(@"{{\*\cs1\additive\ul\cf2 Hyperlink;}}");
            str_builder.AppendLine(@"}");

            //str_builder.AppendLine(@"{\title }");
            //str_builder.AppendLine(@"{\keywords }");
            //str_builder.AppendLine(@"{\author }");
            //str_builder.AppendLine(@"{\title }");
            //str_builder.AppendLine(@"{\subject }");

            if (rtf.DocumentProtected)
                str_builder.AppendLine(@$"\formprot");

            str_builder.AppendLine(@$"\margt{rtf.MarginTop}\margb{rtf.MarginBottom}");
            str_builder.AppendLine(@$"\margl{rtf.MarginLeft}\margr{rtf.MarginRight}");
            str_builder.AppendLine($@"\paperw{rtf.PaperWidth}\paperh{rtf.PaperHeight}");
            str_builder.AppendLine(@"\pgnstart1");
            str_builder.AppendLine(@$"\headery{rtf.HeaderDistanceEdge}\footery{rtf.FooterDistanceEdge}");

            if (page.Header != null && page.Header.Count > 0)
                str_builder.AppendLine(@$"{{\header\pard {GetElements(page.Header, page.Style)}}}");

            if (page.Footer != null && page.Footer.Count > 0)
                str_builder.AppendLine(@$"{{\footer\pard {GetElements(page.Footer, page.Style)}}}");

            str_builder.AppendLine(@$"{{\fs{rtf.FontSize} {GetElements(page.Body, page.Style)}}}");
            str_builder.AppendLine(@"}");

            return str_builder.ToString();
        }

        private string GetElements(List<Element> elements, List<Style> styles)
        {
            var rtf_value = "";

            foreach (var el in elements)
            {
                var format = FormatElement(el);
                var str_styles = "";
                var value = "";

                if (el.Children != null && el.Children.Count > 0)
                {
                    value += GetElements(el.Children, null);
                }

                //el.Styles

                if (styles != null)
                {

                }

                if (el.Styles != null && el.Styles.Count > 0)
                {
                    str_styles += GetStyles(el.Styles);
                }

                if (!string.IsNullOrEmpty(el.Value))
                {
                    value += el.Value;
                }

                value = format.Replace("{STYLES}", str_styles)
                            .Replace("{PAGE}", value);

                rtf_value += value;
            }

            return rtf_value.Trim();
        }


        private string FormatElement(Element element)
        {
            switch (element.Type)
            {
                case ElementType.br:
                    return @"\line";
                case ElementType.hr:
                    return @"\pard\sa130\brdrb\brdrs\brdrw10\brsp20 {\fs4\~}\par\pard ";
                case ElementType.div:
                    return @$"{{\pard\sa60{{STYLES}} {{PAGE}}}}\par";
                case ElementType.span:
                    return @$"{{{{STYLES}} {{PAGE}}}}\par";
                case ElementType.h1:
                    return @$"{{\sb120\sa120\b\fs{Math.Round(rtf.FontSize * 2.0)}{{STYLES}} {{PAGE}}\par}}";
                case ElementType.h2:
                    return @$"{{\sb120\sa120\b\fs{Math.Round(rtf.FontSize * 1.5)}{{STYLES}} {{PAGE}}\par}}";
                case ElementType.h3:
                    return @$"{{\sb120\sa120\b\fs{Math.Round(rtf.FontSize * 1.1)}{{STYLES}} {{PAGE}}\par}}";
                case ElementType.h4:
                    return @$"{{\sb120\sa120\b\fs{Math.Round(rtf.FontSize * 1.0)}{{STYLES}} {{PAGE}}\par}}";
                case ElementType.h5:
                    return @$"{{\sb120\sa120\b\fs{Math.Round(rtf.FontSize * 0.8)}{{STYLES}} {{PAGE}}\par}}";
                case ElementType.h6:
                    return @$"{{\sb120\sa120\b\fs{Math.Round(rtf.FontSize * 0.6)}{{STYLES}} {{PAGE}}\par}}";
                case ElementType.b:
                    return $@"{{\b{{STYLE}} {{PAGE}}}}";
                case ElementType.i:
                    return $@"{{\i{{STYLE}} {{PAGE}}}}";
                case ElementType.sub:
                    return $@"{{\sub{{STYLE}} {{PAGE}}}}";
                case ElementType.sup:
                    return $@"{{\super{{STYLE}} {{PAGE}}}}";
                case ElementType.img:
                    return "TODO";
                default:
                    return $@"{{{{STYLE}} {{PAGE}}}}";
            }
        }

        private static string GetStyles(Dictionary<string, string> listCss)
        {
            var style = "";

            foreach (var css in listCss)
            {
                switch (css.Key)
                {
                    case "text-align":
                        if (css.Value == "center")
                            style += @"\qc";
                        else if (css.Value == "left")
                            style += @"\ql";
                        else if (css.Value == "right")
                            style += @"\qr";
                        else if (css.Value == "justify")
                            style += @"\qj";
                        break;
                    case "margin":
                        style += GetMargins(css.Value);
                        break;
                    case "margin-top":
                        style += @"\sb" + css.Value.GetMargin();
                        break;
                    case "margin-bottom":
                        style += @"\sa" + css.Value.GetMargin();
                        break;
                    case "margin-left":
                        style += @"\li" + css.Value.GetMargin();
                        break;
                    case "margin-right":
                        style += @"\ri" + css.Value.GetMargin();
                        break;
                    case "color":
                        style += @"\cf" + GetColor(css.Value);
                        break;
                    case "background-color":
                        style += @"\highlight" + GetColor(css.Value) + @"\cf" + GetColor(css.Value);
                        break;
                    case "text-decoration":
                        if (css.Value == "underline")
                            style += @"\ul";
                        else if (css.Value == "line-through")
                            style += @"\strike";
                        else if (css.Value == "none")
                            style += @"\ulnone";
                        break;

                    case "font-style":
                        if (css.Value == "italic")
                            style += @"\i";
                        break;
                    case "font-weight":
                        if (css.Value == "bold")
                            style += @"\b";
                        break;
                    case "font-size":
                        style += @"\fs" + css.Value.GetTwips();
                        break;
                    case "border-bottom":
                        if (css.Value.Contains("thin") || css.Value.Contains("solid"))
                        {
                            style += @"\ul";
                        }
                        else if (css.Value.Contains("thick"))
                        {
                            style += @"\ulth";
                        }
                        else if (css.Value.Contains("dotted"))
                        {
                            style += @"\uld";
                        }
                        else if (css.Value.Contains("dashed"))
                        {
                            style += @"\uldash";
                        }
                        break;
                    default:
                        break; // NOT IMPLEMENTED
                }
            }

            return style;
        }

        private static string GetColor(string css)
        {
            var color = System.Drawing.ColorTranslator.FromHtml(css);

            return @$";\red{color.R}\green{color.G}\blue{color.B}";
        }

        private static string GetMargins(string css)
        {
            var cssArray = css.Split(' ');
            var style = "";
            int top;
            int bottom;
            int left;
            int right;

            if (cssArray.Length == 1)
            {
                top = cssArray[0].GetMargin();
                bottom = cssArray[0].GetMargin();
                left = cssArray[0].GetMargin();
                right = cssArray[0].GetMargin();
            }
            else if (cssArray.Length == 2)
            {
                top = cssArray[0].GetMargin();
                bottom = cssArray[1].GetMargin();
                left = cssArray[0].GetMargin();
                right = cssArray[1].GetMargin();
            }
            else if (cssArray.Length == 3)
            {
                top = cssArray[0].GetMargin();
                bottom = cssArray[1].GetMargin();
                left = cssArray[2].GetMargin();
                right = cssArray[1].GetMargin();
            }
            else
            {
                top = cssArray[0].GetMargin();
                bottom = cssArray[1].GetMargin();
                left = cssArray[2].GetMargin();
                right = cssArray[3].GetMargin();
            }

            if (top > 0)
            {
                top += 60;
                style += @"\sb" + top;
            }
            if (bottom > 0)
            {
                bottom += 60;
                style += @"\sa" + bottom;
            }

            if (left > 0)
            {
                left += 60;
                style += @"\li" + left;
            }
            if (right > 0)
            {
                right += 60;
                style += @"\ri" + right;
            }

            return style;
        }
    }
    /*
      a,
        //Table
        table,
        thead,
        tbody,
        tfoot,
        tr,
        th,
        td,
        col,
        colgroup,

        ////// TODO
        article,
        li,
        ul,
        p,
        pre,
        em,
        u,
        q,
        sub,
        sup,
        video,
        audio,
        textarea,
        textbox,
        select,
        input,
        form,

        unknown,
     */


    internal class Rtf_Data
    {
        // CM
        public double PaperWidth { get; set; }
        public double PaperHeight { get; set; }
        public double MarginTop { get; set; }
        public double MarginBottom { get; set; }
        public double MarginLeft { get; set; }
        public double MarginRight { get; set; }

        // PX
        public double FontSize { get; set; }
        public string FontFamily { get; set; }
        public string FontFixed { get; set; }
        public string FontBarcode { get; set; }

        public double HeaderFontSize { get; set; }
        public double HeaderDistanceEdge { get; set; }
        public double FooterFontSize { get; set; }
        public double FooterDistanceEdge { get; set; }

        public bool DocumentProtected { get; set; }

        public Rtf_Data(Dictionary<string, string> args)
        {
            this.PaperWidth = 21;
            this.PaperHeight = 29.7;
            this.MarginTop = 2.54;
            this.MarginBottom = 2.54;
            this.MarginLeft = 2;
            this.MarginRight = 2;

            this.FontSize = 14;
            this.FontFamily = "Times New Roman";
            this.FontFixed = "Courier New";
            this.FontBarcode = "3 of 9 Barcode";

            this.HeaderFontSize = 14;
            this.HeaderDistanceEdge = 1.27;
            this.FooterFontSize = 14;
            this.FooterDistanceEdge = 1.27;
            this.DocumentProtected = false;

            if (args != null)
            {
                string tmp;
                if (args.TryGetValue("paper-width", out tmp))
                    this.PaperWidth = Convert.ToDouble(tmp);

                if (args.TryGetValue("paper-height", out tmp))
                    this.PaperHeight = Convert.ToDouble(tmp);

                if (args.TryGetValue("margin-top", out tmp))
                    this.MarginTop = Convert.ToDouble(tmp);

                if (args.TryGetValue("margin-bottom", out tmp))
                    this.MarginBottom = Convert.ToDouble(tmp);

                if (args.TryGetValue("margin-left", out tmp))
                    this.MarginLeft = Convert.ToDouble(tmp);

                if (args.TryGetValue("margin-right", out tmp))
                    this.MarginRight = Convert.ToDouble(tmp);

                if (args.TryGetValue("font-size", out tmp))
                    this.FontSize = Convert.ToDouble(tmp);

                if (args.TryGetValue("font-family", out tmp))
                    this.FontFamily = tmp;

                if (args.TryGetValue("font-fixed", out tmp))
                    this.FontFixed = tmp;

                if (args.TryGetValue("font-barcode", out tmp))
                    this.FontBarcode = tmp;

                if (args.TryGetValue("header-font-size", out tmp))
                    this.HeaderFontSize = Convert.ToDouble(tmp);

                if (args.TryGetValue("header-distance-edge", out tmp))
                    this.HeaderDistanceEdge = Convert.ToDouble(tmp);

                if (args.TryGetValue("footer-font-size", out tmp))
                    this.FooterFontSize = Convert.ToDouble(tmp);

                if (args.TryGetValue("footer-distance-edge", out tmp))
                    this.FooterDistanceEdge = Convert.ToDouble(tmp);

                if (args.TryGetValue("document-protected", out tmp))
                    this.DocumentProtected = Convert.ToBoolean(tmp);
            }

            this.SetTwips();
        }

        private void SetTwips()
        {
            this.PaperWidth = this.PaperWidth.GetTwips("cm");
            this.PaperHeight = this.PaperHeight.GetTwips("cm");
            this.MarginTop = this.MarginTop.GetTwips("cm");
            this.MarginBottom = this.MarginBottom.GetTwips("cm");
            this.MarginLeft = this.MarginLeft.GetTwips("cm");
            this.MarginRight = this.MarginRight.GetTwips("cm");

            this.HeaderDistanceEdge = this.HeaderDistanceEdge.GetTwips("cm");
            this.FooterDistanceEdge = this.FooterDistanceEdge.GetTwips("cm");

            this.FontSize = this.FontSize.GetTwips("px") / 10;
            this.HeaderFontSize = this.HeaderFontSize.GetTwips("px") / 10;
            this.FooterFontSize = this.FooterFontSize.GetTwips("px") / 10;
        }
    }
}
