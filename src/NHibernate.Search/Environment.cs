namespace NHibernate.Search
{
	public class Environment
	{
		public const string AnalyzerClass = "hibernate.search.analyzer";
		public const string WorkerExecution ="hibernate.search.worker.execution"; 
		/* has no meaning, since we don't support custom thread pools
		public const string WorkerThreadPoolSize ="";
		public const string WorkerThreadQuerySize = "";
		*/
		public const string WorkerBackend = "hibernate.search.worker.backend";

		public const string SourceBase				= "sourceBase";
		public const string Source					= "source";
		public const string Refresh					= "refresh";
		public const string IndexBase				= "indexBase";
		public const string DirectoryProvider		= "directory_provider";
	}
}