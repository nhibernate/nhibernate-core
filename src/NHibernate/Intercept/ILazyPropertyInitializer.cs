using System;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	[Serializable]
	public struct UnfetchedLazyProperty : IEquatable<UnfetchedLazyProperty>
	{
		/// <summary>As long as the other instance is the same type, it's considered equal. This
		/// avoids the issue where a deserialized value fails the base Object.Equals() check due to
		/// both objects being different references.</summary>
		public bool Equals(UnfetchedLazyProperty other) => true;
		public override bool Equals(object obj) => obj is UnfetchedLazyProperty;
		public override int GetHashCode() => typeof(UnfetchedLazyProperty).GetHashCode();
	}

	public struct LazyPropertyInitializer
	{
		/// <summary> Marker value for uninitialized properties</summary>
		public static readonly object UnfetchedProperty = new UnfetchedLazyProperty();
	}

	/// <summary> Contract for controlling how lazy properties get initialized. </summary>
	public interface ILazyPropertyInitializer
	{
		/// <summary> Initialize the property, and return its new value</summary>
		object InitializeLazyProperty(string fieldName, object entity, ISessionImplementor session);
	}
}
