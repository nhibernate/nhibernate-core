using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class Fum : ILifecycle 
	{
		private String _fum;
		private FumCompositeID _id;
		private Fum _fo;
		private Qux[] _quxArray;
		private IList _friends;
		private DateTime _lastUpdated;
	
		public Fum() {}
		public Fum(FumCompositeID id)
		{
			this.id = id;
			friends = new ArrayList();
			FumCompositeID fid = new FumCompositeID();
			fid.date= new DateTime();
			fid.@short= (short) ( id.@short + 33 );
			fid.@string= id.@string + "dd";
			Fum f = new Fum();
			f.id = fid;
			f.fum="FRIEND";
			friends.Add(f);
		}
		public string fum
		{
			get
			{
				return _fum;
			}
			set
			{
				this._fum = value;
			}
		}
	
		public FumCompositeID id
		{
			get
			{
				return _id;
			}
			set
			{
				this._id = value;
			}
		}
		public Fum fo
		{
			get
			{
				return _fo;
			}
			set
			{
				this._fo = value;
			}
		}
	
		public Qux[] quxArray
		{
			get
			{
				return _quxArray;
			}
			set
			{
				this._quxArray = value;
			}
		}
	
		public IList friends
		{
			get
			{
				return _friends;
			}
			set	
			{
				this._friends = value;
			}
		}
	
	
		public LifecycleVeto OnDelete(ISession s) 
		{
			if (friends==null) return LifecycleVeto.NoVeto;
			try 
			{
				IEnumerator iter = friends.GetEnumerator();
				while ( iter.MoveNext() ) 
				{
					s.Delete( iter.Current );
				}
			}
			catch (Exception e) 
			{
				throw new CallbackException(e);
			}
			return LifecycleVeto.NoVeto;
		}
	
	
		public void OnLoad(ISession s, object id) 
		{
		}
	
	
		public LifecycleVeto OnSave(ISession s) 
		{
			if (friends==null) return LifecycleVeto.NoVeto;
			try 
			{
				IEnumerator iter = friends.GetEnumerator();
				while ( iter.MoveNext() ) 
				{
					s.Save( iter.Current );
				}
			}
			catch (Exception e) 
			{
				throw new CallbackException(e);
			}
			return LifecycleVeto.NoVeto;
		}
	
	
		public LifecycleVeto OnUpdate(ISession s) 
		{
			return LifecycleVeto.NoVeto;
		}
	
		public DateTime lastUpdated
		{
			get
			{
				return _lastUpdated;
			}
			set
			{
				_lastUpdated = value;
			}
		}

	}
}