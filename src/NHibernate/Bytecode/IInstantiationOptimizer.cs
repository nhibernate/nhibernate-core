using System;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Represents optimized entity instantiation.
	/// </summary>
	public interface IInstantiationOptimizer
	{
		/// <summary>
		/// Perform instantiation of an instance of the underlying class.
		/// </summary>
		/// <returns>The new instance.</returns>
		object CreateInstance();
	}
}