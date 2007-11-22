using System;
using System.Reflection;

namespace NHibernate.Search.Impl
{
	/// <summary>
	/// work unit. Only make sense inside the same session since it uses the scope principle
	/// </summary>
	public class Work
	{
		private Object entity;
		private Object id;
		private MemberInfo idGetter;
		private WorkType workType;


		public Work(Object entity, Object id, WorkType type)
		{
			this.entity = entity;
			this.id = id;
			this.workType = type;
		}


		public Work(Object entity, MemberInfo idGetter, WorkType type)
		{
			this.entity = entity;
			this.idGetter = idGetter;
			this.workType = type;
		}

		public object Entity
		{
			get { return entity; }
		}

		public object Id
		{
			get { return id; }
		}

		public MemberInfo IdGetter
		{
			get { return idGetter; }
		}

		public WorkType WorkType
		{
			get { return workType; }
		}
	}
}