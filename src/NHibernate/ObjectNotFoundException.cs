using System;
using System.Runtime.Serialization;

namespace NHibernate 
{

	/// <summary>
	/// Thrown when the user tries to pass a deleted object to the <c>ISession</c>.
	/// </summary>
	[Serializable]
	public class ObjectNotFoundException : HibernateException 
	{
		private object identifier;
		private System.Type type;

		public ObjectNotFoundException(string message, object identifier, System.Type type) : base(message) 
		{
			this.identifier = identifier;
			this.type = type;
		}

		public object Identifier 
		{
			get { return identifier; }
		}

		public override string Message 
		{
			get { return base.Message + ": " + identifier + ", of class: " + type.FullName; }
		}

		public System.Type Type 
		{
			get { return type; }
		}
		public ObjectNotFoundException(string message, Exception root) : this(message, root.Message, typeof(ObjectNotFoundException)) {}

		public ObjectNotFoundException(string message) : this(message, message, typeof(ObjectNotFoundException)) {}

		public ObjectNotFoundException(Exception root) : this(root.Message, root.Message, typeof(ObjectNotFoundException)) {}

		protected ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
