using System;
using System.Linq;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	public class AccessorsSerializableTest
	{
		private static System.Type[] accessors = typeof (IPropertyAccessor).Assembly.GetTypes().Where(t => t.Namespace == typeof (IPropertyAccessor).Namespace && t.GetInterfaces().Contains(typeof (IPropertyAccessor))).ToArray();

		[Test, TestCaseSource("accessors")]
		public void AllAccessorsAreMarkedAsSerializable(System.Type concreteAccessor)
		{
			Assert.That(concreteAccessor, Has.Attribute<SerializableAttribute>());
		}

		private static System.Type[] setters = typeof(ISetter).Assembly.GetTypes().Where(t => t.Namespace == typeof(ISetter).Namespace && t.GetInterfaces().Contains(typeof(ISetter))).ToArray();

		[Test, TestCaseSource("setters")]
		public void AllSettersAreMarkedAsSerializable(System.Type concreteAccessor)
		{
			Assert.That(concreteAccessor, Has.Attribute<SerializableAttribute>());
		}

		private static System.Type[] getters = typeof(IGetter).Assembly.GetTypes().Where(t => t.Namespace == typeof(IGetter).Namespace && t.GetInterfaces().Contains(typeof(IGetter))).ToArray();

		[Test, TestCaseSource("getters")]
		public void AllGettersAreMarkedAsSerializable(System.Type concreteAccessor)
		{
			Assert.That(concreteAccessor, Has.Attribute<SerializableAttribute>());
		}
	}
}