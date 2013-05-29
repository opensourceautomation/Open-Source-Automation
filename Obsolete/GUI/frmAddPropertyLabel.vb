Imports MySql.Data.MySqlClient
Public Class frmAddPropertyLabel
    Dim sBack As String
    Dim sFore As String
    Private Sub frmAddPropertyLabel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Load_Add_Object()
        GetColors()
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
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Private Sub cboObjectList1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboObjectList1.SelectedIndexChanged
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName ORDER BY property_name"
        CMD.Parameters.AddWithValue("?ObjectName", cboObjectList1.Text)
        Try
            cboProperty.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboProperty.Items.Add(Convert.ToString(myReader.Item("property_name")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error cboObjectList1: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Private Sub btnPropertyLabelAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPropertyLabelAdd.Click
        Dim sControlName As String
        sBack = cboBackColors.Text
        sFore = cboForeColors.Text

        sControlName = gCurrentScreen & " - " & cboObjectList1.Text & ":" & cboProperty.Text
        OSAEApi.ObjectAdd(sControlName, sControlName, "CONTROL PROPERTY LABEL", "", gCurrentScreen, True)
        OSAEApi.ObjectPropertySet(sControlName, "Object Name", cboObjectList1.Text)
        OSAEApi.ObjectPropertySet(sControlName, "Property Name", cboProperty.Text)
        OSAEApi.ObjectPropertySet(sControlName, "Font Name", txtFont.Text)
        OSAEApi.ObjectPropertySet(sControlName, "Font Size", txtFontSize.Text)
        OSAEApi.ObjectPropertySet(sControlName, "X", 100)
        OSAEApi.ObjectPropertySet(sControlName, "Y", 100)
        OSAEApi.ObjectPropertySet(sControlName, "Back Color", sBack)
        OSAEApi.ObjectPropertySet(sControlName, "Fore Color", sFore)
        OSAEApi.ObjectPropertySet(sControlName, "Prefix", txtPrefix.Text)
        OSAEApi.ObjectPropertySet(sControlName, "Suffix", txtSuffix.Text)
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
            MessageBox.Show("Error Load_Add_Object: " & myerror.Message)
            CN.Close()
        End Try
        GUI.Load_Screen(gCurrentScreen)
        Me.Close()
    End Sub
    Private Sub btnPropertyLabelCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPropertyLabelCancel.Click
        Me.Close()
    End Sub

    Private Sub txtFont_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtFont.MouseClick
        Dim dlgFont As System.Windows.Forms.FontDialog
        dlgFont = New System.Windows.Forms.FontDialog

        'dlgFont.Font = set your font here

        If dlgFont.ShowDialog() = DialogResult.OK Then
            txtFont.Text = dlgFont.Font.FontFamily.Name
            txtFontSize.Text = dlgFont.Font.Size
        End If
        'Debug.Print(dlgFont.Font)
    End Sub

    Private Sub txtFont_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtFont.TextChanged

    End Sub

    Private Sub txtFontSize_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtFontSize.MouseClick
        Dim dlgFont As System.Windows.Forms.FontDialog
        dlgFont = New System.Windows.Forms.FontDialog

        'dlgFont.Font = set your font here

        If dlgFont.ShowDialog() = DialogResult.OK Then
            txtFont.Text = dlgFont.Font.FontFamily.Name
            txtFontSize.Text = dlgFont.Font.Size
        End If
    End Sub

    Private Sub txtFontSize_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtFontSize.TextChanged

    End Sub

    Private Sub GetColors()
        'create a generic list of strings
        'Dim colors As New List(Of String)()
        'get the color names from the Known color enum
        Dim colorNames As String() = [Enum].GetNames(GetType(KnownColor))
        'iterate thru each string in the colorNames array
        For Each colorName As String In colorNames
            'cast the colorName into a KnownColor
            Dim knownColor As KnownColor = DirectCast([Enum].Parse(GetType(KnownColor), colorName), KnownColor)
            'check if the knownColor variable is a System color
            If knownColor > knownColor.Transparent Then
                'add it to our list
                cboForeColors.Items.Add(colorName)
                cboBackColors.Items.Add(colorName)
            End If
        Next
        'return the color list
        cboForeColors.Text = "Black"
        cboBackColors.Text = "White"
    End Sub

    Private Sub cboForeColors_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboForeColors.SelectedIndexChanged
        lblForeColor.ForeColor = System.Drawing.Color.FromName(cboForeColors.Text)
        lblBackColor.ForeColor = System.Drawing.Color.FromName(cboForeColors.Text)
    End Sub


    Private Sub cboBackColors_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBackColors.SelectedIndexChanged
        lblForeColor.BackColor = System.Drawing.Color.FromName(cboBackColors.Text)
        lblBackColor.BackColor = System.Drawing.Color.FromName(cboBackColors.Text)

    End Sub
End Class