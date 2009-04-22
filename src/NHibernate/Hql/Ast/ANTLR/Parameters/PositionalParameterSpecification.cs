using System;
using System.Data;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Parameters
{
	/// <summary>
	/// Parameter bind specification for an explicit  positional (or ordinal) parameter.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	public class PositionalParameterSpecification : AbstractExplicitParameterSpecification 
	{
		private readonly int _hqlPosition;

		/// <summary>
		/// Constructs a position/ordinal parameter bind specification.
		/// </summary>
		/// <param name="sourceLine">sourceLine</param>
		/// <param name="sourceColumn">sourceColumn</param>
		/// <param name="hqlPosition">The position in the source query, relative to the other source positional parameters.</param>
		public PositionalParameterSpecification(int sourceLine, int sourceColumn, int hqlPosition) : base(sourceLine, sourceColumn)
		{
			_hqlPosition = hqlPosition;
		}

		/// <summary>
		/// Bind the appropriate value into the given statement at the specified position.
		/// </summary>
		/// <param name="statement">The statement into which the value should be bound.</param>
		/// <param name="qp">The defined values for the current query execution.</param>
		/// <param name="session">The session against which the current execution is occuring.</param>
		/// <param name="position">The position from which to start binding value(s).</param>
		/// <returns>The number of sql bind positions "eaten" by this bind operation.</returns>
		public override int  Bind(IDbCommand statement, Engine.QueryParameters qp, Engine.ISessionImplementor session, int position)
		{
			IType type = qp.PositionalParameterTypes[_hqlPosition];
			Object value = qp.PositionalParameterValues[_hqlPosition];

			type.NullSafeSet(statement, value, position, session );
			return type.GetColumnSpan( session.Factory );
		}

		public override string RenderDisplayInfo() 
		{
			return "ordinal=" + _hqlPosition + ", expectedType=" + ExpectedType;
		}

		/// <summary>
		/// Getter for property 'hqlPosition'.
		/// </summary>
		public int HqlPosition
		{
			get { return _hqlPosition; }
		}
	}
}
