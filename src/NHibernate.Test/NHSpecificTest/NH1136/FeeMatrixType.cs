using System;
using NHibernate.UserTypes;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public class FeeMatrixType : MilestoneCollectionType<int, decimal>
	{
	}
}