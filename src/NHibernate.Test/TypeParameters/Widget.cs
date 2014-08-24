using System;

namespace NHibernate.Test.TypeParameters
{
	public class Widget
	{
		private int valueOne = 1;
		private int valueTwo = 2;
		private int valueThree = -1;
		private int valueFour = -5;
		private int id;
		private String str;

		public int ValueOne
		{
			get { return valueOne; }
			set { valueOne = value; }
		}

		public int ValueTwo
		{
			get { return valueTwo; }
			set { valueTwo = value; }
		}

		public int ValueThree
		{
			get { return valueThree; }
			set { valueThree = value; }
		}

		public int ValueFour
		{
			get { return valueFour; }
			set { valueFour = value; }
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Str
		{
			get { return str; }
			set { str = value; }
		}
	}
}