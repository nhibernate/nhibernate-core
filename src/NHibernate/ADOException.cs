using System;
using System.Data;

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

		public ADOException(DataException root) : this("DataException occurred", root) { }

		public ADOException(string str, Exception root) : base(str, root) 
		{
			sqle = root;
			log4net.LogManager.GetLogger( typeof(ADOException) ).Error(str, root);  
		}

	}
}
