using System;

namespace NHibernate.Examples.ForumQuestions.T1104613
{
	/// <summary>
	/// Summary description for AOuterJoin.
	/// </summary>
	public class AOuterJoin
	{
		private string _key;
		private string _name;
		
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
	}

	public class AManyToOne : AOuterJoin 
	{
	}

}
