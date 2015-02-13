using System;

namespace NHibernate.Util
{
    public class BacktickQuoteUtil
    {




        public static string ApplyDialectQuotes(string name, Dialect.Dialect dialect)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 3)
                return name;
            if (!name.StartsWith("`")) return name;

            var unquoted = name.Substring(1, name.Length - 2);

            return dialect.Quote(unquoted);
        }



        public static string QuoteWithBackticks(Dialect.Dialect dialect, string name)
        {
            if (String.IsNullOrEmpty(name) || StringHelper.IsBackticksEnclosed(name)) return name;
            
            bool dialectUsesBacktickQuotes = dialect.OpenQuote == '`' && dialect.CloseQuote == '`';
            

            if (dialect.IsQuoted(name))
            {
                if (dialectUsesBacktickQuotes)
                {
                    return name;
                }
                name = dialect.UnQuote(name);
            }

            return "`" + name + "`";
        }



    }
}