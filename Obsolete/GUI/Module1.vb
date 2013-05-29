Imports MySql.Data.MySqlClient
Module Module1
    Public CN As MySqlConnection
    Public CN2 As MySqlConnection
    Public OSAEApi As OSAE
    Public logging As Logging
    Public aScreenObject() As ScreenObject
    Public iObjectCount As Integer, iStateImageCount As Integer, iPropertyLabelCount As Integer
    Public iStaticLabelCount As Integer, iMethodImageCount As Integer, iNavImageCount As Integer
    Public iUserControlCount As Integer
    Public iTimerLabelCount As Integer
    Public iStateImageList As String
    Public gAppName As String = ""
    Public gAppPath As String = ""
    Public gCurrentScreen As String = ""
    Structure ScreenObject
        Dim Control_Name As String
        Dim Control_Type As String
        Dim Control_Index As Integer
        Dim Object_Name As String
        Dim Property_Name As String
        Dim Property_Value As String
        Dim Object_State As String
        Dim Object_State_Time As DateTime
        Dim Object_Last_Updated As DateTime
    End Structure
End Module
