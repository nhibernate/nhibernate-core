using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;

namespace NHibernate.Test.Assertions
{
	public class InheritedAreMarkedSerializable : AbstractAsserter
	{
		private readonly System.Type type;

		public InheritedAreMarkedSerializable(System.Type type, string message, params object[] args)
			: base(message, args)
		{
			this.type = type;
		}

		public override bool Test()
		{
			int failedCount = 0;
			Assembly nhbA = Assembly.GetAssembly(type);
			IList types = ClassList(nhbA);
			foreach (System.Type tp in types)
			{
				object[] atts = tp.GetCustomAttributes(typeof(SerializableAttribute), false);
				if (atts.Length == 0)
				{
					FailureMessage.AddLine(string.Format("    class {0} is not marked as Serializable", tp.FullName));
					failedCount++;
				}
			}
			return (failedCount == 0);
		}

		private IList ClassList(Assembly assembly)
		{
			IList result = new ArrayList();
			if (assembly != null)
			{
				System.Type[] types = assembly.GetTypes();
				foreach (System.Type tp in types)
				{
					if (tp != type && type.IsAssignableFrom(tp) && !tp.IsInterface)
						result.Add(tp);
				}
			}
			return result;
		}
	}
}