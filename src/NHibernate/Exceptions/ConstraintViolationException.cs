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
		public ConstraintViolationException(string message, Exception innerException, string sql, string constraintName)
			: base(message, innerException, sql)
		{
			ConstraintName = constraintName;
		}

		public ConstraintViolationException(string message, Exception innerException, string constraintName)
			: base(message, innerException)
		{
			ConstraintName = constraintName;
		}

		public ConstraintViolationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			foreach (var entry in info)
			{
				if (entry.Name == "ConstraintName")
				{
					ConstraintName = entry.Value?.ToString();
				}
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ConstraintName", ConstraintName);
		}

		/// <summary> 
		/// Returns the name of the violated constraint, if known. 
		/// </summary>
		/// <returns> The name of the violated constraint, or null if not known. </returns>
		public string ConstraintName { get; }
	}
}
