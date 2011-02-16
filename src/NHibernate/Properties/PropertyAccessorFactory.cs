using System;
using System.Collections.Generic;
using NHibernate.Type;
using NHibernate.Util;
using NHibernate.Engine;

namespace NHibernate.Properties
{
	/// <summary>
	/// Factory for creating the various PropertyAccessor strategies.
	/// </summary>
	public sealed class PropertyAccessorFactory
	{
		private static readonly IDictionary<string, IPropertyAccessor> accessors;
		private const string DefaultAccessorName = "property";

		/// <summary>
		/// Initializes the static members in <see cref="PropertyAccessorFactory"/>.
		/// </summary>
		static PropertyAccessorFactory()
		{
			accessors = new Dictionary<string, IPropertyAccessor>(19);
			accessors["property"] = new BasicPropertyAccessor();
			accessors["field"] = new FieldAccessor();
			accessors["backfield"] = new FieldAccessor(new BackFieldStrategy());
			accessors["readonly"] = new ReadOnlyAccessor();
			accessors["field.camelcase"] = new FieldAccessor(new CamelCaseStrategy());
			accessors["field.camelcase-underscore"] = new FieldAccessor(new CamelCaseUnderscoreStrategy());
			accessors["field.camelcase-m-underscore"] = new FieldAccessor(new CamelCaseMUnderscoreStrategy());
			accessors["field.lowercase"] = new FieldAccessor(new LowerCaseStrategy());
			accessors["field.lowercase-underscore"] = new FieldAccessor(new LowerCaseUnderscoreStrategy());
			accessors["field.pascalcase-underscore"] = new FieldAccessor(new PascalCaseUnderscoreStrategy());
			accessors["field.pascalcase-m-underscore"] = new FieldAccessor(new PascalCaseMUnderscoreStrategy());
			accessors["field.pascalcase-m"] = new FieldAccessor(new PascalCaseMStrategy());
			accessors["nosetter.camelcase"] = new NoSetterAccessor(new CamelCaseStrategy());
			accessors["nosetter.camelcase-underscore"] = new NoSetterAccessor(new CamelCaseUnderscoreStrategy());
			accessors["nosetter.camelcase-m-underscore"] = new NoSetterAccessor(new CamelCaseMUnderscoreStrategy());
			accessors["nosetter.lowercase"] = new NoSetterAccessor(new LowerCaseStrategy());
			accessors["nosetter.lowercase-underscore"] = new NoSetterAccessor(new LowerCaseUnderscoreStrategy());
			accessors["nosetter.pascalcase-underscore"] = new NoSetterAccessor(new PascalCaseUnderscoreStrategy());
			accessors["nosetter.pascalcase-m-underscore"] = new NoSetterAccessor(new PascalCaseMUnderscoreStrategy());
			accessors["nosetter.pascalcase-m"] = new NoSetterAccessor(new PascalCaseMStrategy());
			accessors["embedded"] = new EmbeddedPropertyAccessor();
			accessors["noop"] = new NoopAccessor();
			accessors["none"] = new NoopAccessor();
		}

		private PropertyAccessorFactory()
		{
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
		///				Property's get method to retrieve the value and will use the field
		///				to set the value.  This is a good option for &lt;id&gt; Properties because this access method 
		///				allows users of the Class to get the value of the Id but not set the value.
		///			</description>
		///		</item>
		///     <item>
		///			<term>readonly</term>
		///			<description>
		///				The <c>name</c> attribute is the name of the Property.  NHibernate will use the 
		///				Property's get method to retrieve the value but will never set the value back in the domain.
		///				This is used for read-only calculated properties with only a get method.
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
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>fooBar</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>camelcase-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to CamelCase and prefixed with
		///				an underscore to find the field.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>_fooBar</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>camelcase-m-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be changed to CamelCase and prefixed with
		///				an 'm' and underscore to find the field.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>m_fooBar</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>pascalcase-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be prefixed with an underscore
		///				to find the field.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>_FooBar</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>pascalcase-m-underscore</term>
		///			<description>
		///				The <c>name</c> attribute should be prefixed with an 'm' and underscore
		///				to find the field.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>m_FooBar</c>.
		///			</description>
		///		</item>
		///		<item>
		///			<term>pascalcase-m</term>
		///			<description>
		///				The <c>name</c> attribute should be prefixed with an 'm'.
		///				<c>&lt;property name="FooBar" ... &gt;</c> finds a field <c>mFooBar</c>.
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
		///	strategy was specified the Hql statement would have to be <c>from Foo as foo where foo._someProperty</c>
		///	 (assuming CamelCase with an underscore field naming strategy is used).  
		///	</para>
		/// </remarks>
		public static IPropertyAccessor GetPropertyAccessor(string type)
		{
			// if not type is specified then fall back to the default of using
			// the property.
			if (string.IsNullOrEmpty(type))
			{
				type = DefaultAccessorName;
			}

			// attempt to find it in the built in types
			IPropertyAccessor accessor;
			accessors.TryGetValue(type, out accessor);
			if (accessor != null)
			{
				return accessor;
			}

			return ResolveCustomAccessor(type);
		}

		//TODO: ideally we need the construction of PropertyAccessor to take the following:
		//      1) EntityMode
		//      2) EntityMode-specific data (i.e., the classname for pojo entities)
		//      3) Property-specific data based on the EntityMode (i.e., property-name or dom4j-node-name)
		// The easiest way, with the introduction of the new runtime-metamodel classes, would be the
		// the following predicates:
		//      1) PropertyAccessorFactory.getPropertyAccessor() takes references to both a
		//          org.hibernate.metadata.EntityModeMetadata and org.hibernate.metadata.Property
		//      2) What is now termed a "PropertyAccessor" stores any values needed from those two
		//          pieces of information
		//      3) Code can then simply call PropertyAccess.getGetter() with no parameters; likewise with
		//          PropertyAccessor.getSetter()

		/// <summary> Retrieves a PropertyAccessor instance based on the given property definition and entity mode. </summary>
		/// <param name="property">The property for which to retrieve an accessor. </param>
		/// <param name="mode">The mode for the resulting entity. </param>
		/// <returns> An appropriate accessor. </returns>
		public static IPropertyAccessor GetPropertyAccessor(Mapping.Property property, EntityMode? mode)
		{
			//TODO: this is temporary in that the end result will probably not take a Property reference per-se.
			EntityMode modeToUse = mode.HasValue ? mode.Value : EntityMode.Poco;
			switch(modeToUse)
			{
				case EntityMode.Poco:
					return GetPocoPropertyAccessor(property.PropertyAccessorName);
				case EntityMode.Map:
					return DynamicMapPropertyAccessor;
				case EntityMode.Xml:
					return GetXmlPropertyAccessor(property.GetAccessorPropertyName(modeToUse), property.Type, null);
				default:
					throw new MappingException("Unknown entity mode [" + mode + "]");
			}
		}

		private static IPropertyAccessor GetXmlPropertyAccessor(string nodeName, IType type, ISessionFactoryImplementor factory)
		{
			//TODO: need some caching scheme? really comes down to decision 
			//      regarding amount of state (if any) kept on PropertyAccessors
			return new XmlAccessor(nodeName, type, factory);
		}

		private static IPropertyAccessor GetPocoPropertyAccessor(string accessorName)
		{
			string accName = string.IsNullOrEmpty(accessorName) ? DefaultAccessorName : accessorName;

			IPropertyAccessor accessor;
			accessors.TryGetValue(accName, out accessor);
			if (accessor != null)
				return accessor;

			return ResolveCustomAccessor(accName);
		}

		private static IPropertyAccessor ResolveCustomAccessor(string accessorName)
		{
			System.Type accessorClass;
			try
			{
				accessorClass = ReflectHelper.ClassForName(accessorName);
			}
			catch (TypeLoadException tle)
			{
				throw new MappingException("could not find PropertyAccessor type: " + accessorName, tle);
			}

			try
			{
				var result = (IPropertyAccessor) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(accessorClass);
				accessors[accessorName] = result;
				return result;
			}
			catch (Exception e)
			{
				throw new MappingException("could not instantiate PropertyAccessor class: " + accessorName, e);
			}
		}
		[NonSerialized]
		private static readonly IPropertyAccessor MapAccessor = new MapAccessor();
		public static IPropertyAccessor DynamicMapPropertyAccessor
		{
			get { return MapAccessor; }
		}

	}
}
