using System;

namespace NHibernate.Type 
{
	/// <summary>
	/// Thrown when a property cannot be serialized/deserialized
	/// </summary>
	[Serializable]
	public class SerializationException : HibernateException 
	{
		public SerializationException(string msg, Exception root) : base(msg, root) { }
	}
}
