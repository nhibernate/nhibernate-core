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

namespace NHibernate.Loader 
{
	//TODO: this class depends directly upon CriteriaImpl, in the impl package
	// add a ICriteriaImplementor interface
	/// <summary>
	/// A <c>Loader</c> for <see cref="ICriteria"/> queries. 
	/// </summary>
	/// <remarks>
	/// Note that criteria
	/// queries are more like multi-object <c>Load()</c>s than like HQL queries.
	/// </remarks>
	public class CriteriaLoader : AbstractEntityLoader 
	{
		private ICriteria criteria;
		//private static readonly IType[] noTypes = new IType[0];

		public CriteriaLoader(ILoadable persister, ISessionFactoryImplementor factory, ICriteria criteria) : base(persister, factory) 
		{
			this.criteria = criteria;
			
			IEnumerator iter = criteria.IterateExpressions();
			
			StringBuilder orderByBuilder = new StringBuilder(60);

			// TODO: H2.0.3 has code here to iterateExpressions() - our code is in the Expression.ToSqlString()
			// code I believe - verify this.
			bool orderByNeeded = true;
			bool commaNeeded = false;
			iter = criteria.IterateOrderings(); 
			
			while ( iter.MoveNext() ) 
			{ 
				//TODO: H2.0.3 - this is not in H2.0.3 - where did it move to?
				//if(orderByNeeded) orderByBuilder.Append(" ORDER BY ");
				orderByNeeded = false;

				Order ord = (Order) iter.Current; 
				orderByBuilder.Append(ord.ToStringForSql(factory, criteria.PersistentClass, alias));
				
				if(commaNeeded) orderByBuilder.Append(StringHelper.CommaSpace); 
				commaNeeded = true;

			} 

			RenderStatement(criteria.Expression.ToSqlString(factory, criteria.PersistentClass, alias), orderByBuilder.ToString(), factory);
			
			PostInstantiate();
			
		}
	
		public IList List(ISessionImplementor session) 
		{
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();

			IEnumerator iter = criteria.IterateExpressions();
			while ( iter.MoveNext() ) 
			{ 
				Expression.Expression expr = (Expression.Expression) iter.Current;
				TypedValue[] tv = expr.GetTypedValues( session.Factory, criteria.PersistentClass );
				for ( int i=0; i<tv.Length; i++ ) 
				{
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

		protected override object GetResultColumnOrRow(object[] row, IDataReader rs, ISessionImplementor session) 
		{
			return row[ row.Length-1 ];
		}

		/// <summary>
		/// Navigate associations, returning the aliased columns.  Adds extra table joins
		/// to this loader.
		/// </summary>
		/// <param name="pathExpression"></param>
		/// <returns></returns>
		public string[] ToColumns(string pathExpression) 
		{
			return null;
		}

		protected override bool EnableJoinedFetch(bool mappingDefault, string path, string table, string[] foreignKeyColumns)
		{
			FetchMode fm = criteria.GetFetchMode(path);
			//fm==null ||  - an Enum can't be null
			if (fm==FetchMode.Default) 
			{
				return mappingDefault;
			}
			else 
			{
				return fm == FetchMode.Eager;
			}
		}

		protected override bool UseQueryWhereFragment
		{
			get	{return true;}
		}

	}
}