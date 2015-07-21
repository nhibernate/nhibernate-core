using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1217
{
	public interface INode
	{
		IRoot Root { get; set; }

		String Label { get; set; }

		ISet<IEdge> FromEdges { get; set; }

		ISet<IEdge> ToEdges { get; set; }
	}
}