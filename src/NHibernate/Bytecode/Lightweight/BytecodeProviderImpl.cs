#if NET_2_0
using System;

using log4net;

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
		private static readonly ILog log = LogManager.GetLogger( typeof( BytecodeProviderImpl ) );

		/// <summary>
		/// Private ctor. Can't create an empty object
		/// </summary>
		public BytecodeProviderImpl()
		{
		}

		/// <summary>
		/// Generate the IReflectionOptimizer object
		/// </summary>
		/// <param name="mappedClass">The target class</param>
		/// <param name="setters">Array of setters</param>
		/// <param name="getters">Array of getters</param>
		/// <returns><c>null</c> if the generation fails</returns>
		public IReflectionOptimizer GetReflectionOptimizer(
			System.Type mappedClass, IGetter[] getters, ISetter[] setters )
		{
			return new ReflectionOptimizer( mappedClass, getters, setters );
		}
	}
}
#endif