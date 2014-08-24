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

		protected TooManyRowsAffectedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.expectedRowCount = info.GetInt32("expectedRowCount");
			this.actualRowCount = info.GetInt32("actualRowCount");
		}

#if NET_4_0
		[SecurityCritical]
#else
		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
#endif
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