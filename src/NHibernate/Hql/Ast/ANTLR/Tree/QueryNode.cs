using Antlr.Runtime;
using log4net;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	public class QueryNode : AbstractRestrictableStatement, ISelectExpression
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(QueryNode));

		private OrderByClause _orderByClause;

		public QueryNode(IToken token) : base(token)
		{
		}

		protected override ILog GetLog()
		{
			return log;
		}

		protected override int GetWhereClauseParentTokenType()
		{
			return HqlSqlWalker.FROM;
		}

		public override bool NeedsExecutor
		{
			get { return false; }
		}

		public override int StatementType
		{
			get { return HqlSqlWalker.QUERY; }
		}

		public override IType DataType
		{
			get
			{
				return ((ISelectExpression)GetSelectClause().GetFirstSelectExpression()).DataType;
			}
			set
			{
				base.DataType = value;
			}
		}

		public void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i);
		}

		public FromElement FromElement
		{
			get { return null; }
		}

		public bool IsConstructor
		{
			get { return false; }
		}

		public bool IsReturnableEntity
		{
			get { return false; }
		}

		public bool IsScalar
		{
			get { return true; }
		}

		public string Alias
		{
			get;
			set;
		}

		public OrderByClause GetOrderByClause() 
		{
			if (_orderByClause == null) 
			{
				_orderByClause = LocateOrderByClause();

				// if there is no order by, make one
				if (_orderByClause == null)
				{
					log.Debug( "getOrderByClause() : Creating a new ORDER BY clause" );
					_orderByClause = (OrderByClause) Walker.ASTFactory.CreateNode(HqlSqlWalker.ORDER, "ORDER");

					// Find the WHERE; if there is no WHERE, find the FROM...
					IASTNode prevSibling = ASTUtil.FindTypeInChildren(this, HqlSqlWalker.WHERE);

					if ( prevSibling == null ) 
					{
						prevSibling = ASTUtil.FindTypeInChildren(this, HqlSqlWalker.FROM);
					}

					// Now, inject the newly built ORDER BY into the tree
					prevSibling.AddSibling(_orderByClause);
				}
			}
			return _orderByClause;
		}

		/// <summary>
		/// Locate the select clause that is part of this select statement.
		/// Note, that this might return null as derived select clauses (i.e., no
		/// select clause at the HQL-level) get generated much later than when we
		/// get created; thus it depends upon lifecycle.
		/// </summary>
		/// <returns>Our select clause, or null.</returns>
		public SelectClause GetSelectClause() 
		{
			// Due to the complexity in initializing the SelectClause, do not generate one here.
			// If it is not found; simply return null...
			//
			// Also, do not cache since it gets generated well after we are created.
			return ( SelectClause ) ASTUtil.FindTypeInChildren( this, HqlSqlWalker.SELECT_CLAUSE );
		}

		private OrderByClause LocateOrderByClause()
		{
			return (OrderByClause)ASTUtil.FindTypeInChildren(this, HqlSqlWalker.ORDER);
		}
	}
}
