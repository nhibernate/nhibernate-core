using System.Collections;
using Antlr.Runtime;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	class TransparentCastNode : MethodNode, IExpectedTypeAwareNode
	{
		private IType _expectedType;

		public const string Name = "transparentcast";

		public static bool IsTransparentCast(IASTNode node)
		{
			return node.Type == HqlSqlWalker.METHOD_CALL && node.Text == Name;
		}

		public TransparentCastNode(IToken token) : base(token)
		{
		}

		public IType ExpectedType
		{
			get => _expectedType;
			set
			{
				_expectedType = value;
				var node = GetChild(0).NextSibling.GetChild(0);
				//transparentcast on parameters is a special use case - skip it.
				if (node.Type != HqlSqlWalker.NAMED_PARAM && node is IExpectedTypeAwareNode  typeNode && typeNode.ExpectedType == null)
					typeNode.ExpectedType = value;
			}
		}

		public override SqlString Render(IList args)
		{
			return ExpectedType != null
				// Provide expected type in case transparent cast is transformed to real
				? ((CastFunction) SQLFunction).Render(args, ExpectedType, SessionFactoryHelper.Factory)
				: base.Render(args);
		}
	}
}
