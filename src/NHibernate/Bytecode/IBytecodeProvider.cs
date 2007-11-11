using NHibernate.Property;

namespace NHibernate.Bytecode
{
	public interface IBytecodeProvider
	{
		/// <summary> 
		/// The specific factory for this provider capable of
		/// generating run-time proxies for lazy-loading purposes.
		///  </summary>
		IProxyFactoryFactory ProxyFactoryFactory { get; set;}
		// NH specific: we add the set method because NH-975; the responsability of
		// ProxyFactory is of BytecodeProvider like H3.2

		/// <summary>
		/// Retrieve the <see cref="IReflectionOptimizer" /> delegate for this provider
		/// capable of generating reflection optimization components.
		/// </summary>
		/// <param name="clazz">The class to be reflected upon.</param>
		/// <param name="getters">All property getters to be accessed via reflection.</param>
		/// <param name="setters">All property setters to be accessed via reflection.</param>
		/// <returns>The reflection optimization delegate.</returns>
		IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters);

		/// <summary> Generate a ClassTransformer capable of performing bytecode manipulation. </summary>
		/// <param name="classFilter">
		/// filter used to limit which classes are to be instrumented via this ClassTransformer.
		/// </param>
		/// <param name="fieldFilter">
		/// filter used to limit which fields are to be instrumented
		/// via this ClassTransformer.
		/// </param>
		/// <returns> The appropriate ClassTransformer. </returns>
		// Not ported
		//ClassTransformer getTransformer(ClassFilter classFilter, FieldFilter fieldFilter);

	}
}