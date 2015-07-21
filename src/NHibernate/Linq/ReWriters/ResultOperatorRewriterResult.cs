namespace NHibernate.Linq.ReWriters
{
    using System.Collections.Generic;

    using Remotion.Linq.Clauses;
    using Remotion.Linq.Clauses.StreamedData;

    /// <summary>
    /// Result of <see cref="ResultOperatorRewriter.Rewrite" />.
    /// </summary>
    public class ResultOperatorRewriterResult
    {
        public ResultOperatorRewriterResult(IEnumerable<ResultOperatorBase> rewrittenOperators, IStreamedDataInfo evaluationType)
        {
            this.RewrittenOperators = rewrittenOperators;
            this.EvaluationType = evaluationType;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}" /> of <see cref="ResultOperatorBase" /> implementations that were
        /// rewritten.
        /// </summary>
        public IEnumerable<ResultOperatorBase> RewrittenOperators { get; private set; }

        /// <summary>
        /// Gets the <see cref="IStreamedDataInfo" /> representing the type of data that the operator works upon.
        /// </summary>
        public IStreamedDataInfo EvaluationType { get; private set; }
    }
}
