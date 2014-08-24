using System;
using System.Collections.Generic;
using System.Linq;
using Antlr.Runtime;
using NHibernate.Param;
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

		public override SqlString RenderText(Engine.ISessionFactoryImplementor sessionFactory)
		{
			var result = SqlString.Parse(Text);
			// query-parameter = the parameter specified in the NHibernate query
			// sql-parameter = real parameter/s inside the final SQL
			// here is where we suppose the SqlString has all sql-parameters in sequence for a given query-parameter.
			// This happen when the query-parameter spans multiple columns (components,custom-types and so on).
			if (HasEmbeddedParameters)
			{
				var parameters = result.GetParameters().ToArray();
				var sqlParameterPos = 0;
				var paramTrackers = _embeddedParameters.SelectMany(specification => specification.GetIdsForBackTrack(sessionFactory));
				foreach (var paramTracker in paramTrackers)
				{
					parameters[sqlParameterPos++].BackTrack = paramTracker;
				}
			}
			return result;
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
