namespace NHibernate.Test.NHSpecificTest.NH1250
{
	public abstract class Party
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

		private int classTypeId; // something for NHibernate to write to
		public abstract int ClassTypeId
		{
			get;
		}
	}

	public class Person : Party
	{
		public override int ClassTypeId
		{
			get { return 1; }
		}
	}

	public class Company : Party
	{
		public override int ClassTypeId
		{
			get { return 2; }
		}
	}

	public class Client
	{
		private int id;
		private Party party;
		private string paymentTerms;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Party Party
		{
			get { return party; }
			set { party = value; }
		}

		public virtual string PaymentTerms
		{
			get { return paymentTerms; }
			set { paymentTerms = value; }
		}
	}

	public class ClassWithFormula
	{
		private int id;
		private string name;
		private int age;
		private bool isAdult;

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

		public virtual int Age
		{
			get { return age; }
			set { age = value; }
		}

		public virtual bool IsAdult
		{
			get { return isAdult; }
			set { isAdult = value; }
		}
	}
}
