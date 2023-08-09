using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Intercept
{
	/// <summary> Contract for field interception handlers. </summary>
	public interface IFieldInterceptor
	{
		/// <summary> Is the entity considered dirty? </summary>
		/// <value> True if the entity is dirty; otherwise false. </value>
		bool IsDirty { get; }

		/// <summary> Use to associate the entity to which we are bound to the given session. </summary>
		ISessionImplementor Session { get; set; }

		/// <summary> Is the entity to which we are bound completely initialized? </summary>
		// Since 5.3
		[Obsolete("This property is not used and will be removed in a future version.")]
		bool IsInitialized { get; }

		/// <summary> The the given field initialized for the entity to which we are bound? </summary>
		/// <param name="field">The name of the field to check </param>
		/// <returns> True if the given field is initialized; otherwise false.</returns>
		bool IsInitializedField(string field);

		/// <summary> Forcefully mark the entity as being dirty.</summary>
		void MarkDirty();

		/// <summary> Clear the internal dirty flag.</summary>
		void ClearDirty();

		/// <summary> Intercept field set/get </summary>
		// Since v5.3
		[Obsolete("Please use 'Intercept(object target, string fieldName, object value, bool setter)' extension method instead")]
		object Intercept(object target, string fieldName, object value);

		/// <summary> Get the entity-name of the field DeclaringType.</summary>
		string EntityName { get; }

		/// <summary> Get the MappedClass (field container).</summary>
		System.Type MappedClass { get; }
	}

	public static class FieldInterceptorExtensions
	{
		// 6.0 TODO: merge into IFieldInterceptor
		internal static ISet<string> GetUninitializedFields(this IFieldInterceptor interceptor)
		{
			if (interceptor is AbstractFieldInterceptor fieldInterceptor)
			{
				return fieldInterceptor.GetUninitializedFields();
			}

#pragma warning disable 618
			if (interceptor.IsInitialized)
#pragma warning restore 618
			{
				return CollectionHelper.EmptySet<string>();
			}

			return null; // The caller should use all lazy properties as the result
		}

		// 6.0 TODO: merge into IFieldInterceptor
		public static object Intercept(this IFieldInterceptor interceptor, object target, string fieldName, object value, bool setter)
		{
			if (interceptor is AbstractFieldInterceptor fieldInterceptor)
			{
				return fieldInterceptor.Intercept(target, fieldName, value, setter);
			}
#pragma warning disable 618
			return interceptor.Intercept(target, fieldName, value);
#pragma warning restore 618
		}
	}
}
