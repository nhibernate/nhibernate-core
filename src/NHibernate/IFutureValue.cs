namespace NHibernate
{
	public interface IFutureValue<T>
	{
		T Value { get; }
	}
}