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
		private static readonly IPropertyAccessor fieldCamelCaseAccessor = new FieldAccessor( new CamelCaseStrategy() );
		private static readonly IPropertyAccessor fieldCamelCaseUnderscoreAccessor = new FieldAccessor( new CamelCaseUnderscoreStrategy() );
		private static readonly IPropertyAccessor fieldPascalCaseMUnderscoreAccessor = new FieldAccessor( new PascalCaseMUnderscoreStrategy() );


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
				case "field.camelcase" :
					return fieldCamelCaseAccessor;
				case "field.camelcase-underscore" :
					return fieldCamelCaseUnderscoreAccessor;
				case "field.pascalcase-m-underscore" :
					return fieldPascalCaseMUnderscoreAccessor;
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
