using System;

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

		public ObjectNotFoundException(string msg, object identifier, System.Type type) : base(msg) 
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
	}
}
