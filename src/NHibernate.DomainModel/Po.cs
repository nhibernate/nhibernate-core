using System;
using System.Collections;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Po.
	/// </summary>
	public class Po 
	{

		private long _id;
		private string _value;
		//<set> mapping
		private Iesi.Collections.ISet _set;
		private IList _list;

		public long Id
		{
			get {return _id;}
			set {_id = value;}
		}

		public string Value
		{
			get {return _value;}
			set {_value = value;}
		}

		public Iesi.Collections.ISet Set
		{
			get {return _set;}
			set {_set = value;}
		}

		public IList List
		{
			get { return _list; }
			set { _list = value; }
		}


	}
}
