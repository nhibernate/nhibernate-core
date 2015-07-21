using System.Collections;
using NUnit.Framework;
using NHibernate.Type;

namespace NHibernate.Test.Interceptor
{
	[TestFixture]
	public class InterceptorFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[] { "Interceptor.User.hbm.xml", "Interceptor.Image.hbm.xml" };
			}
		}

		[Test]
		public void CollectionIntercept()
		{
			ISession s = OpenSession(new CollectionInterceptor());
			ITransaction t = s.BeginTransaction();
			User u = new User("Gavin", "nivag");
			s.Persist(u);
			u.Password = "vagni";
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			u = s.Get<User>("Gavin");
			Assert.AreEqual(2, u.Actions.Count);
			s.Delete(u);
			t.Commit();
			s.Close();
		}

		[Test]
		public void PropertyIntercept()
		{
			ISession s = OpenSession(new PropertyInterceptor());
			ITransaction t = s.BeginTransaction();
			User u = new User("Gavin", "nivag");
			s.Persist(u);
			u.Password = "vagni";
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			u = s.Get<User>("Gavin");
			Assert.IsTrue(u.Created.HasValue);
			Assert.IsTrue(u.LastUpdated.HasValue);
			s.Delete(u);
			t.Commit();
			s.Close();
		}

		private class HHH1921Interceptor : EmptyInterceptor
		{
			public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
			{
				currentState[0] = "test";
				return true;
			}
		}

		///
		///Here the interceptor resets the
		///current-state to the same thing as the current db state; this
		///causes EntityPersister.FindDirty() to return no dirty properties.
		///
		[Test]
		public void PropertyIntercept2()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			User u = new User("Josh", "test");
			s.Persist(u);
			t.Commit();
			s.Close();

			s = OpenSession(new HHH1921Interceptor());
			t = s.BeginTransaction();
			u = s.Get<User>(u.Name);
			u.Password = "nottest";
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			u = s.Get<User>("Josh");
			Assert.AreEqual("test", u.Password);
			s.Delete(u);
			t.Commit();
			s.Close();
		}

		private class MyComponentInterceptor : EmptyInterceptor
		{
			readonly int checkPerm;
			readonly string checkComment;
			public MyComponentInterceptor(int checkPerm, string checkComment)
			{
				this.checkPerm = checkPerm;
				this.checkComment = checkComment;
			}

			public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
			{
				if (state[0] == null)
				{
					Image.Detail detail = new Image.Detail();
					detail.Perm1 = checkPerm;
					detail.Comment = checkComment;
					state[0] = detail;
				}
				return true;
			}
		}

		[Test]
		public void ComponentInterceptor()
		{
			const int checkPerm = 500;
			const string checkComment = "generated from interceptor";

			ISession s = OpenSession(new MyComponentInterceptor(checkPerm, checkComment));
			ITransaction t = s.BeginTransaction();
			Image i = new Image();
			i.Name = "compincomp";
			i = (Image)s.Merge(i);
			Assert.IsNotNull(i.Details);
			Assert.AreEqual(checkPerm, i.Details.Perm1);
			Assert.AreEqual(checkComment, i.Details.Comment);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			i = s.Get<Image>(i.Id);
			Assert.IsNotNull(i.Details);
			Assert.AreEqual(checkPerm, i.Details.Perm1);
			Assert.AreEqual(checkComment, i.Details.Comment);
			s.Delete(i);
			t.Commit();
			s.Close();
		}

		[Test]
		public void StatefulIntercept()
		{
			StatefulInterceptor statefulInterceptor = new StatefulInterceptor();
			ISession s = OpenSession(statefulInterceptor);
			Assert.IsNotNull(statefulInterceptor.Session);

			ITransaction t = s.BeginTransaction();
			User u = new User("Gavin", "nivag");
			s.Persist(u);
			u.Password = "vagni";
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			IList logs = s.CreateCriteria(typeof(Log)).List();
			Assert.AreEqual(2, logs.Count);
			s.Delete(u);
			s.Delete("from Log");
			t.Commit();
			s.Close();
		}
	}
}