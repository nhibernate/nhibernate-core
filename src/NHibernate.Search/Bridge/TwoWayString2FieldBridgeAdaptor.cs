using System;
using Lucene.Net.Documents;

namespace NHibernate.Search.Bridge
{
    public class TwoWayString2FieldBridgeAdaptor : String2FieldBridgeAdaptor, ITwoWayFieldBridge
    {
        private readonly ITwoWayStringBridge stringBridge;

        public TwoWayString2FieldBridgeAdaptor(ITwoWayStringBridge stringBridge) : base(stringBridge)
        {
            this.stringBridge = stringBridge;
        }

        public Object Get(String name, Document document)
        {
            Field field = document.GetField(name);
            if (field == null)
                return null;
            return stringBridge.StringToObject(field.StringValue());
        }

        public string ObjectToString(object obj)
        {
            return stringBridge.ObjectToString(obj);
        }
    }
}