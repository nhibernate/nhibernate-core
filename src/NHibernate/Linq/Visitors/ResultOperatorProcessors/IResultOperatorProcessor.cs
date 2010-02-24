namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
    public interface IResultOperatorProcessor<T>
    {
        ProcessResultOperatorReturn Process(T resultOperator, QueryModelVisitor queryModelVisitor);
    }
}