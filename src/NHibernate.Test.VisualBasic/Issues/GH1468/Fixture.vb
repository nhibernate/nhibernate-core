Imports NUnit.Framework

Namespace Issues.GH1468
    <TestFixture>
    Public Class Fixture
        Inherits IssueTestCase

        Protected Overrides Sub OnSetUp()

            Using session As ISession = OpenSession()

                Using transaction As ITransaction = session.BeginTransaction()

                    Dim e1 = New Entity
                    e1.Date1 = new Date(2017, 12, 3)
                    session.Save(e1)

                    Dim e2 = New Entity
                    e2.Date1 = new Date(2017, 12, 1)
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
        Public Sub ShouldBeAbleToCompareNullableDate()

            Using session As ISession = OpenSession()
                Using session.BeginTransaction()

                    Dim d = New Date(2017, 12, 2)
                    Dim result = (From e In session.Query(Of Entity)
                                  Where e.Date1 >= d
                                  Select e).ToList()

                    Assert.AreEqual(1, result.Count)
                End Using
            End Using
        End Sub

        <Test>
        Public Sub ShouldBeAbleToCompareNullableDateGetValueOrDefault()

            Using session As ISession = OpenSession()
                Using session.BeginTransaction()

                    Dim d = New Date(2017, 12, 2)
                    Dim result = (From e In session.Query(Of Entity)
                                  Where (e.Date1 >= d).GetValueOrDefault()
                                  Select e).ToList()

                    Assert.AreEqual(1, result.Count)
                End Using
            End Using
        End Sub
    End Class
End Namespace
