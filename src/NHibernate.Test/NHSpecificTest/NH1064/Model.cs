using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1064
{
	public class TypeA
	{
		public TypeA() { }
		public TypeA(string name) { this._Name = name; }

		private int _Id;
		public virtual int Id
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

		private ISet<TypeB> _Bs = new HashSet<TypeB>();
		public virtual ISet<TypeB> Bs
		{
			get { return _Bs; }
			set { _Bs = value; }
		}

		private TypeC _C;
		public virtual TypeC C
		{
			get { return _C; }
			set { _C = value; }
		}
	}

	public class TypeB
	{
		public TypeB() { }
		public TypeB(string name) { this._Name = name; }

		private int _Id;
		public virtual int Id
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

		private TypeA _A;
		public virtual TypeA A
		{
			get { return _A; }
			set { _A = value; }
		}
	}

	public class TypeC
	{
		public TypeC() { }
		public TypeC(string id, string name)
		{
			this._Id = id;
			this._Name = name;
		}
		
		private string _Id;
		public virtual string Id
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
	}
}
