Imports System.AddIn
Imports OpenSourceAutomation
<AddIn("PHIDGET-IK", Version:="1.0.0")>
Public Class PhidgetIK
    Implements IOpenSourceAutomationAddIn

    Dim WithEvents phidgetIFK As Phidgets.InterfaceKit
    'Private CN As MySqlConnection
    Private OSAEApi As New OSAE("PHIDGET-IK")

    Private gAppName As String = ""
    Private gAttached As Boolean
    Private gName As String = ""
    Private gSerial As String = ""
    Private gVersion As String = ""
    Private gSensitivity As Integer
    Private gRatiometric As Integer
    Private gDigitalInputs As Integer
    Private gDINames(8) As String
    Private gAINames(8) As String
    Private gDONames(8) As String
    Private gDigitalOutputs As Integer
    Private gAnalogInputs As Integer
    Private gIsLoading As Boolean = True
    Private tmrStartup As New Timers.Timer

    Public Sub ProcessCommand(ByVal table As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        Dim sAddress As String, iState As Integer
        Dim row As System.Data.DataRow
        For Each row In table.Rows
            Try
                If row("method_name").ToString = "SET SENSITIVITY" Then
                    Try
                        Dim i As Integer
                        For i = 0 To phidgetIFK.sensors.Count - 1
                            phidgetIFK.sensors(i).Sensitivity = row("parameter_1").ToString
                        Next
                    Catch ex As Exception
                        OSAEApi.AddToLog("Error SET SENSITIVITY - " & ex.Message, True)
                    End Try
                ElseIf row("method_name").ToString = "SET RADIOMETERIC" Then
                    Try
                        If phidgetIFK.Attached Then
                            phidgetIFK.ratiometric = row("parameter_1").ToString
                        End If
                    Catch ex As Exception
                        OSAEApi.AddToLog("Error SET RADIOMETERIC - " & ex.Message, True)
                    End Try
                Else
                    sAddress = row("address").ToString.Replace((gSerial & "-DO"), "")
                    If row("method_name").ToString = "ON" Then iState = 1
                    If row("method_name").ToString = "OFF" Then iState = 0
                    phidgetIFK.outputs(sAddress) = iState
                    OSAEApi.ObjectStateSet(row("object_name").ToString, row("method_name").ToString)
                    OSAEApi.AddToLog("Executed " & row("object_name").ToString & " " & row("method_name").ToString & "(" & row("address").ToString & " " & row("method_name").ToString & ")", True)
                End If
            Catch ex As Exception
                OSAEApi.AddToLog("Error ProcessCommand - " & ex.Message, True)
            End Try
        Next
    End Sub

    Public Sub RunInterface(ByVal pluginName As String) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.RunInterface
        gAppName = pluginName
        OSAEApi.AddToLog("Found my Object: " & gAppName, True)
        Try
            phidgetIFK = New Phidgets.InterfaceKit
            phidgetIFK.open()
        Catch ex As Exception
            OSAEApi.AddToLog(gAppName & " Error: Form1_Load: " & ex.ToString(), True)
        End Try
        tmrStartup.Interval = 5000
        AddHandler tmrStartup.Elapsed, AddressOf ResetStartupFlag
        tmrStartup.Enabled = True
        OSAEApi.AddToLog("Finished Loading", True)
    End Sub

    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        RemoveHandler phidgetIFK.Attach, AddressOf phidgetIFK_Attach
        RemoveHandler phidgetIFK.Detach, AddressOf phidgetIFK_Detach
        RemoveHandler phidgetIFK.Error, AddressOf phidgetIFK_Error
        RemoveHandler phidgetIFK.InputChange, AddressOf phidgetIFK_InputChange
        RemoveHandler phidgetIFK.OutputChange, AddressOf phidgetIFK_OutputChange
        RemoveHandler phidgetIFK.SensorChange, AddressOf phidgetIFK_SensorChange
    End Sub

    Public Sub ResetStartupFlag()
        gIsLoading = False
        tmrStartup.Enabled = False
    End Sub

    Private Sub phidgetIFK_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetIFK.Attach
        gAttached = phidgetIFK.Attached.ToString()
        OSAEApi.ObjectPropertySet(gAppName, "Attached", gAttached)
        OSAEApi.AddToLog("Attached set to: " & gAttached, True)

        gName = phidgetIFK.Name
        OSAEApi.ObjectPropertySet(gAppName, "Name", gName)
        OSAEApi.AddToLog("Name set to: " & gName, True)

        gVersion = sender.Version.ToString
        OSAEApi.ObjectPropertySet(gAppName, "Version", gVersion)
        OSAEApi.AddToLog("Version set to: " & gVersion, True)

        gSerial = sender.SerialNumber.ToString()
        OSAEApi.ObjectPropertySet(gAppName, "Serial", gSerial)
        OSAEApi.AddToLog("Serial set to: " & gSerial, True)

        gDigitalInputs = phidgetIFK.inputs.Count.ToString()
        OSAEApi.ObjectPropertySet(gAppName, "Digital Inputs", gDigitalInputs)
        OSAEApi.AddToLog("Digital Inputs set to: " & gDigitalInputs, True)

        gDigitalOutputs = sender.outputs.Count.ToString()
        OSAEApi.ObjectPropertySet(gAppName, "Digital Outputs", gDigitalOutputs)
        OSAEApi.AddToLog("Digital Outputs set to: " & gDigitalOutputs, True)

        gAnalogInputs = sender.sensors.Count.ToString()
        OSAEApi.ObjectPropertySet(gAppName, "Analog Inputs", gAnalogInputs)
        OSAEApi.AddToLog("Analog Inputs set to: " & gAnalogInputs, True)

        Dim i As Integer
        For i = 0 To gDigitalInputs - 1
            Try
                Dim bExists = OSAEApi.ObjectExists(gSerial & "-DI" & i)
                If Not bExists = True Then
                    OSAEApi.ObjectAdd("Unknown Phidget " & gSerial & "-DI" & i, "Unknown Phidget " & gSerial & "-DI" & i, "PHIDGET DIGITAL INPUT", gSerial & "-DI" & i, "", True)
                End If
                Dim oObject As OSAEObject = OSAEApi.GetObjectByAddress(gSerial & "-DI" & i)
                gDINames(i) = oObject.Name.ToString()
                OSAEApi.AddToLog("Digital Input " & i & " Object: " & gDINames(i), True)
            Catch ex As Exception
                OSAEApi.AddToLog("Error loading Object for Digital Input: " & ex.ToString(), True)
            End Try
        Next i

        For i = 0 To phidgetIFK.outputs.Count - 1
            Try
                Dim bExists = OSAEApi.ObjectExists(gSerial & "-DO" & i)
                If Not bExists = True Then
                    OSAEApi.ObjectAdd("Unknow Phidget " & gSerial & "-DO" & i, "Unknow Phidget " & gSerial & "-DO" & i, "PHIDGET DIGITAL OUTPUT", gSerial & "-DO" & i, "", True)
                End If
                Dim oObject As OSAEObject = OSAEApi.GetObjectByAddress(gSerial & "-DO" & i)
                gDONames(i) = oObject.Name.ToString()
                OSAEApi.AddToLog("Digital Output " & i & " Object: " & gDONames(i), True)
            Catch ex As Exception
                OSAEApi.AddToLog("Error loading Object for Digital Output: " & ex.ToString(), True)
            End Try
        Next i

        For i = 0 To phidgetIFK.sensors.Count - 1
            Try
                Dim bExists = OSAEApi.ObjectExists(gSerial & "-AI" & i)
                If Not bExists = True Then
                    OSAEApi.ObjectAdd("Unknow Phidget " & gSerial & "-AI" & i, "Unknow Phidget " & gSerial & "-AI" & i, "PHIDGET ANALOG INPUT", gSerial & "-AI" & i, "", True)
                End If
                Dim oObject As OSAEObject = OSAEApi.GetObjectByAddress(gSerial & "-AI" & i)
                gAINames(i) = oObject.Name.ToString()
                OSAEApi.AddToLog("Analog Input " & i & " Object: " & gAINames(i), True)
            Catch ex As Exception
                OSAEApi.AddToLog("Error loading Object for Analog Input: " & ex.ToString(), True)
            End Try
        Next i

        If phidgetIFK.sensors.Count > 0 Then
            gSensitivity = phidgetIFK.sensors(0).Sensitivity
            OSAEApi.ObjectPropertySet(gAppName, "Sensitivity", gSensitivity)
            OSAEApi.AddToLog("Sensitivity set to: " & gSensitivity, True)
            gRatiometric = phidgetIFK.ratiometric
        End If
    End Sub

    Private Sub phidgetIFK_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetIFK.Detach
        gAttached = phidgetIFK.Attached.ToString()
        gName = ""
        gSerial = ""
        gVersion = ""
        gDigitalInputs = ""
        gDigitalOutputs = ""
        gAnalogInputs = ""
        gSensitivity = 0
        gRatiometric = 0
    End Sub

    Private Sub phidgetIFK_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetIFK.Error
        OSAEApi.AddToLog("Error: phidgetIFK_Error: " & e.Description, True)
    End Sub

    Private Sub phidgetIFK_InputChange(ByVal sender As Object, ByVal e As Phidgets.Events.InputChangeEventArgs) Handles phidgetIFK.InputChange
        If gIsLoading = True Then Exit Sub
        OSAEApi.AddToLog("Digital Input Changed", True)
        Dim sState As String
        If e.Value = True Then sState = "ON" Else sState = "OFF"
        OSAEApi.AddToLog(gDINames(e.Index) & " " & sState, True)
        OSAEApi.ObjectStateSet(gDINames(e.Index), sState)
    End Sub

    Private Sub phidgetIFK_OutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.OutputChangeEventArgs) Handles phidgetIFK.OutputChange
        If gIsLoading = True Then Exit Sub
        OSAEApi.AddToLog("Digital Output Changed", True)
        Dim sState As String
        If e.Value = True Then sState = "ON" Else sState = "OFF"
        OSAEApi.AddToLog(gDONames(e.Index) & " " & sState, True)
        OSAEApi.ObjectStateSet(gDONames(e.Index), sState)
    End Sub

    Private Sub phidgetIFK_SensorChange(ByVal sender As Object, ByVal e As Phidgets.Events.SensorChangeEventArgs) Handles phidgetIFK.SensorChange
        If gIsLoading = True Then Exit Sub
        OSAEApi.AddToLog("Analog Input Changed", True)
        Dim sState As String
        If e.Value > 0 Then sState = "ON" Else sState = "OFF"
        OSAEApi.AddToLog(gAINames(e.Index) & " " & sState, True)
        OSAEApi.ObjectStateSet(gDONames(e.Index), sState)
        Dim SC As New MSScriptControl.ScriptControl
        SC.Language = "VBSCRIPT"
        Dim iResults As Integer
        Dim sFormula As String = OSAEApi.GetObjectPropertyValue(gDONames(e.Index), "Formula").Value.ToString()
        sFormula = sFormula.Replace("Analog Value", e.Value.ToString())
        If sFormula = "" Then
            iResults = e.Value.ToString()
        Else
            iResults = SC.Eval(sFormula)
        End If
        OSAEApi.ObjectPropertySet(gAINames(e.Index), "Analog Value", iResults)
    End Sub

    'Modify the sensitivity of the analog inputs. In other words, the amount that the inputs must change between sensorchange events.
    'Private Sub inputTrk_Scroll(ByVal sender As Object, ByVal e As System.EventArgs)

    'End Sub

    'Private Sub ratioChk_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ratioChk.CheckedChanged
    '    If phidgetIFK.Attached Then
    '        phidgetIFK.ratiometric = ratioChk.Checked
    '    End If
    'End Sub

    'When the application is terminating, close the Phidget.

End Class
