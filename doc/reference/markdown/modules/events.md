# Interceptors and events

It is often useful for the application to react to certain events that
occur inside NHibernate. This allows implementation of certain kinds of
generic functionality, and extension of NHibernate functionality.

# Interceptors

The `IInterceptor` interface provides callbacks from the session to the
application allowing the application to inspect and/or manipulate
properties of a persistent object before it is saved, updated, deleted
or loaded. One possible use for this is to track auditing information.
For example, the following `IInterceptor` automatically sets the
`createTimestamp` when an `IAuditable` is created and updates the
`lastUpdateTimestamp` property when an `IAuditable` is updated.

You may either implement `IInterceptor` directly or (better) extend
`EmptyInterceptor`.

    using System;
        
    using NHibernate;
    using NHibernate.Type;
    
    public class AuditInterceptor : EmptyInterceptor {
    
        private int updates;
        private int creates;
        private int loads;
    
        public override void OnDelete(object entity,
                                      object id,
                                      object[] state,
                                      string[] propertyNames,
                                      IType[] types)
        {
            // do nothing
        }
    
        public override bool OnFlushDirty(object entity, 
                                          object id, 
                                          object[] currentState,
                                          object[] previousState,
                                          string[] propertyNames,
                                          IType[] types)
        {
            if ( entity is IAuditable ) {
                updates++;
                for ( int i=0; i < propertyNames.Length; i++ ) {
                    if ( "lastUpdateTimestamp".Equals( propertyNames[i] ) ) {
                        currentState[i] = new DateTime();
                        return true;
                    }
                }
            }
            return false;
        }
    
        public override bool OnLoad(object entity, 
                                    object id, 
                                    object[] state,
                                    string[] propertyNames,
                                    IType[] types)
        {
            if ( entity is IAuditable ) {
                loads++;
            }
            return false;
        }
    
        public override bool OnSave(object entity, 
                                    object id, 
                                    object[] state,
                                    string[] propertyNames,
                                    IType[] types)
        {
            if ( entity is IAuditable ) {
                creates++;
                for ( int i=0; i<propertyNames.Length; i++ ) {
                    if ( "createTimestamp".Equals( propertyNames[i] ) ) {
                        state[i] = new DateTime();
                        return true;
                    }
                }
            }
            return false;
        }
    
        public override void AfterTransactionCompletion(ITransaction tx)
        {
            if ( tx.WasCommitted ) {
                System.Console.WriteLine(
                    "Creations: " + creates +
                    ", Updates: " + updates +
                    ", Loads: " + loads);
            }
            updates=0;
            creates=0;
            loads=0;
        }
    
    }

Interceptors come in two flavors: `ISession`-scoped and
`ISessionFactory`-scoped.

An `ISession`-scoped interceptor is specified when a session is opened
using one of the overloaded ISessionFactory.OpenSession() methods
accepting an `IInterceptor`.

    ISession session = sf.OpenSession( new AuditInterceptor() );

An `ISessionFactory`-scoped interceptor is registered with the
`Configuration` object prior to building the `ISessionFactory`. In this
case, the supplied interceptor will be applied to all sessions opened
from that `ISessionFactory`; this is true unless a session is opened
explicitly specifying the interceptor to use. `ISessionFactory`-scoped
interceptors must be thread safe, taking care to not store
session-specific state since multiple sessions will use this interceptor
(potentially) concurrently.

    new Configuration().SetInterceptor( new AuditInterceptor() );

# Event system

If you have to react to particular events in your persistence layer, you
may also use the NHibernate2 *event* architecture. The event system can
be used in addition or as a replacement for interceptors.

Essentially all of the methods of the `ISession` interface correlate to
an event. You have a `LoadEvent`, a `FlushEvent`, etc (consult the XML
configuration-file XSD or the `NHibernate.Event` namespace for the full
list of defined event types). When a request is made of one of these
methods, the `ISession` generates an appropriate event and passes it to
the configured event listeners for that type. Out-of-the-box, these
listeners implement the same processing in which those methods always
resulted. However, you are free to implement a customization of one of
the listener interfaces (i.e., the `LoadEvent` is processed by the
registered implementation of the `ILoadEventListener` interface), in
which case their implementation would be responsible for processing any
`Load()` requests made of the `ISession`.

The listeners should be considered effectively singletons; meaning, they
are shared between requests, and thus should not save any state as
instance variables.

A custom listener should implement the appropriate interface for the
event it wants to process and/or extend one of the convenience base
classes (or even the default event listeners used by NHibernate
out-of-the-box as their methods are declared virtual for this purpose).
Custom listeners can either be registered programmatically through the
`Configuration` object, or specified in the NHibernate configuration
XML. Here's an example of a custom load event listener:

    public class MyLoadListener : ILoadEventListener 
    {
        // this is the single method defined by the LoadEventListener interface
        public void OnLoad(LoadEvent theEvent, LoadType loadType)
        {
            if ( !MySecurity.IsAuthorized( theEvent.EntityClassName, theEvent.EntityId ) ) {
                throw new MySecurityException("Unauthorized access");
            }
        }
    }

You also need a configuration entry telling NHibernate to use the
listener in addition to the default listener:

    <hibernate-configuration>
        <session-factory>
            ...
            <event type="load">
                <listener class="MyLoadListener"/>
                <listener class="NHibernate.Event.Default.DefaultLoadEventListener"/>
            </event>
        </session-factory>
    </hibernate-configuration>

Instead, you may register it programmatically:

    Configuration cfg = new Configuration();
    ILoadEventListener[] stack =
        new ILoadEventListener[] { new MyLoadListener(), new DefaultLoadEventListener() };
    cfg.EventListeners.LoadEventListeners = stack;

Listeners registered declaratively cannot share instances. If the same
class name is used in multiple `<listener/>` elements, each reference
will result in a separate instance of that class. If you need the
capability to share listener instances between listener types you must
use the programmatic registration approach.

Why implement an interface and define the specific type during
configuration? Well, a listener implementation could implement multiple
event listener interfaces. Having the type additionally defined during
registration makes it easier to turn custom listeners on or off during
configuration.
