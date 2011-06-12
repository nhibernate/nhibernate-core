using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	public abstract class AbstractExplicitParameterSpecification : IExplicitParameterSpecification
	{
		private readonly int sourceColumn;
		private readonly int sourceLine;

		/// <summary>
		/// Constructs an AbstractExplicitParameterSpecification.
		/// </summary>
		/// <param name="sourceLine">sourceLine</param>
		/// <param name="sourceColumn">sourceColumn</param>
		protected AbstractExplicitParameterSpecification(int sourceLine, int sourceColumn)
		{
			this.sourceLine = sourceLine;
			this.sourceColumn = sourceColumn;
		}

		#region IExplicitParameterSpecification Members

		public int SourceLine
		{
			get { return sourceLine; }
		}

		public int SourceColumn
		{
			get { return sourceColumn; }
		}

		public IType ExpectedType { get; set; }

		public abstract string RenderDisplayInfo();
		public abstract IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory);
		public abstract void Bind(IDbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session);
		public abstract void Bind(IDbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session);
		public abstract void SetEffectiveType(QueryParameters queryParameters);

		#endregion

		protected int GetParemeterSpan(IMapping sessionFactory)
		{
			if (sessionFactory == null)
			{
				throw new ArgumentNullException("sessionFactory");
			}
			if (ExpectedType != null)
			{
				// TODO: we have to find a way to set all expected types during the query parsing
				var paremeterSpan = ExpectedType.GetColumnSpan(sessionFactory);
				// NOTE: the OneToOneType does not return the real ColumnSpan
				return paremeterSpan == 0 ? 1 : paremeterSpan;
			}
			// TODO: (see above) when the ExpectedType is null we will set the BackTrackId just for the first position (not a big problem because IType does not support something different... so far)
			return 1;
		}
	}
}