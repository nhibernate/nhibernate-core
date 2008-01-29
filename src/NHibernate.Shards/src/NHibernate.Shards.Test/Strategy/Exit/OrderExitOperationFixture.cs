using System;
using System.Collections.Generic;
using NHibernate.Shards.Strategy.Exit;
using NHibernate.Shards.Test.Mock;
using NUnit.Framework;

namespace NHibernate.Shards.Test.Strategy.Exit
{
	[TestFixture]
	public class OrderExitOperationFixture : TestFixtureBaseWithMock
	{
		private List<object> data;
		private List<object> shuffledList;
		private List<object> nonNullData;

		private class MyInt
		{
			private readonly int i;

			private readonly String name;

			private MyInt innerMyInt;

			public MyInt(int i, String name)
			{
				this.i = i;
				this.name = name;
			}

			public MyInt getInnerMyInt()
			{
				return innerMyInt;
			}

			public void setInnerMyInt(MyInt innerMyInt)
			{
				this.innerMyInt = innerMyInt;
			}

			public long getValue()
			{
				return i;
			}

			public String getName()
			{
				return name;
			}

			public override bool Equals(Object obj)
			{
				MyInt myInt = (MyInt) obj;
				return
					this.getName().Equals(myInt.getName()) &&
					this.getValue().Equals(myInt.getValue());
			}
		}


		protected override void OnSetUp()
		{
			String[] names = {"tomislav", "max", "maulik", "gut", "null", "bomb"};
			data = new List<object>();
			for(int i = 0; i < 6; i++)
			{
				if (i == 4)
					data.Add(null);
				else
					data.Add(new MyInt(i, names[i]));
			}

			nonNullData = (List<object>) ExitOperationUtils.GetNonNullList(data);

			shuffledList = (List<object>) Collections.RandomList(data);
		}

		[Test]
		public void test01()
		{
		}
	}
}