using System;
using System.Collections;
using log4net;
using NHibernate.Cache;
using NHibernate.Persister;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Declaration of a System.Type by using the <c>&lt;class&gt;</c> element.
	/// </summary>
	/// <remarks>
	/// <p>The <c>&lt;class&gt;</c> element has the following attributes available:</p>
	/// <list type="table">
	///		<listheader>
	///			<term>Attribute</term>
	///			<description>Possible Values</description>
	///		</listheader>
	///		<item>
	///			<term>name</term>
	///			<description>The fully qualified TypeName so it can be loaded by Reflection</description>
	///		</item>
	///		<item>
	///			<term>table</term>
	///			<description>The name of its database table.</description>
	///		</item>
	///		<item>
	///			<term>discriminator-value</term>
	///			<description>
	///				(optional - defaults to the FullClassName)  A value that distinguishes individual
	///				subclasses, used for polymorphic behavior.
	///			</description>
	///		</item>
	///		<item>
	///			<term>mutable</term>
	///			<description>
	///				(optional - defaults to <c>true</c>)  Specifies that instances of the class
	///				are (not) mutable.
	///			</description>
	///		</item>
	///		<item>
	///			<term>schema</term>
	///			<description>(optional)  Override the schema name specified by the root <c>&lt;hibernate-mapping&gt;</c> element.</description>
	///		</item>
	///		<item>
	///			<term>proxy</term>
	///			<description>
	///				(optional) Specifies an interface to use for lazy initializing proxies.  
	///				You may specify the name of the class itself. 
	///				(TODO: update once Proxies are implemented)
	///			</description>
	///		</item>
	///		<item>
	///			<term>dynamic-update</term>
	///			<description>
	///				(optional - defaults to <c>false</c>) Specifies the <c>UPDATE</c> SQL should 
	///				be generated at runtime and contain only those columns whose values have changed.
	///			</description>
	///		</item>
	///		<item>
	///			<term>dynamic-insert</term>
	///			<description>
	///				(optional - defaults to <c>false</c>)  Specifies the <c>INSERT</c> SQL should 
	///				be generated at runtime and contain only those columns whose values are not null.
	///			</description>
	///		</item>
	///		<item>
	///			<term>polymorphism</term>
	///			<description>
	///				(optional, defaults to <c>implicit</c>)  Determines whether implicit or explicit
	///				query polymorphism is used.	
	///			</description>
	///		</item>
	///		<item>
	///			<term>where</term>
	///			<description>
	///				(optional) Specify an arbitrary SQL <c>WHERE</c> condition to be used 
	///				when retrieving objects of this class.
	///			</description>
	///		</item>
	///		<item>
	///			<term>persister</term>
	///			<description>(optional)  Specifies a custom <see cref="IClassPersister"/>.</description>
	///		</item>
	/// </list>
	/// </remarks>
	public class RootClass : PersistentClass
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( RootClass ) );

		/// <summary></summary>
		public const string DefaultIdentifierColumnName = "id";
		/// <summary></summary>
		public const string DefaultDiscriminatorColumnName = "class";

		private Property identifierProperty;
		private Value identifier;
		private Property version;
		private bool polymorphic;
		private ICacheConcurrencyStrategy cache;
		private Value discriminator;
		private bool mutable;
		private bool embeddedIdentifier = false;
		private bool explicitPolymorphism;
		private System.Type persister;
		private bool forceDiscriminator;
		private string where;

		/// <summary></summary>
		public bool Polymorphic
		{
			set { polymorphic = value; }
		}

		/// <summary></summary>
		public override Property IdentifierProperty
		{
			get { return identifierProperty; }
			set { identifierProperty = value; }
		}

		/// <summary></summary>
		public override Value Identifier
		{
			get { return identifier; }
			set { identifier = value; }
		}

		/// <summary></summary>
		public override bool HasIdentifierProperty
		{
			get { return identifierProperty != null; }
		}

		/// <summary></summary>
		public override Value Discriminator
		{
			get { return discriminator; }
			set { discriminator = value; }
		}

		/// <summary></summary>
		public override bool IsInherited
		{
			get { return false; }
		}

		/// <summary>
		/// Indicates if the object has subclasses
		/// </summary>
		/// <remarks>
		/// This value is set to True when a subclass is added and should not be set
		/// through any other method - so no setter is declared for this property.
		/// </remarks>
		public override bool IsPolymorphic
		{
			get { return polymorphic; }
		}

		/// <summary></summary>
		public override RootClass RootClazz
		{
			get { return this; }
		}

		/// <summary></summary>
		public override ICollection PropertyClosureCollection
		{
			get { return PropertyCollection; }
		}

		/// <summary>
		/// Returns all of the Tables the Root class covers.
		/// </summary>
		/// <remarks>
		/// The RootClass should only have one item in the Collection - the Table that it comes from.
		/// </remarks>
		public override ICollection TableClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.Add( Table );
				return retVal;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subclass"></param>
		public override void AddSubclass( Subclass subclass )
		{
			base.AddSubclass( subclass );
			polymorphic = true;
		}

		/// <summary></summary>
		public override bool IsExplicitPolymorphism
		{
			get { return explicitPolymorphism; }
			set { explicitPolymorphism = value; }
		}

		/// <summary>
		/// Gets or Sets the <see cref="Property"/> to use as the Version Property
		/// </summary>
		/// <value>The <see cref="Property"/> to use for Versioning.</value>
		/// <remarks>
		/// <para>
		/// The &lt;version&gt; element is optional and indicates that the table contains versioned data. 
		/// This is particularly useful if you plan to use long transactions (see below). 
		/// </para>
		/// <para>
		/// <list type="table">
		///		<listheader>
		///			<term>Attribute</term>
		///			<description>Possible Values</description>
		///		</listheader>
		///		<item>
		///			<term>column</term>
		///			<description>
		///				The name of the <c>column</c> holding the version number.  
		///				Defaults to the Property name.
		///			</description>
		///		</item>
		///		<item>
		///			<term>name</term>
		///			<description>The name of the Property in the Persistent Class.</description>
		///		</item>
		///		<item>
		///			<term>type</term>
		///			<description>
		///				The <see cref="Type.IType"/> of the Property.  Defaults to an <see cref="Type.Int32Type"/>.  It 
		///				be any <see cref="Type.IVersionType"/>.   
		///			</description>
		///		</item>
		///	</list>
		/// </para>
		/// </remarks>
		public override Property Version
		{
			get { return version; }
			set { version = value; }
		}

		/// <summary>
		/// Gets a value indicating if the <see cref="PersistentClass" /> is versioned
		/// by NHibernate.
		/// </summary>
		/// <value><c>true</c> if there is a version property.</value>
		public override bool IsVersioned
		{
			get { return version != null; }
		}

		/// <summary></summary>
		public override ICacheConcurrencyStrategy Cache
		{
			get { return cache; }
			set { cache = value; }
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return mutable; }
			set { mutable = value; }
		}

		/// <summary></summary>
		public override bool HasEmbeddedIdentifier
		{
			get { return embeddedIdentifier; }
			set { embeddedIdentifier = value; }
		}

		/// <summary></summary>
		public override System.Type Persister
		{
			get { return persister; }
			set { persister = value; }
		}

		/// <summary></summary>
		public override Table RootTable
		{
			get { return Table; }
		}

		/// <summary></summary>
		public override PersistentClass Superclass
		{
			get { return null; }
			set { throw new InvalidOperationException( "Can not set the Superclass on a RootClass." ); }
		}

		/// <summary></summary>
		public override Value Key
		{
			get { return Identifier; }
			set { throw new InvalidOperationException(); }
		}

		/// <summary></summary>
		public override bool IsForceDiscriminator
		{
			get { return forceDiscriminator; }
			set { this.forceDiscriminator = value; }
		}

		/// <summary></summary>
		public override string Where
		{
			get { return where; }
			set { where = value; }
		}

	}
}