using System;

namespace NHibernate.Id 
{
	/// <summary>
	/// Thrown by <c>IIdentifierGenerator</c> implementation class when ID generation fails
	/// </summary>
	[Serializable]
	public class IdentifierGenerationException : HibernateException 
	{
		public IdentifierGenerationException(string message) : base(message) {}

		public IdentifierGenerationException(string message, Exception e) : base(message, e) {}
	}
}
