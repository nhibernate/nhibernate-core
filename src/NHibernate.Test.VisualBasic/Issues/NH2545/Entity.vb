Namespace Issues

    Namespace NH2545

        Public Class Entity

            Private _id As Guid
            Public Overridable Property Id() As Guid
                Get
                    Return _id
                End Get
                Set(ByVal value As Guid)
                    _id = value
                End Set
            End Property

            Private _name As String
            Public Overridable Property Name() As String
                Get
                    Return _name
                End Get
                Set(ByVal value As String)
                    _name = value
                End Set
            End Property

        End Class

    End Namespace

End Namespace