using System;
using NHibernate.Persister.Entity;
using NHibernate.Property;

namespace NHibernate.Bytecode.Lightweight
{
	/// <summary>
	/// Factory that generate object based on IReflectionOptimizer needed to replace the use
	/// of reflection.
	/// </summary>
	/// <remarks>
	/// Used in <see cref="AbstractEntityPersister"/> and
	/// <see cref="NHibernate.Type.ComponentType"/>
	/// </remarks>
	public class BytecodeProviderImpl : IBytecodeProvider
	{
		#region IBytecodeProvider Members

		public virtual IProxyFactoryFactory ProxyFactoryFactory
		{
			get { return new DefaultProxyFactoryFactory(); }
			set { throw new NotSupportedException(); }
		}

		#endregion

		/// <summary>
		/// Generate the IReflectionOptimizer object
		/// </summary>
		/// <param name="mappedClass">The target class</param>
		/// <param name="setters">Array of setters</param>
		/// <param name="getters">Array of getters</param>
		/// <returns><see langword="null" /> if the generation fails</returns>
		public IReflectionOptimizer GetReflectionOptimizer(
			System.Type mappedClass, IGetter[] getters, ISetter[] setters)
		{
			return new ReflectionOptimizer(mappedClass, getters, setters);
		}
	}
}

