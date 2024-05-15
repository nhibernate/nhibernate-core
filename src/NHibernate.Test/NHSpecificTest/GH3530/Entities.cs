using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Test.NHSpecificTest.GH3530
{
	public class LocaleEntity
	{
		public virtual Guid Id { get; set; }
		public virtual int? IntegerValue { get; set; }
		public virtual DateTime? DateTimeValue { get; set; }
		public virtual double? DoubleValue { get; set; }
		public virtual decimal? DecimalValue { get; set; }
	}
}
