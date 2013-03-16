Imports MySql.Data.MySqlClient
Public Class frmAddStaticLabel
    Dim sBack As String
    Dim sFore As String
    Private Sub btnStaticLabelAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim sName As String = gCurrentScreen & " - " & txtStaticName.Text
        OSAEApi.ObjectAdd(sName, sName, "CONTROL STATIC LABEL", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sName, "Value", txtStaticText.Text)
        OSAEApi.ObjectPropertySet(sName, "X", 100)
        OSAEApi.ObjectPropertySet(sName, "Y", 100)
    End Sub
    Private Sub btnStaticLabelCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStaticLabelCancel.Click
        Me.Close()
    End Sub
    Private Sub btnStaticLabelAdd_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStaticLabelAdd.Click
        Dim sControlName As String
        sControlName = gCurrentScreen & " - " & txtStaticName.Text
        OSAEApi.ObjectAdd(sControlName, sControlName, "STATIC LABEL", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sControlName, "Font Name", "Times New Roman")
        OSAEApi.ObjectPropertySet(sControlName, "Font Size", 8)
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
        CMD.Parameters.AddWithValue("?control", sControlName)
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
End Class