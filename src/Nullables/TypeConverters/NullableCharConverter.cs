using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Nullables.TypeConverters
{
	public class NullableCharConverter : TypeConverter
	{
		public NullableCharConverter()
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
				return NullableChar.Default;
			}
			if (value is string)
			{
				string stringValue = ((string)value).Trim();

				if (stringValue == string.Empty)
					return NullableChar.Default;

				//get underlying types converter
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Char));
				
				Char newValue = (Char)converter.ConvertFromString(context, culture, stringValue);

				return new NullableChar(newValue);
			}
			else
			{
				return base.ConvertFrom (context, culture, value);
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is NullableChar)
			{
				NullableChar nullable = (NullableChar)value;
				
				Type[] constructorArgTypes = new Type[1] { typeof(Char) } ;
				ConstructorInfo constructor = typeof(NullableChar).GetConstructor(constructorArgTypes);

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
			return new NullableChar((Char)propertyValues["Value"]);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(NullableChar), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
