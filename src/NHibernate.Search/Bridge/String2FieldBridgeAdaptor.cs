using System;
using Lucene.Net.Documents;
using NHibernate.Search.Bridge;
using NHibernate.Util;

namespace NHibernate.Search.Bridge
{
#if NET_2_0
#else
	[CLSCompliant(false)]
#endif
    public class String2FieldBridgeAdaptor : IFieldBridge
    {
        private readonly IStringBridge stringBridge;

        public String2FieldBridgeAdaptor(IStringBridge stringBridge)
        {
            this.stringBridge = stringBridge;
        }

#if NET_2_0
		public void Set(String name, Object value, Document document, Field.Store store, Field.Index index, float? boost)
#else
        public void Set(String name, Object value, Document document, Field.Store store, Field.Index index, float boost)
#endif
        {
            String indexedString = stringBridge.ObjectToString(value);
            //Do not add fields on empty strings, seems a sensible default in most situations
            if (StringHelper.IsNotEmpty(indexedString))
            {
                Field field = new Field(name, indexedString, store, index);
#if NET_2_0
                if (boost != null)
                    field.SetBoost(boost.Value);
#else
				if (boost != 0F)
                    field.SetBoost(boost);
#endif
                    document.Add(field);
            }
        }
    }
}