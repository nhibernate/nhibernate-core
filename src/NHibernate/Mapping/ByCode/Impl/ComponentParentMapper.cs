using System;
using System.Reflection;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Mapping.ByCode.Impl
{
	public class ComponentParentMapper : IComponentParentMapper
	{
		private readonly AccessorPropertyMapper accessorPropertyMapper;

		public ComponentParentMapper(HbmParent parent, MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			parent.name = member.Name;
			accessorPropertyMapper = new AccessorPropertyMapper(member.DeclaringType, member.Name, x => parent.access = x);
		}

		#region IComponentParentMapper Members

		public void Access(Accessor accessor)
		{
			accessorPropertyMapper.Access(accessor);
		}

		public void Access(System.Type accessorType)
		{
			accessorPropertyMapper.Access(accessorType);
		}

		#endregion
	}
}