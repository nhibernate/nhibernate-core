using System;

namespace NHibernate.Test.Component.Basic
{
	public class Person
	{
		private string name;
		private DateTime dob;
		private string address;
		private string currentAddress;
		private string previousAddress;
		private int yob;		
		private double heightInches;	
		
		public virtual string Name 
		{
			get { return name; }
			set { name = value; }
		}
		
		public virtual DateTime Dob 
		{
			get { return dob; }
			set { dob = value; }
		}		
		
		public virtual string Address 
		{
			get { return address; }
			set { address = value; }
		}
		
		public virtual string CurrentAddress 
		{
			get { return currentAddress; }
			set { currentAddress = value; }
		}
		
		public virtual string PreviousAddress 
		{
			get { return previousAddress; }
			set { previousAddress = value; }
		}
		
		public virtual int Yob 
		{
			get { return yob; }
			set { yob = value; }
		}
		
		public virtual double HeightInches 
		{
			get { return heightInches; }
			set { heightInches = value; }
		}
		
		public Person()
		{
		}
		
		public Person(String name, DateTime dob, String address) 
		{
			this.name = name;
			this.dob = dob;
			this.address = address;
			this.currentAddress = address;
		}
		
		public virtual void ChangeAddress(String add) 
		{
			this.PreviousAddress = this.Address;
			this.Address = add;
		}		
	}
}