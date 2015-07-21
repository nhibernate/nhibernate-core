Imports NUnit.Framework
Imports NHibernate.Test.NHSpecificTest
Imports NHibernate.Linq

Namespace Issues

    Namespace NH0000

        <TestFixture()> _
        Public Class Fixture
            Inherits IssueTestCase

            Protected Overrides Sub OnSetUp()

                Using session As ISession = OpenSession()

                    Using transaction As ITransaction = session.BeginTransaction()

                        Dim e1 = New Entity
                        e1.Name = "Bob"
                        session.Save(e1)

                        Dim e2 = New Entity
                        e2.Name = "Sally"
                        session.Save(e2)

                        session.Flush()
                        transaction.Commit()

                    End Using

                End Using

            End Sub

            Protected Overrides Sub OnTearDown()

                Using session As ISession = OpenSession()

                    Using transaction As ITransaction = session.BeginTransaction()

                        session.Delete("from System.Object")

                        session.Flush()
                        transaction.Commit()

                    End Using

                End Using

            End Sub

            <Test()> _
            Public Sub YourTestName()

                Using session As ISession = OpenSession()

                    Using session.BeginTransaction()

                        Dim result = From e In session.Query(Of Entity)() _
                                     Where e.Name = "Bob" _
                                     Select e

                        Assert.AreEqual(1, result.ToList().Count)

                    End Using

                End Using

            End Sub

        End Class

    End Namespace

End Namespace
