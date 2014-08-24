Imports NHibernate.Test.NHSpecificTest

Public Class IssueTestCase
    Inherits BugTestCase

    Protected Overrides ReadOnly Property MappingsAssembly As String
        Get
            Return "NHibernate.Test.VisualBasic"
        End Get
    End Property

    Protected Overrides ReadOnly Property Mappings As IList
        Get
            Return New String() {"Issues." + BugNumber + ".Mappings.hbm.xml"}
        End Get
    End Property

End Class
