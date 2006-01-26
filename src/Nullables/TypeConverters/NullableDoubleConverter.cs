using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Nullables.TypeConverters
{
	public class NullableDoubleConverter : TypeConverter
	{
		public NullableDoubleConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom (context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			else
				return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value == null)
			{
				return NullableDouble.Default;
			}
			if (value is string)
			{
				string stringValue = ((string)value).Trim();

				if (stringValue == string.Empty)
					return NullableDouble.Default;

				//get underlying types converter
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Double));
				
				Double newValue = (Double)converter.ConvertFromString(context, culture, stringValue);

				return new NullableDouble(newValue);
			}
			else
			{
				return base.ConvertFrom (context, culture, value);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is NullableDouble)
			{
				NullableDouble nullable = (NullableDouble)value;
				
				Type[] constructorArgTypes = new Type[1] { typeof(Double) } ;
				ConstructorInfo constructor = typeof(NullableDouble).GetConstructor(constructorArgTypes);

				if (constructor != null)
				{
					object[] constructorArgValues = new object[1] { nullable.Value } ;
					return new InstanceDescriptor(constructor, constructorArgValues);
				}
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			return new NullableDouble((Double)propertyValues["Value"]);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(NullableDouble), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
