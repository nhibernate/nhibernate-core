using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Collection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader 
{

	public enum OuterJoinLoaderType 
	{
		Lazy = -1, 
		Auto = 0,
		Eager = 1
	}
	
	public class OuterJoinLoader : Loader 
	{
		
		protected static readonly IType[] NoTypes = new IType[0];
		protected static readonly string[][] NoStringArrays = new string[0][];
		protected static readonly string[] NoStrings = new string[0];
		protected static readonly ILoadable[] NoPersisters = new ILoadable[0];
		protected ILoadable[] classPersisters;
		protected LockMode[] lockModeArray;
		
		protected SqlString sqlString;
		protected string[] suffixes;

		
		public OuterJoinLoader(Dialect.Dialect dialect) : base(dialect)
		{
		}

		/// <summary>
		/// Override on subclasses to enable or suppress joining of some associations
		/// </summary>
		/// <param name="mappingDefault"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <returns></returns>
		protected virtual bool EnableJoinedFetch(bool mappingDefault, string path, string table, string[] foreignKeyColumns) {
			return mappingDefault;
		}

		public sealed class OuterJoinableAssociation {
			public ILoadable Subpersister;
			public string[] ForeignKeyColumns;
			public string Subalias;
		}

		/// <summary>
		/// For an entity class, return a list of associations to be fetched by outerjion
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		public IList WalkTree(ILoadable persister, string alias, ISessionFactoryImplementor session) 
		{
			IList associations = new ArrayList();
			WalkClassTree(persister, alias, associations, new ArrayList(), String.Empty, session);
			return associations;
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected IList WalkCollectionTree(CollectionPersister persister, string alias, ISessionFactoryImplementor session) 
		{
			IList associations = new ArrayList();

			if ( session.EnableJoinedFetch ) 
			{
				IType type = persister.ElementType;
				if (type.IsEntityType) 
				{
					EntityType etype = (EntityType) type;
					// we do NOT need to call this.EnableJoinedFetch() here
					if ( AutoEager( persister.EnableJoinFetch, etype, session) ) 
					{
						string[] columns = StringHelper.Prefix( persister.ElementColumnNames, alias + StringHelper.Dot);
						WalkAssociationTree(etype, columns, persister, alias, associations, new ArrayList(),  String.Empty, session);
					}
				} 
				else if (type.IsComponentType) 
				{
					WalkCompositeElementTree( (IAbstractComponentType) type, persister.ElementColumnNames, persister, alias, associations, new ArrayList(), String.Empty, session);
				}
			}

			return associations;
		}

		/// <summary>
		///  Add on association (one-to-one or many-to-one) to a list of associations be fetched by outerjoin (if necessary)
		/// </summary>
		/// <param name="type"></param>
		/// <param name="columns"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="classPersisters"></param>
		/// <param name="path"></param>
		/// <param name="session"></param>
		private void WalkAssociationTree(
			EntityType type,
			string[] columns,
			object persister,
			string alias,
			IList associations,
			IList classPersisters,
			string path,
			ISessionFactoryImplementor session) 
		{

			ILoadable subpersister = (ILoadable)session.GetPersister(type.PersistentClass);

			// to avoid navigating back up bidirectional associations (and circularities) 
			if(!classPersisters.Contains(subpersister)) {
				OuterJoinableAssociation assoc = new OuterJoinableAssociation();
				associations.Add(assoc);
				classPersisters.Add(persister);
				assoc.Subpersister = subpersister;
				assoc.ForeignKeyColumns = columns;
				string subalias = Alias(subpersister.ClassName, associations.Count);
				assoc.Subalias = subalias;

				WalkClassTree(subpersister, subalias, associations, classPersisters, path, session);
			
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="etype"></param>
		/// <param name="persister"></param>
		/// <param name="propertyNumber"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="classPersisters"></param>
		/// <param name="path"></param>
		/// <param name="session"></param>
		private void WalkAssociationTree (
			EntityType etype,
			ILoadable persister,
			int propertyNumber,
			string alias,
			IList associations,
			IList classPersisters,
			string path,
			ISessionFactoryImplementor session) {

			bool autoEager = AutoEager(persister.EnableJoinedFetch(propertyNumber), etype, session);
			string[] columns;

			if(etype.IsOneToOne) {
				//TODO: NOTE: workaround for problem with 1-to-1 defined on a subclass "accidently" picking up an object
				if(persister.IsDefinedOnSubclass(propertyNumber)) return;

				columns = StringHelper.Prefix(
					persister.IdentifierColumnNames, //The cast is safe because collections can't contain a 1-to-1
					alias + StringHelper.Dot);
			}
			else {
				columns = persister.ToColumns(alias, propertyNumber);
			}
            
			string subpath = SubPath(path, persister.GetSubclassPropertyName(propertyNumber));
			bool enable = EnableJoinedFetch(autoEager, subpath, persister.GetSubclassPropertyTableName(propertyNumber), persister.GetSubclassPropertyColumnNames(propertyNumber) );
			if (enable) WalkAssociationTree(etype, columns, persister, alias, associations, classPersisters, subpath, session);
		}


		/// <summary>
		/// For an entity class, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="classPersisters"></param>
		/// <param name="path"></param>
		/// <param name="session"></param>
		private void WalkClassTree(ILoadable persister, string alias, IList associations, IList classPersisters, string path, ISessionFactoryImplementor session) 
		{
			if ( !session.EnableJoinedFetch ) return;

			int n = persister.CountSubclassProperties();
			for (int i=0; i<n; i++) 
			{
				IType type = persister.GetSubclassPropertyType(i);
				if (type.IsEntityType) {
					WalkAssociationTree (
						(EntityType)type,
						persister,
						i,
						alias,
						associations,
						classPersisters,
						path,
						session
					);
					
				} 
				else if ( type.IsComponentType ) 
				{
					string subpath = SubPath(path, persister.GetSubclassPropertyName(i) );
					string[] columns = persister.GetSubclassPropertyColumnNames(i);
					string[] aliasedColumns = persister.ToColumns(alias, i);

					WalkComponentTree( 
						(IAbstractComponentType) type, 
						i, 
						columns, 
						aliasedColumns,
						persister, 
						alias, 
						associations, 
						classPersisters, 
						subpath, 
						session
					);
				}
			}
		}

		/// <summary>
		/// For a component, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="act"></param>
		/// <param name="cols"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="assocaitions"></param>
		/// <param name="classPersisters"></param>
		/// <param name="session"></param>
		private void WalkComponentTree(
			IAbstractComponentType act, 
			int propertyNumber, 
			string[] cols, 
			string[] aliasedCols, 
			ILoadable persister, 
			string alias, 
			IList associations, 
			IList classPersisters, 
			string path,
			ISessionFactoryImplementor session) 
		{

			if (!session.EnableJoinedFetch ) return;

			IType[] types = act.Subtypes;
			string[] propertyNames = act.PropertyNames;
			int begin = 0;
			for (int i=0; i<types.Length; i++) 
			{
				int length = types[i].GetColumnSpan(session);
				string[] range = ArrayHelper.Slice(cols, begin, length);
				string[] aliasedRange = ArrayHelper.Slice(aliasedCols, begin, length);

				if ( types[i].IsEntityType ) 
				{
					EntityType etype = (EntityType) types[i];
					
					//TODO: workaround for problem with 1-to-1 defined on a subclass
					if (etype.IsOneToOne) continue;
					
					string subpath = SubPath(path, propertyNames[i]);
					bool autoEager = AutoEager(act.EnableJoinedFetch(i), etype, session);
				
					bool enable = EnableJoinedFetch(autoEager, subpath, persister.GetSubclassPropertyTableName(propertyNumber), range);
					
					if(enable)
						WalkAssociationTree(etype, aliasedRange, persister, alias, associations, classPersisters, subpath, session);

				} 
				else if ( types[i].IsComponentType ) 
				{
					string subpath = SubPath(path, propertyNames[i]);

					WalkComponentTree ( (IAbstractComponentType) types[i], propertyNumber, range, aliasedRange, persister, alias, associations, classPersisters, subpath, session);
				}

				begin+=length;
			}
		}

		/// <summary>
		/// For a composite element, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="act"></param>
		/// <param name="cols"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="classPersisters"></param>
		/// <param name="path"></param>
		/// <param name="session"></param>
		private void WalkCompositeElementTree (
			IAbstractComponentType act,
			string[] cols,
			CollectionPersister persister,
			string alias,
			IList associations,
			IList classPersisters,
			string path,
			ISessionFactoryImplementor session ) 
		{
			if(!session.EnableJoinedFetch) return;

			IType[] types = act.Subtypes;
			string[] propertyNames = act.PropertyNames;
			int begin = 0;

			for(int i=0; i < types.Length; i++)
			{
				int length = types[i].GetColumnSpan(session);
				string[] range = ArrayHelper.Slice(cols, begin, length);
 
				if(types[i].IsEntityType) 
				{
					EntityType etype = (EntityType) types[i];
					string subpath = SubPath(path, propertyNames[i]);
					bool autoEager = AutoEager(act.EnableJoinedFetch(i), etype, session );
					bool enable = EnableJoinedFetch(autoEager, subpath, persister.QualifiedTableName, range);

					if(enable) 
					{
						string[] columns = StringHelper.Prefix(range, alias + StringHelper.Dot);
						WalkAssociationTree(etype, columns, persister, alias, associations, classPersisters, subpath, session);
					}
				}
				else if(types[i].IsComponentType) 
				{
					string subpath = SubPath(path, propertyNames[i]);
					WalkCompositeElementTree(
						(IAbstractComponentType) types[i],
						range,
						persister,
						alias,
						associations,
						classPersisters,
						subpath,
						session
					);
															
				}
				begin+=length;
			}
			
		}


		protected bool AutoEager(OuterJoinLoaderType config, EntityType type, ISessionFactoryImplementor session) 
		{
			if (config==OuterJoinLoaderType.Eager) return true;
			if (config==OuterJoinLoaderType.Lazy) return false;
			IClassPersister persister = session.GetPersister( type.PersistentClass );
			return !persister.HasProxy || ( type.IsOneToOne && ((OneToOneType) type).IsNullable );
		}

		public override SqlString SqlString 
		{
			get { return sqlString;}
		}


		public override ILoadable[] Persisters 
		{
			get { return classPersisters; } 
		}

		/// <summary>
		/// Generate a select list of columns containing all properties of the entity classes
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		public string SelectString(IList associations) {
			StringBuilder buf = new StringBuilder( associations.Count * 100 );
			for (int i=0; i<associations.Count; i++) 
			{
				OuterJoinableAssociation join = (OuterJoinableAssociation) associations[i];
				AppendSelectString( buf, join.Subpersister, join.Subalias, Suffixes[i] );
				if ( i<associations.Count-1) buf.Append(StringHelper.CommaSpace);
			}
			return buf.ToString();
		}

		/// <summary>
		/// Generate a list of columns containing all properties of the entity class
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		protected static string SelectString(ILoadable persister, string alias, string suffix) 
		{
			StringBuilder buf = new StringBuilder(30);
			AppendSelectString(buf, persister, alias, suffix);
			return buf.ToString();
		}

		/// <summary>
		/// Append a list of columns containing all properties of the entity class
		/// </summary>
		/// <param name="buf"></param>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="suffix"></param>
		private static void AppendSelectString(StringBuilder buf, ILoadable persister, string alias, string suffix) 
		{
			buf.Append( persister.IdentifierSelectFragment(alias,suffix) )
				.Append( persister.PropertySelectFragment(alias, suffix) );
		}

		protected override string[] Suffixes 
		{
			get { return suffixes; }
			set { suffixes = value; }
		}

		protected string Alias(string tableName, int n) 
		{
			tableName = StringHelper.Unqualify(tableName); //TODO: this is broken if we have quoted identifier with a "."

			//TODO: H2.0.3 - changes tableName to lower case - don't know why it is needed...
			return (new Alias(10, n.ToString() + StringHelper.Underscore)).ToAliasString(tableName, dialect);
		}

		protected override CollectionPersister CollectionPersister 
		{
			get { return null; }
		}

		/// <summary>
		/// Generate a sequence of <c>LEFT OUTER JOIN</c> clauses for the given associations
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		public JoinFragment OuterJoins(IList associations) 
		{
			JoinFragment outerjoin = dialect.CreateOuterJoinFragment();
			foreach(OuterJoinLoader.OuterJoinableAssociation oj in associations) 
			{
				outerjoin.AddJoin(
					oj.Subpersister.TableName,
					oj.Subalias,
					oj.ForeignKeyColumns,
					oj.Subpersister.IdentifierColumnNames,
					JoinType.LeftOuterJoin
					);
				outerjoin.AddJoins(
					oj.Subpersister.FromJoinFragment(oj.Subalias, false, true),
					oj.Subpersister.WhereJoinFragment(oj.Subalias, false, true)
					);
			}
			return outerjoin;
		}

		protected override LockMode[] GetLockModes(IDictionary lockModes)
		{
			return lockModeArray;
		}

		protected LockMode[] createLockModeArray(int length, LockMode lockMode) 
		{
			LockMode[] lmArray = new LockMode[length];
			for(int i = 0 ; i < length; i++) 
			{
				lmArray[i] = lockMode;
			}
			return lmArray;
		}

		private string SubPath(string path, string property) 
		{
			if(path==null || path.Length==0) 
			{
				return property;
			}
			else 
			{
				return path + StringHelper.Dot + property;
			}
		}
		
	}
}
