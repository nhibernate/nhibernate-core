using System;
using System.Collections.Generic;

// This test is explicitly to ensure varius aliases beginning with underscore
// can be handled properly, since some dialect had problems with it.
// ReSharper disable InconsistentNaming
namespace NHibernate.Test.NHSpecificTest.NH386
{

	public class _Parent
	{
		public int _Id { get; set; }

		public ISet<_Child> _Children { get; set; }
	}
}
// ReSharper restore InconsistentNaming
