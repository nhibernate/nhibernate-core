using System;
using NHibernate.Property;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// A <see cref="IBytecodeProvider" /> implementation that returns
	/// <c>null</c>, disabling reflection optimization.
	/// </summary>
	public class NullBytecodeProvider : IBytecodeProvider
	{
		public IReflectionOptimizer GetReflectionOptimizer(System.Type clazz, IGetter[] getters, ISetter[] setters)
		{
			return null;
		}
	}
}