using System;
using System.Data;
using System.Collections;

using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Mapping;
using NHibernate.Metadata;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection {

	/// <summary>
	/// Plugs into an instance of <c>PersistentCollection</c>, in order to implement
	/// persistence of that collection while in a particular role.
	/// </summary>
	/// <remarks>
	/// May be considered an immutable view of the mapping object
	/// </remarks>
	public sealed class CollectionPersister { //: ICollectionMetadata {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(CollectionPersister));

		private string sqlSelectString;
		private string sqlDeleteString;
		private string sqlInsertRowString;
		private string sqlUpdateRowString;
		private string sqlDeleteRowString;
		private string sqlOrderByString;
		private string sqlWhereString;
		private bool hasOrder;
		private bool hasWhere;
		private bool isSet;
		private IType keyType;
		private IType indexType;
		private IType elementType;
		private string[] keyColumnNames;
		private string[] indexColumnNames;
		private string[] unquotedIndexColumnNames;
		private string[] elementColumnNames;
		private string[] unquotedElementColumnNames;
		private string[] rowSelectColumnNames;
		private IType rowSelectType;
		private bool primitiveArray;
		private bool array;
		private bool isOneToMany;
		private string qualifiedTableName;
		private bool hasIndex;
		private bool isLazy;
		private bool isInverse;
		private System.Type elementClass;
		private ICacheConcurrencyStrategy cache;
		private PersistentCollectionType collectionType;
		private int enableJoinedFetch;
		private System.Type ownerClass;

		private ICollectionInitializer loader;

		private string role;

		//TODO: Finish


		public string QualifiedTableName {
			get { return qualifiedTableName; }
		}
	}
}
