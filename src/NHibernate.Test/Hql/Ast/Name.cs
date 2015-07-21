namespace NHibernate.Test.Hql.Ast
{
	public class Name
	{
		private string first;
		private char initial;
		private string last;

		public string First
		{
			get { return first; }
			set { first = value; }
		}

		public char Initial
		{
			get { return initial; }
			set { initial = value; }
		}

		public string Last
		{
			get { return last; }
			set { last = value; }
		}
	}
}