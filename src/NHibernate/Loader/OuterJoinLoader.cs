using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader
{
	/// <summary>
	/// Implements logic for walking a tree of associated classes.
	/// </summary>
	/// <remarks>
	/// Generates an SQL select string containing all properties of those classes.
	/// Tablse are joined using an ANSI-style left outer join.
	/// </remarks>
	public class OuterJoinLoader : BasicLoader
	{
		// Having these fields as protected prevents CLS compliance, so they are
		// private in NHibernate, and setters are created for the relevant
		// properties.
		private ILoadable[] persisters;
		private ICollectionPersister[] collectionPersisters;
		private int[] collectionOwners;
		private string[] aliases;
		private LockMode[] lockModeArray;
		private int[] owners;
		private EntityType[] ownerAssociationTypes;
		private SqlString sql;
		private string[] suffixes;
		private string[] collectionSuffixes;

		private readonly IDictionary<string, IFilter> enabledFilters;

		public OuterJoinLoader(ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory)
		{
			this.enabledFilters = enabledFilters;
		}

		protected override string[] Suffixes
		{
			get { return suffixes; }
			set { suffixes = value; }
		}

		protected override string[] CollectionSuffixes
		{
			get { return collectionSuffixes; }
			set { collectionSuffixes = value; }
		}

		protected internal override SqlString SqlString
		{
			get { return sql; }
			set { sql = value; }
		}

		protected internal override ILoadable[] EntityPersisters
		{
			get { return persisters; }
			set { persisters = value; }
		}

		protected override int[] Owners
		{
			get { return owners; }
			set { owners = value; }
		}

		protected override EntityType[] OwnerAssociationTypes
		{
			get { return ownerAssociationTypes; }
		}

		protected internal override LockMode[] GetLockModes(IDictionary lockModes)
		{
			return lockModeArray;
		}

		public IDictionary<string, IFilter> EnabledFilters
		{
			get { return enabledFilters; }
		}

		protected override string[] Aliases
		{
			get { return aliases; }
		}

		protected override ICollectionPersister[] CollectionPersisters
		{
			get { return collectionPersisters; }
		}

		protected override int[] CollectionOwners
		{
			get { return collectionOwners; }
		}

		protected void InitFromWalker(JoinWalker walker)
		{
			persisters = walker.Persisters;
			collectionPersisters = walker.CollectionPersisters;
			ownerAssociationTypes = walker.OwnerAssociationTypes;
			lockModeArray = walker.LockModeArray;
			suffixes = walker.Suffixes;
			collectionSuffixes = walker.CollectionSuffixes;
			owners = walker.Owners;
			collectionOwners = walker.CollectionOwners;
			sql = walker.SqlString;
			aliases = walker.Aliases;
		}
	}
}