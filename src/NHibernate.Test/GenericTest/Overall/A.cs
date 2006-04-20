using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.GenericTest.Overall
{
	/// <summary>
	/// This class is used in <see cref="Fixture" /> with
	/// <c>int</c> substituted for <typeparamref name="T" />.
	/// </summary>
	public class A<T>
	{
		private int? id;
		private IList<T> collection;
		private T property;

		public virtual int? Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual T Property
		{
			get { return property; }
			set { property = value; }
		}

		public virtual IList<T> Collection
		{
			get { return collection; }
			set { collection = value; }
		}
	}
}
