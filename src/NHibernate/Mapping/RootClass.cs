using System;
using System.Collections;
using NHibernate.Cache;

namespace NHibernate.Mapping {
	
	public class RootClass : PersistentClass {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RootClass));
		public const string DefaultIdentifierColumnName = "id";
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

		public override Property IdentifierProperty {
			get { return identifierProperty; }
			set { identifierProperty = value; }
		}
		public override Value Identifier {
			get { return identifier; }
			set { identifier = value; }
		}
		public override bool HasIdentifierProperty {
			get { return identifierProperty != null; }
		}

		public override Value Discriminator {
			get { return discriminator; }
			set { discriminator = value; }
		}

		public override bool IsInherited {
			get { return false; }
		}
		
		/// <summary>
		/// Indicates if the object has subclasses
		/// </summary>
		/// <remarks>
		/// This value is set to True when a subclass is added and should not be set
		/// through any other method - so no setter is declared for this property.
		/// </remarks>
		public override bool IsPolymorphic {
			get { return polymorphic; }
			//set { polymorphic = value; }
		}

		public override RootClass RootClazz {
			get { return this; }
		}
		public override ICollection PropertyClosureCollection {
			get { return PropertyCollection; }
		}
		
		/// <summary>
		/// Returns all of the Tables the Root class covers.
		/// </summary>
		/// <remarks>
		/// The RootClass should only have one item in the Collection - the Table that it comes from.
		/// TODO: why an ICollection - I don't know why the signature calls for an ICollection.
		/// </remarks>
		public override ICollection TableClosureCollection {
			get { 
				ArrayList retVal = new ArrayList();
				retVal.Add( Table );
				return retVal;
			}
		}
		public override void AddSubclass(Subclass subclass) {
			base.AddSubclass(subclass);
			polymorphic = true;
		}

		public override bool IsExplicitPolymorphism {
			get { return explicitPolymorphism; }
			set { explicitPolymorphism = value; }
		}

		public override Property Version {
			get { return version; }
			set { version = value; }
		}
		public override bool IsVersioned {
			get { return version != null; }
		}

		public override ICacheConcurrencyStrategy Cache {
			get { return cache; }
			set { cache = value; }
		}

		public override bool IsMutable {
			get { return mutable; }
			set { mutable = value; }
		}
		public override bool HasEmbeddedIdentifier {
			get { return embeddedIdentifier; }
			set { embeddedIdentifier = value; }
		}

		public override System.Type Persister {
			get { return persister; }
			set { persister = value; }
		}

		public override Table RootTable {
			get { return Table; }
		}

		public override PersistentClass Superclass {
			get { return null; }
			set { throw new InvalidOperationException(); }
		}

		public override Value Key {
			get { return Identifier; }
			set { throw new InvalidOperationException(); }
		}

		public override bool IsForceDiscriminator {
			get { return forceDiscriminator; }
			set { this.forceDiscriminator = value; }
		}
	}
}
