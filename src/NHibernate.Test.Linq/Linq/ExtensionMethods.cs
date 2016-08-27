using NHibernate.Linq;

namespace NHibernate.Test.Linq
{
    static class ExtensionMethods
    {
        [LinqExtensionMethod("Replace")]
        public static string ReplaceExtension(this string subject, string search, string replaceWith)
        {
            return null;
        }

        [LinqExtensionMethod]
        public static string Replace(this string subject, string search, string replaceWith)
        {
            return null;
        }
    }
}
