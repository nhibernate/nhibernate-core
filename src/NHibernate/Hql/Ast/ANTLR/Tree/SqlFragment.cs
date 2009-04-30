using System;
using System.Collections.Generic;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Parameters;
using NHibernate.SqlCommand;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents an SQL fragment in the AST.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class SqlFragment : SqlNode, IParameterContainer 
	{
		private JoinFragment _joinFragment;
		private FromElement _fromElement;

		public SqlFragment(IToken token) : base(token)
		{
		}

		public void SetJoinFragment(JoinFragment joinFragment) 
		{
			_joinFragment = joinFragment;
		}

		public bool HasFilterCondition
		{
			get { return _joinFragment.HasFilterCondition; }
		}

		public FromElement FromElement
		{
			get { return _fromElement; }
			set { _fromElement = value; }
		}

		// ParameterContainer impl ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		private List<IParameterSpecification> _embeddedParameters;

		public void AddEmbeddedParameter(IParameterSpecification specification) 
		{
			if ( _embeddedParameters == null ) 
			{
				_embeddedParameters = new List<IParameterSpecification>();
			}
			_embeddedParameters.Add( specification );
		}

		public bool HasEmbeddedParameters 
		{
			get { return _embeddedParameters != null && _embeddedParameters.Count != 0; }
		}

		public IParameterSpecification[] GetEmbeddedParameters() 
		{
			return _embeddedParameters.ToArray();
		}
	}
}
