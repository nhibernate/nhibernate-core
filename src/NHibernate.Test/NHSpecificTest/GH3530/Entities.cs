using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.SqlCommand;

namespace NHibernate.Test.NHSpecificTest.GH3530
{
	public abstract class Entity
	{
		public virtual Guid Id { get; set; }
	}

	public abstract class Entity<T>:Entity where T : struct
	{
		public virtual T DataValue { get; set; }
	}

	public class IntegerEntity : Entity<int> { }
	public class DateTimeEntity : Entity<DateTime> { }

	public class DoubleEntity : Entity<double> { }
	public class DecimalEntity : Entity<decimal> { }
}
