using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Expressions;
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

			public MyInt InnerMyInt
			{
				get { return innerMyInt; }
				set { innerMyInt = value; }
			}

			public long Value
			{
				get { return i; }
			}

			public String Name
			{
				get { return name; }
			}

			public override bool Equals(Object obj)
			{
				MyInt myInt = (MyInt) obj;
				return this.Name.Equals(myInt.Name) && this.Value.Equals(myInt.Value);
			}

			public override int GetHashCode()
			{
				return Value.GetHashCode();
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

			shuffledList = (List<object>) Collections.RandomList(nonNullData);
		}

		[Test]
		public void Apply()
		{
			Order order = Order.Asc("Value");
			OrderExitOperation oeo = new OrderExitOperation(order);
			IList unRandomList = oeo.Apply(shuffledList);

			for(int i = 0; i < unRandomList.Count; i++)
			{
				Assert.IsTrue(unRandomList[i].Equals(nonNullData[i]));
			}
		}

		[Test]
		public void MultipleOrderings()
		{
			Order orderValue = Order.Asc("Value");
			Order orderName = Order.Desc("Name");

			OrderExitOperation oeoValue = new OrderExitOperation(orderValue);
			OrderExitOperation oeoName = new OrderExitOperation(orderName);

			List<MyInt> answer = new List<MyInt>(new MyInt[]
			                                     	{
			                                     		new MyInt(0, "tomislav"),
			                                     		new MyInt(1, "max"),
			                                     		new MyInt(2, "maulik"),
			                                     		new MyInt(3, "gut"),
			                                     		new MyInt(5, "bomb")
			                                     	});

			IList unShuffledList = oeoName.Apply(oeoValue.Apply(shuffledList));

			for(int i=0;i< answer.Count;i++)
			{
				Assert.AreEqual(answer[i],unShuffledList[i],"The element {0} of the collecion is not equal",i);
			}
		}
	}
}