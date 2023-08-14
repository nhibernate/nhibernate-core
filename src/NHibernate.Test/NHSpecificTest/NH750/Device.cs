using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH750
{
	public class Device
	{
		private int _id;
		private Device _template;
		private IList<Drive> _drives = new List<Drive>();
		private IList<Drive> _drivesNotIgnored = new List<Drive>();

		public Device() : base()
		{
		}

		public Device(string manifacturer)
			: this()
		{
			_manifacturer = manifacturer;
		}

		public virtual Device Template
		{
			get => _template;
			set => _template = value;
		}

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private string _manifacturer;

		public virtual string Manifacturer
		{
			get { return _manifacturer; }
			set { _manifacturer = value; }
		}


		public virtual IList<Drive> Drives
		{
			get { return _drives; }
			set { _drives = value; }
		}

		public virtual IList<Drive> DrivesNotIgnored
		{
			get => _drivesNotIgnored;
			set => _drivesNotIgnored = value;
		}
	}
}
