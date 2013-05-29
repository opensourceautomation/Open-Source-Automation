Imports System.Windows.Forms.Integration
Imports OSAE.UI.Controls

Public Class frmAddCameraViewer

    Private host As ElementHost
    Private addNewCameraViewerControl As AddNewCameraViewer

    Private Sub frmAddCameraViewer_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        GUI.Load_Screen(gCurrentScreen)
    End Sub

    Private Sub frmObject_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        host = New ElementHost
        host.Dock = DockStyle.Fill

        addNewCameraViewerControl = New AddNewCameraViewer
        addNewCameraViewerControl.currentScreen = gCurrentScreen
        host.Child = addNewCameraViewerControl
        Me.Controls.Add(host)


    End Sub


End Class