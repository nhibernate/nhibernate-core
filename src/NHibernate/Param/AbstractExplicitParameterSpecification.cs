using System.Data;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Param
{
	public abstract class AbstractExplicitParameterSpecification : IExplicitParameterSpecification 
	{
		private readonly int _sourceLine;
		private readonly int _sourceColumn;
		private IType _expectedType;

		/// <summary>
		/// Constructs an AbstractExplicitParameterSpecification.
		/// </summary>
		/// <param name="sourceLine">sourceLine</param>
		/// <param name="sourceColumn">sourceColumn</param>
		protected AbstractExplicitParameterSpecification(int sourceLine, int sourceColumn) {
			_sourceLine = sourceLine;
			_sourceColumn = sourceColumn;
		}

		public int SourceLine 
		{
			get { return _sourceLine; }
		}

		public int SourceColumn
		{
			get { return _sourceColumn; }
		}

		public IType ExpectedType 
		{
			get { return _expectedType; }
			set { _expectedType = value; }
		}

		public abstract string RenderDisplayInfo();
		public abstract int Bind(IDbCommand statement, QueryParameters qp, ISessionImplementor session, int position);
	}
}