using System;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Exceptions
{
	/// <summary> 
	/// Implementation of ADOException indicating that the requested DML operation
	/// resulted in a violation of a defined integrity constraint. 
	/// </summary>
	[Serializable]
	public class ConstraintViolationException : ADOException
	{
		private readonly string constraintName;
		public ConstraintViolationException(string message, Exception innerException, string sql, string constraintName)
			: base(message, innerException, sql)
		{
			this.constraintName = constraintName;
		}

		public ConstraintViolationException(string message, Exception innerException, string constraintName)
			: base(message, innerException)
		{
			this.constraintName = constraintName;
		}

		protected ConstraintViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.constraintName = info.GetString("constraintName");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("constraintName", this.constraintName);
		}

		/// <summary> 
		/// Returns the name of the violated constraint, if known. 
		/// </summary>
		/// <returns> The name of the violated constraint, or null if not known. </returns>
		public string ConstraintName
		{
			get { return constraintName; }
		}
	}
}
