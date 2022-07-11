using System;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}


namespace html2rtf
{
    public static class Utils
    {
        public static int GetMargin(this string value) {
            return (GetTwips(value) / 10) + 60;
        }

        public static int GetTwips(this double value, string type)
        {
            return type switch
            {
                "px" => Convert.ToInt32(value * 15),
                "pt" => Convert.ToInt32(value * 20),
                "em" => Convert.ToInt32(value * 239.1018),
                "rem" => Convert.ToInt32(value * 239.9984),
                "cm" => Convert.ToInt32(value * 566.9219),
                _ => Convert.ToInt32(value),
            };
            //return type switch
            //{
            //    "px" => Convert.ToInt32(value * 1.499),
            //    "pt" => Convert.ToInt32(value * 1.99),
            //    "em" or "rem" => Convert.ToInt32(value * 23.9),
            //    "cm" => Convert.ToInt32(value * 53.6),
            //    _ => Convert.ToInt32(value),
            //};
        }

        public static int GetTwips(this string value)
        {
            value = value.Trim();
            int i = 0;

            for (i = value.Length - 1; i >= 0; i--)
                if (value[i].IsNumber()) break;

            var type = value[(i+1)..].Trim();
            value = value[0..(i+1)].Trim();
            return GetTwips(Convert.ToDouble(value), type);
        }

        public static bool IsNumber(this char c)
        {
            if (int.TryParse(c.ToString(), out int _))
                return true;

            return false;
        }
    }
}
