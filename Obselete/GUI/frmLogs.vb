
Imports System.Windows.Forms.Integration
Imports OSAE.UI.Controls

Public Class frmLogs
    Private host As ElementHost
    Private logsControl As Logs

    Private Sub logs_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        host = New ElementHost
        host.Dock = DockStyle.Fill

        logsControl = New Logs

        host.Child = logsControl
        Me.Controls.Add(host)

    End Sub
End Class