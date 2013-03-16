Imports MySql.Data.MySqlClient
Imports System.IO
Public Class frmAddMethodImage
    Private Sub frmAllMethodImage_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Add_Object()
    End Sub
    Private Sub Load_Add_Object()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type<>'CONTROL' ORDER BY object_name"
        Try
            cboObjects.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboObjects.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Public Sub Load_Methods()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        cboMethods.Items.Clear()
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT method_label FROM osae_v_object_method WHERE UPPER(object_name)=UPPER(?ObjectName) ORDER BY method_label"
        CMD.Parameters.AddWithValue("?ObjectName", cboObjects.Text)
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboMethods.Items.Add(Convert.ToString(myReader.Item("method_label")))
            End While
            CN.Close()
            If cboMethods.Items.Count > 0 Then cboMethods.SelectedIndex = 0
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Methods: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Private Sub btnMethodImageCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMethodImageCancel.Click
        Me.Close()
    End Sub
    Private Sub btnMethodImageAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMethodImageAdd.Click
        Dim sName As String = gCurrentScreen & " - " & cboObjects.Text & " - " & cboMethods.Text
        OSAEApi.ObjectAdd(sName, sName, "CONTROL METHOD IMAGE", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sName, "Object Name", cboObjects.Text)
        OSAEApi.ObjectPropertySet(sName, "Method Name", cboMethods.Text)
        OSAEApi.ObjectPropertySet(sName, "Image", txtMethodImage.Text)
        OSAEApi.ObjectPropertySet(sName, "Param 1", txtPararm1.Text)
        OSAEApi.ObjectPropertySet(sName, "Param 2", txtPararm2.Text)
        OSAEApi.ObjectPropertySet(sName, "X", 100)
        OSAEApi.ObjectPropertySet(sName, "Y", 100)
        OSAEApi.ObjectPropertySet(sName, "ZOrder", 1)

        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_screen_object_add"
        CMD.Parameters.AddWithValue("?screen", gCurrentScreen)
        CMD.Parameters.AddWithValue("?object", cboObjects.Text)
        CMD.Parameters.AddWithValue("?control", sName)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            CN.Close()
        Catch ex As Exception
            MessageBox.Show("Error Load_Add_Object: " & ex.Message)
            CN.Close()
            Exit Sub
        End Try
        GUI.Load_Screen(gCurrentScreen)
        Me.Close()
    End Sub
    Private Sub cboObjects_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboObjects.SelectedIndexChanged
        Load_Methods()
    End Sub
    Private Sub btnOnImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOnImage.Click
        Dim res As DialogResult = file1.ShowDialog()
        If res = DialogResult.OK Then
            txtMethodImage.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
            If File.Exists(file1.FileName) Then
                picON.Image = Image.FromFile(file1.FileName)
            End If
        End If
    End Sub
End Class