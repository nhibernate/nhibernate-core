using System;
using System.Collections;
using System.Data;
using System.Text;

using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Type;
using NHibernate.Impl;
using NHibernate.Persister;

namespace NHibernate.Loader {
	/// <summary>
	/// Summary description for CriteriaLoader.
	/// </summary>
	public class CriteriaLoader : AbstractEntityLoader {
		
		private CriteriaImpl criteria;
		//private static readonly IType[] NoTypes = new IType[0];

		public CriteriaLoader(ILoadable persister, ISessionFactoryImplementor factory, CriteriaImpl criteria) : base(persister, factory) {

			this.criteria = criteria;
			StringBuilder condition = new StringBuilder(32);
			
			IEnumerator iter = criteria.IterateExpressions();

			//--- PORT NOTE ---
			//This block would be better with IIterator.

			if ( !iter.MoveNext() ) condition.Append(" 1=1"); //TODO: fix this ugliness
			iter.Reset();
			while ( iter.MoveNext() ) {
				Expression.Expression expr = (Expression.Expression) iter.Current;

				condition.Append( expr.ToSqlString(factory, criteria.PersistentClass, alias) );

				condition.Append(" and ");

			}
			condition.Remove(condition.Length-5,5); //remove last " and "
			
			//---

			RenderStatement( condition.ToString(), factory );
		}
	
		public IList List(ISessionImplementor session) {
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();
			IEnumerator iter = criteria.IterateExpressions();
			while ( iter.MoveNext() ) { //is the first lost?
				Expression.Expression expr = (Expression.Expression) iter.Current;
				TypedValue[] tv = expr.GetTypedValues( session.Factory, criteria.PersistentClass );
				for ( int i=0; i<tv.Length; i++ ) {
					values.Add( tv[i].Value );
					types.Add( tv[i].Type );
				}
			}
			object[] valueArray = values.ToArray();
			IType[] typeArray = (IType[]) types.ToArray(typeof(IType[]));
		
			RowSelection selection = new RowSelection();
			selection.FirstRow = criteria.FirstResult;
			selection.MaxRows = criteria.MaxResults;
			selection.Timeout = criteria.Timeout;
		
			return Find(session, valueArray, typeArray, true, selection, null);
		}

		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) {
			return row[ row.Length-1 ];
		}
	}
}