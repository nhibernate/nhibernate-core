using System;
using NHibernate.Properties;

namespace NHibernate.Bytecode.Lightweight
{
	/// <summary>
	/// Contains the <see cref="ISetter"/> instance with an optional optimized delegate.
	/// </summary>
	public class Setter
	{
		public Setter(ISetter @default) : this(@default, null)
		{
		}

		public Setter(ISetter @default, Action<object, object> optimized)
		{
			Default = @default;
			Optimized = optimized;
		}

		/// <summary>
		/// The default <see cref="ISetter"/> instance.
		/// </summary>
		public ISetter Default { get; }

		/// <summary>
		/// Optimized setter delegate.
		/// </summary>
		public Action<object, object> Optimized { get; }
	}
}
