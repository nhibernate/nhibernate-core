using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH3121
{
	public class Report
	{
		public virtual Guid Id { get; set; }

		public virtual Byte[] UnsizedArray { get; set; }
		public virtual System.Drawing.Image Image { get; set; }
		public virtual ISerializable SerializableImage { get; set; }
	}
}
