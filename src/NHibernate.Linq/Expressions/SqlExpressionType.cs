using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Linq.Expressions
{
	public enum SqlExpressionType
	{
		Entity=100,
		Select,
		MemberAccess,
		Projection,
	}
}
