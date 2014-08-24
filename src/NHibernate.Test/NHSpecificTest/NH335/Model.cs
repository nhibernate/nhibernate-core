using System;

namespace NHibernate.Test.NHSpecificTest.NH335
{
	public class Utils
	{
		private static Random random = new Random();

		/// <summary>
		/// Generate a random string of the given length
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string GetRandomID()
		{
			int length = 32;
			char[] id = new char[length];
			for (int i = 0; i < length; i++)
			{
				// get a random char between 'a' and 'z'
				id[i] = (char) random.Next((int) 'a', ((int) 'z') + 1);
			}
			return new string(id);
		}
	}

	public class Thing
	{
		public Thing()
		{
		}

		public Thing(string classType)
		{
			this._ClassType = classType;
		}

		private string _ID;

		public virtual string ID
		{
			get { return _ID; }
			set { _ID = value; }
		}

		private string _Name;

		public virtual string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		private string _CommonValue;

		public virtual string CommonValue
		{
			get { return _CommonValue; }
			set { _CommonValue = value; }
		}

		protected string _ClassType;

		public virtual string ClassType
		{
			get { return _ClassType; }
		}
	}

	public class AbcThing : Thing
	{
		private static Random random = new Random();

		public AbcThing()
			: base()
		{
			// randomly assign a class type of 'a', 'b', or 'c'
			char c = (char) random.Next((int) 'a', ((int) 'c') + 1);
			_ClassType = new string(c, 1);
		}
	}

	public class OtherThing : Thing
	{
		public OtherThing()
			: base()
		{
			_ClassType = "X";
		}
	}
}