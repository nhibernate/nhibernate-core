using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	public class Device
	{
		public Device() : base()
		{
		}

		public Device(string manifacturer)
			: this()
		{
			_manifacturer = manifacturer;
		}

		private int _id;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _manifacturer;

		public string Manifacturer
		{
			get { return _manifacturer; }
			set { _manifacturer = value; }
		}

		private IList _drives = new ArrayList();

		public IList Drives
		{
			get { return _drives; }
			set { _drives = value; }
		}
	}
}