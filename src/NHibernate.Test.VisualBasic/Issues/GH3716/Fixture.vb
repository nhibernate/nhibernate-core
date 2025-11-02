Imports NHibernate.Linq
Imports NUnit.Framework

Namespace Issues.GH3716
    <TestFixture>
    Public Class Fixture
        Inherits IssueTestCase

        Protected Overrides Sub OnSetUp()

            Using session As ISession = OpenSession()

                Using transaction As ITransaction = session.BeginTransaction()

                    Dim e1 = New Entity
                    e1.Date1 = New Date(2017, 12, 3)
                    session.Save(e1)

                    Dim e2 = New Entity
                    e2.Date1 = New Date(2017, 12, 1)
                    session.Save(e2)

                    Dim e3 = New Entity
                    session.Save(e3)

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

        <Test>
        Public Sub ShouldBeAbleToUpdateWithAnonymousType()

            Using session As ISession = OpenSession()
                session.Query(Of Entity).Update(Function(x) New With {.Date1 = Date.Today})
            End Using
        End Sub
    End Class
End Namespace
