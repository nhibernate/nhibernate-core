using System;
using System.Collections;
using System.Text;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {
	/// <summary>
	/// Load an entity using outerjoin fetching to fetch associated entities
	/// </summary>
	public class EntityLoader : AbstractEntityLoader, IUniqueEntityLoader {
		private IType[] idType;

		public EntityLoader(ILoadable persister, ISessionFactoryImplementor factory) : base(persister, factory) {
			idType = new IType[] { persister.IdentifierType };
		
			SqlSelectBuilder selectBuilder = new SqlSelectBuilder(factory);
			selectBuilder.SetWhereClause(alias, persister.IdentifierColumnNames, persister.IdentifierType);
			RenderStatement(selectBuilder, factory);

			this.sqlString = selectBuilder.ToSqlString();
			PostInstantiate();

		}

		public object Load(ISessionImplementor session, object id, object obj) {
			
			IList list = LoadEntity(session, new object[] { id }, idType, obj, id, false);
			if (list.Count==1) {
				int len = classPersisters.Length;
				return ((object[]) list[0])[len-1];
			} 
			else if (list.Count==0) {
				return null;
			} 
			else {
				throw new HibernateException(
					"More than one row with the given identifier was found: " +
					id +
					", for class: " +
					persister.ClassName);
			}
		}
	}
}
