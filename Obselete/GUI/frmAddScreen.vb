Imports System.IO
Public Class frmAddScreen
    Private sChosenPath As String
    Private sLocalPath As String
    Private sFileName As String
    Private Sub btnFile4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFile4.Click
        Try
            Dim res As DialogResult = file1.ShowDialog()
            If res = DialogResult.OK Then
                sChosenPath = file1.FileName
                txtScreenImage.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
                If File.Exists(sChosenPath) Then
                    Dim msScrren As MemoryStream = New MemoryStream(File.ReadAllBytes(file1.FileName))
                    picScreen.Image = Image.FromStream(msScrren)
                Else
                    MessageBox.Show("I could not find: " & txtScreenImage.Text & vbCrLf & "OSAE Path = " & OSAEApi.APIpath & vbCrLf & "If this path is not your installation folder, it is an API issue", "Add Scrren")
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error Selecting Image Path." & vbCrLf & ex.Message & vbCrLf & "OSAE Path = " & OSAEApi.APIpath, "Add Scrren")
        End Try
    End Sub
    Private Sub btnNewScreenAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewScreenAdd.Click

        If String.IsNullOrEmpty(txtNewScreenName.Text) Then
            MessageBox.Show("Please specify a name for the screen")
            Return
        End If

        If String.IsNullOrEmpty(txtScreenImage.Text) Then
            MessageBox.Show("Please specify an image for the screen")
            Return
        End If

        If File.Exists(OSAEApi.APIpath & txtScreenImage.Text) Then
            Dim sName As String
            sName = "Screen - " & txtNewScreenName.Text
            txtNewScreenName.Text = ""
            OSAEApi.ObjectAdd(sName, sName, "SCREEN", "", sName, True)
            OSAEApi.ObjectPropertySet(sName, "Background Image", txtScreenImage.Text)
            GUI.Load_Screen(sName)
            Me.Close()
        Else
            MessageBox.Show("I could not find: " & OSAEApi.APIpath & txtScreenImage.Text & vbCrLf & "OSAE Path = " & OSAEApi.APIpath & vbCrLf & "If this path is not your installation folder, it is an API issue", "Add Scrren")
        End If
    End Sub
    Private Sub btnNewScreenCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNewScreenCancel.Click
        Me.Close()
    End Sub
End Class