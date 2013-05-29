Imports MySql.Data.MySqlClient
Imports System.IO
Public Class frmAddImageState
    Dim dvObjects As DataView
    Private Sub frmAddImageState_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Add_Object1()
    End Sub
    Private Sub Load_Add_Object1()
        Dim CMD As New MySqlCommand
        Dim myAdapter As New MySqlDataAdapter
        Dim myDataset As New DataSet

        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        If iStateImageCount > 0 Then
            CMD.CommandText = "SELECT object_name, object_id FROM osae_v_object WHERE base_type<>'CONTROL' AND object_name NOT IN(" & iStateImageList & ") ORDER BY object_name"
        Else
            CMD.CommandText = "SELECT object_name, object_id FROM osae_v_object WHERE base_type<>'CONTROL' ORDER BY object_name"
        End If
        Try
            cboAddObject.Items.Clear()
            myAdapter.SelectCommand = CMD
            CN.Open()
            myAdapter.Fill(myDataset, "Objects")
            dvObjects = myDataset.Tables("Objects").DefaultView
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
        cboAddObject.DataSource = dvObjects
        cboAddObject.DisplayMember = "object_name"
        cboAddObject.ValueMember = "object_id"
    End Sub
    Private Sub Load_Add_Object()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        If iStateImageCount > 0 Then
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type<>'CONTROL' AND object_name NOT IN(" & iStateImageList & ") ORDER BY object_name"
        Else
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type<>'CONTROL' ORDER BY object_name"
        End If
        Try
            cboAddObject.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboAddObject.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Private Sub cboAddObject_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboAddObject.SelectedIndexChanged
        Dim CMD As New MySqlCommand, sStateName As String = ""
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        If cboAddObject.SelectedIndex > -1 Then
            btnAdd.Visible = True
            lblObject.ForeColor = Color.Black
            CMD.CommandText = "SELECT state_label FROM osae_v_object_state WHERE object_name=?ObjectName AND state_name='ON'"
            CMD.Parameters.AddWithValue("?ObjectName", cboAddObject.Text)
            Try
                CN.Open()
                sStateName = CMD.ExecuteScalar
                CN.Close()
                If sStateName <> "" Then
                    lblON.Text = sStateName
                    lblON.Visible = True
                    lblImage1.Visible = True
                    txtON.Visible = True
                    btnOnImage.Visible = True
                    picON.Visible = True
                End If
            Catch myerror As MySqlException
                MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
                CN.Close()
            End Try
            CMD.Parameters.Clear()
            CMD.CommandText = "SELECT state_label FROM osae_v_object_state WHERE object_name=?ObjectName AND state_name='OFF'"
            CMD.Parameters.AddWithValue("?ObjectName", cboAddObject.Text)
            Try
                CN.Open()
                sStateName = CMD.ExecuteScalar
                CN.Close()
                If sStateName <> "" Then
                    lblOFF.Text = sStateName
                    lblOFF.Visible = True
                    lblImage2.Visible = True
                    txtOFF.Visible = True
                    btnOffImage.Visible = True
                    picOFF.Visible = True
                End If
            Catch myerror As MySqlException
                MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
                CN.Close()
            End Try
            CMD.Parameters.Clear()
            CMD.CommandText = "SELECT state_label FROM osae_v_object_state WHERE object_name=?ObjectName AND state_name='ALARM'"
            CMD.Parameters.AddWithValue("?ObjectName", cboAddObject.Text)
            Try
                CN.Open()
                sStateName = CMD.ExecuteScalar
                CN.Close()
                If sStateName <> "" Then
                    lblALARM.Text = sStateName
                    lblALARM.Visible = True
                    lblImage3.Visible = True
                    txtALARM.Visible = True
                    btnALARMImage.Visible = True
                    picAlarm.Visible = True
                End If
            Catch myerror As MySqlException
                MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
                CN.Close()
            End Try
            CMD.Parameters.Clear()
        Else
            lblObject.ForeColor = Color.Red
            btnAdd.Visible = False
        End If
    End Sub
    Private Sub btnOffImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOffImage.Click
        Dim res As DialogResult = file1.ShowDialog()
        If res = DialogResult.OK Then
            txtOFF.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
            If File.Exists(file1.FileName) Then
                Dim msImage As MemoryStream = New MemoryStream(File.ReadAllBytes(file1.FileName))
                picOFF.Image = Image.FromStream(msImage)
            End If
        End If
    End Sub
    Private Sub btnOnImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOnImage.Click
        Dim res As DialogResult = file1.ShowDialog()
        If res = DialogResult.OK Then
            txtON.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
            If File.Exists(file1.FileName) Then
                Dim msImage As MemoryStream = New MemoryStream(File.ReadAllBytes(file1.FileName))
                picON.Image = Image.FromStream(msImage)
            End If
        End If
    End Sub
    Private Sub btnALARMImage_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnALARMImage.Click
        Dim res As DialogResult = file1.ShowDialog()
        If res = DialogResult.OK Then
            txtALARM.Text = file1.FileName.Replace(OSAEApi.APIpath, "")
            If File.Exists(file1.FileName) Then
                Dim msImage As MemoryStream = New MemoryStream(File.ReadAllBytes(file1.FileName))
                picAlarm.Image = Image.FromStream(msImage)
            End If
        End If
    End Sub
    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim sName As String = gCurrentScreen & " - " & cboAddObject.Text
        OSAEApi.ObjectAdd(sName, sName, "CONTROL STATE IMAGE", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sName, "Object Name", cboAddObject.Text)
        OSAEApi.ObjectPropertySet(sName, "State 1 Name", "ON")
        OSAEApi.ObjectPropertySet(sName, "State 1 Image", txtON.Text)
        OSAEApi.ObjectPropertySet(sName, "State 1 X", 100)
        OSAEApi.ObjectPropertySet(sName, "State 1 Y", 100)
        OSAEApi.ObjectPropertySet(sName, "State 2 Name", "OFF")
        OSAEApi.ObjectPropertySet(sName, "State 2 Image", txtOFF.Text)
        OSAEApi.ObjectPropertySet(sName, "State 2 X", 100)
        OSAEApi.ObjectPropertySet(sName, "State 2 Y", 100)
        OSAEApi.ObjectPropertySet(sName, "Zorder", 1)

        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_screen_object_add"
        CMD.Parameters.AddWithValue("?screen", gCurrentScreen)
        CMD.Parameters.AddWithValue("?object", cboAddObject.Text)
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
    Private Sub btnCancelAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelAdd.Click
        Me.Close()
    End Sub
End Class