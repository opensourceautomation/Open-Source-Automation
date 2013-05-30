Imports System.Windows.Forms.Integration
Imports OSAE.UI.Controls

Public Class frmAddObject

    Private host As ElementHost
    Private addNewObjectControl As AddNewObject

    Private Sub frmObject_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        host = New ElementHost
        host.Dock = DockStyle.Fill

        addNewObjectControl = New AddNewObject

        host.Child = addNewObjectControl
        Me.Controls.Add(host)
    End Sub
End Class