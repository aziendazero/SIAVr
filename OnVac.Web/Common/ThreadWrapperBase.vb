Public MustInherit Class ThreadWrapperBase
    Private m_Done As Boolean

    Public ReadOnly Thread As System.Threading.Thread

    Sub New()

        Me.Thread = New System.Threading.Thread(AddressOf Me.RunThread)

    End Sub

    Public Overridable Sub Start()

        Me.Thread.Start()

    End Sub

    Private Sub RunThread()

        m_Done = False

        OnStart()

        m_Done = True

    End Sub

    Public ReadOnly Property Done() As Boolean

        Get

            Return m_Done

        End Get

    End Property

    Protected MustOverride Sub OnStart()

End Class
