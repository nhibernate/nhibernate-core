using System;
using System.Collections;
using NHibernate.Cache;
using NHibernate.SqlCommand;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Base class for the <see cref="RootClass" /> mapped by &lt;class&gt; and a 
	/// <see cref="Subclass"/> that is mapped by &lt;subclass&gt; or &lt;joined-subclass&gt;
	/// </summary>
	public abstract class PersistentClass
	{
		private static readonly Alias PKAlias = new Alias( 15, "PK" );

		private System.Type persistentClass;
		private string discriminatorValue;
		private ArrayList properties = new ArrayList();
		private Table table;
		private System.Type proxyInterface;
		private readonly ArrayList subclasses = new ArrayList();
		private readonly ArrayList subclassProperties = new ArrayList();
		private readonly ArrayList subclassTables = new ArrayList();
		private bool dynamicInsert;
		private bool dynamicUpdate;

		/// <summary>
		/// Gets or Sets if the Insert Sql is built dynamically.
		/// </summary>
		/// <value><c>true</c> if the Sql is built at runtime.</value>
		/// <remarks>
		/// The value of this is set by the <c>dynamic-insert</c> attribute. 
		/// </remarks>
		public virtual bool DynamicInsert
		{
			get { return dynamicInsert; }
			set { dynamicInsert = value; }
		}

		/// <summary>
		/// Gets or Sets if the Update Sql is built dynamically.
		/// </summary>
		/// <value><c>true</c> if the Sql is built at runtime.</value>
		/// <remarks>
		/// The value of this is set by the <c>dynamic-update</c> attribute. 
		/// </remarks>
		public virtual bool DynamicUpdate
		{
			get { return dynamicUpdate; }
			set { dynamicUpdate = value; }
		}

		/// <summary>
		/// Gets or Sets the value to use as the discriminator for the Class.
		/// </summary>
		/// <value>
		/// A value that distinguishes this subclass in the database.
		/// </value>
		/// <remarks>
		/// The value of this is set by the <c>discriminator-value</c> attribute.  Each &lt;subclass&gt;
		/// in a heirarchy must define a unique <c>discriminator-value</c>.  The default value 
		/// is the class name if no value is supplied.
		/// </remarks>
		public virtual string DiscriminatorValue
		{
			get { return discriminatorValue; }
			set { discriminatorValue = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subclass"></param>
		public virtual void AddSubclass( Subclass subclass )
		{
			subclasses.Add( subclass );
		}

		/// <summary></summary>
		public virtual bool HasSubclasses
		{
			get { return subclasses.Count > 0; }
		}

		/// <summary></summary>
		public virtual int SubclassSpan
		{
			get
			{
				int n = subclasses.Count;
				foreach( Subclass sc in subclasses )
				{
					n += sc.SubclassSpan;
				}
				return n;
			}
		}

		/// <summary>
		/// Gets the Collection of Subclasses for this PersistentClass.  
		/// </summary>
		/// <remarks>
		/// It will recursively go through Subclasses so that if a Subclass has Subclasses
		/// it will pick those up also.
		/// </remarks>
		public virtual ICollection SubclassCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();

				// check to see if there are any subclass in our subclasses 
				// and add them into the collection
				foreach( Subclass sc in subclasses )
				{
					retVal.AddRange( sc.SubclassCollection );
				}

				// finally add the subclasses from this PersistentClass into
				// the collection to return
				retVal.AddRange( subclasses );

				return retVal;
			}
		}

		/// <summary></summary>
		public virtual ICollection DirectSubclasses
		{
			get { return subclasses; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public virtual void AddProperty( Property p )
		{
			properties.Add( p );
		}

		/// <summary>
		/// Gets or Sets the <see cref="Table"/> that this class is stored in.
		/// </summary>
		/// <value>The <see cref="Table"/> this class is stored in.</value>
		/// <remarks>
		/// The value of this is set by the <c>table</c> attribute. 
		/// </remarks>
		public virtual Table Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary></summary>
		public virtual ICollection PropertyCollection
		{
			get { return properties; }
		}

		/// <summary></summary>
		public virtual System.Type PersistentClazz
		{
			get { return persistentClass; }
			set { persistentClass = value; }
		}

		/// <summary></summary>
		public virtual string Name
		{
			get { return persistentClass.FullName; }
		}

		/// <summary></summary>
		public abstract bool IsMutable { get; set; }

		/// <summary></summary>
		public abstract bool HasIdentifierProperty { get; }

		/// <summary></summary>
		public abstract Property IdentifierProperty { get; set; }

		/// <summary></summary>
		public abstract Value Identifier { get; set; }

		/// <summary></summary>
		public abstract Property Version { get; set; }

		/// <summary></summary>
		public abstract Value Discriminator { get; set; }

		/// <summary></summary>
		public abstract bool IsInherited { get; }

		// see the comment in RootClass about why the polymorphic setter is commented out
		/// <summary></summary>
		public abstract bool IsPolymorphic { get; }

		/// <summary></summary>
		public abstract bool IsVersioned { get; }

		/// <summary></summary>
		public abstract ICacheConcurrencyStrategy Cache { get; set; }

		/// <summary></summary>
		public abstract PersistentClass Superclass { get; set; }

		/// <summary></summary>
		public abstract bool IsExplicitPolymorphism { get; set; }

		/// <summary></summary>
		public abstract ICollection PropertyClosureCollection { get; }

		/// <summary></summary>
		public abstract ICollection TableClosureCollection { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public virtual void AddSubclassProperty( Property p )
		{
			subclassProperties.Add( p );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public virtual void AddSubclassTable( Table table )
		{
			subclassTables.Add( table );
		}

		/// <summary></summary>
		public virtual ICollection SubclassPropertyClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange( PropertyClosureCollection );
				retVal.AddRange( subclassProperties );
				return retVal;
			}
		}

		/// <summary>
		/// Returns an ICollection of all of the Tables that the subclass finds its information
		/// in.  
		/// </summary>
		/// <remarks>It adds the TableClosureCollection and the subclassTables into the ICollection.</remarks>
		public virtual ICollection SubclassTableClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange( TableClosureCollection );
				retVal.AddRange( subclassTables );
				return retVal;
			}
		}

		/// <summary></summary>
		public virtual System.Type ProxyInterface
		{
			get { return proxyInterface; }
			set { proxyInterface = value; }
		}

		/// <summary></summary>
		public virtual bool IsForceDiscriminator
		{
			get { return false; }
			set { throw new NotImplementedException( "subclasses need to override this method" ); }
		}

		/// <summary></summary>
		public abstract bool HasEmbeddedIdentifier { get; set; }

		/// <summary></summary>
		public abstract System.Type Persister { get; set; }

		/// <summary></summary>
		public abstract Table RootTable { get; }

		/// <summary></summary>
		public abstract RootClass RootClazz { get; }

		/// <summary></summary>
		public abstract Value Key { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		public virtual void CreatePrimaryKey( Dialect.Dialect dialect )
		{
			PrimaryKey pk = new PrimaryKey();
			pk.Table = table;
			pk.Name = PKAlias.ToAliasString( table.Name, dialect );
			table.PrimaryKey = pk;

			foreach( Column col in Key.ColumnCollection )
			{
				pk.AddColumn( col );
			}
		}

		/// <summary></summary>
		public abstract string Where { get; set; }

	}
}