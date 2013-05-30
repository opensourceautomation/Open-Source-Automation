Imports System.Data
Imports MySql.Data.MySqlClient
Imports System.Text.RegularExpressions



Public Class frmScriptEditor
    Private CN As MySqlConnection
    Private iLine As Integer
    Private iLineStart As Integer
    Private iLineEnd As Integer
    Private iLineCursor As Integer
    Private sCurrentPart As String = ""
    Private sScriptParameter As String = ""
    Private Sub ScriptEditor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For i = 1 To 100
            lstLine.Items.Add(i)
        Next
        DB_Connection()
        Load_Objects()
        Load_Patterns()
    End Sub

    Public Sub DB_Connection()
        CN = New MySqlConnection
        CN.ConnectionString = "server=" & OSAEApi.DBConnection & ";Port=" & OSAEApi.DBPort & ";Database=" & OSAEApi.DBName & ";Password=" & OSAEApi.DBPassword & ";use procedure bodies=false;Persist Security Info=True;User ID=" & OSAEApi.DBUsername
        Try
            CN.Open()
            CN.Close()
        Catch myerror As MySqlException
            logging.AddToLog("Error Connecting to Database: " & myerror.Message, True)
        End Try
    End Sub
    Private Sub btnRunScript_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunScript.Click


        Dim sType As String = ""
        Dim iLoop As Integer
        Dim sObject As String = "", sOption As String = "", sMethod As String = "", sParam1 As String = "", sParam2 As String = ""
        Dim iObjectPos As Integer, iOptionPos As Integer, iMethodPos As Integer
        Dim iParam1Pos As Integer ', iParam2Pos As Integer
        Dim iQuotePos As Integer, iCommaPos As Integer
        Dim sOperator As String, iOperatorPos As Integer, lSeconds As ULong
        Dim sValue As String, sState As String
        Dim sConditionResults As String = ""
        Dim dtStartTime As Date, dtEndTime As Date
        Dim sWorking As String, sProperty As String = "", pProperty As New ObjectProperty
        Dim sNesting(20) As String, iNestingLevel As Integer = 0
        Dim sScript As String
        Dim sSubScript As String = "", sSubScriptName As String = ""
        Dim sTempLine As String = ""
        lstResults.Items.Clear()
        sNesting(0) = "PASS"
        sScript = txtScript.Text
        Dim iEmbeddedScriptStart As Integer = 0
        Dim iEmbeddedScriptEnd As Integer = 0
        Try
            iEmbeddedScriptStart = sScript.IndexOf("Script:", iEmbeddedScriptStart)
            Do While iEmbeddedScriptStart > 0
                iEmbeddedScriptEnd = sScript.IndexOf(vbCrLf, iEmbeddedScriptStart)
                sTempLine = sScript.Substring(iEmbeddedScriptStart, (iEmbeddedScriptEnd - iEmbeddedScriptStart))
                sSubScriptName = sScript.Substring(iEmbeddedScriptStart, (iEmbeddedScriptEnd - iEmbeddedScriptStart)).Replace("Script:", "")
                Dim dsResults As DataSet = OSAEApi.RunSQL("SELECT Script FROM OSAE_Pattern WHERE Pattern='" & sSubScriptName & "'")
                sSubScript = Convert.ToString(dsResults.Tables(0).Rows(0)("Script"))
                sScript = sScript.Replace(sTempLine, sSubScript)
                iEmbeddedScriptStart = sScript.IndexOf("Script:", iEmbeddedScriptEnd)
            Loop
        Catch ex As Exception
            Display_Results("Error in Script: :" & ex.Message)
        End Try

        'This regex removes c# style like: //comments
        Static removeComments As New Regex("([\r\n ]*//[^\r\n]*)+")
        sScript = removeComments.Replace(sScript, "")

        Dim scriptArray() = sScript.Split(vbCrLf)
        txtEcho.Text = ""

        For iLoop = 0 To scriptArray.Length - 1
            Try
                scriptArray(iLoop) = scriptArray(iLoop).Trim
                txtEcho.Text += scriptArray(iLoop) & Environment.NewLine
                If scriptArray(iLoop).Length > 2 Then
                    scriptArray(iLoop) = scriptArray(iLoop).Replace("SCRIPT.PARAMETER", (sScriptParameter))
                    If scriptArray(iLoop).ToUpper.Trim.StartsWith("IF") Then
                        sType = "IF"
                        iNestingLevel += 1
                        sNesting(iNestingLevel) = sNesting(iNestingLevel - 1)
                    ElseIf scriptArray(iLoop).ToUpper.Trim.StartsWith("ENDIF") Then
                        sType = "ENDIF"
                        iNestingLevel -= 1
                        'sNesting(iNestingLevel)
                    ElseIf scriptArray(iLoop).ToUpper.Trim.StartsWith("ELSEIF") Then
                        sType = "ELSEIF"
                    ElseIf scriptArray(iLoop).ToUpper.Trim.StartsWith("ELSE") Then
                        sType = "ELSE"
                    Else
                        sType = "SET"
                    End If
                End If
                '------------------------------------------------------------------------------------------------
                ' This Section Executes Commands ------
                If sType = "SET" And sNesting(iNestingLevel) <> "FAIL" Then
                    'If outValue.EndsWith(".") Then
                    iObjectPos = scriptArray(iLoop).IndexOf(".")
                    If iObjectPos > 0 Then
                        sObject = scriptArray(iLoop).Substring(0, iObjectPos)
                        sWorking = scriptArray(iLoop).Substring(iObjectPos + 1, scriptArray(iLoop).Length - (iObjectPos + 1))
                        If sObject.ToUpper = "SQL" Then
                            OSAEApi.RunSQL(sWorking)
                        Else
                            iOptionPos = sWorking.IndexOf(".")
                            If iOptionPos > 0 Then
                                sOption = sWorking.Substring(0, iOptionPos).Trim
                                sWorking = sWorking.Substring(iOptionPos + 1, sWorking.Length - (iOptionPos + 1)).Trim
                                If sOption.ToUpper = "SET PROPERTY" Then
                                    sMethod = sWorking.Substring(0, iMethodPos)
                                Else

                                    iMethodPos = sWorking.IndexOf(".")
                                    If iMethodPos = -1 Then
                                        sMethod = sWorking
                                        sWorking = ""
                                    Else
                                        sMethod = sWorking.Substring(0, iMethodPos)
                                        sWorking = sWorking.Substring(iMethodPos + 1, sWorking.Length - (iMethodPos + 1))
                                    End If
                                End If

                                ' Find First parameter based on a piar of "" or a Comma
                                sParam1 = ""
                                iParam1Pos = sWorking.IndexOf(Chr(34))
                                iQuotePos = sWorking.IndexOf(Chr(34))
                                iCommaPos = sWorking.IndexOf(",")
                                ' Check to see if first parameter is using quote by comparing to position of ,
                                If (iQuotePos < iCommaPos And iQuotePos >= 0) Or (iQuotePos >= 0 And iCommaPos = -1) Then
                                    'First Parameter is quoted.  Delete first quote and find closing quote
                                    sWorking = sWorking.Substring(iQuotePos + 1, sWorking.Length - (iQuotePos + 1))
                                    ' Now we reuse the Quote variable and look for the closing quote
                                    iQuotePos = sWorking.IndexOf(Chr(34))
                                    sParam1 = sWorking.Substring(0, iQuotePos)
                                    sWorking = sWorking.Replace(sParam1, "")
                                    iCommaPos = sWorking.IndexOf(",")
                                    If iCommaPos > -1 Then
                                        'Set the 2nd parameter
                                        sWorking = sWorking.Substring(iCommaPos + 1, sWorking.Length - (iCommaPos + 1))
                                        sParam2 = sWorking.Replace(Chr(34), "")
                                    End If
                                ElseIf iQuotePos >= 0 And iCommaPos = -1 Then
                                    'Only 1 paramter, and it is quoted
                                    sWorking = sWorking.Substring(iQuotePos + 1, sWorking.Length - (iQuotePos + 1))
                                    ' Now we reuse the Quote variable and look for the closing quote
                                    iQuotePos = sWorking.IndexOf(Chr(34))
                                    sParam1 = sWorking.Substring(0, iQuotePos)
                                    sWorking = sWorking.Replace(sParam1, "")
                                ElseIf iQuotePos = -1 And iCommaPos = -1 Then
                                    'Only 1 paramter, and it is NOT quoted
                                    'sParam1 = ""
                                    'This was changed in 0.3.1 due to a ticket saying command was showing up as first parameter
                                    sParam1 = sWorking
                                ElseIf iQuotePos = -1 And iCommaPos > -1 Then
                                    'Only 1 paramter, and it is NOT quoted
                                    sParam1 = sWorking.Substring(0, iCommaPos)
                                    sParam2 = sWorking.Substring(iCommaPos + 1, sWorking.Length - (iCommaPos + 1))
                                ElseIf iQuotePos > -1 And iQuotePos > iCommaPos Then
                                    'Only 1 paramter, and it is NOT quoted
                                    sParam1 = sWorking.Substring(0, iCommaPos)
                                    sParam2 = sWorking.Substring(iCommaPos + 1, sWorking.Length - (iCommaPos + 1))
                                    sParam2 = sParam2.Replace(Chr(34), "")
                                End If
                                If sOption.ToUpper = "RUN METHOD" Then
                                    OSAEApi.MethodQueueAdd(sObject, sMethod, sParam1, sParam2)
                                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Ran Method: " & sObject & "." & sMethod & " (" & sParam1 & "," & sParam2 & ")")
                                ElseIf sOption.ToUpper = "SET STATE" Then
                                    OSAEApi.ObjectStateSet(sObject, sParam1)
                                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Set State: " & sObject & "." & sParam1)
                                ElseIf sOption.ToUpper = "SET PROPERTY" Then
                                    iOptionPos = sWorking.IndexOf("+=")
                                    If iOptionPos <= 0 Then iOptionPos = sWorking.IndexOf("=")
                                    If iOptionPos > 0 Then
                                        sParam1 = sWorking.Substring(0, iOptionPos)
                                        sWorking = sWorking.Substring(iOptionPos, sWorking.Length - (iOptionPos))
                                        iOptionPos = sWorking.IndexOf(" ")
                                        If iOptionPos > 0 Then
                                            sMethod = sWorking.Substring(0, iOptionPos)
                                            sWorking = sWorking.Substring(iOptionPos + 1, sWorking.Length - (iOptionPos + 1))
                                            sValue = sWorking
                                            If sMethod = "=" Then
                                                OSAEApi.ObjectPropertySet(sObject, sParam1, sValue)
                                                Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Set Property: " & sParam1 & " = " & sValue)
                                            ElseIf sMethod = "+=" Then
                                                sWorking = OSAEApi.GetObjectPropertyValue(sObject, sParam1).Value
                                                OSAEApi.ObjectPropertySet(sObject, sParam1, Val(sWorking) + Val(sValue))
                                                Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Set Property: " & sParam1 & " = " & sValue)
                                            End If
                                        End If

                                    End If
                                    'OSAEApi.ObjectPropertySet(sObject, sParam1, sParam2)
                                    'lstResults.Items.Add("Set Property: " & sObject & "." & sParam1 & " = " & sParam2)
                                End If

                            Else
                                'Set properties for an object
                                iOptionPos = sWorking.IndexOf("+=")
                                If iOptionPos = 0 Then iOptionPos = sWorking.IndexOf("=")
                                If iOptionPos > 0 Then
                                    sParam1 = sWorking.Substring(0, iOptionPos)
                                    sWorking = sWorking.Substring(iOptionPos, sWorking.Length - (iOptionPos))
                                    iOptionPos = sWorking.IndexOf(" ")
                                    If iOptionPos > 0 Then
                                        sMethod = sWorking.Substring(0, iOptionPos)
                                        sWorking = sWorking.Substring(iOptionPos + 1, sWorking.Length - (iOptionPos + 1))
                                        sValue = sWorking
                                        If sMethod = "=" Then
                                            OSAEApi.ObjectPropertySet(sObject, sParam1, sValue)
                                            Display_Results(iLoop & ": Set Property: " & sObject & "." & sParam1)
                                        ElseIf sMethod = "+=" Then
                                            sWorking = OSAEApi.GetObjectPropertyValue(sObject, sParam1).Value
                                            OSAEApi.ObjectPropertySet(sObject, sParam1, Val(sWorking) + Val(sValue))
                                        End If
                                    End If

                                End If
                            End If
                        End If
                    End If

                ElseIf sType = "SET" And sNesting(iNestingLevel) = "FAIL" Then
                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Command Skipped because Logic = Failed")
                ElseIf sType = "ENDIF" Then
                    Display_Results(iLoop & ": " & iNestingLevel & " - EndIf, Nesting Level Reduced")
                ElseIf sType = "ELSE" And sNesting(iNestingLevel) = "FAIL" Then
                    sNesting(iNestingLevel) = "PASS"
                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - ELSE, Relevant and Running")

                ElseIf sType = "ELSE" And sNesting(iNestingLevel) = "PASS" Then
                    sNesting(iNestingLevel) = "FAIL"
                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - ELSE, Not Relevant and Ignoring")
                ElseIf (sType = "IF" And sNesting(iNestingLevel) = "PASS") Or (sType = "ELSEIF" And sNesting(iNestingLevel) = "FAIL" And sNesting(iNestingLevel - 1) = "PASS") Then
                    ' This section will only process IF statements, so we can trim off the IF & THEN
                    sNesting(iNestingLevel) = "PASS"
                    sWorking = scriptArray(iLoop).Replace("ELSEIF ", "")
                    sWorking = sWorking.Replace("IF ", "")
                    sWorking = sWorking.Replace(" THEN", "")
                    iObjectPos = sWorking.IndexOf(".")
                    If iObjectPos > 0 Then
                        sObject = sWorking.Substring(0, iObjectPos)
                        sWorking = sWorking.Replace(sObject & ".", "")
                        iOptionPos = sWorking.IndexOf("=")
                        If iOptionPos <= 0 Then iOptionPos = sWorking.IndexOf("<")
                        If iOptionPos <= 0 Then iOptionPos = sWorking.IndexOf(">")
                        If iOptionPos <= 0 Then iOptionPos = sWorking.IndexOf("!")
                        sOption = sWorking.Substring(0, iOptionPos).Trim
                        sWorking = sWorking.Replace(sOption, "").Trim
                        iOperatorPos = sWorking.IndexOf(" ")

                        sOperator = sWorking.Substring(0, iOperatorPos).Trim
                        sValue = sWorking.Replace(sOperator, "").Trim
                        If sOperator = "=" Then
                            Try
                                If sOption.ToUpper = "STATE" Then
                                    sState = OSAEApi.GetObjectStateValue(sObject).Value


                                    If sState.ToUpper <> sValue.ToUpper Then sNesting(iNestingLevel) = "FAIL"
                                Else
                                    pProperty = OSAEApi.GetObjectPropertyValue(sObject, sOption)
                                    If pProperty.DataType = "String" Then pProperty.Value = pProperty.Value.ToUpper
                                    If Not IsNumeric(sValue) Then sValue = sValue.ToUpper
                                    If pProperty.Value <> sValue Then
                                        sNesting(iNestingLevel) = "FAIL"
                                    End If
                                End If
                            Catch ex As Exception
                                Display_Results("Error in IF = :" & ex.Message)
                            End Try


                        ElseIf sOperator = "<>" Or sOperator = "!=" Then
                            Try
                                If sOption.ToUpper = "STATE" Then
                                    sState = OSAEApi.GetObjectStateValue(sObject).Value
                                    If sState.ToUpper = sValue.ToUpper Then
                                        sNesting(iNestingLevel) = "FAIL"
                                    End If
                                Else
                                    pProperty = OSAEApi.GetObjectPropertyValue(sObject, sOption)
                                    If pProperty.DataType = "String" Then pProperty.Value = pProperty.Value.ToUpper
                                    If Not IsNumeric(sValue) Then sValue = sValue.ToUpper
                                    If pProperty.Value = sValue Then
                                        sNesting(iNestingLevel) = "FAIL"
                                    End If
                                End If
                            Catch ex As Exception
                                Display_Results("Error in IF <>,!= :" & ex.Message)
                            End Try
                        ElseIf sOperator = ">" Then
                            Try
                                If sOption.ToUpper = "TIMEINSTATE" Then
                                    lSeconds = ReturnSeconds(sValue)
                                    If lSeconds > OSAEApi.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    pProperty = OSAEApi.GetObjectPropertyValue(sObject, sOption)
                                    If pProperty.DataType = "String" Then pProperty.Value = pProperty.Value.ToUpper
                                    If Not IsNumeric(sValue) Then sValue = sValue.ToUpper
                                    If pProperty.DataType = "DateTime" Then
                                        dtStartTime = pProperty.Value
                                        dtEndTime = sValue
                                        If dtStartTime <= dtEndTime Then sNesting(iNestingLevel) = "FAIL"
                                    Else
                                        If pProperty.Value <= sValue Then sNesting(iNestingLevel) = "FAIL"
                                    End If
                                End If
                            Catch ex As Exception
                                Display_Results("Error in IF > :" & ex.Message)
                            End Try
                        ElseIf sOperator = "<" Then
                            Try
                                If sOption.ToUpper = "TIMEINSTATE" Then
                                    lSeconds = ReturnSeconds(sValue)
                                    If lSeconds < OSAEApi.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    pProperty = OSAEApi.GetObjectPropertyValue(sObject, sOption)
                                    If IsNothing(pProperty) = False Then
                                        If pProperty.DataType = "String" Then pProperty.Value = pProperty.Value.ToUpper
                                        If Not IsNumeric(sValue) Then sValue = sValue.ToUpper
                                        If pProperty.DataType = "DateTime" Then
                                            dtStartTime = pProperty.Value
                                            dtEndTime = sValue
                                            If dtStartTime >= dtEndTime Then sNesting(iNestingLevel) = "FAIL"
                                        Else
                                            If pProperty.Value >= sValue Then sNesting(iNestingLevel) = "FAIL"
                                        End If
                                    Else
                                        sNesting(iNestingLevel) = "FAIL"
                                    End If
                                End If
                            Catch ex As Exception
                                Display_Results("Error in IF < :" & ex.Message)
                            End Try
                        ElseIf sOperator = "=>" Or sOperator = ">=" Then
                            Try
                                If sOption.ToUpper = "TIMEINSTATE" Then
                                    'pState = OSAEApi.GetObjectStateValue(sObject)
                                    lSeconds = ReturnSeconds(sValue)
                                    If lSeconds >= OSAEApi.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    sProperty = OSAEApi.GetObjectPropertyValue(sObject, sOption).Value
                                    If sProperty.ToUpper < sValue.ToUpper Then sNesting(iNestingLevel) = "FAIL"
                                End If
                            Catch ex As Exception
                                Display_Results("Error in IF =>,>= :" & ex.Message)
                            End Try
                        ElseIf sOperator = "=<" Or sOperator = "<=" Then
                            Try
                                If sOption.ToUpper = "TIMEINSTATE" Then
                                    'pState = OSAEApi.GetObjectStateValue(sObject)
                                    lSeconds = ReturnSeconds(sValue)
                                    If lSeconds <= OSAEApi.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    sProperty = OSAEApi.GetObjectPropertyValue(sObject, sProperty).Value
                                    If sProperty.ToUpper > sValue.ToUpper Then sNesting(iNestingLevel) = "FAIL"
                                End If
                            Catch ex As Exception
                                Display_Results("Error in IF =<,<= :" & ex.Message)
                            End Try
                        End If
                        Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Logic = " & sNesting(iNestingLevel))
                    End If
                ElseIf sType = "ELSEIF" And sNesting(iNestingLevel) = "PASS" Then
                    sNesting(iNestingLevel) = "FAIL"
                End If
            Catch ex As Exception
                Display_Results("Error RunScript - " & ex.Message)
            End Try
        Next iLoop
    End Sub

    Public Sub Display_Results(ByVal sText As String)
        lstResults.Items.Add(sText)
    End Sub

    Private Function ReturnSeconds(ByVal strTime As String) As Integer
        Dim strHours As String
        Dim strMinutes As String
        Dim strSeconds As String
        Dim arrTime() As String
        arrTime = strTime.Split(":")
        strHours = arrTime(0)
        strMinutes = arrTime(1)
        strSeconds = arrTime(2)
        ReturnSeconds = strHours * 60 * 60
        ReturnSeconds += strMinutes * 60
        ReturnSeconds += strSeconds
    End Function


    Private Sub moveMouse(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        'txtY.Text = iCurrentLine
        'ShowSuggestions()
    End Sub

    Public Sub Load_Objects()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT DISTINCT object_name FROM osae_v_object_event WHERE event_name<>'' ORDER BY object_name"
        Try
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comObject.Items.Add(Convert.ToString(myReader.Item("object_name")))
            End While
            CN.Close()
            'comObject.Text = frmObjects.dgvObjects("object_name", frmObjects.dgvObjects.CurrentCell.RowIndex).Value
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Containers: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Public Sub Load_Object_Events()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT event_label FROM osae_v_object_event WHERE object_name=?pname ORDER BY event_label"
        CMD.Parameters.AddWithValue("?pname", comObject.Text)
        Try
            comObjectEvents.Visible = False
            comObjectEvents.Items.Clear()
            comObjectEvents.Text = ""
            btnAdd.Visible = False
            btnUpdate.Visible = False
            btnDelete.Visible = False
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                comObjectEvents.Items.Add(Convert.ToString(myReader.Item("event_label")))
                comObjectEvents.Visible = True
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Events: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Public Sub Load_Patterns()
        Dim CMD As New MySqlCommand
        Dim myReader As MySqlDataReader
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT pattern FROM osae_pattern ORDER BY pattern"
        Try
            btnAdd.Visible = False
            btnUpdate.Visible = False
            btnDelete.Visible = False
            CN.Open()
            myReader = CMD.ExecuteReader
            While myReader.Read
                cboPatterns.Items.Add(Convert.ToString(myReader.Item("pattern")))
            End While
            CN.Close()
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Patterns: " & myerror.Message)
            CN.Close()
        End Try
    End Sub

    Private Sub comObject_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comObject.SelectedIndexChanged
        Load_Object_Events()
    End Sub

    Public Sub ValidateSave()
        Dim CMD As New MySqlCommand
        Dim iCount As Integer
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        If radEvent.Checked = True Then
            CMD.CommandText = "SELECT COUNT(event_label) as results FROM osae_v_object_event_script WHERE object_name=?pname AND event_label=?pevent"
            CMD.Parameters.AddWithValue("?pname", comObject.Text)
            CMD.Parameters.AddWithValue("?pevent", comObjectEvents.Text)
            Try
                CN.Open()
                iCount = CMD.ExecuteScalar
                CN.Close()
            Catch myerror As MySqlException
                MessageBox.Show("Error Load_Object_Events: " & myerror.Message)
                CN.Close()
            End Try
            If iCount > 0 Then
                btnAdd.Visible = False
                btnUpdate.Visible = True
                btnDelete.Visible = True
            Else
                btnUpdate.Visible = False
                btnAdd.Visible = True
                btnDelete.Visible = False
            End If
        Else
            btnUpdate.Visible = True
            btnAdd.Visible = False
            btnDelete.Visible = False
        End If
    End Sub

    Private Sub comObjectEvents_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles comObjectEvents.SelectedIndexChanged
        Dim CMD As New MySqlCommand
        Dim sScript As String = ""
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT event_script FROM osae_v_object_event_script WHERE object_name=?pname AND event_label=?pevent"
        CMD.Parameters.AddWithValue("?pname", comObject.Text)
        CMD.Parameters.AddWithValue("?pevent", comObjectEvents.Text)
        Try
            CN.Open()
            sScript = CMD.ExecuteScalar
            txtScript.Text = sScript
            CN.Close()
            'For iLoop = 1 To 10
            'textArray(iLoop).Text = ""
            ' Next iLoop
            'If sScript <> "" Then
            '    Dim sArray() As String = sScript.Split(vbCrLf)
            '    For iLoop = 0 To sArray.Length - 2
            '        textArray(iLoop + 1).Text = sArray(iLoop)
            '    Next iLoop
            'End If
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Events: " & myerror.Message)
            CN.Close()
        End Try
        ValidateSave()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        'Dim sScript As String = "", iLoop As Integer
        'For iLoop = 1 To 10
        '    sScript = sScript & textArray(iLoop).Text & vbCrLf
        'Next
        OSAEApi.ObjectEventScriptAdd(comObject.Text, comObjectEvents.Text, txtScript.Text)
        ValidateSave()
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        If radEvent.Checked = True Then
            OSAEApi.ObjectEventScriptUpdate(comObject.Text, comObjectEvents.Text, txtScript.Text)
        Else
            Dim CMD As New MySqlCommand
            CMD.Connection = CN
            CMD.CommandType = CommandType.StoredProcedure
            CMD.CommandText = "osae_sp_pattern_update"
            CMD.Parameters.AddWithValue("?poldpattern", cboPatterns.Text)
            CMD.Parameters.AddWithValue("?pnewpattern", cboPatterns.Text)
            CMD.Parameters.AddWithValue("?script", txtScript.Text)
            Try
                CN.Open()
                CMD.ExecuteNonQuery()
                CN.Close()
            Catch myerror As MySqlException
                MessageBox.Show("Error Load_Object_Events: " & myerror.Message)
                CN.Close()
            End Try
        End If

        ValidateSave()
    End Sub

    'Private Sub ShowSuggestions()
    '    If txtScript.SelectionStart = 0 Then
    '        Debug.Print(" -- Skipped ShowSuggestions as selected index is 0")
    '        Exit Sub
    '    End If
    '    Debug.Print(" -- ShowSuggestions")
    '    Dim sdebug As String
    '    Dim sOption As String = "", sObject As String = "", sMethod As String = "", sParam1 As String = "", sParam2 As String = ""
    '    Dim sWorkLine As String = "", iOperatorIndex As Integer, sWorking As String = ""
    '    sCurrentPart = ""
    '    If txtCurrentLine.Text.Length > 2 Then
    '        If txtCurrentLine.Text.Substring(0, 3).ToUpper <> "IF " Then
    '            txtType.Text = "SET"
    '        Else
    '            txtType.Text = "GET"
    '        End If
    '    End If
    '    If txtCurrentLine.Text.Trim.Length > 0 Then
    '        Dim sWords() As String = txtCurrentLine.Text.Split(".")
    '        If sWords.Length >= 0 Then
    '            sWords(0) = sWords(0).Trim
    '            sWords(0) = sWords(0).Replace("IF ", "")
    '            txtObject.Text = ""
    '            txtObject.Text = sWords(0)
    '            txtObjectPos1.Text = txtStart.Text + txtCurrentLine.Text.IndexOf(sWords(0).Trim)
    '            txtObjectPos2.Text = txtObjectPos1.Text + sWords(0).Length
    '            sdebug = txtScript.SelectionStart
    '            If txtScript.SelectionStart <= txtObjectPos2.Text Then
    '                txtWord.Text = "Object Name"
    '                sCurrentPart = "Object Name"
    '                SuggestionObject()
    '            End If

    '            If sWords.Length > 1 Then
    '                If txtType.Text = "SET" Then
    '                    sWords(1) = sWords(1).Trim
    '                    txtSet.Text = sWords(1)
    '                    txtSetPos.Text = txtObjectPos2.Text + 1 + sWords(1).Length
    '                Else
    '                    iOperatorIndex = sWords(1).IndexOf("=")
    '                    If iOperatorIndex = 0 Then iOperatorIndex = sWords(1).IndexOf("<")
    '                    If iOperatorIndex = 0 Then iOperatorIndex = sWords(1).IndexOf(">")
    '                    If iOperatorIndex > 0 Then
    '                        sWorking = sWords(1).Substring(iOperatorIndex)
    '                        sWords(1) = sWords(1).Substring(0, iOperatorIndex).Trim
    '                    Else
    '                        sWords(1) = sWords(1).Trim
    '                    End If

    '                    txtSet.Text = sWords(1)
    '                    txtSetPos.Text = txtObjectPos2.Text + 1 + sWords(1).Length
    '                End If
    '                If txtScript.SelectionStart > txtObjectPos2.Text And txtScript.SelectionStart <= txtSetPos.Text Then
    '                    txtWord.Text = "Option Name"
    '                    sCurrentPart = "Option Name"
    '                    SuggestionSet()
    '                End If

    '                If sWords.Length > 2 Then
    '                    sWords(2) = sWords(2).Trim
    '                    txtOption.Text = sWords(2)
    '                    txtOptionPos.Text = txtSetPos.Text + sWords(2).Length + 1
    '                    If txtScript.SelectionStart > txtSetPos.Text And txtScript.SelectionStart <= txtOptionPos.Text Then
    '                        txtWord.Text = "Option Value"
    '                        sCurrentPart = "Option Value"
    '                        SuggestionOption()
    '                    End If

    '                    If sWords.Length > 3 Then
    '                        sWords(3) = sWords(3).Trim
    '                        txtParam1.Text = sWords(3)
    '                        txtParam1Pos.Text = txtOptionPos.Text + sWords(3).Length + 1
    '                        If txtScript.SelectionStart > txtOptionPos.Text And txtScript.SelectionStart <= txtParam1Pos.Text Then
    '                            txtWord.Text = "Param 1"
    '                            sCurrentPart = "Param 1"
    '                            SuggestionParam1()
    '                        End If

    '                        If sWords.Length > 4 Then
    '                            sWords(4) = sWords(4).Trim
    '                            txtParam2.Text = sWords(4)
    '                            txtParam2Pos.Text = txtParam1Pos.Text + sWords(4).Length + 1
    '                            If txtScript.SelectionStart > txtParam1Pos.Text And txtScript.SelectionStart <= txtParam2Pos.Text Then
    '                                txtWord.Text = "Param 2"
    '                                sCurrentPart = "Param 2"
    '                            End If
    '                        Else
    '                            txtParam2.Text = "" : txtParam2Pos.Text = ""
    '                            lstParam2.Visible = False
    '                            lstParam2.Items.Clear()
    '                        End If
    '                    Else
    '                        txtParam1.Text = "" : txtParam1Pos.Text = ""
    '                        lstParam1.Visible = False
    '                        lstParam1.Items.Clear()
    '                        txtParam2.Text = "" : txtParam2Pos.Text = ""
    '                        lstParam2.Visible = False
    '                        lstParam2.Items.Clear()
    '                    End If
    '                Else
    '                    txtOption.Text = "" : txtOptionPos.Text = ""
    '                    lstOptions.Visible = False
    '                    lstOptions.Items.Clear()
    '                    txtParam1.Text = "" : txtParam1Pos.Text = ""
    '                    lstParam1.Visible = False
    '                    lstParam1.Items.Clear()
    '                    txtParam2.Text = "" : txtParam2Pos.Text = ""
    '                    lstParam2.Visible = False
    '                    lstParam2.Items.Clear()
    '                End If
    '            Else
    '                txtSet.Text = "" : txtSetPos.Text = ""
    '                lstSet.Visible = False
    '                lstSet.Items.Clear()
    '                txtOption.Text = "" : txtOptionPos.Text = ""
    '                lstOptions.Visible = False
    '                lstOptions.Items.Clear()
    '                txtParam1.Text = "" : txtParam1Pos.Text = ""
    '                lstParam1.Visible = False
    '                lstParam1.Items.Clear()
    '                txtParam2.Text = "" : txtParam2Pos.Text = ""
    '                lstParam2.Visible = False
    '                lstParam2.Items.Clear()
    '            End If
    '        End If
    '    Else
    '        ClearSuggestions()
    '    End If
    '    ' txtX.Text = textArray(iCurrentLine).SelectionStart
    'End Sub

    'Private Sub SuggestionObject()
    '    Debug.Print(" -- SuggestionObject")
    '    Dim CMD As New MySqlCommand
    '    Dim myReader As MySqlDataReader
    '    ClearSuggestions()
    '    CMD.Connection = CN
    '    CMD.CommandType = CommandType.Text
    '    CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE UPPER(object_name) LIKE UPPER(?pmatch) ORDER BY object_name"
    '    CMD.Parameters.AddWithValue("?pmatch", txtObject.Text.Trim & "%")
    '    CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text.Trim)
    '    Try
    '        CN.Open()
    '        myReader = CMD.ExecuteReader
    '        While myReader.Read
    '            lstObjects.Items.Add(Convert.ToString(myReader.Item("object_name")))
    '            lstObjects.Visible = True
    '        End While
    '        CN.Close()
    '        CMD.Parameters.Clear()
    '        If lstObjects.Items.Count > 0 Then
    '            lstObjects.Top = 2 + (txtScript.Font.Height) * (iLine + 1)
    '            lstObjects.Left = txtScript.Left + txtScript.Font.Size * (txtObjectPos1.Text - txtStart.Text)
    '            lstObjects.Height = (lstObjects.Font.Height + 6) * lstObjects.Items.Count
    '            lstObjects.SelectedIndex = 0
    '            lstObjects.Visible = True
    '        Else
    '            ClearSuggestions()
    '        End If
    '    Catch myerror As MySqlException
    '        MessageBox.Show("Error SuggestionObject(): " & myerror.Message)
    '        CN.Close()
    '    End Try
    'End Sub

    'Private Sub SuggestionSet()
    '    Dim cRunMethod As String = "Run Method"
    '    Dim cFeignEvent As String = "Feign Event"
    '    Dim cSetProperty As String = "Set Property"
    '    Dim cSetState As String = "Set State"
    '    ClearSuggestions()
    '    If txtType.Text = "SET" Then
    '        If txtSet.Text.Trim.Length = 0 Then
    '            lstSet.Items.Add(cFeignEvent)
    '            lstSet.Items.Add(cRunMethod)
    '            lstSet.Items.Add(cSetProperty)
    '            lstSet.Items.Add(cSetState)
    '        Else
    '            If cFeignEvent.ToUpper.StartsWith(txtSet.Text.ToUpper) Then 'And cFeignEvent.ToUpper <> txtOption.Text.ToUpper Then
    '                lstSet.Items.Add(cFeignEvent)
    '            End If
    '            If cRunMethod.ToUpper.StartsWith(txtSet.Text.ToUpper) Then ' And cRunMethod.ToUpper <> txtOption.Text.ToUpper Then
    '                lstSet.Items.Add(cRunMethod)
    '            End If
    '            If cSetProperty.ToUpper.StartsWith(txtSet.Text.ToUpper) Then ' And cSetProperty.ToUpper <> txtOption.Text.ToUpper Then
    '                lstSet.Items.Add(cSetProperty)
    '            End If
    '            If cSetState.ToUpper.StartsWith(txtSet.Text.ToUpper) Then ' And cSetState.ToUpper <> txtOption.Text.ToUpper Then
    '                lstSet.Items.Add(cSetState)
    '            End If
    '        End If
    '    Else
    '    End If
    '    If lstSet.Items.Count > 0 Then
    '        lstSet.Top = 2 + (txtScript.Font.Height) * (iLine + 1)
    '        lstSet.Left = txtScript.Left + txtScript.Font.Size * (txtObjectPos2.Text - txtStart.Text - 2)
    '        lstSet.Height = (lstSet.Font.Height + 6) * (lstSet.Items.Count)
    '        lstSet.SelectedIndex = 0
    '        lstSet.Visible = True
    '    Else
    '        ClearSuggestions()
    '    End If
    'End Sub

    'Private Sub SuggestionOption()
    '    Dim CMD As New MySqlCommand
    '    Dim myReader As MySqlDataReader
    '    ClearSuggestions()
    '    CMD.Connection = CN
    '    CMD.CommandType = CommandType.Text
    '    If txtSet.Text.ToUpper = "RUN METHOD" Then
    '        CMD.CommandText = "SELECT method_label FROM osae_v_object_method WHERE object_name=?pfullmatch AND method_label LIKE ?poption ORDER BY method_label"
    '        CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text)
    '        CMD.Parameters.AddWithValue("?poption", txtOption.Text & "%")
    '        CMD.Parameters.AddWithValue("?pmethod", txtOption.Text)
    '        Try
    '            CN.Open()
    '            myReader = CMD.ExecuteReader
    '            While myReader.Read
    '                lstOptions.Items.Add(Convert.ToString(myReader.Item("method_label")))
    '            End While
    '            CN.Close()
    '            CMD.Parameters.Clear()
    '        Catch myerror As MySqlException
    '            MessageBox.Show("Error txtProperty_TextChanged: " & myerror.Message)
    '            CN.Close()
    '        End Try
    '    ElseIf txtSet.Text.ToUpper = "FEIGN EVENT" Then
    '        CMD.CommandText = "SELECT event_label FROM osae_v_object_event WHERE object_name =?pfullmatch ORDER BY event_label"
    '        'sType = outValue.Substring(0, outValue.Length + 1 - iPos1)
    '        CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text)
    '        Try
    '            CN.Open()
    '            myReader = CMD.ExecuteReader
    '            While myReader.Read
    '                lstOptions.Items.Add(Convert.ToString(myReader.Item("event_label")))
    '            End While
    '            CN.Close()
    '            CMD.Parameters.Clear()
    '        Catch myerror As MySqlException
    '            MessageBox.Show("Error txtProperty_TextChanged: " & myerror.Message)
    '            CN.Close()
    '        End Try
    '    ElseIf txtSet.Text.ToUpper = "SET STATE" Then
    '        CMD.CommandText = "SELECT state_label FROM osae_v_object_state WHERE object_name =?pfullmatch ORDER BY state_label"
    '        'sType = outValue.Substring(0, outValue.Length + 1 - iPos1)
    '        CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text)
    '        Try
    '            CN.Open()
    '            myReader = CMD.ExecuteReader
    '            While myReader.Read
    '                lstOptions.Items.Add(Convert.ToString(myReader.Item("state_label")))
    '            End While
    '            CN.Close()
    '            CMD.Parameters.Clear()
    '        Catch myerror As MySqlException
    '            MessageBox.Show("Error txtProperty_TextChanged: " & myerror.Message)
    '            CN.Close()
    '        End Try
    '    ElseIf txtSet.Text.ToUpper = "SET PROPERTY" Then
    '        CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name =?pfullmatch ORDER BY property_name"
    '        'sType = outValue.Substring(0, outValue.Length + 1 - iPos1)
    '        CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text)
    '        Try
    '            CN.Open()
    '            myReader = CMD.ExecuteReader
    '            While myReader.Read
    '                lstOptions.Items.Add(Convert.ToString(myReader.Item("property_name")))
    '            End While
    '            CN.Close()
    '            CMD.Parameters.Clear()

    '        Catch myerror As MySqlException
    '            MessageBox.Show("Error txtProperty_TextChanged: " & myerror.Message)
    '            CN.Close()
    '        End Try
    '    End If
    '    If lstOptions.Items.Count > 0 Then
    '        lstOptions.Top = 2 + (txtScript.Font.Height) * (iLine + 1)
    '        lstOptions.Left = txtScript.Left + txtScript.Font.Size * (txtSetPos.Text - txtStart.Text - 4)
    '        lstOptions.Height = (lstOptions.Font.Height + 6) * (lstOptions.Items.Count)
    '        lstOptions.SelectedIndex = 0
    '        lstOptions.Visible = True
    '    Else
    '        ClearSuggestions()
    '    End If
    'End Sub

    'Private Sub SuggestionParam1()
    '    Dim CMD As New MySqlCommand
    '    Dim myReader As MySqlDataReader
    '    ClearSuggestions()
    '    CMD.Connection = CN
    '    CMD.CommandType = CommandType.Text
    '    If txtSet.Text.ToUpper = "RUN METHOD" Then
    '        CMD.CommandText = "SELECT COALESCE(param_1_label,'') FROM osae_v_object_method WHERE UPPER(object_name)=?pfullmatch AND (UPPER(Method_Name)=?pmethod OR UPPER(Method_Label)=?pmethod)"
    '        CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text.ToUpper)
    '        CMD.Parameters.AddWithValue("?pmethod", txtOption.Text.ToUpper)
    '        Try
    '            CN.Open()
    '            myReader = CMD.ExecuteReader
    '            While myReader.Read
    '                If Convert.ToString(myReader.Item(0)) <> "" Then
    '                    lstParam1.Items.Add(Convert.ToString(myReader.Item(0)))
    '                End If
    '            End While
    '            CN.Close()
    '            CMD.Parameters.Clear()

    '        Catch myerror As MySqlException
    '            MessageBox.Show("Error SuggestionParam1: " & myerror.Message)
    '            CN.Close()
    '        End Try
    '    End If
    '    If lstParam1.Items.Count > 0 Then
    '        lstParam1.Top = 2 + (txtScript.Font.Height) * (iLine + 1)
    '        lstParam1.Left = txtScript.Left + txtScript.Font.Size * (txtOptionPos.Text - txtStart.Text - 6)
    '        lstParam1.Height = (lstParam1.Font.Height + 6) * (lstParam1.Items.Count)
    '        lstParam1.SelectedIndex = 0
    '        lstParam1.Visible = True
    '    End If
    'End Sub

    'Private Sub SuggestionParam2()
    '    Dim CMD As New MySqlCommand
    '    Dim myReader As MySqlDataReader
    '    ClearSuggestions()
    '    CMD.Connection = CN
    '    CMD.CommandType = CommandType.Text
    '    If txtSet.Text.ToUpper = "RUN METHOD" Then
    '        CMD.CommandText = "SELECT COALESCE(param_2_label,'') FROM osae_v_object_method WHERE object_name=?pfullmatch"
    '        CMD.Parameters.AddWithValue("?pfullmatch", txtObject.Text)
    '        Try
    '            CN.Open()
    '            myReader = CMD.ExecuteReader
    '            While myReader.Read
    '                If Convert.ToString(myReader.Item("param_2_label")) <> "" Then
    '                    lstParam2.Items.Add(Convert.ToString(myReader.Item("param_2_label")))
    '                End If
    '            End While
    '            CN.Close()
    '            CMD.Parameters.Clear()
    '        Catch myerror As MySqlException
    '            MessageBox.Show("Error SuggestionParam1: " & myerror.Message)
    '            CN.Close()
    '        End Try
    '    End If
    '    If lstParam2.Items.Count > 0 Then
    '        lstParam2.Top = 2 + (txtScript.Font.Height) * (iLine + 1)
    '        lstParam2.Left = txtScript.Left + txtScript.Font.Size * (txtParam1Pos.Text - txtStart.Text - 8)
    '        lstParam2.Height = (lstParam2.Font.Height + 6) * (lstParam2.Items.Count)
    '        lstParam2.SelectedIndex = 0
    '        lstParam2.Visible = True
    '    End If
    'End Sub

    'Private Sub ClearSuggestions()
    '    Debug.Print(" -- ClearSuggestions")
    '    lstObjects.Visible = False
    '    lstObjects.Items.Clear()
    '    lstSet.Visible = False
    '    lstSet.Items.Clear()
    '    lstOptions.Visible = False
    '    lstOptions.Items.Clear()
    '    lstParam1.Visible = False
    '    lstParam1.Items.Clear()
    '    lstParam2.Visible = False
    '    lstParam2.Items.Clear()
    'End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        ValidateSave()
    End Sub

    Private Sub radPattern_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radPattern.CheckedChanged
        If radPattern.Checked = True Then
            cboPatterns.Visible = True
            lblPattern.Visible = True
            grpEvent.Visible = False
        End If
    End Sub

    Private Sub radEvent_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radEvent.CheckedChanged
        If radEvent.Checked = True Then
            grpEvent.Visible = True
            cboPatterns.Visible = False
            lblPattern.Visible = False
        End If
    End Sub

    Private Sub cboPatterns_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPatterns.SelectedIndexChanged
        Dim CMD As New MySqlCommand
        Dim sScript As String = ""
        CMD.Connection = CN
        CMD.CommandType = CommandType.Text
        CMD.CommandText = "SELECT script FROM osae_pattern WHERE pattern=?pname"
        CMD.Parameters.AddWithValue("?pname", cboPatterns.Text)
        Try
            CN.Open()
            sScript = CMD.ExecuteScalar
            txtScript.Text = sScript
            CN.Close()
            'For iLoop = 1 To 10
            '    textArray(iLoop).Text = ""
            'Next iLoop
            If sScript <> "" Then
                txtScript.Text = sScript
                'Dim sArray() As String = sScript.Split(vbCrLf)
                'For iLoop = 0 To sArray.Length - 2
                '    textArray(iLoop + 1).Text = sArray(iLoop)
                'Next iLoop
            End If
        Catch myerror As MySqlException
            MessageBox.Show("Error Load_Object_Events: " & myerror.Message)
            CN.Close()
        End Try
        ValidateSave()
    End Sub

    Private Sub txtScript_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtScript.KeyDown
        Debug.Print(" - txtScript_KeyDown")
        If e.KeyCode = Keys.OemPeriod Then
            'e.Handled = True
            If sCurrentPart = "Object Name" And lstObjects.Visible = True Then
                Select_Object()
                txtScript.SelectionStart += 1
            ElseIf sCurrentPart = "Option Name" And lstSet.Visible = True Then
                Select_Set()
                txtScript.SelectionStart += 1
            ElseIf sCurrentPart = "Option Value" And lstOptions.Visible = True Then
                Select_Option()
                txtScript.SelectionStart += 1
            End If
            'Script_Moved()
        ElseIf (e.KeyCode = 38) Then
            If lstObjects.Visible Then
                If lstObjects.SelectedIndex > 0 Then
                    lstObjects.SelectedIndex -= 1
                    e.Handled = True
                End If
            End If

            If lstSet.Visible Then
                If lstSet.SelectedIndex > 0 Then
                    lstSet.SelectedIndex -= 1
                    e.Handled = True
                End If
            End If

            If lstOptions.Visible Then
                If lstOptions.SelectedIndex > 0 Then
                    lstOptions.SelectedIndex -= 1
                    e.Handled = True
                End If
            End If
        ElseIf (e.KeyCode = 40) Then 'ArrowDn
            If lstObjects.Visible Then
                If lstObjects.SelectedIndex < lstObjects.Items.Count - 1 Then
                    lstObjects.SelectedIndex += 1
                    e.Handled = True
                End If
            End If
            If lstSet.Visible Then
                If lstSet.SelectedIndex < lstSet.Items.Count - 1 Then
                    lstSet.SelectedIndex += 1
                    e.Handled = True
                End If
            End If
            If lstOptions.Visible Then
                If lstOptions.SelectedIndex < lstOptions.Items.Count - 1 Then
                    lstOptions.SelectedIndex += 1
                    e.Handled = True
                End If

            End If
        End If
    End Sub

    Private Sub txtScript_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtScript.KeyPress
        'Debug.Print(" -- txtScript_KeyPress")
        'Script_Moved()
    End Sub

    Private Sub txtScript_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtScript.KeyUp
        If e.KeyCode = Keys.Left Or e.KeyCode = Keys.Right Then
            Debug.Print(" --- txtScript_KeyUp ")
            Script_Moved()
        End If
        If e.KeyCode = Keys.Up Or e.KeyCode = Keys.Down Then
            'If lstObjects.Visible = False And lstSet.Visible = False And lstOptions.Visible = False Then
            Debug.Print(" --- kkkkkkkkkkkkkkkktxtScript_KeyUp ")
            Script_Moved()
            'End If
        End If
    End Sub

    Private Sub lstObjects_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstObjects.MouseClick
        txtScript.Focus()
    End Sub

    Private Sub lstObjects_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstObjects.MouseDoubleClick
        Debug.Print(" -- lstObjects_MouseDoubleClick")
        txtScript.Focus()
        Select_Object()
        Script_Moved()
    End Sub

    Private Sub lstSet_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstSet.MouseClick
        txtScript.Focus()
    End Sub

    Private Sub lstSet_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstSet.MouseDoubleClick
        Debug.Print(" -- lstSet_MouseDoubleClick")
        txtScript.Focus()
        Select_Set()
        Script_Moved()
    End Sub

    Private Sub lstOptions_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstOptions.MouseClick
        txtScript.Focus()
    End Sub

    Private Sub lstOptions_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lstOptions.MouseDoubleClick
        Debug.Print(" -- lstOptions_MouseDoubleClick")
        txtScript.Focus()
        Select_Option()
        Script_Moved()
    End Sub

    Private Sub Select_Object()
        Debug.Print(" -- Select_Object")
        Dim sBefore As String = "", sAfter As String = ""
        If txtObjectPos1.TextLength > 0 Then
            sBefore = txtScript.Text.Trim.Substring(0, txtObjectPos1.Text)
        Else
            sBefore = ""
        End If
        sAfter = txtScript.Text.Substring(txtObjectPos1.Text + txtObject.TextLength, txtScript.Text.Length - (txtObjectPos1.Text + txtObject.TextLength))
        txtScript.Text = sBefore & lstObjects.Text & sAfter
        txtScript.SelectionStart = txtObjectPos1.Text + lstObjects.Text.Length
        lstObjects.Visible = False
    End Sub

    Private Sub Select_Set()
        Debug.Print(" -- Select_Set")
        Dim sBefore As String = "", sAfter As String = ""
        If txtSetPos.Text > 0 Then
            sBefore = txtScript.Text.Trim.Substring(0, txtObjectPos2.Text + 1) 'txtCurrentLine.Text.Substring(0, txtObjectPos.Text) & "."
        Else
            sBefore = ""
        End If
        sAfter = txtScript.Text.Substring(txtSetPos.Text, txtScript.Text.Length - txtSetPos.Text)
        txtScript.Text = sBefore & lstSet.Text & sAfter
        txtScript.SelectionStart = txtSetPos.Text + lstSet.Text.Length
        lstSet.Visible = False
    End Sub

    Private Sub Select_Option()
        Debug.Print(" -- Select_Option")
        Dim sBefore As String = "", sAfter As String = ""
        If txtSetPos.Text > 0 Then
            sBefore = txtScript.Text.Substring(0, txtSetPos.Text + 1) 'txtCurrentLine.Text.Substring(0, txtObjectPos.Text) & "."
        Else
            sBefore = ""
        End If
        sAfter = txtScript.Text.Substring(txtOptionPos.Text, txtScript.Text.Length - txtOptionPos.Text)
        txtScript.Text = sBefore & lstOptions.Text & sAfter
        txtScript.SelectionStart = txtOptionPos.Text + lstOptions.Text.Length
        lstOptions.Visible = False
    End Sub

    Public Sub Script_Moved()
        Debug.Print(" -- Script_Moved")
        txtSize.Text = txtScript.TextLength

        iLineCursor = txtScript.SelectionStart
        txtPosition.Text = iLineCursor

        iLine = txtScript.GetLineFromCharIndex(iLineCursor)
        txtY.Text = iLine + 1
        lstLine.SelectedIndex = iLine
        iLineStart = txtScript.GetFirstCharIndexFromLine(iLine)
        txtStart.Text = iLineStart

        iLineEnd = txtScript.GetFirstCharIndexFromLine(iLine + 1) - 1
        If iLineEnd < 0 Then iLineEnd = txtScript.TextLength
        txtEnd.Text = iLineEnd

        txtCurrentLine.Text = txtScript.Text.Substring(iLineStart, iLineEnd - iLineStart).Replace(vbCrLf, "*")


        txtX.Text = iLineCursor - iLineStart

        'ShowSuggestions()
    End Sub

    Private Sub txtObject_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtObject.TextChanged
        'txtObject.Text = txtObject.Text.Trim
        'txtObjectPos1.Text = txtStart.Text
        'txtObjectPos2.Text = txtStart.Text + txtObject.Text
    End Sub

    Private Sub txtScript_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtScript.MouseClick
        Script_Moved()
    End Sub

    Private Sub txtScript_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtScript.TextChanged
        Debug.Print(" -- txtScript_TextChanged")
        Script_Moved()
    End Sub

    Private Sub txtTestParameter_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtTestParameter.TextChanged
        sScriptParameter = txtTestParameter.Text
    End Sub


End Class