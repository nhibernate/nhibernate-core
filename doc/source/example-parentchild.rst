*********************
Example: Parent/Child
*********************

One of the very first things that new users try to do with NHibernate is to
model a parent / child type relationship. There are two different approaches to
this. For various reasons the most convenient approach, especially for new
users, is to model both ``Parent`` and ``Child`` as entity classes with a
``<one-to-many>`` association from ``Parent`` to ``Child``. (The alternative
approach is to declare the ``Child`` as a ``<composite-element>``.) Now, it
turns out that default semantics of a one to many association (in NHibernate)
are much less close to the usual semantics of a parent / child relationship than
those of a composite element mapping. We will explain how to use a
*bidirectional one to many association with cascades* to model a parent / child
relationship efficiently and elegantly. It”s not at all difficult!

A note about collections
=========================

NHibernate collections are considered to be a logical part of their owning
entity; never of the contained entities. This is a crucial distinction! It has
the following consequences:

-  When we remove / add an object from / to a collection, the version number of
-  the collection owner is incremented.

-  If an object that was removed from a collection is an instance of a value
   type (eg, a composite element), that object will cease to be persistent and
   its state will be completely removed from the database. Likewise, adding a
   value type instance to the collection will cause its state to be immediately
   persistent.

-  On the other hand, if an entity is removed from a collection (a one-to-many
   or many-to-many association), it will not be deleted, by default. This
   behavior is completely consistent - a change to the internal state of another
   entity should not cause the associated entity to vanish! Likewise, adding an
   entity to a collection does not cause that entity to become persistent, by
   default.

Instead, the default behavior is that adding an entity to a collection merely
creates a link between the two entities, while removing it removes the link.
This is very appropriate for all sorts of cases. Where it is not appropriate at
all is the case of a parent / child relationship, where the life of the child is
bound to the lifecycle of the parent.

Bidirectional one-to-many
==========================

Suppose we start with a simple ``<one-to-many>`` association from ``Parent`` to
``Child``.

.. code:: xml

  <set name="Children">
    <key column="parent_id" />
    <one-to-many class="Child" />
  </set>

If we were to execute the following code

.. code:: csharp

  Parent p = .....;
  Child c = new Child();
  p.Children.Add(c);
  session.Save(c);
  session.Flush();

NHibernate would issue two SQL statements:

-  an ``INSERT`` to create the record for ``c``

-  an ``UPDATE`` to create the link from ``p`` to ``c``

This is not only inefficient, but also violates any ``NOT NULL`` constraint on
the ``parent_id`` column.

The underlying cause is that the link (the foreign key ``parent_id``) from ``p``
to ``c`` is not considered part of the state of the ``Child`` object and is
therefore not created in the ``INSERT``. So the solution is to make the link
part of the ``Child`` mapping.

.. code:: xml

  <many-to-one name="Parent" column="parent_id" not-null="true"/>

(We also need to add the ``Parent`` property to the ``Child`` class.)

Now that the ``Child`` entity is managing the state of the link, we tell the
collection not to update the link. We use the ``inverse`` attribute.

.. code:: xml

  <set name="Children" inverse="true">
      <key column="parent_id"/>
      <one-to-many class="Child"/>
  </set>

The following code would be used to add a new ``Child``.

.. code:: csharp

  Parent p = session.Load<Parent>(pid);
  Child c = new Child();
  c.Parent = p;
  p.Children.Add(c);
  session.Save(c);
  session.Flush();

And now, only one SQL ``INSERT`` would be issued!

To tighten things up a bit, we could create an ``AddChild()`` method of
``Parent``.

.. code:: csharp

  public void AddChild(Child c)
  {
      c.Parent = this;
      children.Add(c);
  }

Now, the code to add a ``Child`` looks like

.. code:: csharp

  Parent p = session.Load<Parent>(pid);
  Child c = new Child();
  p.AddChild(c);
  session.Save(c);
  session.Flush();

Cascading lifecycle
====================

The explicit call to ``Save()`` is still annoying. We will address this by using
cascades.

.. code:: xml

  <set name="Children" inverse="true" cascade="all">
    <key column="parent_id"/>
    <one-to-many class="Child"/>
  </set>

This simplifies the code above to

.. code:: csharp

  Parent p = session.Load<Parent>(pid);
  Child c = new Child();
  p.AddChild(c);
  session.Flush();

Similarly, we don”t need to iterate over the children when saving or deleting a
``Parent``. The following removes ``p`` and all its children from the database.

.. code:: csharp

  Parent p = session.Load<Parent>(pid);
  session.Delete(p);
  session.Flush();

However, this code

.. code:: csharp

  Parent p = session.Load<Parent>(pid);
  // Get one child out of the set
  IEnumerator childEnumerator = p.Children.GetEnumerator();
  childEnumerator.MoveNext();
  Child c = (Child) childEnumerator.Current;

  p.Children.Remove(c);
  c.Parent = null;
  session.Flush();

will not remove ``c`` from the database; it will only remove the link to ``p``
(and cause a ``NOT NULL`` constraint violation, in this case). You need to
explicitly ``Delete()`` the ``Child``.

.. code:: csharp

  Parent p = session.Load<Parent>(pid);
  // Get one child out of the set
  IEnumerator childEnumerator = p.Children.GetEnumerator();
  childEnumerator.MoveNext();
  Child c = (Child) childEnumerator.Current;

  p.Children.Remove(c);
  session.Delete(c);
  session.Flush();

Now, in our case, a ``Child`` can”t really exist without its parent. So if we
remove a ``Child`` from the collection, we really do want it to be deleted. For
this, we must use ``cascade="all-delete-orphan"``.

.. code:: xml

  <set name="Children" inverse="true" cascade="all-delete-orphan">
    <key column="parent_id"/>
    <one-to-many class="Child"/>
  </set>

Note: even though the collection mapping specifies ``inverse="true"``, cascades
are still processed by iterating the collection elements. So if you require that
an object be saved, deleted or updated by cascade, you must add it to the
collection. It is not enough to simply set its parent.

Using cascading ``Update()``
=============================

Suppose we loaded up a ``Parent`` in one ``ISession``, made some changes in a UI
action and wish to persist these changes in a new ISession (by calling
``Update()``). The ``Parent`` will contain a collection of children and, since
cascading update is enabled, NHibernate needs to know which children are newly
instantiated and which represent existing rows in the database. Let”s assume
that both ``Parent`` and ``Child`` have (synthetic) identifier properties of
type ``long``. NHibernate will use the identifier property value to determine
which of the children are new. (You may also use the version or timestamp
property, see :ref:`manipulatingdata-updating-detached`.)

The ``unsaved-value`` attribute is used to specify the identifier value of a
newly instantiated instance. *In NHibernate it is not necessary to specify
unsaved-value explicitly.*

The following code will update ``parent`` and ``child`` and insert ``newChild``.

.. code:: csharp

  //parent and child were both loaded in a previous session
  parent.AddChild(child);
  Child newChild = new Child();
  parent.AddChild(newChild);
  session.Update(parent);
  session.Flush();

Well, that is all very well for the case of a generated identifier, but what
about assigned identifiers and composite identifiers? This is more difficult,
since ``unsaved-value`` can”t distinguish between a newly instantiated object
(with an identifier assigned by the user) and an object loaded in a previous
session. In these cases, you will probably need to give NHibernate a hint;
either

-  define an ``unsaved-value`` on a ``<version>`` or ``<timestamp>`` property
   mapping for the class.

-  set ``unsaved-value="none"`` and explicitly ``Save()`` newly instantiated
   children before calling ``Update(parent)``

-  set ``unsaved-value="any"`` and explicitly ``Update()`` previously persistent
   children before calling ``Update(parent)``

``null`` is the default ``unsaved-value`` for assigned identifiers, ``none`` is
the default ``unsaved-value`` for composite identifiers.

There is one further possibility. There is a new ``IInterceptor`` method named
``IsTransient()`` which lets the application implement its own strategy for
distinguishing newly instantiated objects. For example, you could define a base
class for your persistent classes.

.. code:: csharp

  public class Persistent
  {
    private bool _saved = false;

    public void OnSave()
    {
      _saved = true;
    }

    public void OnLoad()
    {
      _saved = true;
    }

    public void OnDelete()
    {
      _saved = false;
    }

    ......

    public bool IsSaved
    {
      get { return _saved; }
    }
  }

(The ``saved`` property is non-persistent.) Now implement ``IsTransient()``,
along with ``OnLoad()``, ``OnSave()`` and ``OnDelete()`` as follows.

.. code:: csharp

  public object IsTransient(object entity)
  {
    if (entity is Persistent)
    {
      return !( (Persistent) entity ).IsSaved;
    }
    else
    {
      return null;
    }
  }

  public bool OnLoad(object entity,
      object id,
      object[] state,
      string[] propertyNames,
      IType[] types)
  {
    if (entity is Persistent) ( (Persistent) entity ).OnLoad();
    return false;
  }

  public boolean OnSave(object entity,
      object id,
      object[] state,
      string[] propertyNames,
      IType[] types)
  {
    if (entity is Persistent) ( (Persistent) entity ).OnSave();
    return false;
  }

  public virtual void OnDelete(object entity,
    object id,
    object[] state,
    string[] propertyNames,
    IType[] types)
  {
      if (entity is Persistent) ( (Persistent) entity ).OnDelete();
  }

Conclusion
===========

There is quite a bit to digest here and it might look confusing first time
around. However, in practice, it all works out quite nicely. Most NHibernate
applications use the parent / child pattern in many places.

We mentioned an alternative in the first paragraph. None of the above issues
exist in the case of ``<composite-element>`` mappings, which have exactly the
semantics of a parent / child relationship. Unfortunately, there are two big
limitations to composite element classes: composite elements may not own
collections, and they should not be the child of any entity other than the
unique parent. (However, they *may* have a surrogate primary key, using an
``<idbag>`` mapping.)
