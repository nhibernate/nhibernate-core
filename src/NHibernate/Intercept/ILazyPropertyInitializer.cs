using System;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	[Serializable]
	public struct UnfetchedLazyProperty
	{
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