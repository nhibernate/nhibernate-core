using System;
using System.Runtime.Serialization;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Assertions
{
	public class IsSerializable : AbstractAsserter
	{
		private readonly object obj;

		public IsSerializable(object obj, string message, params object[] args)
			: base(message, args)
		{
			this.obj = obj;
		}

		public override bool Test()
		{
			bool result = true;
			try
			{
				byte[] bytes = SerializationHelper.Serialize(obj);
				object ds = SerializationHelper.Deserialize(bytes);
			}
			catch (SerializationException e)
			{
				FailureMessage.WriteLine(string.Format("class {0} is not serializable: {1}", obj.GetType().FullName, e.Message));
				result = false;
			}
			return result;
		}
	}
}