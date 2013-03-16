Imports System.IO
Imports MySql.Data.MySqlClient
Public Class frmAddNavImage
    Private Sub frmAddNavImage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Screens()
    End Sub
    Private Sub btnImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImage.Click
        Dim res As DialogResult = file1.ShowDialog()
        If res = DialogResult.OK Then
            txtNavImage.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
            If File.Exists(file1.FileName) Then
                picImage.Image = Image.FromFile(file1.FileName)
            End If
        End If
    End Sub
    Private Sub btnNavCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNavCancel.Click
        Me.Close()
    End Sub
    Private Sub btnNavAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNavAdd.Click
        Dim sName As String
        sName = "Screen - Nav - " & txtNavName.Text
        txtNavName.Text = ""
        OSAEApi.ObjectAdd(sName, sName, "CONTROL NAVIGATION IMAGE", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sName, "Image", txtNavImage.Text.Replace(OSAEApi.APIpath, "."))
        OSAEApi.ObjectPropertySet(sName, "Screen", cboTarget.Text)
        OSAEApi.ObjectPropertySet(sName, "X", 100)
        OSAEApi.ObjectPropertySet(sName, "Y", 100)
        OSAEApi.ObjectPropertySet(sName, "ZOrder", 1)
        'OSAEApi.ObjectUpdate(sName, sName, sName, "CONTROL NAVIGATION IMAGE", "", sName, 1)

        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_screen_object_add"
        CMD.Parameters.AddWithValue("?screen", gCurrentScreen)
        CMD.Parameters.AddWithValue("?object", gAppName)
        CMD.Parameters.AddWithValue("?control", sName)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            '    sStateName = CMD.ExecuteScalar
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
        GUI.Load_Screen(gCurrentScreen)
        Me.Close()
    End Sub
    Public Sub Load_Screens()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        Dim iCounter As Integer = 0
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type='SCREEN' ORDER BY object_name"
        Try
            cboTarget.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboTarget.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Dropdowns: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
End Class