using System;
using System.Collections;

namespace NHibernate.DomainModel
{

	public class Glarch : Super, GlarchProxy, ILifecycle
	{
	
		private int _version;

		private int _x;
	
		public int x
		{
			get
			{
				return _x;
			}
			set
			{
				this._x = value;
			}
		}
	
		public int version
		{
			get
			{
				return _version;
			}
			set
			{
				this._version = value;
			}
		}

		/// <summary>
		/// Holds the _next
		/// </summary> 
		private GlarchProxy _next;

		/// <summary>
		/// Gets or sets the _next
		/// </summary> 
		public GlarchProxy next
		{
			get 
			{
				return _next; 
			}
			set 
			{
				_next = value;
			}
		}

		/// <summary>
		/// Holds the _order
		/// </summary> 
		private short _order;

		/// <summary>
		/// Gets or sets the _order
		/// </summary> 
		public short order
		{
			get 
			{
				return _order; 
			}
			set 
			{
				_order = value;
			}
		}

		/// <summary>
		/// Holds the _strings
		/// </summary> 
		private IList _strings;

		/// <summary>
		/// Gets or sets the _strings
		/// </summary> 
		public IList strings
		{
			get 
			{
				return _strings; 
			}
			set 
			{
				_strings = value;
			}
		}

		/// <summary>
		/// Holds the _stringSets
		/// </summary> 
		private IDictionary _stringSets;

		/// <summary>
		/// Gets or sets the _stringSets
		/// </summary> 
		public IDictionary stringSets
		{
			get 
			{
				return _stringSets; 
			}
			set 
			{
				_stringSets = value;
			}
		}

		/// <summary>
		/// Holds the _fooComponents
		/// </summary> 
		private IList _fooComponents;

		/// <summary>
		/// Gets or sets the _fooComponents
		/// </summary> 
		public IList fooComponents
		{
			get 
			{
				return _fooComponents; 
			}
			set 
			{
				_fooComponents = value;
			}
		}

		/// <summary>
		/// Holds the _proxyArray
		/// </summary> 
		private GlarchProxy[] _proxyArray;

		/// <summary>
		/// Gets or sets the _proxyArray
		/// </summary> 
		public GlarchProxy[] proxyArray
		{
			get 
			{
				return _proxyArray; 
			}
			set 
			{
				_proxyArray = value;
			}
		}

		/// <summary>
		/// Holds the _proxySet
		/// </summary> 
		private IList _proxySet;

		/// <summary>
		/// Gets or sets the _proxySet
		/// </summary> 
		public IList proxySet
		{
			get 
			{
				return _proxySet; 
			}
			set 
			{
				_proxySet = value;
			}
		}
	
		public LifecycleVeto OnDelete(ISession s)
		{
			return LifecycleVeto.NoVeto;
		}
	
		public void OnLoad(ISession s, object id) 
		{
			if ( ! ( ( (String) id ).Length==32 ) ) throw new ArgumentException("id problem");
		}
	
		public LifecycleVeto OnSave(ISession s)
		{
			/*		DynaClass dc = new BasicDynaClass(
						"dyna", 
						BasicDynaBean.class,
						new DynaProperty[] {
							new DynaProperty("foo", tString.class),
							new DynaProperty("bar", Integer.class)
						} 
					);
					try {
						dynaBean = dc.newInstance();
					}
					catch (Exception e) {
						throw new CallbackException(e);
					}
					dynaBean.set("foo", "foo");
					dynaBean.set("bar", new Integer(66));
					immutable="never changes!";
			*/
			return LifecycleVeto.NoVeto;
		}
	
		public LifecycleVeto OnUpdate(ISession s) 
		{
			return LifecycleVeto.NoVeto;
		}
	
		/// <summary>
		/// Holds the _dynaBean
		/// </summary> 
		private object _dynaBean;

		/// <summary>
		/// Gets or sets the _dynaBean
		/// </summary> 
		public object dynaBean
		{
			get 
			{
				return _dynaBean; 
			}
			set 
			{
				_dynaBean = value;
			}
		}

		/// <summary>
		/// Holds the _immutable
		/// </summary> 
		private string _immutable;

		/// <summary>
		/// Gets or sets the _immutable
		/// </summary> 
		public string immutable
		{
			get 
			{
				return _immutable; 
			}
			set 
			{
				_immutable = value;
			}
		}

		/// <summary>
		/// Holds the _derivedVersion
		/// </summary> 
		private int _derivedVersion;

		/// <summary>
		/// Gets or sets the _derivedVersion
		/// </summary> 
		public int derivedVersion
		{
			get 
			{
				return _derivedVersion; 
			}
			set 
			{
				_derivedVersion = value;
			}
		}

		/// <summary>
		/// Holds the _any
		/// </summary> 
		private object _any;

		/// <summary>
		/// Gets or sets the _any
		/// </summary> 
		public object any
		{
			get 
			{
				return _any; 
			}
			set 
			{
				_any = value;
			}
		}

		/// <summary>
		/// Holds the _multiple
		/// </summary> 
		private Multiplicity _multiple;

		/// <summary>
		/// Gets or sets the _multiple
		/// </summary> 
		public Multiplicity multiple
		{
			get 
			{
				return _multiple; 
			}
			set 
			{
				_multiple = value;
			}
		}

		public new string name
		{
			get
			{
				return base.name;
			}
			set
			{
				this.name = value;
			}
		}

	}
}



