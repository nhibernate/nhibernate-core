//using NHMA = NHibernate.Mapping.Attributes;

namespace NHibernate.Test.NHSpecificTest.NH521
{
//	[NHMA.Class(Lazy=true)]
	public class LazyEntity
	{
		private int _id = 0;

//		[NHMA.Id(Name="Id")]
//			[NHMA.Generator(1, Class="native")]
		public virtual int Id
		{
			get { return _id; }
		}

		public LazyEntity()
		{
		}
	}

//	[NHMA.Class]
	public class ReferringEntity
	{
		private int _id = 0;

//		[NHMA.Id(Name="Id")]
//			[NHMA.Generator(1, Class="native")]
		public virtual int Id
		{
			get { return _id; }
		}

		private LazyEntity _referenceToLazyEntity = new LazyEntity();

//		[NHMA.ManyToOne(Cascade=NHMA.CascadeStyle.All)]
		public virtual LazyEntity ReferenceToLazyEntity
		{
			get { return _referenceToLazyEntity; }
		}

		public ReferringEntity()
		{
		}
	}
}