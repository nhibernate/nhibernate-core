using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary>
	/// Thrown if Hibernate can't instantiate an entity or component class at runtime.
	/// </summary>
	[Serializable]
	public class InstantiationException : HibernateException 
	{
		private System.Type type;

		public InstantiationException(string message, System.Type type, Exception root) 
			: base(message, root) 
		{
			this.type = type;
		}

		public System.Type PersistentType 
		{
			get { return type; }
		}

		public override string Message 
		{
			get { return base.Message + type.FullName; }
		}

		public InstantiationException(string message, Exception root) : this(message, typeof(InstantiationException), root) {}

		public InstantiationException(string message) : this(message, typeof(InstantiationException), new InvalidOperationException("Invalid Operation")) {}

		public InstantiationException() : this("Exception occured", typeof(InstantiationException), new InvalidOperationException("Invalid Operation")) {}

		protected InstantiationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
