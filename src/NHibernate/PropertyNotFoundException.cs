using System;

namespace NHibernate 
{
	/// <summary>
	/// Indicates that an expected getter or setter method could not be found on a class
	/// </summary>
	[Serializable]
	public class PropertyNotFoundException : MappingException 
	{
		public PropertyNotFoundException(string message, Exception root) : base(message, root) {}
		public PropertyNotFoundException(Exception root) : base(root) {}
		public PropertyNotFoundException(string message) : base(message) {}
	}
}
