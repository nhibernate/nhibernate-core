using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remotion.Linq;

namespace NHibernate.Linq.Visitors
{
	public interface IQueryModelRewriterFactory
	{
		QueryModelVisitorBase CreateVisitor(VisitorParameters parameters);
	}
}
