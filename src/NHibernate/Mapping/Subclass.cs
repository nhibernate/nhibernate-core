using System;
using System.Collections;
using NHibernate.Cache;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Subclass : PersistentClass
	{
		private PersistentClass superclass;
		private Value key;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="superclass"></param>
		public Subclass( PersistentClass superclass )
		{
			this.superclass = superclass;
		}

		/// <summary></summary>
		public override ICacheConcurrencyStrategy Cache
		{
			get { return Superclass.Cache; }
			set { Superclass.Cache = value; }
		}

		/// <summary></summary>
		public override RootClass RootClazz
		{
			get { return Superclass.RootClazz; }
		}

		/// <summary></summary>
		public override PersistentClass Superclass
		{
			get { return superclass; }
			set { this.superclass = superclass; }
		}

		/// <summary></summary>
		public override Property IdentifierProperty
		{
			get { return Superclass.IdentifierProperty; }
			set { Superclass.IdentifierProperty = value; }
		}

		/// <summary></summary>
		public override Value Identifier
		{
			get { return Superclass.Identifier; }
			set { Superclass.Identifier = value; }
		}

		/// <summary></summary>
		public override bool HasIdentifierProperty
		{
			get { return Superclass.HasIdentifierProperty; }
		}

		/// <summary></summary>
		public override Value Discriminator
		{
			get { return Superclass.Discriminator; }
			set { Superclass.Discriminator = value; }
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return Superclass.IsMutable; }
			set { Superclass.IsMutable = value; }
		}

		/// <summary></summary>
		public override bool IsInherited
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool IsPolymorphic
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public override void AddProperty( Property p )
		{
			base.AddProperty( p );
			Superclass.AddSubclassProperty( p );
		}

		/// <summary></summary>
		public override Table Table
		{
			get { return base.Table; }
			set
			{
				base.Table = value;
				Superclass.AddSubclassTable( value );
			}
		}

		/// <summary></summary>
		public override ICollection PropertyClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange( PropertyCollection );
				retVal.AddRange( Superclass.PropertyClosureCollection );
				return retVal;
			}
		}

		/// <summary></summary>
		public override ICollection TableClosureCollection
		{
			get
			{
				ArrayList retVal = new ArrayList();
				retVal.AddRange( Superclass.TableClosureCollection );
				retVal.Add( Table );
				return retVal;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		public override void AddSubclassProperty( Property p )
		{
			base.AddSubclassProperty( p );
			Superclass.AddSubclassProperty( p );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public override void AddSubclassTable( Table table )
		{
			base.AddSubclassTable( table );
			Superclass.AddSubclassTable( table );
		}

		/// <summary></summary>
		public override bool IsVersioned
		{
			get { return Superclass.IsVersioned; }
		}

		/// <summary></summary>
		public override Property Version
		{
			get { return Superclass.Version; }
			set { Superclass.Version = value; }
		}

		/// <summary></summary>
		public override bool HasEmbeddedIdentifier
		{
			get { return Superclass.HasEmbeddedIdentifier; }
			set { Superclass.HasEmbeddedIdentifier = value; }
		}

		/// <summary></summary>
		public override System.Type Persister
		{
			get { return Superclass.Persister; }
			set { Superclass.Persister = value; }
		}

		/// <summary></summary>
		public override Table RootTable
		{
			get { return Superclass.RootTable; }
		}

		/// <summary></summary>
		public override Value Key
		{
			get
			{
				if( key == null )
				{
					return Identifier;
				}
				else
				{
					return key;
				}
			}
			set { key = value; }
		}

		/// <summary></summary>
		public override bool IsExplicitPolymorphism
		{
			get { return Superclass.IsExplicitPolymorphism; }
			set { Superclass.IsExplicitPolymorphism = value; }
		}

		/// <summary></summary>
		public override string Where
		{
			get { return Superclass.Where; }
			set { throw new NotImplementedException( "The Where string can not be set on the Subclass - use the RootClass instead." ); }
		}


	}
}