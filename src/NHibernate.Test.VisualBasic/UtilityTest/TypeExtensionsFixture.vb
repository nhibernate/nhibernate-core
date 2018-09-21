Imports System.Reflection
Imports NUnit.Framework

Namespace UtilityTest

    <TestFixture> _
    Public Class TypeExtensionsFixture
        Inherits Test.UtilityTest.TypeExtensionsFixture
    
        <Test> _
        Public Overrides Sub GetUserDeclaredFieldsIgnoreAnonymousBackingField()
            ' Anonymous types are implemented with properties, their backing fields should be ignored
            Assert.That(GetUserDeclaredFields(New With { .a = 0 }.GetType()), [Is].Empty)
        End Sub

        <Test> _
        Public Overrides Sub GetUserDeclaredFieldsIgnoreAutoPropertiesBackingField()
            Assert.That(
                   GetUserDeclaredFields(GetType(TestClass)),
                   Has.All.Property(nameof(MemberInfo.Name)).EqualTo("_field"))
        End Sub

        Private Class TestClass
            Public Property Prop As Integer

            Private _field As Integer

            Public Sub DoSomething()
                _field = _field + Prop
                If _field > 10
                    _field = 0
                End If
            End Sub
        End Class
    End Class

End NameSpace
