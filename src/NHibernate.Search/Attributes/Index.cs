namespace NHibernate.Search.Attributes
{
    /// <summary>
    /// The choices of how to index a field
    /// </summary>
    public enum Index
    {
        /// <summary>
        /// Index the field's value without an Analyzer, and disable
        /// the storing of norms.  No norms means that index-time boosting
        /// and field length normalization will be disabled.  The benefit is
        /// less memory usage as norms take up one byte per indexed field
        /// for every document in the index.
        /// </summary>
        NoNormalization,
        /// <summary>
        /// Do not index the field value. This field can thus not be searched,
        /// but one can still access its contents provided it is
        /// </summary>
        No,
        /// <summary>
        /// Index the field's value so it can be searched. An Analyzer will be used
        /// to tokenize and possibly further normalize the text before its
        /// terms will be stored in the index. This is useful for common text.
        /// </summary>
        Tokenized,
        /// <summary>
        /// Index the field's value without using an Analyzer, so it can be searched.
        /// As no analyzer is used the value will be stored as a single term. This is
        /// useful for unique Ids like product numbers.
        /// </summary>
        UnTokenized
    }
}