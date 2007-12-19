using System;

namespace NHibernate.Validator.Event
{
    using Cfg;
    using NHibernate.Event;

    /// <summary>
    /// 
    /// </summary>
    public class ValidateEventListener :
        IPreInsertEventListener,
        IPreUpdateEventListener,
        IInitializable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public bool OnPreInsert(PreInsertEvent @event)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cfg"></param>
        public void Initialize(Configuration cfg)
        {
            throw new NotImplementedException();
        }
    }
}