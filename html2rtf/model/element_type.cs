
namespace html2rtf
{
    // elementos de Teste
    public enum ElementType
    {
        a,
        h1, h2, h3,
        h4, h5, h6,
        hr,
        div,
        span,
        img,
        br,
        style,
        plain_text,

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
        b,
        i,
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
    }


    public static class Static_ElementType
    {
        public static ElementType GetElementType(string type)
        {
            try
            {
                return (ElementType)System.Enum.Parse(typeof(ElementType), type);
            }
            catch (System.Exception) {
                return ElementType.unknown;
            }
        }
    }
}