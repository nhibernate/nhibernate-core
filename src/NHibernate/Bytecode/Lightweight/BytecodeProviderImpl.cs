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
			return new ReflectionOptimizer(mappedClass, getters, setters, null, null);
		}

		#endregion

		/// <summary>
		/// Retrieve the <see cref="IReflectionOptimizer" /> delegate for this provider
		/// capable of generating reflection optimization components.
		/// </summary>
		/// <param name="mappedClass">The class to be reflected upon.</param>
		/// <param name="getters">All property getters to be accessed via reflection.</param>
		/// <param name="setters">All property setters to be accessed via reflection.</param>
		/// <param name="specializedGetter">The specialized getter for the given type.</param>
		/// <param name="specializedSetter">The specialized setter for the given type.</param>
		/// <returns>The reflection optimization delegate.</returns>
		internal IReflectionOptimizer GetReflectionOptimizer(
			System.Type mappedClass, IGetter[] getters, ISetter[] setters, IGetter specializedGetter, ISetter specializedSetter)
		{
			return new ReflectionOptimizer(mappedClass, getters, setters, specializedGetter, specializedSetter);
		}
	}
}
