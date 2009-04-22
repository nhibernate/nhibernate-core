using System;
using System.Data;
using NHibernate.Engine;

namespace NHibernate.Hql.Ast.ANTLR.Parameters
{
	/// <summary>
	/// Parameter bind specification for an explicit named parameter.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public class NamedParameterSpecification : AbstractExplicitParameterSpecification 
	{
		private readonly string _name;

		/// <summary>
		/// Constructs a named parameter bind specification.
		/// </summary>
		/// <param name="sourceLine">sourceLine</param>
		/// <param name="sourceColumn">sourceColumn</param>
		/// <param name="name">The named parameter name.</param>
		public NamedParameterSpecification(int sourceLine, int sourceColumn, String name) : base ( sourceLine, sourceColumn )
		{
			_name = name;
		}

		/// <summary>
		/// Bind the appropriate value into the given statement at the specified position.
		/// </summary>
		/// <param name="statement">The statement into which the value should be bound.</param>
		/// <param name="qp">The defined values for the current query execution.</param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <param name="position">The position from which to start binding value(s).</param>
		/// <returns>The number of sql bind positions "eaten" by this bind operation.</returns>
		public override int Bind(IDbCommand statement, QueryParameters qp, ISessionImplementor session, int position)
		{
			TypedValue typedValue = qp.NamedParameters[_name];
			typedValue.Type.NullSafeSet(statement, typedValue.Value, position, session );
			return typedValue.Type.GetColumnSpan( session.Factory );
		}

		public override string RenderDisplayInfo() {
			return "name=" + _name + ", expectedType=" + ExpectedType;
		}

		/// <summary>
		/// Getter for property 'name'.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}
	}

}
