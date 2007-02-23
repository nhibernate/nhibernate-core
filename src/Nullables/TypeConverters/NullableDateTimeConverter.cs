using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Nullables.TypeConverters
{
	public class NullableDateTimeConverter : TypeConverter
	{
		public NullableDateTimeConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else if (sourceType == typeof(DateTime))
				return true;
			else if (sourceType == typeof(DBNull))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			else if (destinationType == typeof(DateTime))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return NullableDateTime.Default;
			}
			if (value is DateTime)
			{
				return new NullableDateTime((DateTime) value);
			}
			if (value is DBNull)
			{
				return NullableDateTime.Default;
			}
			if (value is string)
			{
				string stringValue = ((string) value).Trim();

				if (stringValue == string.Empty)
					return NullableDateTime.Default;

				//get underlying types converter
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(DateTime));

				DateTime newValue = (DateTime) converter.ConvertFromString(context, culture, stringValue);

				return new NullableDateTime(newValue);
			}
			else
			{
				return base.ConvertFrom(context, culture, value);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
		                                 Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is NullableDateTime)
			{
				NullableDateTime nullable = (NullableDateTime) value;

				Type[] constructorArgTypes = new Type[1] {typeof(DateTime)};
				ConstructorInfo constructor = typeof(NullableDateTime).GetConstructor(constructorArgTypes);

				if (constructor != null)
				{
					object[] constructorArgValues = new object[1] {nullable.Value};
					return new InstanceDescriptor(constructor, constructorArgValues);
				}
			}
			else if (destinationType == typeof(DateTime))
			{
				NullableDateTime ndt = (NullableDateTime) value;

				if (ndt.HasValue)
					return ndt.Value;
				else
					return DBNull.Value;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new NullableDateTime((DateTime) propertyValues["Value"]);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
		                                                           Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(NullableDateTime), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}