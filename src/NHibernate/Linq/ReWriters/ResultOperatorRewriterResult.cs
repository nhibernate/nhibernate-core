using System.Collections.Generic;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace NHibernate.Linq.ReWriters
{
	/// <summary>
	/// Result of <see cref="ResultOperatorRewriter.Rewrite" />.
	/// </summary>
	public class ResultOperatorRewriterResult
	{
		public ResultOperatorRewriterResult(IEnumerable<ResultOperatorBase> rewrittenOperators, IStreamedDataInfo evaluationType)
		{
			RewrittenOperators = rewrittenOperators;
			EvaluationType = evaluationType;
		}

		/// <summary>
		/// Gets an <see cref="IEnumerable{T}" /> of <see cref="ResultOperatorBase" /> implementations that were
		/// rewritten.
		/// </summary>
		public IEnumerable<ResultOperatorBase> RewrittenOperators { get; }

		/// <summary>
		/// Gets the <see cref="IStreamedDataInfo" /> representing the type of data that the operator works upon.
		/// </summary>
		public IStreamedDataInfo EvaluationType { get; }
	}
}
