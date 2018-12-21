using System;
using NHibernate.Properties;

namespace NHibernate.Bytecode.Lightweight
{
	/// <summary>
	/// Contains the <see cref="IGetter"/> instance with an optional optimized delegate.
	/// </summary>
	public class Getter
	{
		public Getter(IGetter @default) : this(@default, null)
		{
		}

		public Getter(IGetter @default, Func<object, object> optimized)
		{
			Default = @default;
			Optimized = optimized;
		}

		/// <summary>
		/// The default <see cref="IGetter"/> instance.
		/// </summary>
		public IGetter Default { get; }

		/// <summary>
		/// Optimized getter delegate.
		/// </summary>
		public Func<object, object> Optimized { get; }
	}
}
