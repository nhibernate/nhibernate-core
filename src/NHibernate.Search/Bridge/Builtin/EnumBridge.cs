using System;

namespace NHibernate.Search.Bridge.Builtin
{
    public class EnumBridge : SimpleBridge
    {
        private readonly System.Type type;

        public EnumBridge(System.Type type)
        {
            this.type = type;
        }

        public override object StringToObject(string stringValue)
        {
            return Enum.Parse(type, stringValue);
        }
    }
}