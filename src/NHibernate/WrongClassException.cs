using System;

namespace NHibernate 
{
	/// <summary>
	/// Thrown when <c>ISession.Load()</c> selects a row with the given primary key (identifier value)
	/// but the row's discriminator value specifies a different subclass from the one requested
	/// </summary>
	[Serializable]
	public class WrongClassException : HibernateException 
	{
		private object identifier;
		private System.Type type;

		public WrongClassException(string message, object identifier, System.Type type) : base(message) 
		{
			this.identifier = identifier;
			this.type = type;
		}

		public object Identifier 
		{
			get { return identifier; }
		}

		public System.Type Type 
		{
			get { return type; }
		}

		public override string Message 
		{
			get {
				return "Object with id: " + identifier
					+ " was not of the specified sublcass: " + type.FullName
					+ " (" +  base.Message + ")" ;
			}
		}

	}
}
