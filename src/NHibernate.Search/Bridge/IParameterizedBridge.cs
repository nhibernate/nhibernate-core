namespace NHibernate.Search.Bridge
{
    public interface IParameterizedBridge
    {
        void SetParameterValues(object[] parameters);
    }
}