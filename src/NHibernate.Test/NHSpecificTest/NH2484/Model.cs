using System;
#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

namespace NHibernate.Test.NHSpecificTest.NH2484
{
	public class ClassWithImage
	{
		public virtual int Id { get; set; }
		public virtual System.Drawing.Image Image { get; set; }
	}
	
	public class ClassWithSerializableType
	{
		public virtual int Id { get; set; }
#if FEATURE_SERIALIZATION
		public virtual ISerializable Image { get; set; }
#endif
	}
}
