using System;
using NHibernate.Engine;

namespace NHibernate.Intercept
{
	public struct LazyPropertyInitializer
	{
		/// <summary> Marker value for uninitialized properties</summary>
		public readonly static object UnfetchedProperty = new Guid(0x8cbd57d5, 0xe58, 0x48a6, 0xa8, 0xf3, 0xec, 0xd4, 0xc2, 0x16, 0x35, 0xee);
	}

	/// <summary> Contract for controlling how lazy properties get initialized. </summary>
	public interface ILazyPropertyInitializer
	{
		/// <summary> Initialize the property, and return its new value</summary>
		object InitializeLazyProperty(string fieldName, object entity, ISessionImplementor session);
	}
}