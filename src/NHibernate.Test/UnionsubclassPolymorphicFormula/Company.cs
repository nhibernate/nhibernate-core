using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.UnionsubclassPolymorphicFormula
{
	public class Company : Party
	{
		public override string Name { get { return this.CompanyName; } }
		public virtual string CompanyName { get; set; }
	}
}
