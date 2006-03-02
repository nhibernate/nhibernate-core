using System;
using System.Collections;
using NHibernate.Util;

namespace NHibernate.Property
{
	/// <summary>
	/// Factory for creating the various PropertyAccessor strategies.
	/// </summary>
	public sealed class PropertyAccessorFactory
	{
		private static IDictionary accessors;

		/// <summary>
		/// Initializes the static members in <see cref="PropertyAccessorFactory"/>.
		/// </summary>
		static PropertyAccessorFactory()
		{
			accessors = new Hashtable( 13 );
			accessors[ "property" ] = new BasicPropertyAccessor();
			accessors[ "field" ] = new FieldAccessor();
			accessors[ "field.camelcase" ] = new FieldAccessor( new CamelCaseStrategy() );
			accessors[ "field.camelcase-underscore" ] = new FieldAccessor( new CamelCaseUnderscoreStrategy() );
			accessors[ "field.lowercase" ] = new FieldAccessor( new LowerCaseStrategy() );
			accessors[ "field.lowercase-underscore" ] = new FieldAccessor( new LowerCaseUnderscoreStrategy() );
			accessors[ "field.pascalcase-underscore" ] = new FieldAccessor( new PascalCaseUnderscoreStrategy() );
			accessors[ "field.pascalcase-m-underscore" ] = new FieldAccessor( new PascalCaseMUnderscoreStrategy() );
			accessors[ "nosetter.camelcase" ] = new NoSetterAccessor( new CamelCaseStrategy() );
			accessors[ "nosetter.camelcase-underscore" ] = new NoSetterAccessor( new CamelCaseUnderscoreStrategy() );
			accessors[ "nosetter.lowercase" ] = new NoSetterAccessor( new LowerCaseStrategy() );
			accessors[ "nosetter.lowercase-underscore" ] = new NoSetterAccessor( new LowerCaseUnderscoreStrategy() );
			accessors[ "nosetter.pascalcase-underscore" ] = new NoSetterAccessor( new PascalCaseUnderscoreStrategy() );
			accessors[ "nosetter.pascalcase-m-underscore" ] = new NoSetterAccessor( new PascalCaseMUnderscoreStrategy() );
		}

		private PropertyAccessorFactory()
		{
			throw new NotSupportedException( "Should not be creating a PropertyAccessorFactory - only use the static methods." );
		}

		/// <summary>
		/// Gets or creates the <see cref="IPropertyAccessor" /> specified by the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns>The <see cref="IPropertyAccessor"/> specified by the type.</returns>
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
		///			<term>pascalcase-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be prefixed with an underscore
		///				to find the field.
		///				<c>&lt;property name="Foo" ... &gt;</c> finds a field <c>_Foo</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>pascalcase-m-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be prefixed with an 'm' and underscore
		///				to find the field.
		///				<c>&lt;property name="Foo" ... &gt;</c> finds a field <c>m_Foo</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>lowercase</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to lowercase to find the field.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>foobar</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>lowercase-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to lowercase and prefixed with
		///				and underscore to find the field.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>_foobar</c>.
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
		public static IPropertyAccessor GetPropertyAccessor( string type )
		{
			// if not type is specified then fall back to the default of using
			// the property.
			if( type == null )
			{
				type = "property";
			}

			// attempt to find it in the built in types
			IPropertyAccessor accessor = accessors[ type ] as IPropertyAccessor;
			if( accessor != null )
			{
				return accessor;
			}

			// was not a built in type so now check to see if it is custom
			// accessor.
			System.Type accessorClass;
			try
			{
				accessorClass = ReflectHelper.ClassForName( type );
			}
			catch( TypeLoadException tle )
			{
				throw new MappingException( "could not find PropertyAccessor type: " + type, tle );
			}

			try
			{
				accessor = ( IPropertyAccessor ) Activator.CreateInstance( accessorClass );
				accessors[ type ] = accessor;
				return accessor;
			}
			catch( Exception e )
			{
				throw new MappingException( "could not instantiate PropertyAccessor type: " + type, e );
			}

		}
	}
}