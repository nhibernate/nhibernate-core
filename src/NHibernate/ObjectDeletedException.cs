using System;
using System.Runtime.Serialization;

namespace NHibernate 
{
	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class ObjectDeletedException : HibernateException 
	{
		private object identifier;

		public ObjectDeletedException(string message, object identifier) : base(message) 
		{
			this.identifier = identifier;
		}

		public object Identifier 
		{
			get { return identifier; }
		}

		public override string Message 
		{
			get { return base.Message + ": " + identifier; }
		}

		public ObjectDeletedException(string message, Exception root) : this(message, root.Message) {}

		public ObjectDeletedException(string message) : this(message, message) {}

		public ObjectDeletedException(Exception root) : this(root.Message, root.Message) {}

		public ObjectDeletedException() : this(string.Empty, string.Empty) {}

		protected ObjectDeletedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
