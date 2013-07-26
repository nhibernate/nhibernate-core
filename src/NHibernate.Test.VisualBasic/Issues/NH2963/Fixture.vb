Imports NHibernate.Mapping.ByCode
Imports NUnit.Framework

Namespace Issues.NH2963
    <TestFixture()>
    Public Class Fixture
        <Test>
        Public Sub GenericDecodeMemberAccessExpressionForCollectionAsEnumerableShouldReturnMemberOfDeclaringClass()
            TypeExtensions.DecodeMemberAccessExpression (Of Entity, IEnumerable(Of String))(Function(mc) mc.Childs)
        End Sub

        <Test>
        Public Sub GenericDecodeMemberAccessExpressionOfForCollectionAsEnumerableShouldReturnMemberOfDeclaringClass()
            TypeExtensions.DecodeMemberAccessExpressionOf (Of Entity, IEnumerable(Of String))(Function(mc) mc.Childs)
        End Sub
    End Class
End Namespace