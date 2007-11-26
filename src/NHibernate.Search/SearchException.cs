namespace NHibernate.Search.Impl
{
	[System.Serializable]
	public class SearchException : System.Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public SearchException() { }
		public SearchException( string message ) : base( message ) { }
		public SearchException( string message, System.Exception inner ) : base( message, inner ) { }
		protected SearchException( 
			System.Runtime.Serialization.SerializationInfo info, 
			System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
	}
}