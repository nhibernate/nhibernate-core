namespace NHibernate.Mapping
{
	public interface IValueVisitor
	{
		object Accept(IValue visited);
	}

	public interface IValueVisitor<T> : IValueVisitor where T: IValue 
	{
		object Accept(T visited);
	}
}