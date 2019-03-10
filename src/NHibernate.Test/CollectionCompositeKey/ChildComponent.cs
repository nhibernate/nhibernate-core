
namespace NHibernate.Test.CollectionCompositeKey
{
	public class ChildComponent
	{
		public virtual Parent Parent { get; set; }

		public virtual string ParentCode { get; set; }

		public virtual int ParentNumber { get; set; }
	}
}
