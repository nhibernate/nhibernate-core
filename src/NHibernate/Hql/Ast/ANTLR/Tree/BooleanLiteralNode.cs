using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	///<summary>
	/// Represents a boolean literal within a query.
	///</summary>
	[CLSCompliant(false)]
	public class BooleanLiteralNode : LiteralNode, IExpectedTypeAwareNode 
	{
		private IType _expectedType;

		public BooleanLiteralNode(IToken token) : base(token)
		{
		}

		public override IType DataType
		{
			get
			{
				return _expectedType ?? NHibernateUtil.Boolean;
			}
			set
			{
				base.DataType = value;
			}
		}

		private BooleanType GetTypeInternal() 
		{
			return ( BooleanType ) DataType;
		}

		private bool GetValue() {
			return Type == HqlSqlWalker.TRUE ? true : false;
		}

		/**
		 * Expected-types really only pertinent here for boolean literals...
		 *
		 * @param expectedType
		 */
		public IType ExpectedType
		{
			get { return _expectedType; }
			set { _expectedType = value; }
		}

		public override SqlString RenderText(ISessionFactoryImplementor sessionFactory) 
		{
			try
			{
				return new SqlString(GetTypeInternal().ObjectToSQLString( GetValue(), sessionFactory.Dialect ));
			}
			catch( Exception t )
			{
				throw new QueryException( "Unable to render boolean literal value", t );
			}
		}
	}
}
