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
		public BooleanLiteralNode(IToken token) : base(token)
		{
		}

		public override IType DataType
		{
			get { return ExpectedType ?? NHibernateUtil.Boolean; }
			set { base.DataType = value; }
		}

		private ILiteralType TypeAsLiteralType()
		{
			return (ILiteralType) DataType;
		}

		private bool GetValue() {
			return Type == HqlSqlWalker.TRUE;
		}

		public IType ExpectedType { get; set; }

		public override SqlString RenderText(ISessionFactoryImplementor sessionFactory) 
		{
			try
			{
				return new SqlString(TypeAsLiteralType().ObjectToSQLString( GetValue(), sessionFactory.Dialect ));
			}
			catch( Exception t )
			{
				throw new QueryException( "Unable to render boolean literal value", t );
			}
		}
	}
}
