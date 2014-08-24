using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH1061
{
	public class TestNH1061
	{
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
	}
}
