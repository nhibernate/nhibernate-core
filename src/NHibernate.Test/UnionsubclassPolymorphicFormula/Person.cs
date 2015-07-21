using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.UnionsubclassPolymorphicFormula
{
	public class Person : Party
	{
		public override string Name { get { return this.FirstName + " " + this.LastName; } }
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
	}
}
