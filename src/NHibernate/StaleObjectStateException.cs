using System;

namespace NHibernate 
{
	/// <summary>
	/// Thrown when a version number check failed, indicating that the 
	/// <c>ISession</c> contained stale data (when using long transactions with
	/// versioning).
	/// </summary>
	[Serializable]
	public class StaleObjectStateException : HibernateException 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(StaleObjectStateException));
		private System.Type persistentType;
		private object identifier;

		public StaleObjectStateException(System.Type persistentType, object identifier) : base("Row was updated or deleted by another transaction") 
		{
			this.persistentType = persistentType;
			this.identifier = identifier;
			log4net.LogManager.GetLogger( typeof(StaleObjectStateException) ).Error("An operation failed due to stale data", this);
		}

		public System.Type PersistentType 
		{ 
			get { return persistentType; }
		}

		public object Identifier 
		{
			get { return identifier; }
		}

		public override string Message 
		{
			get { return base.Message + " for " + persistentType.FullName + " instance with identifier: " + identifier; }
		}
	}
}
