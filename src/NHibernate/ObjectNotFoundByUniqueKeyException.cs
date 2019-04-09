using System;
using System.Runtime.Serialization;
using System.Security;
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

		#region Serialization

		protected ObjectNotFoundByUniqueKeyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Key = info.GetValue(nameof(Key), typeof(object));
			PropertyName = info.GetString(nameof(PropertyName));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(Key), Key);
			info.AddValue(nameof(PropertyName), PropertyName);
		}

		#endregion Serialization
	}
}
