using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Holder.
	/// </summary>
	public class Holder : INamed
	{
		private string _id;
		private IList _ones;
		private Foo[] _fooArray;
		private Iesi.Collections.ISet _foos;// <set> mapping
		private string _name;

		
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IList Ones
		{
			get { return _ones; }
			set { _ones = value; }
		}

		public Foo[] FooArray
		{
			get { return _fooArray; }
			set { _fooArray = value; }
		}

		public Iesi.Collections.ISet Foos
		{
			get { return _foos; }
			set { _foos = value; }
		}
		
		#region INamed Members

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion
	}
}
