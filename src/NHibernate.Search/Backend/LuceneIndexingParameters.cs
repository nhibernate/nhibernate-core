namespace NHibernate.Search.Backend
{
    /// <summary>
    /// Wrapper class around the Lucene indexing parameters <i>mergeFactor</i>, <i>maxMergeDocs</i> and
    /// <i>maxBufferedDocs</i>.
    /// <p>
    /// There are two sets of these parameters. One is for regular indexing the other is for batch indexing
    /// triggered by <code>FullTextSessoin.index(Object entity)</code>
    /// </summary>
    public class LuceneIndexingParameters
    {
        private int transactionMergeFactor = 10;
        private int transactionMaxMergeDocs = int.MaxValue;
        private int transactionMaxBufferedDocs = 10;
        private int batchMergeFactor = 10;
        private int batchMaxMergeDocs = int.MinValue;
        private int batchMaxBufferedDocs = 10;

        // the defaults settings
        private const int DEFAULT_MERGE_FACTOR = 10;
        private const int DEFAULT_MAX_MERGE_DOCS = int.MinValue;
        private const int DEFAULT_MAX_BUFFERED_DOCS = 10;

        #region Constructors

        /// <summary>
        /// Constructor which instantiates a new parameter object with the the default values.
        /// </summary>    
        public LuceneIndexingParameters()
        {
            transactionMergeFactor = DEFAULT_MERGE_FACTOR;
            batchMergeFactor = DEFAULT_MERGE_FACTOR;
            transactionMaxMergeDocs = DEFAULT_MAX_MERGE_DOCS;
            batchMaxMergeDocs = DEFAULT_MAX_MERGE_DOCS;
            transactionMaxBufferedDocs = DEFAULT_MAX_BUFFERED_DOCS;
            batchMaxBufferedDocs = DEFAULT_MAX_BUFFERED_DOCS;
        }

        #endregion

        #region Property methods

        /// <summary>
        /// 
        /// </summary>
        public int TransactionMaxMergeDocs
        {
            get { return transactionMaxMergeDocs; }
            set { transactionMaxMergeDocs = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TransactionMergeFactor
        {
            get { return transactionMergeFactor; }
            set { transactionMergeFactor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BatchMaxMergeDocs
        {
            get { return batchMaxMergeDocs; }
            set { batchMaxMergeDocs = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BatchMergeFactor
        {
            get { return batchMergeFactor; }
            set { batchMergeFactor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int BatchMaxBufferedDocs
        {
            get { return batchMaxBufferedDocs; }
            set { batchMaxBufferedDocs = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int TransactionMaxBufferedDocs
        {
            get { return transactionMaxBufferedDocs; }
            set { transactionMaxBufferedDocs = value; }
        }

        #endregion
    }
}