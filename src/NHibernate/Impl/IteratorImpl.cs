using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Portable;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Summary description for IteratorImpl.
	/// </summary>
	public class IteratorImpl : IIterator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(IteratorImpl));
		
		private IDataReader         rs;
		private ISessionImplementor sess;
		private IType[]             types;
		private bool                single;
		private object[]            nextResults;
		private object[]            currentResults;
		private bool                hasNext;
		private string[][]          names;
		
		public IteratorImpl(IDataReader rs, ISessionImplementor sess, IType[] types, string[][] columnNames)
		{
			this.rs    = rs;
			this.sess  = sess;
			this.types = types;
			this.names = columnNames;
			
			single     = types.Length == 1;
			
			PostNext(rs.Read());
		}
		
		private void  PostNext(bool hasNext)
		{
			this.hasNext = hasNext;
			if (!hasNext)
			{
				nextResults = null;
				rs.Close();
			}
			else
			{
				nextResults = new object[types.Length];
				for (int i = 0; i < types.Length; i++)
				{
					nextResults[i] = types[i].NullSafeGet(rs, names[i], sess, null);
				}
			}
		}
		
		public bool HasNext
		{
			get { return hasNext; }
		}
		
		public object Next()
		{
			if (nextResults == null)
			{
				throw new Exception("No more results");
			}
			try
			{
				currentResults = nextResults;
				PostNext(rs.Read());
				if (single)
				{
					return currentResults[0];
				}
				else
				{
					return currentResults;
				}
			}
			catch (Exception sqle)
			{
				log.Error("could not get next result", sqle);

				throw new LazyInitializationException(sqle);
			}
		}
		
		public void  Remove()
		{
			if (!single)
			{
				throw new NotSupportedException("Not a single column hibernate query result set");
			}
			if (currentResults == null)
			{
				throw new System.SystemException("Called IIterator.Remove() before Next()");
			}
			try
			{
				sess.Delete(currentResults[0]);
			}
			catch (Exception sqle)
			{
				log.Error("could not remove", sqle);

				throw new LazyInitializationException(sqle);
			}
		}
	}
}
