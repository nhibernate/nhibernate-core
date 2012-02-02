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
            if (@event.Entity != null)
            {
                if (@event.Entity is Parent)
                {
                    ParentSaveEventCount++;
                    Parent entity = (Parent) @event.Entity;
                    @event.Session.Query<Parent>().Where(item => item.Name == entity.Name).ToList();
                }
                if (@event.Entity is Child)
                {
                    ChildSaveEventCount++;
                    Child entity = (Child) @event.Entity;
                    @event.Session.Query<Child>().Where(item => item.Parent.Name == entity.Parent.Name).ToList();
                }
            }
        }
    }
}