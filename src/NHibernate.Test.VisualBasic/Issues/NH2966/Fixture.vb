Imports NHibernate.Linq
Imports NUnit.Framework

Namespace Issues.NH2966
    <TestFixture()> _
    Public Class Fixture
        Inherits IssueTestCase

        Protected Overrides Sub OnSetUp()

            Using session = OpenSession()
                Using transaction = session.BeginTransaction()
                    Dim c1 = New Client
                    c1.Name = "Bob"
                    session.Save(c1)

                    Dim c2 = New Client
                    c2.Name = "Sally"
                    session.Save(c2)

                    session.Flush()

                    transaction.Commit()
                End Using
            End Using
        End Sub

        Protected Overrides Sub OnTearDown()

            Using session = OpenSession()
                Using transaction = session.BeginTransaction()
                    session.Delete("from System.Object")
                    session.Flush()

                    transaction.Commit()
                End Using
            End Using
        End Sub

        <Test()> _
        Public Sub FetchMany()

            Using session = OpenSession()
                Using transaction = session.BeginTransaction()

                    Dim list = session.Query(Of Client)() _
                            .Where(Function(c) c.Name = "Bob") _
                            .FetchMany(Function(c) c.Orders) _
                            .ToList()

                    Assert.AreEqual(1, list.Count)

                    transaction.Rollback()
                End Using
            End Using
        End Sub
    End Class
End Namespace

