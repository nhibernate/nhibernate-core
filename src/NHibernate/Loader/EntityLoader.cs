using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader 
{
	/// <summary>
	/// Load an entity using outerjoin fetching to fetch associated entities
	/// </summary>
	public class EntityLoader : AbstractEntityLoader, IUniqueEntityLoader 
	{
		private IType[] idType;

		public EntityLoader(ILoadable persister, ISessionFactoryImplementor factory) : base(persister, factory) 
		{
			idType = new IType[] { persister.IdentifierType };
		
			SqlSelectBuilder selectBuilder = new SqlSelectBuilder(factory);
			selectBuilder.SetWhereClause( Alias, persister.IdentifierColumnNames, persister.IdentifierType );
			
			RenderStatement(selectBuilder, factory);
			this.SqlString = selectBuilder.ToSqlString();
			
			PostInstantiate();
		}

		public object Load(ISessionImplementor session, object id, object obj) 
		{
			IList list = LoadEntity(session, new object[] { id }, idType, obj, id, false);
			if (list.Count==1) 
			{
				return list[0];
			} 
			else if (list.Count==0) 
			{
				return null;
			} 
			else 
			{
				throw new HibernateException(
					"More than one row with the given identifier was found: " +
					id +
					", for class: " +
					persister.ClassName);
			}
		}

		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) 
		{
			return row[row.Length - 1];
		}
	}
}
