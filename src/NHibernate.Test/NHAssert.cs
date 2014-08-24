using System;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test
{
	public static class NHAssert
	{
		#region Serializable

		public static void HaveSerializableAttribute(System.Type clazz)
		{
			HaveSerializableAttribute(clazz, null, null);
		}

		public static void HaveSerializableAttribute(System.Type clazz, string message, params object[] args)
		{
			Assert.That(clazz, Has.Attribute<SerializableAttribute>(), message, args);
		}

		public static void InheritedAreMarkedSerializable(System.Type clazz)
		{
			InheritedAreMarkedSerializable(clazz, null, null);
		}

		public static void InheritedAreMarkedSerializable(System.Type clazz, string message, params object[] args)
		{
			var faulty = new List<System.Type>();

			Assembly nhbA = Assembly.GetAssembly(clazz);
			var types = ClassList(nhbA, clazz);
			foreach (System.Type tp in types)
			{
				object[] atts = tp.GetCustomAttributes(typeof(SerializableAttribute), false);
				if (atts.Length == 0)
					faulty.Add(tp);
			}

			if (faulty.Count > 0)
			{
				var classes = string.Join(Environment.NewLine, faulty.Select(t => "    " + t.FullName).ToArray());
				Assert.Fail("The following classes was expected to be marked as Serializable:" + Environment.NewLine + classes);
			}
		}

		public static void IsSerializable(object obj)
		{
			IsSerializable(obj, null, null);
		}

		public static void IsSerializable(object obj, string message, params object[] args)
		{
			Assert.That(obj, Is.BinarySerializable, message, args);
		}

		#endregion
		private static IEnumerable<System.Type> ClassList(Assembly assembly, System.Type type)
		{
			IList<System.Type> result = new List<System.Type>();
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