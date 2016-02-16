using System.Collections.Generic;
using NHibernate.Test.NHSpecificTest.NH3845.Interfaces;

namespace NHibernate.Test.NHSpecificTest.NH3845.Concrete
{
	public class AnotherEntity : IAnotherEntity
	{
		private ISet<IPropertyEntityC> _childEntities = new HashSet<IPropertyEntityC>();
		public virtual int AnotherEntityId { get; set; }
		public virtual string Text { get; set; }
		public virtual ISet<IPropertyEntityC> ChildEntities
		{
			get { return _childEntities; }
			protected set { _childEntities = value ?? new HashSet<IPropertyEntityC>(); }
		}
	}
}
