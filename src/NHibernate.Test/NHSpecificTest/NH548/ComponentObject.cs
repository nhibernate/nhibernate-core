using System;
using System.Reflection;
using log4net;

namespace NHibernate.Test.NHSpecificTest.NH548
{
	public class ComponentObject
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private MainObject _parent;
		private string _note;

		private ComponentObject()
		{
		}

		internal ComponentObject(MainObject parent)
		{
			_parent = parent;
		}

		public MainObject Parent
		{
			get
			{
				if (log.IsDebugEnabled) log.DebugFormat("get_Parent: {0}", _parent);
				return _parent;
			}
			set
			{
				if (log.IsDebugEnabled) log.DebugFormat("set_Parent: {0}", value);
				_parent = value;
			}
		}

		public string Note
		{
			get { return _note; }
			set { _note = value; }
		}
	}
}