using System;

namespace NHibernate.Id 
{
	/// <summary>
	/// Thrown by <c>IIdentifierGenerator</c> implementation class when ID generation fails
	/// </summary>
	public class IdentifierGenerationException : HibernateException 
	{
		
		public IdentifierGenerationException(string msg) : base(msg) {}

		public IdentifierGenerationException(string msg, Exception e) : base(msg, e) {}
	}
}
