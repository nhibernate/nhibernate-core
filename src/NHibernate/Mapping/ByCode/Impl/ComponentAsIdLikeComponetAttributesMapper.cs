using System;
using System.Reflection;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentAsIdLikeComponetAttributesMapper : IComponentAttributesMapper
	{
		private readonly IComponentAsIdMapper realMapper;

		public ComponentAsIdLikeComponetAttributesMapper(IComponentAsIdMapper realMapper)
		{
			if (realMapper == null)
			{
				throw new ArgumentNullException("realMapper");
			}
			this.realMapper = realMapper;
		}

		#region IComponentAttributesMapper Members

		public void Access(Accessor accessor)
		{
			realMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			realMapper.Access(accessorType);
		}

		public void OptimisticLock(bool takeInConsiderationForOptimisticLock) {}

		public void Parent(MemberInfo parent)
		{
			// the mapping of the Parent can be used as a ManyToOne but could be strange to have a bidirectional relation in the PK
		}

		public void Parent(MemberInfo parent, Action<IComponentParentMapper> parentMapping) {}

		public void Update(bool consideredInUpdateQuery) {}

		public void Insert(bool consideredInInsertQuery) {}

		public void Lazy(bool isLazy) {}

		public void Unique(bool unique) {}

		public void Class(System.Type componentType)
		{
			realMapper.Class(componentType);
		}

		#endregion
	}
}