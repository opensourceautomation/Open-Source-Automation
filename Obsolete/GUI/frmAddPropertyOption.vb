Imports MySql.Data.MySqlClient
Public Class frmAddPropertyOption
    Public iObjectType As String
    Public iProperty As String

    Private Sub frmAddPropertyOption_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadGrid()
    End Sub

    Private Sub LoadGrid()
        Dim ds As DataSet = OSAEApi.GetObjectTypePropertyOptions(iObjectType, iProperty)
        Dim MyDT As New DataTable
        MyDT = ds.Tables(0)
        dgvPropertyOptions.DataSource = MyDT
    End Sub

    Private Sub dgvPropertyOptions_CurrentCellChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dgvPropertyOptions.CurrentCellChanged
        Try
            txbxPropOption.Text = "" & dgvPropertyOptions("Column1", dgvPropertyOptions.CurrentCell.RowIndex).Value
        Catch ex As Exception

        End Try

    End Sub

    Private Sub btnAddPropOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddPropOption.Click
        OSAEApi.ObjectTypePropertyOptionAdd(iObjectType, iProperty, txbxPropOption.Text)
        LoadGrid()
    End Sub

    Private Sub btnUpdatePropOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdatePropOption.Click
        OSAEApi.ObjectTypePropertyOptionUpdate(iObjectType, iProperty, txbxPropOption.Text, dgvPropertyOptions("Column1", dgvPropertyOptions.CurrentCell.RowIndex).Value)
        LoadGrid()
    End Sub

    Private Sub btnDeletePropOption_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeletePropOption.Click
        OSAEApi.ObjectTypePropertyOptionDelete(iObjectType, iProperty, dgvPropertyOptions("Column1", dgvPropertyOptions.CurrentCell.RowIndex).Value)
        LoadGrid()
    End Sub
End Class