Imports System.Collections

Imports NHibernate.Cfg

Imports NUnit.Framework

Namespace QuickStart

    <TestFixture()> _
    Public Class UserFixture

        <Test()> _
        Public Sub ValidateQuickStart()
            Dim cfg As New Configuration
            cfg.AddAssembly("NHibernate.Examples.VB")

            Dim factory As ISessionFactory = cfg.BuildSessionFactory()
            Dim session As ISession = factory.OpenSession()
            Dim trx As ITransaction = session.BeginTransaction()

            Dim newUser As New User

            newUser.Id = "joe_cool"
            newUser.UserName = "Joseph Cool"
            newUser.Password = "abc123"
            newUser.EmailAddress = "joe@cool.com"
            newUser.LastLogon = DateTime.Now

            ' tell NHibernate that this object should be saved
            session.Save(newUser)

            ' commit all of the changes to the DB and close the ISession
            trx.Commit()
            session.Close()

            ' open another session to retreive the just inserted user
            session = factory.OpenSession()

            Dim joeCool As User
            joeCool = CType(session.Load(GetType(User), "joe_cool"), User)

            ' set Joe Cool's Last Login proeprty
            joeCool.LastLogon = DateTime.Now

            ' flush the changes from the ISession to the Database
            session.Flush()

            Dim criteria As ICriteria
            Dim recentUsers As IList

            criteria = session.CreateCriteria(GetType(User))

            criteria.Add(Expression.Expression.Gt("LastLogon", New DateTime(2004, 3, 14, 20, 0, 0)))
            recentUsers = criteria.List()

            Dim recentUser As User
            For Each recentUser In recentUsers
                Assert.IsTrue(recentUser.LastLogon > New DateTime(2004, 3, 14, 20, 0, 0))
            Next recentUser

            session.Close()

        End Sub
    End Class

End Namespace
