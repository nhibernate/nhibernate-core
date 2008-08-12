using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Linq.Util
{
    /// <remarks>
	/// Copied from internal class of same name in System.Core assembly.
    /// </remarks>
	[DebuggerStepThrough, DebuggerNonUserCode]
    public static class TypeSystem
    {
        public static System.Type GetElementType(System.Type seqType)
        {
            System.Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static System.Type FindIEnumerable(System.Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                foreach (System.Type arg in seqType.GetGenericArguments())
                {
                    System.Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            System.Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (System.Type iface in ifaces)
                {
                    System.Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }
}
