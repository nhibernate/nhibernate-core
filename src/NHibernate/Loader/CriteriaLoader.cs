using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate;
using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Impl;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {
	/// <summary>
	/// Summary description for CriteriaLoader.
	/// </summary>
	public class CriteriaLoader : AbstractEntityLoader {
		
		private ICriteria criteria;
		
		public CriteriaLoader(ILoadable persister, ISessionFactoryImplementor factory, ICriteria criteria) : base(persister, factory) {

			this.criteria = criteria;
			
			IEnumerator iter = criteria.IterateExpressions();
			
			StringBuilder orderByBuilder = new StringBuilder(60);

			bool orderByNeeded = true;
			bool commaNeeded = false;
			iter = criteria.IterateOrderings(); 
			
			while ( iter.MoveNext() ) { 
				if(orderByNeeded) orderByBuilder.Append(" ORDER BY ");//condition.Append(" order by ");
				orderByNeeded = false;

				Order ord = (Order) iter.Current; 
				orderByBuilder.Append(ord.ToStringForSql(factory, criteria.PersistentClass, alias));

				if(commaNeeded) orderByBuilder.Append(StringHelper.CommaSpace); //condition.Append(", ");
				commaNeeded = true;

			} 

			//RenderStatement(criteria.Expression.ToSqlFragment(factory, criteria.PersistentClass, alias), orderByBuilder.ToString(), factory);
			RenderStatement(criteria.Expression.ToSqlString(factory, criteria.PersistentClass, alias), orderByBuilder.ToString(), factory);
			
			PostInstantiate();
			
		}
	
		public IList List(ISessionImplementor session) {
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();
			IEnumerator iter = criteria.IterateExpressions();
			while ( iter.MoveNext() ) { 
				Expression.Expression expr = (Expression.Expression) iter.Current;
				TypedValue[] tv = expr.GetTypedValues( session.Factory, criteria.PersistentClass );
				for ( int i=0; i<tv.Length; i++ ) {
					values.Add( tv[i].Value );
					types.Add( tv[i].Type );
				}
			}
			object[] valueArray = values.ToArray();
			IType[] typeArray = (IType[]) types.ToArray(typeof(IType));
		
			RowSelection selection = new RowSelection();
			selection.FirstRow = criteria.FirstResult;
			selection.MaxRows = criteria.MaxResults;
			selection.Timeout = criteria.Timeout;
		
			return Find(session, valueArray, typeArray, true, selection, null, null);
		}

		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) {
			return row[ row.Length-1 ];
		}
	}
}