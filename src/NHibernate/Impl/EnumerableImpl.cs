using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl 
{
	/// <summary>
	/// Implements enumerable
	/// </summary>
	/// <remarks>
	/// This is the IteratorImpl in H2.0.3
	/// </remarks>
	internal class EnumerableImpl : IEnumerable, IEnumerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EnumerableImpl));
		
		private IDataReader rs;
		private ISessionImplementor sess;
		private IType[] types;
		private bool single;
		private object[] currentResults;
		private bool hasNext;
		private string[][] names;
		private IDbCommand cmd;

		public EnumerableImpl(IDataReader rs, IDbCommand cmd, ISessionImplementor sess, IType[] types, string[][] columnNames) 
		{
			this.rs = rs;
			this.cmd = cmd;
			this.sess = sess;
			this.types = types;
			this.names = columnNames;

			single = types.Length==1;
		}

		private void PostMoveNext(bool hasNext) 
		{
			this.hasNext = hasNext;
			
			// there are no more records in the DataReader so clean up
			if (!hasNext) 
			{
				log.Debug("exhausted results");
				currentResults = null;
				rs.Close();
				//TODO: H2.0.3 code to synch here to close the QueryStatement
				//sess.Batcher.CloseQueryStatement( cmd, rs );
			} 
			else 
			{
				log.Debug("retreiving next results");
				currentResults = new object[types.Length];
				for (int i=0; i<types.Length; i++) 
				{
					currentResults[i] = types[i].NullSafeGet(rs, names[i], sess, null);
				}
			}
		}

		
		public IEnumerator GetEnumerator() 
		{
			this.Reset();
			return this;
		}

		public object Current 
		{
			get 
			{
				if (single) 
				{
					return currentResults[0];
				} 
				else 
				{
					return currentResults;
				}
			}
		}

		public bool MoveNext() 
		{
			PostMoveNext( rs.Read() );

			return hasNext;
		}

		public void Reset() 
		{
			//can't reset the reader...we are SOL
		}


	}
}
