using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate.SqlTypes;

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
				this.enumType = enumType;
			}
			else
			{
				throw new MappingException(enumType.Name + " did not inherit from System.Enum");
			}
			defaultValue = Enum.GetValues(enumType).GetValue(0);
		}

		private readonly object defaultValue;
		private readonly System.Type enumType;

		public override System.Type ReturnedClass
		{
			get { return enumType; }
		}


		#region IIdentifierType Members

		public object StringToObject(string xml)
		{
			return Enum.Parse(enumType, xml);
		}

		#endregion


		public override object FromStringValue(string xml)
		{
			return StringToObject(xml);
		}

		public override System.Type PrimitiveClass
		{
			get { return this.enumType; }
		}

		public override object DefaultValue
		{
			get { return defaultValue; }
		}
	}
}
