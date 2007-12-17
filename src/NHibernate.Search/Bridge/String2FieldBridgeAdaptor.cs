using System;
using Lucene.Net.Documents;
using NHibernate.Util;

namespace NHibernate.Search.Bridge
{
    public class String2FieldBridgeAdaptor : IFieldBridge
    {
        private readonly IStringBridge stringBridge;

        public String2FieldBridgeAdaptor(IStringBridge stringBridge)
        {
            this.stringBridge = stringBridge;
        }

        public void Set(String name, Object value, Document document, Field.Store store, Field.Index index, float? boost)
        {
            String indexedString = stringBridge.ObjectToString(value);
            //Do not add fields on empty strings, seems a sensible default in most situations
            if (StringHelper.IsNotEmpty(indexedString))
            {
                Field field = new Field(name, indexedString, store, index);
                if (boost != null)
                    field.SetBoost(boost.Value);
                document.Add(field);
            }
        }
    }
}