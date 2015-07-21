Namespace Issues.NH2966
    Public Class Order
        Private _id As Guid

        Public Overridable Property Id As Guid
            Get
                Return _id
            End Get
            Set(ByVal value As Guid)
                _id = value
            End Set
        End Property
    End Class
End NameSpace