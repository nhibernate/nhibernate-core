using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Test.NHSpecificTest.NH3121
{
	public class Report
	{
		public virtual Guid Id { get; set; }

		public virtual Byte[] UnsizedArray { get; set; }
		public virtual System.Drawing.Image Image { get; set; }
#if FEATURE_SERIALIZATION
		public virtual ISerializable SerializableImage { get; set; }
#endif
	}
}
