using System;
using System.Reflection;

using NHibernate.Util;

namespace NHibernate.Property
{
	/// <summary>
	/// Factory for creating the various PropertyAccessor strategies.
	/// </summary>
	public class PropertyAccessorFactory
	{
		private static readonly IPropertyAccessor basicPropertyAccessor = new BasicPropertyAccessor();
		private static readonly IPropertyAccessor fieldAccessor = new FieldAccessor();
		private static readonly IPropertyAccessor fieldUnderscoreAccessor = new FieldUnderscorePrefixAccessor();
		private static readonly IPropertyAccessor fieldMUnderscoreAccessor = new FieldMUnderscorePrefixAccessor();


		private PropertyAccessorFactory()
		{
			throw new NotSupportedException("Should not be creating a PropertyAccessorFactory - only use the static methods.");
		}

		public static IPropertyAccessor GetPropertyAccessor(string type) 
		{
			if( type==null || "property".Equals(type) ) return basicPropertyAccessor;
			
			switch(type) 
			{
				case "field" :
					return fieldAccessor;
				case "field.underscore" :
					return fieldUnderscoreAccessor;
				case "field.munderscore" :
					return fieldMUnderscoreAccessor;
			}
			
			System.Type accessorClass;
			try 
			{
				accessorClass = ReflectHelper.ClassForName(type);
			}
			catch(TypeLoadException tle) 
			{
				throw new MappingException("could not find PropertyAccessor type: " + type, tle);
			}

			try 
			{
				return (IPropertyAccessor) Activator.CreateInstance(accessorClass);
			}
			catch(Exception e) 
			{
				throw new MappingException("could not instantiate PropertyAccessor type: " + type, e );
			}

		}
	}
}
