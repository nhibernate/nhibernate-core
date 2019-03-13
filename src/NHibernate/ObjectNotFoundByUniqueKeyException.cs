using System;
using NHibernate.Impl;

namespace NHibernate
{
	/// <summary>
	/// Thrown when entity can't be found by given unique key
	/// </summary>
	[Serializable]
	public class ObjectNotFoundByUniqueKeyException : HibernateException
	{
		/// <summary>
		/// Property name
		/// </summary>
		public string PropertyName { get; }
		
		/// <summary>
		/// Key
		/// </summary>
		public object Key { get; }

		/// <summary>
		/// Thrown when entity can't be found by given unique key
		/// </summary>
		/// <param name="entityName">Entity name</param>
		/// <param name="propertyName">Property name</param>
		/// <param name="key">Key</param>
		public ObjectNotFoundByUniqueKeyException(string entityName, string propertyName, object key)
			: base("No row with given unique key found " + MessageHelper.InfoString(entityName, propertyName, key))
		{
			Key = key;
			PropertyName = propertyName;
		}
	}
}
