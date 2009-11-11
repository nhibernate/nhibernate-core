namespace NHibernate.Linq.Functions
{
    public interface IHqlGeneratorForType
    {
        void Register(FunctionRegistry functionRegistry);
    }
}