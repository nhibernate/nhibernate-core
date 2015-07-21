using System;

// This test is explicitly to ensure varius aliases beginning with underscore
// can be handled properly, since some dialect had problems with it.
// ReSharper disable InconsistentNaming
namespace NHibernate.Test.NHSpecificTest.NH386
{
	public class _Child
	{
		public int _Id { get; set; }
	}
}
// ReSharper restore InconsistentNaming
