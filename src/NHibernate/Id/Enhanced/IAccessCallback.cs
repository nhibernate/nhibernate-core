namespace NHibernate.Id.Enhanced
{
	/// <summary>
	/// Contract for providing callback access to an <see cref="IDatabaseStructure"/>,
	/// typically from the <see cref="IOptimizer"/>.
	/// </summary>
	public interface IAccessCallback
	{
		/// <summary>
		/// Retrieve the next value from the underlying source.
		/// </summary>
		long GetNextValue();
	}
}