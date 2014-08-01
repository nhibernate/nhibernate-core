using System.Collections.Generic;
using NHibernate.Event;

namespace NHibernate.Test.Events
{
    public class MyEntityForTesting
    {
        private readonly IList<string> _calledMethods = new List<string>();

        public virtual int Id { get; set; }

        public virtual string SomeProperty { get; set; }

        public virtual string[] PropertyNames
        {
            get { return new[] { "SomeProperty" }; }
        }

        public MyEntityForTesting()
        {
            SomeProperty = "default";
        }

        public virtual IList<string> CalledMethods
        {
            get { return _calledMethods; }
        }

        [PostInsert]
        public virtual void PostInsert()
        {
            _calledMethods.Add("PostInsert");
        }

        [PostInsert]
        public virtual void PostInsertWithSession(ISession session)
        {
            _calledMethods.Add("PostInsert(session)");
        }

        [PostDelete]
        public virtual void PostDelete()
        {
            _calledMethods.Add("PostDelete");
        }

        [PostDelete]
        public virtual void PostDeleteWithSession(ISession session)
        {
            _calledMethods.Add("PostDelete(session)");
        }

        [PostUpdate]
        public virtual void PostUpdate()
        {
            _calledMethods.Add("PostUpdate");
        }

        [PostUpdate]
        public virtual void PostUpdateWithSession(ISession session)
        {
            _calledMethods.Add("PostUpdate(session)");
        }

        [PreDelete]
        public virtual void PreDelete()
        {
            _calledMethods.Add("PreDelete");
        }

        [PreDelete]
        public virtual void PreDeleteWithSession(ISession session)
        {
            _calledMethods.Add("PreDelete(session)");
        }

        [PreUpdate]
        public virtual void PreUpdate()
        {
            _calledMethods.Add("PreUpdate");
            SomeProperty = "preupdate";
        }

        [PreUpdate]
        public virtual void PreUpdateWithSession(ISession session)
        {
            _calledMethods.Add("PreUpdate(session)");
        }

        [PreInsert]
        public virtual void PreInsert()
        {
            _calledMethods.Add("PreInsert");
            SomeProperty = "preinsert";
        }

        [PreInsert]
        public virtual void PreInsertWithSession(ISession session)
        {
            _calledMethods.Add("PreInsert(session)");
        }
    }
}
