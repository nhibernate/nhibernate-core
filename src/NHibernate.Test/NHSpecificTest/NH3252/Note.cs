using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3252
{
	class Note
	{
		public virtual int Id { get; protected set; }

		public virtual string Text { get; set; }
	}
}
