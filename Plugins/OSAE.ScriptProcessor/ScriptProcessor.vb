Option Strict Off
Option Explicit On
Imports System.Text.RegularExpressions

Public Class ScriptProcessor
    Inherits OSAEPluginBase
    'Private OSAEApi As New OSAE("Script Processor")
    Private Shared logging As Logging = Logging.GetLogger("Script Processor")
    Private gAppName As String = ""
    Private scriptArray() As String



    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim sScript As String = "", sScriptName As String = ""

        If method.MethodName = "RUN SCRIPT" Then
            Try
                Dim dsResults As DataSet = OSAESql.RunSQL("SELECT script,script_name FROM osae_script WHERE script_id=" & method.Parameter1)
                sScript = Convert.ToString(dsResults.Tables(0).Rows(0)("script"))
                sScriptName = Convert.ToString(dsResults.Tables(0).Rows(0)("script_name"))
                logging.AddToLog("Found Script: " & sScriptName, True)
                RunScript(sScript, method.Parameter2)
                logging.AddToLog("Executed Script", True)
            Catch ex As Exception
                logging.AddToLog("Error ProcessCommand - " & ex.Message, True)
            End Try
        End If
    End Sub

    Public Overrides Sub RunInterface(ByVal sName As String)
        gAppName = sName
        logging.AddToLog("Running Interface!", True)
    End Sub

    Public Overrides Sub Shutdown()
        logging.AddToLog("*** Received Shutdown", True)
    End Sub

    Private Sub RunScript(ByVal scriptText As String, ByVal sScriptParameter As String)
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
        Dim sWorking As String, sProperty As String = "", pProperty As New OSAEObjectProperty
        Dim sNesting(20) As String, iNestingLevel As Integer = 0
        Dim sScript As String
        Dim sSubScript As String = "", sSubScriptName As String = ""
        Dim sTempLine As String = ""
        sNesting(0) = "PASS"
        sScript = scriptText
        Dim iEmbeddedScriptStart As Integer = 0
        Dim iEmbeddedScriptEnd As Integer = 0
        Try
            iEmbeddedScriptStart = sScript.IndexOf("Script:", iEmbeddedScriptStart)
            Do While iEmbeddedScriptStart > 0
                iEmbeddedScriptEnd = sScript.IndexOf(vbCrLf, iEmbeddedScriptStart)
                sTempLine = sScript.Substring(iEmbeddedScriptStart, (iEmbeddedScriptEnd - iEmbeddedScriptStart))
                sSubScriptName = sScript.Substring(iEmbeddedScriptStart, (iEmbeddedScriptEnd - iEmbeddedScriptStart)).Replace("Script:", "")
                Dim dsResults As DataSet = OSAESql.RunSQL("SELECT script FROM osae_script WHERE script_name='" & sSubScriptName & "'")
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
        Dim sParams() As String = sScriptParameter.Split(",")
        Dim iCounter As Integer = 0
        For Each tempparam As String In sParams
            iCounter += 1
            sScript = sScript.Replace("PARAM" & iCounter.ToString(), tempparam)
        Next


        Dim scriptArray() = sScript.Split(vbCrLf)

        For iLoop = 0 To scriptArray.Length - 1
            Try
                scriptArray(iLoop) = scriptArray(iLoop).Trim
                'txtEcho.Text += scriptArray(iLoop) & Environment.NewLine
                If scriptArray(iLoop).Length > 2 Then
                    'scriptArray(iLoop) = scriptArray(iLoop).Replace("SCRIPT.PARAMETER", (sScriptParameter))
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
                    iObjectPos = scriptArray(iLoop).IndexOf(".")
                    'Looking for Object Name.
                    If iObjectPos > 0 Then
                        sObject = scriptArray(iLoop).Substring(0, iObjectPos)
                        Dim oCurrentObject As OSAEObject
                        oCurrentObject = OSAEObjectManager.GetObjectByName(sObject)
                        sWorking = scriptArray(iLoop).Substring(iObjectPos + 1, scriptArray(iLoop).Length - (iObjectPos + 1))
                        If sObject.ToUpper = "SQL" Then
                            OSAESql.RunSQL(sWorking)
                        Else
                            'Now we have our object, and the rest of the line is stored in sWorking
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

                                ' Find First parameter based on a pair of "" or a Comma
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
                                    OSAEMethodManager.MethodQueueAdd(sObject, sMethod, sParam1, sParam2, gAppName)
                                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Ran Method: " & sObject & "." & sMethod & " (" & sParam1 & "," & sParam2 & ")")
                                ElseIf sOption.ToUpper = "SET STATE" Then
                                    OSAEObjectStateManager.ObjectStateSet(sObject, sParam1, gAppName)
                                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Set State: " & sObject & "." & sParam1)
                                ElseIf sOption.ToUpper = "SET CONTAINER" Then
                                    OSAEObjectManager.ObjectUpdate(oCurrentObject.Name, oCurrentObject.Name, oCurrentObject.Description, oCurrentObject.Type, oCurrentObject.Address, sMethod, oCurrentObject.Enabled)
                                    Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Set Container: " & sObject & "." & sMethod)
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
                                                OSAEObjectPropertyManager.ObjectPropertySet(sObject, sParam1, sValue, gAppName)
                                                Display_Results(iLoop + 1 & ": (" & iNestingLevel & ") - Set Property: " & sParam1 & " = " & sValue)
                                            ElseIf sMethod = "+=" Then
                                                sWorking = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sParam1).Value
                                                OSAEObjectPropertyManager.ObjectPropertySet(sObject, sParam1, Val(sWorking) + Val(sValue), gAppName)
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
                                            OSAEObjectPropertyManager.ObjectPropertySet(sObject, sParam1, sValue, gAppName)
                                            Display_Results(iLoop & ": Set Property: " & sObject & "." & sParam1)
                                        ElseIf sMethod = "+=" Then
                                            sWorking = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sParam1).Value
                                            OSAEObjectPropertyManager.ObjectPropertySet(sObject, sParam1, Val(sWorking) + Val(sValue), gAppName)
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
                                    sState = OSAEObjectStateManager.GetObjectStateValue(sObject).Value


                                    If sState.ToUpper <> sValue.ToUpper Then sNesting(iNestingLevel) = "FAIL"
                                Else
                                    pProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sOption)
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
                                    sState = OSAEObjectStateManager.GetObjectStateValue(sObject).Value
                                    If sState.ToUpper = sValue.ToUpper Then
                                        sNesting(iNestingLevel) = "FAIL"
                                    End If
                                Else
                                    pProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sOption)
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
                                    If lSeconds > OSAEObjectStateManager.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    pProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sOption)
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
                                    If lSeconds < OSAEObjectStateManager.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    pProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sOption)
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
                                    If lSeconds >= OSAEObjectStateManager.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    sProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sOption).Value
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
                                    If lSeconds <= OSAEObjectStateManager.GetObjectStateValue(sObject).TimeInState Then sNesting(iNestingLevel) = "FAIL"
                                ElseIf sOption.ToUpper <> "STATE" Then
                                    sProperty = OSAEObjectPropertyManager.GetObjectPropertyValue(sObject, sProperty).Value
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
        logging.AddToLog(sText, False)
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
End Class
