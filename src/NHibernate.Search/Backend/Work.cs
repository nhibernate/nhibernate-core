using System;
using System.Reflection;
using NHibernate.Search.Backend;

namespace NHibernate.Search.Backend
{
    /// <summary>
    /// work unit. Only make sense inside the same session since it uses the scope principle
    /// </summary>
    public class Work
    {
        private readonly Object entity;
        private readonly Object id;
        private readonly MemberInfo idGetter;
        private readonly WorkType workType;

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