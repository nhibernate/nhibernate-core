using System;
using System.Text;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {

	public enum OuterJoinLoaderType {
		Eager = 1,
		Auto = 0,
		Lazy = -1
	}
	
	public class OuterJoinLoader : Loader {
		
		protected static readonly IType[] NoTypes = new IType[0];
		protected static readonly string[][] NoStringArrays = new string[0][];
		protected static readonly string[] NoStrings = new string[0];
		protected static readonly ILoadable[] NoPersisters = new ILoadable[0];
		protected ILoadable[] classPersisters;
		protected string sql;
		protected string[] suffixes;
		private Dialect.Dialect dialect;

		public OuterJoinLoader(Dialect.Dialect dialect) {
			this.dialect = dialect;
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
		public IList WalkTree(ILoadable persister, string alias, ISessionFactoryImplementor session) {
			IList associations = new ArrayList();
			WalkTree(persister, alias, associations, new ArrayList(), session);
			return associations;
		}

		/// <summary>
		/// For a collection role, return a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected IList WalkTree(CollectionPersister persister, string alias, ISessionFactoryImplementor session) {
			IList associations = new ArrayList();

			if ( session.EnableJoinedFetch ) {

				IType type = persister.ElementType;
				if (type.IsEntityType) {
					EntityType etype = (EntityType) type;
					if ( AutoEager( persister.EnableJoinFetch, etype, session) ) {
						string[] columns = StringHelper.Prefix( persister.ElementColumnNames, alias + StringHelper.Dot);
						WalkTree(etype, columns, persister, associations, new ArrayList(), session);
					}
				} else if (type.IsComponentType) {
					WalkTree( (IAbstractComponentType) type, persister.ElementColumnNames, persister, alias, associations, new ArrayList(), session);
				}
			}

			return associations;
		}

		/// <summary>
		/// For an entity class, add to a list of associations to be fetched by outerjoin
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="alias"></param>
		/// <param name="associations"></param>
		/// <param name="classPersisters"></param>
		/// <param name="session"></param>
		private void WalkTree(ILoadable persister, string alias, IList associations, IList classPersisters, ISessionFactoryImplementor session) {
			if ( !session.EnableJoinedFetch ) return;

			int n = persister.CountSubclassProperties();
			for (int i=0; i<n; i++) {
				IType type = persister.GetSubclassPropertyType(i);
				if (type.IsEntityType) {
					EntityType etype = (EntityType) type;
					if (AutoEager ( persister.EnableJoinedFetch(i), etype, session)) {
						
						string[] columns;
						if (etype.IsOneToOne) {

							if ( persister.IsDefinedOnSubclass(i) ) continue;

							columns = StringHelper.Prefix(
								( (ILoadable) persister ).IdentifierColumnNames,
								alias + StringHelper.Dot
								);
						} else {
							columns = persister.ToColumns(alias, i);
						}

						WalkTree(etype, columns, persister, associations, classPersisters, session);
					}
				} else if ( type.IsComponentType ) {
					string[] columns = persister.GetSubclassPropertyColumnNames(i);
					WalkTree( (IAbstractComponentType) type, columns, persister, alias, associations, classPersisters, session);
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
		private void WalkTree(IAbstractComponentType act, string[] cols, object persister, string alias, IList associations, IList classPersisters, ISessionFactoryImplementor session) {
			if (
				!session.EnableJoinedFetch ||
				persister is NormalizedEntityPersister
				) return;

			IType[] types = act.Subtypes;
			int begin = 0;
			for (int i=0; i<types.Length; i++) {
				int length = types[i].GetColumnSpan(session);
				string[] range = ArrayHelper.Slice(cols, begin, length);
				if ( types[i].IsEntityType ) {
					EntityType etype = (EntityType) types[i];
					if ( AutoEager( act.EnableJoinedFetch(i), etype, session ) ) {
						string[] columns = StringHelper.Prefix( range, alias + StringHelper.Dot);
						WalkTree(etype, columns, persister, associations, classPersisters, session);
					}
				} else if ( types[i].IsComponentType ) {
					WalkTree ( (IAbstractComponentType) types[i], range, persister, alias, associations, classPersisters, session);
				}
				begin+=length;
			}
		}

		protected bool AutoEager(OuterJoinLoaderType config, EntityType type, ISessionFactoryImplementor session) {
			if (config==OuterJoinLoaderType.Eager) return true;
			if (config==OuterJoinLoaderType.Lazy) return false;
			IClassPersister persister = session.GetPersister( type.PersistentClass );
			return !persister.HasProxy || ( type.IsOneToOne && ((OneToOneType) type).IsNullable );
		}

		private void WalkTree(EntityType type, string[] columns, object persister, IList associations, IList classPersisters, ISessionFactoryImplementor session) {

			ILoadable subpersister = (ILoadable) session.GetPersister( type.PersistentClass );

			if ( !classPersisters.Contains(subpersister) ) {
				OuterJoinableAssociation assoc = new OuterJoinableAssociation();
				associations.Add(assoc);
				classPersisters.Add(persister);
				assoc.Subpersister = subpersister;
				assoc.ForeignKeyColumns = columns;
				string subalias = Alias( subpersister.ClassName, associations.Count );
				assoc.Subalias = subalias;
				WalkTree(subpersister, subalias, associations, classPersisters, session);
			}
		}

		public override string SQLString {
			get { return sql; }
		}
		public override ILoadable[] Persisters {
			get { return classPersisters; } 
		}

		/// <summary>
		/// Generate a select list of columns containing all properties of the entity classes
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		public string SelectString(IList associations) {
			StringBuilder buf = new StringBuilder( associations.Count * 100 );
			for (int i=0; i<associations.Count; i++) {
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
		protected string SelectString(ILoadable persister, string alias, string suffix) {
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
		private void AppendSelectString(StringBuilder buf, ILoadable persister, string alias, string suffix) {
			buf.Append( persister.IdentifierSelectFragment(alias,suffix) )
				.Append( persister.PropertySelectFragment(alias, suffix) );
		}

		protected override string[] Suffixes {
			get { return suffixes; }
			set { suffixes = value; }
		}

		protected string Alias(string tableName, int n) {
			tableName = StringHelper.Unqualify(tableName);

			return StringHelper.Suffix(
				tableName.Length <=5 ? tableName : tableName.Substring(0,5),
				n.ToString() + StringHelper.Underscore
				);
		}

		protected override CollectionPersister CollectionPersister {
			get { return null; }
		}

		/// <summary>
		/// Generate a sequence of <c>LEFT OUTER JOIN</c> clauses for the given associations
		/// </summary>
		/// <param name="associations"></param>
		/// <returns></returns>
		public JoinFragment OuterJoins(IList associations) {
			JoinFragment outerjoin = dialect.CreateOuterJoinFragment();
			foreach(OuterJoinLoader.OuterJoinableAssociation oj in associations) {
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
		
	}
}
