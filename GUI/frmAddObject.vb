Imports MySql.Data.MySqlClient

Public Class frmAddObject
    Private g_toolTip As ToolTip = Nothing
    Private iProblems As Integer
    Private Sub frmObject_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        g_toolTip = New ToolTip
        Load_Object_Types()
        Validate_Add()
    End Sub

    Private Sub Load_Object_Types()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_type FROM osae_v_object_type WHERE base_type<>'CONTROL' ORDER BY object_type"
        Try
            cboNewObjectType.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboNewObjectType.Items.Add(Convert.ToString(myReader.Item("object_type")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Types: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Private Sub btnObjectAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnObjectAdd.Click
        If iProblems > 0 Then
            MsgBox(iProblems & " Fields have Issues!")
        Else
            OSAEApi.ObjectAdd(txtNewObjectName.Text, txtNewObjectDesc.Text, cboNewObjectType.Text, txtAddressAdd.Text, cboContainerAdd.Text, True)
            Me.Close()
        End If
    End Sub

    Private Sub btnObjectCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnObjectCancel.Click
        Me.Close()
    End Sub

    Private Sub txtNewObjectDesc_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtNewObjectDesc.GotFocus
        If txtNewObjectDesc.TextLength = 0 Then txtNewObjectDesc.Text = txtNewObjectName.Text
    End Sub

    Private Sub Validate_Add()
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        iProblems = 0
        If txtNewObjectName.TextLength = 0 Then
            g_toolTip.SetToolTip(txtNewObjectName, "Name Is A Required Field")
            g_toolTip.SetToolTip(lblName, "Name Is A Required Field")
            lblName.ForeColor = Color.Red
            iProblems += 1
        Else
            CMD.Connection = CN
            CMD.CommandType = CommandType.Text
            CMD.CommandText = "SELECT count(object_name) as Results FROM osae_v_object WHERE object_name=?pname"
            CMD.Parameters.AddWithValue("?pname", txtNewObjectName.Text)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
                CMD.Parameters.Clear()
            Catch myerror As MySqlException
                MessageBox.Show("Error Validate_Object: " & myerror.Message)
                CN.Close()
            End Try
            If iCount > 0 Then
                g_toolTip.SetToolTip(txtNewObjectName, "Name Is Already Used")
                g_toolTip.SetToolTip(lblName, "Name Is Already Used")
                lblName.ForeColor = Color.Red
                iProblems += 1
            Else
                g_toolTip.SetToolTip(txtNewObjectName, "Name looks OK")
                g_toolTip.SetToolTip(lblName, "Name looks OK")
                lblName.ForeColor = Color.Black
            End If
        End If
        If cboNewObjectType.SelectedIndex = -1 Then
            g_toolTip.SetToolTip(cboNewObjectType, "Object Type Is A Required Field")
            g_toolTip.SetToolTip(lblType, "Object Type Is A Required Field")
            lblType.ForeColor = Color.Red
            iProblems += 1
        Else
            g_toolTip.SetToolTip(cboNewObjectType, "Object Type looks OK")
            g_toolTip.SetToolTip(lblType, "Object Type looks OK")
            lblType.ForeColor = Color.Black
        End If
        If iProblems = 0 Then
            btnObjectAdd.ForeColor = Color.Black
            g_toolTip.SetToolTip(btnObjectAdd, "Required Fields all look OK.")
        Else
            btnObjectAdd.ForeColor = Color.LightGray
            g_toolTip.SetToolTip(btnObjectAdd, iProblems & " Fields have problems.")
        End If
    End Sub

    Private Sub txtNewObjectName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtNewObjectName.TextChanged
        Validate_Add()
    End Sub

    Private Sub cboNewObjectType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboNewObjectType.SelectedIndexChanged
        Validate_Add()
    End Sub
End Class