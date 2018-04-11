using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{

	/// <summary>
	/// Base class for enum types.
	/// </summary>
	[Serializable]
	public abstract class AbstractEnumType : PrimitiveType, IDiscriminatorType
	{
		protected AbstractEnumType(SqlType sqlType,System.Type enumType)
			: base(sqlType)
		{
			if (enumType.IsEnum)
			{
				_enumType = enumType;
			}
			else
			{
				throw new MappingException(enumType.Name + " did not inherit from System.Enum");
			}
			_defaultValue = Enum.ToObject(enumType, 0);
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_serializableEnumType = SerializableSystemType.Wrap(_enumType);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			_enumType = _serializableEnumType?.GetSystemType();
		}

		private readonly object _defaultValue;
		[NonSerialized]
		private System.Type _enumType;
		private SerializableSystemType _serializableEnumType;

		public override System.Type ReturnedClass => _enumType;


		#region IIdentifierType Members

		public object StringToObject(string xml)
		{
			return Enum.Parse(_enumType, xml);
		}

		#endregion


		public override object FromStringValue(string xml)
		{
			return StringToObject(xml);
		}

		public override System.Type PrimitiveClass => _enumType;

		public override object DefaultValue => _defaultValue;
	}
}
