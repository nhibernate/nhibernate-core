using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Contained.
	/// </summary>
	public class Contained
	{
		private Container _container;
		private long _id;
		private IList _bag = new ArrayList();
		private IList _lazyBag = new ArrayList();

		# region object overrides
		public override bool Equals(object obj)
		{
			if(obj==null) return false;

			return _id==( (Contained)obj).Id;
		}

		public override int GetHashCode()
		{
			return _id.GetHashCode();
		}

		#endregion 

		public Container Container 
		{
			get { return _container; }
			set { _container = value; }
		}

		public long Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public IList Bag 
		{
			get { return _bag; }
			set { _bag = value; }
		}

		public IList LazyBag 
		{
			get { return _lazyBag; }
			set { _lazyBag = value; }
		}
	}
}
