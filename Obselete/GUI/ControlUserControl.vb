Public Class ControlUserControl
    Inherits System.Collections.CollectionBase
    Private ReadOnly HostForm As System.Windows.Forms.Form
    Public Function AddNewPanel() As System.Windows.Forms.Panel
        ' Create a new instance of the .Panel class.
        Dim aPic As New System.Windows.Forms.Panel
        ' Add the Label to the collection's internal list.
        Me.List.Add(aPic)
        ' Add the Label to the Controls collection of the Form referenced by the HostForm field.
        HostForm.Controls.Add(aPic)
        ' Set intial properties for the Label object.
        'aPic.SizeMode = PictureBoxSizeMode.AutoSize
        'If Me.Count = 1 Then
        aPic.Visible = False
        'End If
        Return aPic
    End Function
    Public Sub New(ByVal host As System.Windows.Forms.Form)
        HostForm = host
        Me.AddNewPanel()
        'HostForm.Controls.Remove(Me(Me.Count - 1))
    End Sub
    Default Public ReadOnly Property Item(ByVal Index As Integer) As System.Windows.Forms.Panel
        Get
            If Index > 0 Then
                Return CType(Me.List.Item(Index - 1), System.Windows.Forms.Panel)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public Sub Remove()
        ' Check to be sure there is a Label to remove.
        If Me.Count > 0 Then
            ' Remove the last Label added to the array from the host form controls collection. 
            ' Note the use of the default property in accessing the array.
            HostForm.Controls.Remove(Me(Me.Count))
            Me.List.RemoveAt(Me.Count - 1)
        End If
    End Sub
    Public Sub RemoveAll()
        ' Check to be sure there is a Label to remove.
        Do Until Me.Count = 0
            ' Remove the last Label added to the array from the host form controls collection. 
            ' Note the use of the default property in accessing the array.
            Me(Me.Count).Visible = False
            Application.DoEvents()
            HostForm.Controls.Remove(Me(Me.Count))
            Me.List.RemoveAt(Me.Count - 1)
        Loop

    End Sub
End Class
