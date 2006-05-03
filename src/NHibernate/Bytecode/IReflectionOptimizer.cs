using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Represents reflection optimization for a particular class.
	/// </summary>
	public interface IReflectionOptimizer
	{
		IAccessOptimizer AccessOptimizer { get; }
		IInstantiationOptimizer InstantiationOptimizer { get; }
	}
}
