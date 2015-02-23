using NHibernate.Properties;

namespace NHibernate.Bytecode
{
	public interface IBytecodeProvider
	{
		/// <summary> 
		/// The specific factory for this provider capable of
		/// generating run-time proxies for lazy-loading purposes.
		///  </summary>
		IProxyFactoryFactory ProxyFactoryFactory { get;}

		/// <summary>
		/// Retrieve the <see cref="IReflectionOptimizer" /> delegate for this provider
		/// capable of generating reflection optimization components.
		/// </summary>
		/// <param name="clazz">The class to be reflected upon.</param>
		/// <param name="getters">All property getters to be accessed via reflection.</param>
		/// <param name="setters">All property setters to be accessed via reflection.</param>
		/// <returns>The reflection optimization delegate.</returns>
		IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters);

		/// <summary>
		/// NHibernate's object instantiator.
		/// </summary>
		/// <remarks>
		/// For entities <see cref="IReflectionOptimizer"/> and its implementations.
		/// </remarks>
		IObjectsFactory ObjectsFactory { get; }

		/// <summary>
		/// Instantiator of NHibernate's collections default types.
		/// </summary>
		ICollectionTypeFactory CollectionTypeFactory { get; }

		// <summary> Generate a ClassTransformer capable of performing bytecode manipulation. </summary>
		// <param name="classFilter">
		// filter used to limit which classes are to be instrumented via this ClassTransformer.
		// </param>
		// <param name="fieldFilter">
		// filter used to limit which fields are to be instrumented
		// via this ClassTransformer.
		// </param>
		// <returns> The appropriate ClassTransformer. </returns>
		// Not ported
		//ClassTransformer getTransformer(ClassFilter classFilter, FieldFilter fieldFilter);
	}
}