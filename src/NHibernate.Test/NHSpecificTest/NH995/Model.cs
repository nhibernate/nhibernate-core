using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH995
{
	[Serializable]
	public class ClassA
	{
		private int id;
		private string name;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public override bool Equals(object obj)
		{
			ClassA other = obj as ClassA;
			if (other == null) return false;
			return this.Id == other.Id;
		}

		public override int GetHashCode()
		{
			return id;
		}
	}

	[Serializable]
	public class ClassBId
	{
		private string code;
		private ClassA a;

		public static int counter = 0;
		public static Dictionary<int, ClassBId> IdInstances = new Dictionary<int, ClassBId>();
		public readonly int ID;

		private ClassBId()
		{
			this.ID = ++counter;
			IdInstances.Add(this.ID, this);
		}

		public ClassBId(string code, ClassA a) 
			: this()
		{
			this.code = code;
			this.a = a;
		}

		public virtual string Code
		{
			get { return code; }
			set { code = value; }
		}

		public virtual ClassA A
		{
			get { return a; }
			set { a = value; }
		}

		public override bool Equals(object obj)
		{
			ClassBId other = obj as ClassBId;
			if (other == null) return false;

			if (this.code != other.code) return false;

			return this.A.Id == other.A.Id;
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode();
		}
	}

	[Serializable]
	public class ClassB
	{
		private ClassBId id;
		private string someProp;

		public virtual ClassBId Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string SomeProp
		{
			get { return someProp; }
			set { someProp = value; }
		}
	}

	[Serializable]
	public class ClassC
	{
		private int id;
		private ClassB b;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual ClassB B
		{
			get { return b; }
			set { b = value; }
		}
	}
}
