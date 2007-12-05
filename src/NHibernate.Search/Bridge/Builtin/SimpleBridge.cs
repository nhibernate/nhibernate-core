using System;
using System.Collections;
using Lucene.Net.Documents;
using NHibernate.Search.Attributes;
using NHibernate.Util;

namespace NHibernate.Search.Bridge.Builtin
{
    public abstract class SimpleBridge : ITwoWayStringBridge
    {
        public abstract object StringToObject(string stringValue);

        public string ObjectToString(object obj)
        {
            if(obj==null)
                return null;
            return obj.ToString();
        }
    }
}