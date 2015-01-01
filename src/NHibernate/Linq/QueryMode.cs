using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Linq
{
	public enum QueryMode
	{
		Select,
		Delete,
		Update,
		UpdateVersioned,
		Insert
	}
}
