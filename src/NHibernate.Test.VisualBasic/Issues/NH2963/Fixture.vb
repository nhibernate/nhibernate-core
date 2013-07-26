Imports NHibernate.Mapping.ByCode
Imports NUnit.Framework
Namespace Issues.NH2963

  <TestFixture()>
  Public Class Fixture

    <Test>
    Public Sub GenericDecodeMemberAccessExpressionForCollectionAsEnumerableShouldReturnMemberOfDeclaringClass()
      Mapping.ByCode.TypeExtensions.DecodeMemberAccessExpression(Of Entity, IEnumerable(Of String))(Function(mc) mc.Childs)
    End Sub

  End Class
End Namespace