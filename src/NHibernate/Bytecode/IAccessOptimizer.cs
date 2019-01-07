using System;
using NHibernate.Bytecode.Lightweight;

namespace NHibernate.Bytecode
{
	/// <summary>
	/// Represents optimized entity property access.
	/// </summary>
	public interface IAccessOptimizer
	{
		object[] GetPropertyValues(object target);
		void SetPropertyValues(object target, object[] values);
	}

	public static class AccessOptimizerExtensions
	{
		/// <summary>
		/// Get the property value on the given index.
		/// </summary>
		//6.0 TODO: Merge into IAccessOptimizer.
		public static object GetPropertyValue(this IAccessOptimizer optimizer, object target, int i)
		{
			if (optimizer is AccessOptimizer accessOptimizer)
			{
				return accessOptimizer.GetPropertyValue(target, i);
			}

			throw new InvalidOperationException($"{optimizer.GetType()} does not support {nameof(GetPropertyValue)} method.");
		}

		/// <summary>
		/// Set the property value on the given index.
		/// </summary>
		//6.0 TODO: Merge into IAccessOptimizer.
		public static void SetPropertyValue(this IAccessOptimizer optimizer, object target, int i, object value)
		{
			if (optimizer is AccessOptimizer accessOptimizer)
			{
				accessOptimizer.SetPropertyValue(target, i, value);
				return;
			}

			throw new InvalidOperationException($"{optimizer.GetType()} does not support {nameof(SetPropertyValue)} method.");
		}

		/// <summary>
		/// Get the specialized property value.
		/// </summary>
		//6.0 TODO: Merge into IAccessOptimizer.
		internal static object GetSpecializedPropertyValue(this IAccessOptimizer optimizer, object target)
		{
			if (optimizer is AccessOptimizer accessOptimizer)
			{
				return accessOptimizer.GetSpecializedPropertyValue(target);
			}

			throw new InvalidOperationException($"{optimizer.GetType()} does not support {nameof(GetPropertyValue)} method.");
		}

		/// <summary>
		/// Set the specialized property value.
		/// </summary>
		//6.0 TODO: Merge into IAccessOptimizer.
		internal static void SetSpecializedPropertyValue(this IAccessOptimizer optimizer, object target, object value)
		{
			if (optimizer is AccessOptimizer accessOptimizer)
			{
				accessOptimizer.SetSpecializedPropertyValue(target, value);
				return;
			}

			throw new InvalidOperationException($"{optimizer.GetType()} does not support {nameof(SetPropertyValue)} method.");
		}
	}
}
