using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// A <see cref="IBytecodeProvider" /> implementation that returns
	/// <c>null</c>, disabling reflection optimization.
	/// </summary>
	public class NullBytecodeProvider : IBytecodeProvider
	{
		public IReflectionOptimizer GetReflectionOptimizer( System.Type clazz, NHibernate.Property.IGetter[] getters, NHibernate.Property.ISetter[] setters )
		{
			return null;
		}
	}
}
