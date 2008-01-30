using NHibernate.Shards.Strategy.Exit;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Strategy.Exit
{
	[TestFixture]
	public class ExitOperationUtilsFixture
	{
		private class MyInt
		{
			private readonly int i;
			private readonly string name;
			private readonly string rank;

			private MyInt innerMyInt;

			public MyInt(int i, string name, string rank)
			{
				this.i = i;
				this.name = name;
				this.rank = rank;
			}

			// these private methods, while unused, are used to verify that the method
			// works for private methods
			public MyInt InnerMyInt
			{
				get { return innerMyInt; }
				set { innerMyInt = value; }
			}

			private int Value
			{
				get { return i; }
			}

			private string Name
			{
				get { return name; }
			}

			protected string Rank
			{
				get { return rank; }
			}
		}

		private class MySubInt : MyInt
		{
			public MySubInt(int i, string name, string rank) : base(i, name, rank)
			{
			}
		}


		[Test]
		public void GetPropertyValue()
		{
			MyInt myInt = new MySubInt(1, "one", "a");
			myInt.InnerMyInt = new MySubInt(5, "five", "b");

			//TODO See the differences of behavior...consecuences ?
			Assert.AreEqual(1, ExitOperationUtils.GetPropertyValue(myInt, "Value"));
			Assert.AreEqual("one", ExitOperationUtils.GetPropertyValue(myInt, "Name"));
			Assert.AreEqual("a", ExitOperationUtils.GetPropertyValue(myInt, "Rank"));

			//TODO Make this pass!
			//Assert.AreEqual(5, ExitOperationUtils.GetPropertyValue(myInt, "innerMyInt.value"));
			//Assert.AreEqual("five", ExitOperationUtils.GetPropertyValue(myInt, "innerMyInt.name"));
			//Assert.AreEqual("b", ExitOperationUtils.GetPropertyValue(myInt, "innerMyInt.rank"));
		}

		[Test, ExpectedException(typeof(PropertyNotFoundException))]
		public void PropertyDoesNotExist()
		{
			MyInt myInt = new MySubInt(1, "one", "a");
			ExitOperationUtils.GetPropertyValue(myInt, "PropertyDoesNotExist");
		}
	}
}