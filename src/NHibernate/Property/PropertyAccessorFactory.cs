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
		private static readonly IPropertyAccessor noSetterCamelCaseAccessor = new NoSetterAccessor( new CamelCaseStrategy() );
		private static readonly IPropertyAccessor noSetterCamelCaseUnderscoreAccessor = new NoSetterAccessor( new CamelCaseUnderscoreStrategy() );
		private static readonly IPropertyAccessor noSetterPascalCaseMUnderscoreAccessor = new NoSetterAccessor( new PascalCaseMUnderscoreStrategy() );

		private PropertyAccessorFactory()
		{
			throw new NotSupportedException("Should not be creating a PropertyAccessorFactory - only use the static methods.");
		}

		/// <summary>
		/// Gets or creates the IPropertyAccessor specified by the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// The built in ways of accessing the values of Properties in your domain class are:
		/// </para>
		/// <list type="table">
		///		<listheader>
		///			<term>Access Method</term>
		///			<description>How NHibernate accesses the Mapped Class.</description>
		///		</listheader>
		///		<item>
		///			<term>property</term>
		///			<description>
		///				The <c>name</c> attribute is the name of the Property.  This is the 
		///				default implementation.
		///			</description>
		///		</item>
		///		<item>
		///			<term>field</term>
		///			<description>
		///				The <c>name</c> attribute is the name of the field.  If you have any Properties
		///				in the Mapped Class those will be bypassed and NHibernate will go straight to the
		///				field.  This is a good option if your setters have business rules attached to them
		///				or if you don't want to expose a field through a Getter &amp; Setter.
		///			</description>
		///		</item>
		///		<item>
		///			<term>nosetter</term>
		///			<description>
		///				The <c>name</c> attribute is the name of the Property.  NHibernate will use the 
		///				Property's get method to retreive the value and will use the field
		///				to set the value.  This is a good option for &lt;id&gt; Properties because this access method 
		///				allow's users of the Class to get the value of the Id but not set the value.
		///			</description>
		///		</item>
		///		<item>
		///			<term>Assembly Qualified Name</term>
		///			<description>
		///				If NHibernate's built in <see cref="IPropertyAccessor"/>s are not what is needed for your 
		///				situation then you are free to build your own.  Provide an Assembly Qualified Name so that 
		///				NHibernate can call <c>Activator.CreateInstance(AssemblyQualifiedName)</c> to create it.  
		///			</description>
		///		</item>
		///	</list>
		///	<para>
		///	In order for the <c>nosetter</c> to know the name of the field to access NHibernate needs to know
		///	what the naming strategy is.  The following naming strategies are built into NHibernate:
		///	</para>
		///	<list type="table">
		///		<listheader>
		///			<term>Naming Strategy</term>
		///			<description>How NHibernate converts the value of the <c>name</c> attribute to a field name.</description>
		///		</listheader>
		///		<item>
		///			<term>camelcase</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to CamelCase to find the field.
		///				<c>&lt;property name="Foo" ... &gt;</c> finds a field <c>foo</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>camelcase-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to CamelCase and prefixed with
		///				an underscore to find the field.
		///				<c>&lt;property name="Foo" ... &gt;</c> finds a field <c>_foo</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>pascalcase-m-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to CamelCase to find the field.
		///				<c>&lt;property name="Foo" ... &gt;</c> finds a field <c>m_Foo</c>.
		///			</description>
		///		</item>
		///	</list>
		///	<para>
		///	The naming strategy can also be appended at the end of the <c>field</c> access method.  Where
		///	this could be useful is a scenario where you do expose a get and set method in the Domain Class 
		///	but NHibernate should only use the fields.  
		///	</para>
		///	<para>
		///	With a naming strategy and a get/set for the Property available the user of the Domain Class 
		///	could write an Hql statement <c>from Foo as foo where foo.SomeProperty = 'a'</c>.   If no naming 
		///	strategy was specified the Hql statement whould have to be <c>from Foo as foo where foo._someProperty</c>
		///	 (assuming CamelCase with an underscore field naming strategy is used).  
		///	</para>
		/// </remarks>
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

				case "nosetter.camelcase" :
					return noSetterCamelCaseAccessor;

				case "nosetter.camelcase-underscore" :
					return noSetterCamelCaseUnderscoreAccessor;

				case "nosetter.pascalcase-m-underscore" :
					return noSetterPascalCaseMUnderscoreAccessor;

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
