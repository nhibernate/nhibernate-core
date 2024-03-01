using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace NHibernate.AdoNet
{
	[Serializable]
	public class TooManyRowsAffectedException : HibernateException
	{
		private readonly int expectedRowCount;
		private readonly int actualRowCount;

		public TooManyRowsAffectedException(String message, int expectedRowCount, int actualRowCount)
			: base(message)
		{
			this.expectedRowCount = expectedRowCount;
			this.actualRowCount = actualRowCount;
		}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected TooManyRowsAffectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.expectedRowCount = info.GetInt32("expectedRowCount");
			this.actualRowCount = info.GetInt32("actualRowCount");
		}

		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("expectedRowCount", expectedRowCount);
			info.AddValue("actualRowCount", actualRowCount);
		}

		public int ExpectedRowCount
		{
			get { return expectedRowCount; }
		}

		public int ActualRowCount
		{
			get { return actualRowCount; }
		}
	}
}
