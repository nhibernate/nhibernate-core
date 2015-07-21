using System;
using System.Collections.Generic;

namespace NHibernate.Test.Join
{
	public class Person
	{
		public Person()
		{
			OthersPhones = new HashSet<string>();
		}
		private char _Sex;
		public virtual char Sex
		{
			get { return _Sex; }
			set { _Sex = value; }
		}

		private long _Id;
		public virtual long Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private string _Name;
		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private string _Country;
		public virtual string Country
		{
			get { return _Country; }
			set { _Country = value; }
		}

		private string _Zip;
		public virtual string Zip
		{
			get { return _Zip; }
			set { _Zip = value; }
		}

		private string _Address;
		public virtual string Address
		{
			get { return _Address; }
			set { _Address = value; }
		}

		private string _HomePhone;
		public virtual string HomePhone
		{
			get { return _HomePhone; }
			set { _HomePhone = value; }
		}

		private string _BusinessPhone;
		public virtual string BusinessPhone
		{
			get { return _BusinessPhone; }
			set { _BusinessPhone = value; }
		}

		private string _StuffName;
		public virtual string StuffName
		{
			get { return _StuffName; }
			set { _StuffName = value; }
		}

		public virtual ISet<string> OthersPhones { get; set; }
	}
}