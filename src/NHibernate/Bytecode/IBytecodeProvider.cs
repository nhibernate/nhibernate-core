using System;

using NHibernate.Proxy;
using NHibernate.Property;

namespace NHibernate.Bytecode
{
	public interface IBytecodeProvider
	{
		// Not ported: getProxyFactoryFactory() - there aren't any alternative proxy factory
		// implementations yet

		/// <summary>
		/// Retrieve the <see cref="IReflectionOptimizer" /> delegate for this provider
		/// capable of generating reflection optimization components.
		/// </summary>
		/// <param name="clazz">The class to be reflected upon.</param>
		/// <param name="getters">All property getters to be accessed via reflection.</param>
		/// <param name="setters">All property setters to be accessed via reflection.</param>
		/// <returns>The reflection optimization delegate.</returns>
		IReflectionOptimizer GetReflectionOptimizer( System.Type clazz, IGetter[] getters, ISetter[] setters );
	}
}
