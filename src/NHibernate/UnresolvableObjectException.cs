using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using NHibernate.Impl;
using NHibernate.Util;

namespace NHibernate
{
	/// <summary>
	/// Thrown when Hibernate could not resolve an object by id, especially when
	/// loading an association.
	/// </summary>
	[Serializable]
	public class UnresolvableObjectException : HibernateException
	{
		private readonly object _identifier;
		[NonSerialized]
		private readonly System.Type _clazz;
		private readonly string _entityName;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnresolvableObjectException"/> class.
		/// </summary>
		/// <param name="identifier">The identifier of the object that caused the exception.</param>
		/// <param name="clazz">The <see cref="System.Type"/> of the object attempted to be loaded.</param>
		public UnresolvableObjectException(object identifier, System.Type clazz) :
			this("No row with the given identifier exists", identifier, clazz)
		{
		}

		public UnresolvableObjectException(object identifier, string entityName)
			:this("No row with the given identifier exists", identifier, entityName) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="UnresolvableObjectException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="identifier">The identifier of the object that caused the exception.</param>
		/// <param name="clazz">The <see cref="System.Type"/> of the object attempted to be loaded.</param>
		public UnresolvableObjectException(string message, object identifier, System.Type clazz)
			: base(message)
		{
			_identifier = identifier;
			_clazz = clazz;
		}

		public UnresolvableObjectException(string message, object identifier, string entityName)
			: base(message)
		{
			_identifier = identifier;
			_entityName = entityName;
		}

		public object Identifier => _identifier;

		public override string Message => base.Message + MessageHelper.InfoString(EntityName, _identifier);

		public System.Type PersistentClass => _clazz;

		public string EntityName => _clazz != null ? _clazz.FullName : _entityName;

		public static void ThrowIfNull(object o, object id, System.Type clazz)
		{
			if (o == null)
			{
				throw new UnresolvableObjectException(id, clazz);
			}
		}

		public static void ThrowIfNull(object o, object id, string entityName)
		{
			if (o == null)
			{
				throw new UnresolvableObjectException(id, entityName);
			}
		}

		#region ISerializable Members

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identifier", _identifier);
			info.AddValue("clazz", ObjectReferenceSystemType.Wrap(_clazz, true));
			info.AddValue("entityName", _entityName);
		}

		protected UnresolvableObjectException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_identifier = info.GetValue("identifier", typeof(object));
			_entityName = info.GetString("entityName");
			_clazz = info.GetValue<System.Type>("clazz");
		}

		#endregion
	}
}
