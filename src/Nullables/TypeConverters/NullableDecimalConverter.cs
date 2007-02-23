using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Nullables.TypeConverters
{
	public class NullableDecimalConverter : TypeConverter
	{
		public NullableDecimalConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				return NullableDecimal.Default;
			}
			if (value is string)
			{
				string stringValue = ((string) value).Trim();

				if (stringValue == string.Empty)
					return NullableDecimal.Default;

				//get underlying types converter
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Decimal));

				Decimal newValue = (Decimal) converter.ConvertFromString(context, culture, stringValue);

				return new NullableDecimal(newValue);
			}
			else
			{
				return base.ConvertFrom(context, culture, value);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
		                                 Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is NullableDecimal)
			{
				NullableDecimal nullable = (NullableDecimal) value;

				Type[] constructorArgTypes = new Type[1] {typeof(Decimal)};
				ConstructorInfo constructor = typeof(NullableDecimal).GetConstructor(constructorArgTypes);

				if (constructor != null)
				{
					object[] constructorArgValues = new object[1] {nullable.Value};
					return new InstanceDescriptor(constructor, constructorArgValues);
				}
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return new NullableDecimal((Decimal) propertyValues["Value"]);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
		                                                           Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(NullableDecimal), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}