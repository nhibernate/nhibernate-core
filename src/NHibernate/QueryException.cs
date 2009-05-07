using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace NHibernate
{
	/// <summary>
	/// A problem occurred translating a Hibernate query to SQL due to invalid query syntax, etc.
	/// </summary>
	[Serializable]
	public class QueryException : HibernateException, ISerializable
	{
		private string queryString;

		protected QueryException() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		public QueryException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public QueryException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="queryString">The query that contains the error.</param>
		public QueryException(string message, string queryString) : base(message)
		{
			this.queryString = queryString;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public QueryException(Exception innerException) : base(innerException)
		{
		}

		/// <summary>
		/// Gets or sets the <see cref="String"/> of HQL that caused the Exception.
		/// </summary>
		public string QueryString
		{
			get { return queryString; }
			set { queryString = value; }
		}

		/// <summary>
		/// Gets a message that describes the current <see cref="QueryException"/>.
		/// </summary>
		/// <value>The error message that explains the reason for this exception including the HQL.</value>
		public override string Message
		{
			get
			{
				string msg = base.Message;
				if (queryString != null)
				{
					msg += " [" + queryString + "]";
				}
				return msg;
			}
		}

		#region ISerializable Members

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected QueryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			queryString = info.GetString("queryString");
		}

		/// <summary>
		/// Sets the serialization info for <see cref="QueryException"/> after 
		/// getting the info from the base Exception.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		[SecurityPermission(SecurityAction.LinkDemand,
			Flags=SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("queryString", queryString, typeof(String));
		}

		#endregion
	}
}