using System;
using System.Reflection.Emit;

namespace NHibernate.Properties
{
	/// <summary>
	/// An <see cref="IGetter" /> that can emit IL to get the property value.
	/// </summary>
	public interface IOptimizableGetter
	{
		/// <summary>
		/// Emit IL to get the property value from the object on top of the stack.
		/// </summary>
		void Emit(ILGenerator il);
	}
}