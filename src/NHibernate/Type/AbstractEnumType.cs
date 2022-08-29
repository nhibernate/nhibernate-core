using System;
using NHibernate.SqlTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// Base class for enum types.
	/// </summary>
	[Serializable]
	public abstract class AbstractEnumType : PrimitiveType, IDiscriminatorType
	{
		protected AbstractEnumType(SqlType sqlType, System.Type enumType)
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
			defaultValue = Enum.ToObject(enumType, 0);
		}

		private readonly object defaultValue;
		private readonly System.Type enumType;

		public override System.Type ReturnedClass
		{
			get { return enumType; }
		}

		#region IIdentifierType Members

		// 6.0 TODO: rename "xml" parameter as "value": it is not a xml string. The fact it generally comes from a xml
		// attribute value is irrelevant to the method behavior.
		/// <inheritdoc />
		public object StringToObject(string xml)
		{
			return Enum.Parse(enumType, xml);
		}

		#endregion

		// Since 5.2
		[Obsolete("This method has no more usages and will be removed in a future version.")]
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
