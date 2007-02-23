using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Nullables.TypeConverters
{
	public class NullableInt64Converter : TypeConverter
	{
		public NullableInt64Converter()
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
				return NullableInt64.Default;
			}
			if (value is string)
			{
				string stringValue = ((string) value).Trim();

				if (stringValue == string.Empty)
					return NullableInt64.Default;

				//get underlying types converter
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Int64));

				Int64 newValue = (Int64) converter.ConvertFromString(context, culture, stringValue);

				return new NullableInt64(newValue);
			}
			else
			{
				return base.ConvertFrom(context, culture, value);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
		                                 Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is NullableInt64)
			{
				NullableInt64 nullable = (NullableInt64) value;

				Type[] constructorArgTypes = new Type[1] {typeof(Int64)};
				ConstructorInfo constructor = typeof(NullableInt64).GetConstructor(constructorArgTypes);

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
			return new NullableInt64((Int64) propertyValues["Value"]);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
		                                                           Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(NullableInt64), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}