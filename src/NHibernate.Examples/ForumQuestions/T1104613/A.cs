using System;
using System.Collections;

namespace NHibernate.Examples.ForumQuestions.T1104613
{
	/// <summary>
	/// Summary description for A.
	/// </summary>
	public class A
	{
		private string _key;
		private string _name;
		private AManyToOne _manyToOne;
		private IList _outerJoins;

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		
		public AManyToOne ManyToOne
		{
			get { return _manyToOne; }
			set { _manyToOne = value; }
		}

		public IList OuterJoins
		{
			get { return _outerJoins; }
			set { _outerJoins = value; }
		}

	}
}
