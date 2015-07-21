using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.Hql.Ast
{
	public class Human: Mammal
	{
		private Name name;
		private string nickName;
		private ICollection friends;
		private ICollection pets;
		private IDictionary<string, Human> family;
		private double height;

		private long bigIntegerValue;
		private decimal bigDecimalValue;
		private int intValue;
		private float floatValue;

		private ISet<string> nickNames;
		private IDictionary<string, Address> addresses;

		public virtual Name Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string NickName
		{
			get { return nickName; }
			set { nickName = value; }
		}

		public virtual ICollection Friends
		{
			get { return friends; }
			set { friends = value; }
		}

		public virtual ICollection Pets
		{
			get { return pets; }
			set { pets = value; }
		}

		public virtual IDictionary<string, Human> Family
		{
			get { return family; }
			set { family = value; }
		}

		public virtual double Height
		{
			get { return height; }
			set { height = value; }
		}

		public virtual long BigIntegerValue
		{
			get { return bigIntegerValue; }
			set { bigIntegerValue = value; }
		}

		public virtual decimal BigDecimalValue
		{
			get { return bigDecimalValue; }
			set { bigDecimalValue = value; }
		}

		public virtual int IntValue
		{
			get { return intValue; }
			set { intValue = value; }
		}

		public virtual float FloatValue
		{
			get { return floatValue; }
			set { floatValue = value; }
		}

		public virtual ISet<string> NickNames
		{
			get { return nickNames; }
			set { nickNames = value; }
		}

		public virtual IDictionary<string, Address> Addresses
		{
			get { return addresses; }
			set { addresses = value; }
		}
	}
}