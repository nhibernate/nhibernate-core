namespace NHibernate.Search.Bridge.Builtin
{
    public class StringBridge : SimpleBridge
    {
        public override object StringToObject(string stringValue)
        {
            return stringValue;
        }
    }
}