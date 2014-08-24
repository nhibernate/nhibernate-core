using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public abstract class AbstractSelectExpression : HqlSqlWalkerNode, ISelectExpression 
	{
		private string _alias;
		
		protected AbstractSelectExpression(IToken token) : base(token)
		{
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}
		
		public bool IsConstructor
		{
			get { return false; }
		}

		public virtual bool IsReturnableEntity
		{
			get { return false; }
		}

		public virtual FromElement FromElement 
		{
			get { return null; }
			set {}
		}

		public virtual bool IsScalar
		{
			get
			{
				// Default implementation:
				// If this node has a data type, and that data type is not an association, then this is scalar.
				return DataType != null && !DataType.IsAssociationType; // Moved here from SelectClause [jsd]
			}
		}

		public abstract void SetScalarColumnText(int i);
	}
}
