using System;

namespace NHibernate.Type 
{
	/// <summary>
	/// Thrown when a property cannot be serialized/deserialized
	/// </summary>
	[Serializable]
	public class SerializationException : HibernateException 
	{
		public SerializationException(string message, Exception root) : base(message, root) { }
	}
}
