using System;
using Antlr.Runtime;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// A node representing a static Java constant.
	/// 
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class JavaConstantNode : SqlNode, IExpectedTypeAwareNode, ISessionFactoryAwareNode 
	{
		private ISessionFactoryImplementor _factory;
		private Object _constantValue;
		private IType _heuristicType;
		private IType _expectedType;
	    private bool _processedText;

		public JavaConstantNode(IToken token) : base(token)
		{
		}

		public IType ExpectedType
		{
			get { return _expectedType; }
			set { _expectedType = value; }
		}

		public ISessionFactoryImplementor SessionFactory
		{
			set { _factory = value; }
		}

        public override SqlString RenderText(ISessionFactoryImplementor sessionFactory)
        {
            ProcessText();

			IType type = _expectedType ?? _heuristicType;
			return new SqlString(ResolveToLiteralString( type ));
		}

        private string ResolveToLiteralString(IType type)
        {
            try
            {
                ILiteralType literalType = (ILiteralType)type;
                Dialect.Dialect dialect = _factory.Dialect;
                return literalType.ObjectToSQLString(_constantValue, dialect);
            }
            catch (Exception t)
            {
                throw new QueryException(LiteralProcessor.ErrorCannotFormatLiteral + Text, t);
            }
        }

        private void ProcessText()
        {
            if (!_processedText)
            {
                if (_factory == null)
                {
                    throw new InvalidOperationException();
                }
                
                _constantValue = ReflectHelper.GetConstantValue(base.Text, _factory);
                _heuristicType = TypeFactory.HeuristicType(_constantValue.GetType().AssemblyQualifiedName);
                _processedText = true;
            }
        }

	}
}
