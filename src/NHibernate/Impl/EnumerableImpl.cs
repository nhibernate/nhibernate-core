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
	//TODO: revisit this class and make sure the port is what is intended
	internal class EnumerableImpl : IEnumerable, IEnumerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EnumerableImpl));
		
		private IDataReader rs;
		private ISessionImplementor sess;
		private IType[] types;
		private bool single;
		private object[] nextResults;
		private object[] currentResults;
		private bool hasNext;
		private string[][] names;
		private IDbCommand ps;

		//TODO: H2.0.3 change ctor to include ps
		public EnumerableImpl(IDataReader rs, ISessionImplementor sess, IType[] types, string[][] columnNames) 
		{
			this.rs = rs;
			//this.ps = ps;
			this.sess = sess;
			this.types = types;
			this.names = columnNames;

			single = types.Length==1;

			//TODO: find out if we do need to move to the NextResult right away.
			//PostNext(rs.NextResult());
		}

		private void PostNext(bool hasNext) 
		{
			this.hasNext = hasNext;
			if (!hasNext) 
			{
				log.Debug("exhausted results");
				nextResults = null;
				rs.Close();
				//TODO: H2.0.3 code to synch here to close the QueryStatement
			} 
			else 
			{
				log.Debug("retreiving next results");
				nextResults = new object[types.Length];
				for (int i=0; i<types.Length; i++) 
				{
					nextResults[i] = types[i].NullSafeGet(rs, names[i], sess, null);
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
					return nextResults[0];
				} 
				else 
				{
					return nextResults;
				}
			}
		}

		public bool MoveNext() 
		{
			PostNext(rs.Read());

			return hasNext;
		}

		public void Reset() {
			//can't reset the reader...we are SOL
		}


	}
}
