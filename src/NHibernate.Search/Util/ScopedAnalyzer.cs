using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lucene.Net.Analysis;

namespace NHibernate.Search.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class ScopedAnalyzer : Analyzer
    {
        private readonly IDictionary<string, Analyzer> scopedAnalyzers = new Dictionary<string, Analyzer>();
        private Analyzer globalAnalyzer;

        /// <summary>
        /// 
        /// </summary>
        public Analyzer GlobalAnalyzer
        {
            set { globalAnalyzer = value; }
        }

        public void AddScopedAnalyzer(string scope, Analyzer analyzer)
        {
            scopedAnalyzers.Add(scope, analyzer);
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return GetAnalyzer(fieldName).TokenStream(fieldName, reader);
        }

        public override int GetPositionIncrementGap(string fieldName)
        {
            return GetAnalyzer(fieldName).GetPositionIncrementGap(fieldName);
        }

        private Analyzer GetAnalyzer(String fieldName)
        {
            Analyzer analyzer = scopedAnalyzers[fieldName];
            if (analyzer == null)
            {
                analyzer = globalAnalyzer;
            }
            return analyzer;
        }
    }
}
