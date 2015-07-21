namespace NHibernate.AdoNet
{
	/// <summary>
	/// Provide the class of <see cref="IBatcherFactory"/> according to the configuration 
	/// and the capabilities of the driver.
	/// </summary>
	/// <remarks>
	/// By default, .Net doesn't have any batching capabilities, drivers that does have
	/// batching support.
	/// The BatcherFactory trough session-factory configuration section.
	/// This interface was added in NHibernate for backdraw compatibility to have the ability
	/// to specify a default <see cref="IBatcherFactory"/> for a specific <see cref="Driver.IDriver"/>.
	/// </remarks>
	public interface IEmbeddedBatcherFactoryProvider
	{
		System.Type BatcherFactoryClass { get;}
	}
}