using System;
using Lucene.Net.Documents;
using NHibernate.Search.Bridge;

namespace NHibernate.Search.Bridge
{
	public interface ITwoWayFieldBridge : IFieldBridge
    {
        object Get(string value, Document document);
        string ObjectToString(object obj);
    }
}