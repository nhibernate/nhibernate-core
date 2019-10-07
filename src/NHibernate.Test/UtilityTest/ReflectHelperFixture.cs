using System;
using System.Reflection;
using NHibernate.DomainModel;
using NHibernate.Util;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.UtilityTest
{
	/// <summary>
	/// Summary description for ReflectHelperFixture.
	/// </summary>
	[TestFixture]
	public class ReflectHelperFixture
	{
		[Test]
		public void GetConstantValueEnum()
		{
			object result = ReflectHelper.GetConstantValue(typeof(FooStatus), "ON");
			Assert.AreEqual(1, (int) result, "Should have found value of 1");
		}

		public interface IMyBaseWithEqual
		{
			bool Equals(object that);
			int GetHashCode();
		}

		public interface IMyInheritedWithEqual : IMyBaseWithEqual
		{
		}

		public interface IEmpty
		{

		}

		public interface IComplex: IEmpty, IMyInheritedWithEqual
		{
			
		}

		[Test]
		public void OverridesEquals()
		{
			Assert.IsFalse(ReflectHelper.OverridesEquals(this.GetType()), "ReflectHelperFixture does not override equals");
			Assert.IsTrue(ReflectHelper.OverridesEquals(typeof(string)), "String does override equals");
			Assert.IsFalse(ReflectHelper.OverridesEquals(typeof(IDisposable)), "IDisposable does not override equals");
			Assert.IsTrue(ReflectHelper.OverridesEquals(typeof(BRhf)), "Base class overrides equals");
			Assert.That(!ReflectHelper.OverridesEquals(typeof (object)), "System.Object does not override.");
		}

		[Test]
		public void InheritedInterfaceOverridesEquals()
		{
			Assert.That(ReflectHelper.OverridesEquals(typeof(IMyBaseWithEqual)), "does override.");
			Assert.That(ReflectHelper.OverridesEquals(typeof(IMyInheritedWithEqual)), "does override.");
			Assert.That(ReflectHelper.OverridesEquals(typeof(IComplex)), "does override.");
		}

		[Test]
		public void OverridesGetHashCode()
		{
			Assert.IsFalse(ReflectHelper.OverridesGetHashCode(this.GetType()), "ReflectHelperFixture does not override GetHashCode");
			Assert.IsTrue(ReflectHelper.OverridesGetHashCode(typeof(string)), "String does override equals");
			Assert.IsFalse(ReflectHelper.OverridesGetHashCode(typeof(IDisposable)), "IDisposable does not override GetHashCode");
			Assert.IsTrue(ReflectHelper.OverridesGetHashCode(typeof(BRhf)), "Base class overrides GetHashCode");
			Assert.That(!ReflectHelper.OverridesGetHashCode(typeof(object)), "System.Object does not override.");
		}

		[Test]
		public void InheritedInterfaceOverridesGetHashCode()
		{
			Assert.That(ReflectHelper.OverridesGetHashCode(typeof(IMyBaseWithEqual)), "does override.");
			Assert.That(ReflectHelper.OverridesGetHashCode(typeof(IMyInheritedWithEqual)), "does override.");
			Assert.That(ReflectHelper.OverridesGetHashCode(typeof(IComplex)), "does override.");
		}
		[Test]
		public void NoTypeFoundReturnsNull()
		{
			System.Type noType = ReflectHelper.TypeFromAssembly("noclass", "noassembly", false);
			Assert.IsNull(noType);
		}

		[Test, NetFxOnly]
		public void TypeFoundInNotLoadedAssembly()
		{
			System.Type httpRequest = ReflectHelper.TypeFromAssembly("System.Web.HttpRequest", "System.Web", false);
			Assert.IsNotNull(httpRequest);

			System.Type sameType = ReflectHelper.TypeFromAssembly("System.Web.HttpRequest", "System.Web", false);
			Assert.AreEqual(httpRequest, sameType, "should be the exact same Type");
		}

		[Test]
		public void SystemTypes()
		{
			System.Type int32 = ReflectHelper.ClassForName("System.Int32");
			Assert.AreEqual(typeof(Int32), int32);
		}

		[Test]
		public void TryGetMethod()
		{
			//const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
			const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo mig = typeof(MyBaseImplementation).GetMethod("get_Id", bf);
			MethodInfo mis = typeof(MyBaseImplementation).GetMethod("set_Id", bf);
			MethodInfo mng = typeof(MyBaseImplementation).GetMethod("get_Name", bf);
			MethodInfo mns = typeof(MyBaseImplementation).GetMethod("set_Name", bf);

			Assert.That(ReflectHelper.TryGetMethod(typeof(IMyBaseInterface), mig), Is.Not.Null);
			Assert.That(ReflectHelper.TryGetMethod(typeof(IMyBaseInterface), mis), Is.Null);
			Assert.That(ReflectHelper.TryGetMethod(typeof(IMyBaseInterface), mng), Is.Not.Null);
			Assert.That(ReflectHelper.TryGetMethod(typeof(IMyBaseInterface), mns), Is.Not.Null);

			mig = typeof(IMyBaseInterface).GetMethod("get_Id", bf);
			Assert.That(ReflectHelper.TryGetMethod(typeof(IMyInterface), mig), Is.Not.Null);
		}

		[Test]
		public void GetGenericMethodFrom()
		{
			var signature = new[] {typeof (string), typeof (string), typeof (bool)};
			// This is the fixture for this obsolete member, so disable the obsolete warning.
#pragma warning disable CS0618 // Type or member is obsolete
			Assert.That(ReflectHelper.GetGenericMethodFrom<ISomething>("List", new[] {typeof (BRhf)}, signature), Is.Not.Null);
			Assert.That(ReflectHelper.GetGenericMethodFrom<ISomething>("List", new[] { typeof(int), typeof(string) }, signature), Is.Not.Null);
			Assert.That(ReflectHelper.GetGenericMethodFrom<ISomething>("List", new[] { typeof(int), typeof(string) }
				, new[] { typeof(string), typeof(string), typeof(bool), typeof(IComparer<>).MakeGenericType(typeof(int)) }), Is.Not.Null);
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}

	public interface ISomething
	{
		int List(string role, string propertyRef, bool embedded);
		int List<T>(string role, string propertyRef, bool embedded);
		int List<TK, TV>(string role, string propertyRef, bool embedded);
		int List<TK, TV>(string role, string propertyRef, bool embedded, IComparer<TK> comparer);
	}

	public class ARhf
	{
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class BRhf : ARhf
	{
	}

	public interface IMyBaseInterface
	{
		int Id { get; }
		string Name { get; set; }
	}

	public interface IMyInterface: IMyBaseInterface
	{
		string Description { get; }
	}

	public class MyBaseImplementation : IMyBaseInterface
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}

	public class MyDerivedImplementation : MyBaseImplementation, IMyInterface
	{
		public string Description { get; set; }
	}

	public class MyImplementation: IMyInterface
	{
		public int Id{ get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
