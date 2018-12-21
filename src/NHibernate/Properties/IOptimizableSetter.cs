using System;
using System.Reflection.Emit;

namespace NHibernate.Properties
{
	/// <summary>
	/// An <see cref="ISetter" /> that can emit IL to set the property value.
	/// </summary>
	public interface IOptimizableSetter
	{
		/// <summary>
		/// When implemented by a class, gets the <see cref="System.Type"/> of the Property/Field.
		/// </summary>
		/// <value>The <see cref="System.Type"/> of the Property/Field.</value>
		System.Type Type { get; }

		/// <summary>
		/// Emit IL to set the property of an object to the value. The object
		/// is loaded onto the stack first, then the value, then this method
		/// is called.
		/// </summary>
		void Emit(ILGenerator il);
	}

	internal static class OptimizableSetterExtensions
	{
		/// <summary>
		/// Determine if the optimizable setter can actually be optimized.
		/// </summary>
		/// <returns><see langword="true" /> if can be optimized, <see langword="false" /> otherwise.</returns>
		//6.0 TODO: Merge into IOptimizableSetter.
		public static bool CanEmit(this IOptimizableSetter setter)
		{
			if (setter is FieldAccessor.FieldSetter fieldSetter)
			{
				return fieldSetter.CanEmit();
			}

			return true;
		}
	} 
}
