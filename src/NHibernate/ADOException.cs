using System;
using System.Data;
using System.Runtime.Serialization;

namespace NHibernate 
{
	/// <summary>
	/// Wraps an <c>DataException</c>. Indicates that an exception occurred during an ADO.NET call.
	/// </summary>
	[Serializable]
	public class ADOException : HibernateException 
	{
		private Exception sqle;

		public ADOException() : this("DataException occured", new InvalidOperationException("Invalid Operation")) { }

		public ADOException(string message) : this(message, new InvalidOperationException("Invalid Operation")) { }

		public ADOException(DataException root) : this("DataException occurred", root) { }

		public ADOException(string message, Exception root) : base(message, root) 
		{
			sqle = root;
			log4net.LogManager.GetLogger( typeof(ADOException) ).Error(message, root);  
		}

		protected ADOException(SerializationInfo info, StreamingContext context) : base(info, context) { }

	}
}
