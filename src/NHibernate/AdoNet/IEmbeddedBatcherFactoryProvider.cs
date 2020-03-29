namespace NHibernate.AdoNet
{
	/// <summary>
	/// Provides a default <see cref="IBatcherFactory"/> class.
	/// </summary>
	/// <remarks>
	/// This interface allows to specify a default <see cref="IBatcherFactory"/> for a specific
	/// <see cref="Driver.IDriver"/>. The configuration setting <see cref="NHibernate.Cfg.Environment.BatchStrategy"/>
	/// takes precedence over <c>BatcherFactoryClass</c>.
	/// </remarks>
	public interface IEmbeddedBatcherFactoryProvider
	{
		/// <summary>
		/// The <see cref="IBatcherFactory"/> class type.
		/// </summary>
		System.Type BatcherFactoryClass { get; }
	}
}
