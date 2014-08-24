using System;
using System.Collections.Generic;
using NHibernate.Classic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Qux : ILifecycle
	{
		private ISession _session;
		private long _key;

		private FooProxy _foo;
		private bool _created;
		private bool _deleted;
		private bool _loaded;
		private bool _stored;
		private string _stuff;
		private ISet<Fum> _fums;
		private IList<Fum> _moreFums;
		private Qux _child;
		private long _childKey;
		private Holder _holder;

		public Qux()
		{
		}

		public Qux(String s)
		{
			_stuff = s;
		}

		#region ILifecycle members

		public virtual LifecycleVeto OnSave(ISession session)
		{
			_created = true;
			try
			{
				Foo = new Foo();
				session.Save(Foo);
			}
			catch (Exception e)
			{
				throw new CallbackException(e);
			}
			Foo.String = "child of a qux";
			return LifecycleVeto.NoVeto;
		}

		public virtual LifecycleVeto OnDelete(ISession session)
		{
			_deleted = true;
			try
			{
				session.Delete(Foo);
			}
			catch (Exception e)
			{
				throw new CallbackException(e);
			}
			//if (child!=null) session.delete(child);
			return LifecycleVeto.NoVeto;
		}

		public virtual void OnLoad(ISession session, object id)
		{
			_loaded = true;
			_session = session;
		}

		public virtual LifecycleVeto OnUpdate(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		#endregion

		public virtual void Store()
		{
		}

		/// <summary>
		/// Gets or sets the _foo
		/// </summary> 
		public virtual FooProxy Foo
		{
			get { return _foo; }
			set { _foo = value; }
		}

		/// <summary>
		/// Gets or sets the _created
		/// </summary> 
		public virtual bool Created
		{
			get { return _created; }
			set { _created = value; }
		}

		/// <summary>
		/// Gets or sets the _deleted
		/// </summary> 
		public virtual bool Deleted
		{
			get { return _deleted; }
			set { _deleted = value; }
		}

		/// <summary>
		/// Gets or sets the _loaded
		/// </summary> 
		public virtual bool Loaded
		{
			get { return _loaded; }
			set { _loaded = value; }
		}

		/// <summary>
		/// Gets or sets the _stored
		/// </summary> 
		public virtual bool Stored
		{
			get { return _stored; }
			set { _stored = value; }
		}

		/// <summary>
		/// Gets or sets the _key
		/// </summary> 
		public virtual long Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public virtual long TheKey
		{
			set { _key = value; }
		}

		/// <summary>
		/// Gets or sets the _stuff
		/// </summary> 
		public virtual string Stuff
		{
			get { return _stuff; }
			set { _stuff = value; }
		}

		/// <summary>
		/// Gets or sets the _fums (&lt;set&gt;)
		/// </summary> 
		public virtual ISet<Fum> Fums
		{
			get { return _fums; }
			set { _fums = value; }
		}

		/// <summary>
		/// Gets or sets the _moreFums
		/// </summary> 
		public virtual IList<Fum> MoreFums
		{
			get { return _moreFums; }
			set { _moreFums = value; }
		}

		public virtual Qux Child
		{
			get
			{
				_stored = true;
				_childKey = _child == null ? 0 : _child.Key;
				if (_childKey != 0 && _child == null) _child = (Qux) _session.Load(typeof(Qux), _childKey);
				return _child;
			}
			set { _child = value; }
		}

		/// <summary>
		/// Gets or sets the _childKey
		/// </summary> 
		public virtual long ChildKey
		{
			get { return _childKey; }
			set { _childKey = value; }
		}

		public virtual Holder Holder
		{
			get { return _holder; }
			set { _holder = value; }
		}
	}
}
