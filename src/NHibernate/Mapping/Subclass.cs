using System;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Util;

namespace NHibernate.Mapping 
{
	public class Subclass : PersistentClass 
	{
		private PersistentClass superclass;
		private Value key;

		public Subclass(PersistentClass superclass) 
		{
			this.superclass = superclass;
		}

		public override ICacheConcurrencyStrategy Cache 
		{
			get { return Superclass.Cache; }
			set { Superclass.Cache = value; }
		}

		public override RootClass RootClazz 
		{
			get { return Superclass.RootClazz; }
		}

		public override PersistentClass Superclass 
		{
			get { return superclass; }
			set { this.superclass = superclass; }
		}

		public override Property IdentifierProperty 
		{
			get { return Superclass.IdentifierProperty; }
			set { Superclass.IdentifierProperty = value; }
		}

		public override Value Identifier 
		{
			get { return Superclass.Identifier; }
			set { Superclass.Identifier = value; }
		}

		public override bool HasIdentifierProperty 
		{
			get { return Superclass.HasIdentifierProperty; }
		}
		
		public override Value Discriminator 
		{
			get { return Superclass.Discriminator; }
			set { Superclass.Discriminator = value; }
		}
		
		public override bool IsMutable 
		{
			get { return Superclass.IsMutable; }
			set { Superclass.IsMutable = value; }
		}
		
		public override bool IsInherited 
		{
			get { return true; }
		}
		
		public override bool IsPolymorphic 
		{
			get { return true; }
		}
		
		public override void AddProperty(Property p) 
		{
			base.AddProperty(p);
			Superclass.AddSubclassProperty(p);
		}
		
		public override Table Table 
		{
			get { return base.Table; }
			set 
			{
				base.Table = value;
				Superclass.AddSubclassTable(value);
			}
		}
		
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
		
		public override void AddSubclassProperty(Property p) 
		{
			base.AddSubclassProperty(p);
			Superclass.AddSubclassProperty(p);
		}
		
		public override void AddSubclassTable(Table table) 
		{
			base.AddSubclassTable(table);
			Superclass.AddSubclassTable(table);
		}
		
		public override bool IsVersioned 
		{
			get { return Superclass.IsVersioned; }
		}
		
		public override Property Version 
		{
			get { return Superclass.Version; }
			set { Superclass.Version = value; }
		}
		
		public override bool HasEmbeddedIdentifier 
		{
			get { return Superclass.HasEmbeddedIdentifier; }
			set { Superclass.HasEmbeddedIdentifier = value; }
		}
		
		public override System.Type Persister 
		{
			get { return Superclass.Persister; }
			set { Superclass.Persister = value; }
		}
		
		public override Table RootTable 
		{
			get { return Superclass.RootTable; }
		}
		
		public override Value Key 
		{
			get 
			{
				if (key==null) 
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
		
		public override bool IsExplicitPolymorphism 
		{
			get { return Superclass.IsExplicitPolymorphism; }
			set { Superclass.IsExplicitPolymorphism = value; }
		}

		public override string Where
		{
			get	{ return Superclass.Where; }
			set { throw new NotImplementedException("The Where string can not be set on the Subclass - use the RootClass instead."); }
		}


	}
}
