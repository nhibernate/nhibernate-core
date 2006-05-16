using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace NHibernate.Property
{
	/// <summary>
	/// An <see cref="IGetter" /> that can emit IL to get the property value.
	/// </summary>
	public interface IOptimizableGetter
	{
		/// <summary>
		/// Emit IL to get the property value from the object on top of the stack.
		/// </summary>
		void Emit( ILGenerator il );
	}
}
