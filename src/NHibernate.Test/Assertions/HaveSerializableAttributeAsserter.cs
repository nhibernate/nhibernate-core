using System;
using NUnit.Framework;

namespace NHibernate.Test.Assertions
{
	public class HaveSerializableAttributeAsserter : AbstractAsserter
	{
		private readonly System.Type clazz;

		public HaveSerializableAttributeAsserter(System.Type clazz, string message, params object[] args)
			: base(message, args)
		{
			this.clazz = clazz;
		}

		public HaveSerializableAttributeAsserter(System.Type clazz)
			: base(string.Empty)
		{
			this.clazz = clazz;
		}

		public override bool Test()
		{
			object[] atts = clazz.GetCustomAttributes(typeof(SerializableAttribute), false);
			return (atts.Length > 0);
		}

		public override string Message
		{
			get
			{
				if (clazz.IsInterface)
					FailureMessage.WriteLine(string.Format("The class {0} is an interface.", clazz.FullName));
				else
					FailureMessage.WriteLine(string.Format("The class {0} is not marked as Serializable.", clazz.FullName));
				return FailureMessage.ToString();
			}
		}
	}
}