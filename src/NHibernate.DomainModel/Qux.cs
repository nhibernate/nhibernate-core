using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	//TODO: fix up property names 
	public class Qux : ILifecycle 
	{
	
		private ISession session;
	
		public Qux() { }
	
		public Qux(String s) 
		{
			stuff=s;
		}
	
		#region ILifecycle members

		public LifecycleVeto OnSave(ISession session) 
		{
			created=true;
			try 
			{
				foo = new Foo();
				session.Save(foo);
			}
			catch (Exception e) 
			{
				throw new CallbackException(e);
			}
			foo.@string = "child of a qux";
			return LifecycleVeto.NoVeto;
		}
	
		public LifecycleVeto OnDelete(ISession session) 
		{
			deleted=true;
			try 
			{
				session.Delete(foo);
			}
			catch (Exception e) 
			{
				throw new CallbackException(e);
			}
			//if (child!=null) session.delete(child);
			return LifecycleVeto.NoVeto;
		}
	
		public void OnLoad(ISession session, object id) 
		{
			loaded=true;
			this.session=session;
		}
		
		public LifecycleVeto OnUpdate(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}

		#endregion

		public void store() 
		{
		}

		/// <summary>
		/// Holds the _foo
		/// </summary> 
		private FooProxy _foo;

		/// <summary>
		/// Gets or sets the _foo
		/// </summary> 
		public FooProxy foo
		{
			get 
			{
				return _foo; 
			}
			set 
			{
				_foo = value;
			}
		}

		/// <summary>
		/// Holds the _created
		/// </summary> 
		private bool _created;

		/// <summary>
		/// Gets or sets the _created
		/// </summary> 
		public bool created
		{
			get 
			{
				return _created; 
			}
			set 
			{
				_created = value;
			}
		}
		/// <summary>
		/// Holds the _deleted
		/// </summary> 
		private bool _deleted;

		/// <summary>
		/// Gets or sets the _deleted
		/// </summary> 
		public bool deleted
		{
			get 
			{
				return _deleted; 
			}
			set 
			{
				_deleted = value;
			}
		}

		/// <summary>
		/// Holds the _loaded
		/// </summary> 
		private bool _loaded;

		/// <summary>
		/// Gets or sets the _loaded
		/// </summary> 
		public bool loaded
		{
			get 
			{
				return _loaded; 
			}
			set 
			{
				_loaded = value;
			}
		}
	
		/// <summary>
		/// Holds the _stored
		/// </summary> 
		private bool _stored;

		/// <summary>
		/// Gets or sets the _stored
		/// </summary> 
		public bool stored
		{
			get 
			{
				return _stored; 
			}
			set 
			{
				_stored = value;
			}
		}
		/// <summary>
		/// Holds the _key
		/// </summary> 
		private long _key;

		/// <summary>
		/// Gets or sets the _key
		/// </summary> 
		public long key
		{
			get 
			{
				return _key; 
			}
			set 
			{
				_key = value;
			}
		}

	
		public long TheKey
		{
			set
			{
				this.key = value;
			}
		}

		/// <summary>
		/// Holds the _stuff
		/// </summary> 
		private string _stuff;

		/// <summary>
		/// Gets or sets the _stuff
		/// </summary> 
		public string stuff
		{
			get 
			{
				return _stuff; 
			}
			set 
			{
				_stuff = value;
			}
		}

		/// <summary>
		/// Holds the _fums
		/// </summary> 
		private IDictionary _fums;

		/// <summary>
		/// Gets or sets the _fums
		/// </summary> 
		public IDictionary fums
		{
			get 
			{
				return _fums; 
			}
			set 
			{
				_fums = value;
			}
		}

		/// <summary>
		/// Holds the _moreFums
		/// </summary> 
		private IList _moreFums;

		/// <summary>
		/// Gets or sets the _moreFums
		/// </summary> 
		public IList moreFums
		{
			get 
			{
				return _moreFums; 
			}
			set 
			{
				_moreFums = value;
			}
		}
	
		private Qux _child;
		public Qux child
		{
			get
			{
				stored=true;
				this.childKey = child==null ? 0 : child.key;
				if (childKey!=0 && child==null) child = (Qux) session.Load(typeof(Qux), childKey);
				return _child;
			}
			set
			{
				this._child = value;
			}
		}	

		/// <summary>
		/// Holds the _childKey
		/// </summary> 
		private long _childKey;

		/// <summary>
		/// Gets or sets the _childKey
		/// </summary> 
		public long childKey
		{
			get 
			{
				return _childKey; 
			}
			set 
			{
				_childKey = value;
			}
		}
	
		
	}
}