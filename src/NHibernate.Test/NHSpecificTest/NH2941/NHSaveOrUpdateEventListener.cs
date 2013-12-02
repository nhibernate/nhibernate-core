using System.Linq;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2941
{
    /// <summary>
    /// Event listener class that listens for <see cref="NHibernate"/> SaveOrUpdate event.
    /// Class notifies repository of save event.
    /// </summary>
    internal class NHSaveOrUpdateEventListener : DefaultSaveOrUpdateEventListener
    {
        public static int ParentSaveEventCount;
        public static int ChildSaveEventCount;
        
        /// <summary>
        /// Method called from <see cref="NHibernate"/> before performing SaveOrUpdate processing.
        /// </summary>
        /// <param name="event">Information about the save or update pertaining to the entity being saved or updated.</param>
        public override void OnSaveOrUpdate(SaveOrUpdateEvent @event)
        {
            ProcessEntitySaveOrUpdate(@event);
            base.OnSaveOrUpdate(@event);
        }

        /// <summary>
        /// Processes the SaveOrUpdateEvent.
        /// </summary>
        /// <param name="event">Information about the save or update pertaining to the entity being saved or updated.</param>
        internal static void ProcessEntitySaveOrUpdate(SaveOrUpdateEvent @event)
        {
            string entityName = @event.EntityName;
            if (string.IsNullOrEmpty(entityName))
            {
                entityName = @event.Entity.GetType().Name;
            }
            if (!string.IsNullOrEmpty(entityName))
            {
                if (entityName.Contains("Parent"))
                {
                    ParentSaveEventCount++;
                    Parent entity = (Parent) @event.Entity;
                    using (ISession childSession = @event.Session.GetSession(EntityMode.Poco))
                    {
                        //Commenting out the original code and using child session instead, as suggested in the JIRA item comments.
                        //@event.Session.Query<Parent>().Where(item => item.Name == entity.Name).ToList();
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
                        childSession.Query<Parent>().Where(item => item.Name == entity.Name).ToList();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
                    }
                }
                if (entityName.Contains("Child"))
                {
                    ChildSaveEventCount++;
                    Child entity = (Child) @event.Entity;
                    using (ISession childSession = @event.Session.GetSession(EntityMode.Poco))
                    {
                        //Commenting out the original code and using child session instead, as suggested in the JIRA item comments.
                        //@event.Session.Query<Child>().Where(item => item.Parent.Name == entity.Parent.Name).ToList();
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
                        childSession.Query<Child>().Where(item => item.Parent.Name == entity.Parent.Name).ToList();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
                    }
                }
            }
        }
    }
}
