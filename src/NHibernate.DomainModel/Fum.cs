using System;
using System.Collections;
using NHibernate.Classic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class MapComponent
	{
		private IDictionary _fummap = new Hashtable();
		private IDictionary _stringmap = new Hashtable();
		private int _count;

		public IDictionary Fummap
		{
			get { return _fummap; }
			set { _fummap = value; }
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public IDictionary Stringmap
		{
			get { return _stringmap; }
			set { _stringmap = value; }
		}
	}

	public class Fum : ILifecycle 
	{
		private String _fum;
		private FumCompositeID _id;
		private Fum _fo;
		private Qux[] _quxArray;
		private Iesi.Collections.ISet _friends; // <set> mapping
		private DateTime m_LastUpdated;
		private MapComponent _mapComponent = new MapComponent();

		public Fum() 
		{
		}
		
		public Fum(FumCompositeID id)
		{
			_id = id;
			_friends = new Iesi.Collections.HashedSet();
			//TODO: H2.0.3 - this is diff from H2.0.3 because I am getting a null exception
			// when executing the Sql.  H203 uses the CalendarType which we don't have so
			// I am using DateTime instead...
			//m_LastUpdated = DateTime.Now;
			
			FumCompositeID fid = new FumCompositeID();
			fid.Date= new DateTime(2004, 4, 29, 9, 50, 0, 0);
			fid.Short= (short) ( id.Short + 33 );
			fid.String= id.String + "dd";
			
			Fum f = new Fum();
			f.Id = fid;
			f.FumString="FRIEND";
			//TODO: H2.0.3 - this is diff from H2.0.3 because I am getting a null exception
			// when executing the Sql.  H203 uses the CalendarType which we don't have so
			// I am using DateTime instead...
			//f.LastUpdated = DateTime.Now;

			_friends.Add( f );
		}
		
		public string FumString
		{
			get { return _fum; }
			set { this._fum = value; }
		}
	
		public FumCompositeID Id
		{
			get { return _id; }
			set { this._id = value; }
		}
		public Fum Fo
		{
			get { return _fo; }
			set { this._fo = value; }
		}
	
		public Qux[] QuxArray
		{
			get { return _quxArray; }
			set { this._quxArray = value; }
		}
	
		public Iesi.Collections.ISet Friends
		{
			get { return _friends; }
			set	{ this._friends = value; }
		}
	
	
		public LifecycleVeto OnDelete(ISession s) 
		{
			if (_friends==null) return LifecycleVeto.NoVeto;
			try 
			{
				foreach(object obj in _friends) 
				{
					s.Delete( obj );
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
			if (_friends==null) return LifecycleVeto.NoVeto;
			try 
			{
				foreach(object obj in _friends) 
				{
					s.Save( obj );
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
	
		public DateTime LastUpdated
		{
			get { return m_LastUpdated; }
			set { m_LastUpdated = value; }
		}

		public MapComponent MapComponent
		{
			get { return _mapComponent; }
			set { _mapComponent = value; }
		}
	}
}