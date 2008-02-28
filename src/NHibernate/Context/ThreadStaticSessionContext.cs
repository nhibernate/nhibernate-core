using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for each thread using the [<see cref="ThreadStaticAttribute"/>].
	/// To avoid if there are two session factories in the same thread.
	/// </summary>
	[Serializable]
	public class ThreadStaticSessionContext : CurrentSessionContext
	{
		private readonly ILog log = LogManager.GetLogger(typeof (ThreadStaticSessionContext));

		[ThreadStatic] private static IDictionary<ISessionFactory, ISession> context;

		protected readonly ISessionFactoryImplementor factory;




		public ThreadStaticSessionContext(Engine.ISessionFactoryImplementor factory)
		{
			this.factory = factory;
		}

		/// <summary> Gets or sets the currently bound session. </summary>
		protected override ISession Session
		{
			get 
			{
				throw new NotImplementedException();
			}
			set 
			{ 
				throw new NotImplementedException();
			}
		}


		public override ISession CurrentSession()
		{
			ISession current = ExistingSession(factory);
			if (current == null)
			{
				current = buildOrObtainSession();
				
				// wrap the session in the transaction-protection proxy
				if (NeedsWrapping(current))
				{
					current = Wrap(current);
				}
				// then bind it
				DoBind(current, factory);

			}
			return current;
		}

		private static void CleanupAnyOrphanedSession(ISessionFactory factory)
		{
			throw new NotImplementedException();
		}

		public static void Bind(ISession session)
		{
			ISessionFactory factory = session.SessionFactory;
			CleanupAnyOrphanedSession(factory);
			DoBind(session, factory);
		}

		/// <summary>
		/// Unassociate a previously bound session from the current thread of execution.
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		public static ISession Unbind(ISessionFactory factory)
		{
			return DoUnbind(factory, true);
		}

		private static void DoBind(ISession current, ISessionFactory factory)
		{
			context = context ?? new Dictionary<ISessionFactory, ISession>();

			context.Add(factory, current);
		}

		private static ISession DoUnbind(ISessionFactory factory, bool releaseMapIfEmpty)
		{
			ISession session = null;

			if(context != null)
			{
				if (context.ContainsKey(factory))
				{
					session = context[factory];
					context.Remove(factory);
				}

				if(releaseMapIfEmpty && context.Count == 0) 
					context = null;
			}
			return session;
		}

		private ISession Wrap(ISession current)
		{
			return current;
		}

		private bool NeedsWrapping(ISession current)
		{
			return false;
		}

		protected ISession buildOrObtainSession()
		{
			throw new NotImplementedException();

			//return factory.OpenSession(
			//  null,
			//	IsAutoFlushEnabled(),
			//  IsAutoCloseEnabled(),
			//  GetConnectionReleaseMode());
		}

		private ConnectionReleaseMode GetConnectionReleaseMode()
		{
			return factory.Settings.ConnectionReleaseMode;
		}

		protected bool IsAutoCloseEnabled()
		{
			return true;
		}

		protected bool IsAutoFlushEnabled()
		{
			return true;
		}
		
		private static ISession ExistingSession(ISessionFactory factory)
		{
			if (context == null)
				return null;
			else
			{
				if (context.ContainsKey(factory))
					return context[factory];
				else
					return null;
			}
		}
	}
}
