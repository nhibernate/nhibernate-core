using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1905
{
	public class Mas
	{
		private int _Id;

		public virtual int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private ISet<El> _Els;

		public virtual ISet<El> Els
		{
			get { return _Els; }
			set { _Els = value; }
		}
	}


	public class Det
	{
		private int _Id;
		private Mas _Mas;

		public virtual int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		public virtual Mas Mas
		{
			get { return _Mas; }
			set { _Mas = value; }
		}
	}

	public class El
	{
		private int _Id;
		private string _Descr;

		public virtual int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		public virtual string Descr
		{
			get { return _Descr; }
			set { _Descr = value; }
		}
	}
}