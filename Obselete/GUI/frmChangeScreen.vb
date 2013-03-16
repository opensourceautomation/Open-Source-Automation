Imports MySql.Data.MySqlClient
Imports System.IO
Public Class frmChangeScreen
    Private Sub frmChangeScreen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Load_Screens()
    End Sub
    Public Sub Load_Screens()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        Dim sImage As String, iCounter As Integer = 0
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT object_name,property_value FROM osae_v_object_property WHERE base_type='SCREEN' AND property_name='Background Image' ORDER BY object_name"
        Try
            'cboScreens.Items.Clear()
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                iCounter = iCounter + 1
                sImage = (Convert.ToString(myReader.Item("property_value")))
                'sImage = gAppPath & sImage

                ''sImage = sImage.Replace(".\", OSAEApi.APIpath & "\")
                sImage = OSAEApi.APIpath + sImage

                If File.Exists(sImage) Then
                    'aControlStateImage(aScreenObject(iLoop).Control_Index).Image = Image.FromFile(sImage)
                    ImageList1.Images.Add(Image.FromFile(sImage))
                    lsv1.Items.Add(Convert.ToString(myReader.Item("object_name")))
                    lsv1.Items(iCounter - 1).ImageIndex = iCounter - 1
                    'tabScreen.ImageList.Images(iCounter) = Image.FromFile(sImage)
                    If iCounter = 1 Then
                        ' tab1.BackgroundImage = Image.FromFile(sImage)
                    ElseIf iCounter = 2 Then
                        ' tab2.BackgroundImage = Image.FromFile(sImage)
                    End If

                End If

            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Dropdowns: " & myerror.Message)
            CN.Close()
        End Try
    End Sub
    Private Sub lsv1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lsv1.SelectedIndexChanged
        GUI.Load_Screen(lsv1.FocusedItem.Text)
        Me.Close()
    End Sub
End Class