using System;
using NHibernate.Properties;

namespace NHibernate.Bytecode.Lightweight
{
	/// <summary>
	/// Factory that generate object based on IReflectionOptimizer needed to replace the use
	/// of reflection.
	/// </summary>
	/// <remarks>
	/// Used in <see cref="NHibernate.Persister.Entity.AbstractEntityPersister"/> and
	/// <see cref="NHibernate.Type.ComponentType"/>
	/// </remarks>
	public class BytecodeProviderImpl : AbstractBytecodeProvider
	{
        internal static IEntityInjector EntityInjector { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BytecodeProviderImpl()
        {
            if (EntityInjector == null) EntityInjector = new DefaultEntityInjector();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityInjector">Provide a IEntityInjector implementation to support dependency injection</param>
        public BytecodeProviderImpl(IEntityInjector entityInjector)
        {
            if (entityInjector == null) throw new ArgumentNullException("entityInjector");
            EntityInjector = entityInjector;
        }

		#region IBytecodeProvider Members

		/// <summary>
		/// Generate the IReflectionOptimizer object
		/// </summary>
		/// <param name="mappedClass">The target class</param>
		/// <param name="setters">Array of setters</param>
		/// <param name="getters">Array of getters</param>
		/// <returns><see langword="null" /> if the generation fails</returns>
		public override IReflectionOptimizer GetReflectionOptimizer(System.Type mappedClass, IGetter[] getters, ISetter[] setters)
		{
			return new ReflectionOptimizer(mappedClass, getters, setters);
		}

		#endregion
	}
}