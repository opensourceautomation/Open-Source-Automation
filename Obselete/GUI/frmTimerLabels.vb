Imports MySql.Data.MySqlClient
Public Class frmTimerLabels
    Dim sBack As String
    Dim sFore As String
    Private Sub frmTimerLabels_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Add_Object()
    End Sub
    Private Sub Load_Add_Object()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type<>'CONTROL' ORDER BY object_name"
        Try
            cboObjectList1.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboObjectList1.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message, "Add Timer Label")
            CN.Close()
        End Try
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim sControlName As String
        sBack = picBackColor.BackColor.ToString
        sBack = sBack.Replace("Color [", "")
        sBack = sBack.Replace("]", "")

        sFore = picForeColor.BackColor.ToString
        sFore = sFore.Replace("Color [", "")
        sFore = sFore.Replace("]", "")

        If radState.Checked Then
            sControlName = gCurrentScreen & " - " & cboObjectList1.Text & "(State Timer)"
        Else
            sControlName = gCurrentScreen & " - " & cboObjectList1.Text & "(Off Timer)"
        End If
        OSAEApi.ObjectAdd(sControlName, sControlName, "CONTROL TIMER LABEL", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sControlName, "Object Name", cboObjectList1.Text)
        OSAEApi.ObjectPropertySet(sControlName, "Font Name", txtFont.Text)
        OSAEApi.ObjectPropertySet(sControlName, "Font Size", txtFontSize.Text)
        If radState.Checked Then
            OSAEApi.ObjectPropertySet(sControlName, "Type", "State")
        Else
            OSAEApi.ObjectPropertySet(sControlName, "Type", "Property")
        End If
        OSAEApi.ObjectPropertySet(sControlName, "X", 100)
        OSAEApi.ObjectPropertySet(sControlName, "Y", 100)
        OSAEApi.ObjectPropertySet(sControlName, "Back Color", sBack)
        OSAEApi.ObjectPropertySet(sControlName, "Fore Color", sFore)
        OSAEApi.ObjectPropertySet(sControlName, "Zorder", 1)

        Dim CMD As New MySqlCommand
        CMD.Connection = CN
        CMD.CommandType = CommandType.StoredProcedure
        CMD.CommandText = "osae_sp_screen_object_add"
        CMD.Parameters.AddWithValue("?screen", gCurrentScreen)
        CMD.Parameters.AddWithValue("?object", cboObjectList1.Text)
        CMD.Parameters.AddWithValue("?control", sControlName)
        Try
            CN.Open()
            CMD.ExecuteNonQuery()
            '    sStateName = CMD.ExecuteScalar
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error in btnAdd_Click: " & myerror.Message, "Add Timer Label")
            CN.Close()
        End Try
        GUI.Load_Screen(gCurrentScreen)
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub picBackColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picBackColor.Click
        Dim res As DialogResult = ColorDialog1.ShowDialog()
        If res = DialogResult.OK Then
            picBackColor.BackColor = ColorDialog1.Color
            sBack = ColorDialog1.Color.ToString
            sBack = sBack.Replace("Color [", "")
            sBack = sBack.Replace("]", "")
        End If
    End Sub

    Private Sub picForeColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picForeColor.Click
        Dim res As DialogResult = ColorDialog1.ShowDialog()
        If res = DialogResult.OK Then
            picForeColor.BackColor = ColorDialog1.Color
            sFore = ColorDialog1.Color.ToString
            sFore = sFore.Replace("Color [", "")
            sFore = sFore.Replace("]", "")
        End If
    End Sub

End Class