Public Class PhidgetIK
    Inherits OSAEPluginBase
    Dim WithEvents phidgetIFK As Phidgets.InterfaceKit
    Private Log As OSAE.General.OSAELog = New General.OSAELog()
    Private pName As String = ""
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
    Private gDebug As Boolean = False
    Private tmrStartup As New Timers.Timer

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim sAddress As String, iState As Integer

        Try
            If method.MethodName = "SET SENSITIVITY" Then
                Try
                    Dim i As Integer
                    For i = 0 To phidgetIFK.sensors.Count - 1
                        phidgetIFK.sensors(i).Sensitivity = method.Parameter1
                    Next
                Catch ex As Exception
                    Log.Error("Error SET SENSITIVITY - " & ex.Message)
                End Try
            ElseIf method.MethodName = "SET RADIOMETERIC" Then
                Try
                    If phidgetIFK.Attached Then
                        phidgetIFK.ratiometric = method.Parameter1
                    End If
                Catch ex As Exception
                    Log.Error("Error SET RADIOMETERIC - " & ex.Message)
                End Try
            Else
                sAddress = method.Address.Replace((gSerial & "-DO"), "")
                If method.MethodName = "ON" Then iState = 1
                If method.MethodName = "OFF" Then iState = 0
                phidgetIFK.outputs(sAddress) = iState
                OSAEObjectStateManager.ObjectStateSet(method.ObjectName, method.MethodName, pName)
                Log.Info("Executed " & method.ObjectName & " " & method.MethodName & "(" & method.Address & " " & method.MethodName & ")")
            End If
        Catch ex As Exception
            Log.Error("Error ProcessCommand - " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        pName = pluginName
        Log.Info("Found my Object: " & pName)
        gDebug = OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Debug").Value
        Log.Info("Plugin Debug Mode is set to: " & gDebug)

        OwnTypes()
        Try
            phidgetIFK = New Phidgets.InterfaceKit
            phidgetIFK.open()
        Catch ex As Exception
            Log.Error(pName & " Error: Form1_Load: " & ex.ToString())
        End Try
        If phidgetIFK.Attached Then
            Log.Info("Phidget Interface Kit is attached.")
        Else
            Log.Info("Phidget Interface Kit is NOT attached.")
        End If

        tmrStartup.Interval = 5000
        AddHandler tmrStartup.Elapsed, AddressOf ResetStartupFlag
        tmrStartup.Enabled = True
        Log.Info("Finished Loading")
    End Sub

    Public Overrides Sub Shutdown()
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
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        Log.Info("Attached set to: " & gAttached)

        gName = phidgetIFK.Name
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Name", gName, pName)
        Log.Info("Name set to: " & gName)

        gVersion = sender.Version.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Version", gVersion, pName)
        Log.Info("Version set to: " & gVersion)

        gSerial = sender.SerialNumber.ToString()
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Serial", gSerial, pName)
        Log.Info("Serial set to: " & gSerial)

        gDigitalInputs = phidgetIFK.inputs.Count.ToString()
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Digital Inputs", gDigitalInputs, pName)
        Log.Info("Digital Inputs set to: " & gDigitalInputs)

        gDigitalOutputs = sender.outputs.Count.ToString()
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Digital Outputs", gDigitalOutputs, pName)
        Log.Info("Digital Outputs set to: " & gDigitalOutputs)

        gAnalogInputs = sender.sensors.Count.ToString()
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Analog Inputs", gAnalogInputs, pName)
        Log.Info("Analog Inputs set to: " & gAnalogInputs)

        Dim i As Integer
        For i = 0 To gDigitalInputs - 1
            Try
                Dim bExists = OSAEObjectManager.ObjectExists(gSerial & "-DI" & i)
                If Not bExists = True Then
                    OSAEObjectManager.ObjectAdd("Unknown Phidget " & gSerial & "-DI" & i, "Unknown Phidget " & gSerial & "-DI" & i, "PHIDGET DIGITAL INPUT", gSerial & "-DI" & i, "", True)
                End If
                Dim oObject As OSAEObject = OSAEObjectManager.GetObjectByAddress(gSerial & "-DI" & i)
                gDINames(i) = oObject.Name.ToString()
                Log.Info("Digital Input " & i & " Object: " & gDINames(i))
            Catch ex As Exception
                Log.Error("Error loading Object for Digital Input: " & ex.ToString())
            End Try
        Next i

        For i = 0 To phidgetIFK.outputs.Count - 1
            Try
                Dim bExists = OSAEObjectManager.ObjectExists(gSerial & "-DO" & i)
                If Not bExists = True Then
                    OSAEObjectManager.ObjectAdd("Unknow Phidget " & gSerial & "-DO" & i, "Unknow Phidget " & gSerial & "-DO" & i, "PHIDGET DIGITAL OUTPUT", gSerial & "-DO" & i, "", True)
                End If
                Dim oObject As OSAEObject = OSAEObjectManager.GetObjectByAddress(gSerial & "-DO" & i)
                gDONames(i) = oObject.Name.ToString()
                Log.Info("Digital Output " & i & " Object: " & gDONames(i))
            Catch ex As Exception
                Log.Error("Error loading Object for Digital Output: " & ex.ToString())
            End Try
        Next i

        For i = 0 To phidgetIFK.sensors.Count - 1
            Try
                Dim bExists = OSAEObjectManager.ObjectExists(gSerial & "-AI" & i)
                If Not bExists = True Then
                    OSAEObjectManager.ObjectAdd("Unknow Phidget " & gSerial & "-AI" & i, "Unknow Phidget " & gSerial & "-AI" & i, "PHIDGET ANALOG INPUT", gSerial & "-AI" & i, "", True)
                End If
                Dim oObject As OSAEObject = OSAEObjectManager.GetObjectByAddress(gSerial & "-AI" & i)
                gAINames(i) = oObject.Name.ToString()
                Log.Info("Analog Input " & i & " Object: " & gAINames(i))
            Catch ex As Exception
                Log.Error("Error loading Object for Analog Input: " & ex.ToString())
            End Try
        Next i

        If phidgetIFK.sensors.Count > 0 Then
            gSensitivity = phidgetIFK.sensors(0).Sensitivity
            OSAEObjectPropertyManager.ObjectPropertySet(pName, "Sensitivity", gSensitivity, pName)
            Log.Info("Sensitivity set to: " & gSensitivity)
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
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Name", gName, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Version", gVersion, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Serial", gSerial, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Digital Inputs", gDigitalInputs, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Digital Outputs", gDigitalOutputs, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Analog Inputs", gAnalogInputs, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Sensitivity", gSensitivity, pName)
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Ratiometric", gRatiometric, pName)
        Log.Info("*** !!!!!!!!!!!!!!!!!!! ***")
        Log.Info("*** Controller Detached ***")
        Log.Info("*** !!!!!!!!!!!!!!!!!!! ***")
    End Sub

    Private Sub phidgetIFK_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetIFK.Error
        Log.Error("Error: phidgetIFK_Error: " & e.Description)
    End Sub

    Private Sub phidgetIFK_InputChange(ByVal sender As Object, ByVal e As Phidgets.Events.InputChangeEventArgs) Handles phidgetIFK.InputChange
        If gIsLoading = True Then Exit Sub
        Try
            If gDebug Then Log.Debug("Digital Input Changed")
            Dim sState As String
            If e.Value = True Then sState = "ON" Else sState = "OFF"
            Log.Info(gDINames(e.Index) & " " & sState)
            OSAEObjectStateManager.ObjectStateSet(gDINames(e.Index), sState, pName)
        Catch ex As Exception
            Log.Error("phidgetIFK_InputChange: " & ex.Message)
        End Try
    End Sub

    Private Sub phidgetIFK_OutputChange(ByVal sender As Object, ByVal e As Phidgets.Events.OutputChangeEventArgs) Handles phidgetIFK.OutputChange
        If gIsLoading = True Then Exit Sub
        Try
            If gDebug Then Log.Debug("Digital Output Changed")
            Dim sState As String
            If e.Value = True Then sState = "ON" Else sState = "OFF"
            Log.Info(gDONames(e.Index) & " " & sState)
            OSAEObjectStateManager.ObjectStateSet(gDONames(e.Index), sState, pName)
        Catch ex As Exception
            Log.Error("phidgetIFK_OutputChange: " & ex.Message)
        End Try
    End Sub

    Private Sub phidgetIFK_SensorChange(ByVal sender As Object, ByVal e As Phidgets.Events.SensorChangeEventArgs) Handles phidgetIFK.SensorChange
        If gIsLoading = True Then Exit Sub
        Try
            If gDebug Then Log.Debug("Analog Input Changed")
            Dim sState As String
            If e.Value > 0 Then sState = "ON" Else sState = "OFF"
            Log.Info(gAINames(e.Index) & " " & sState)
            OSAEObjectStateManager.ObjectStateSet(gDONames(e.Index), sState, pName)
            Dim SC As New MSScriptControl.ScriptControl
            SC.Language = "VBSCRIPT"
            Dim iResults As Integer
            Dim sFormula As String = OSAEObjectPropertyManager.GetObjectPropertyValue(gDONames(e.Index), "Formula").Value.ToString()
            sFormula = sFormula.Replace("Analog Value", e.Value.ToString())
            If sFormula = "" Then
                iResults = e.Value.ToString()
            Else
                iResults = SC.Eval(sFormula)
            End If
            OSAEObjectPropertyManager.ObjectPropertySet(gAINames(e.Index), "Analog Value", iResults, pName)
        Catch ex As Exception
            Log.Error("phidgetIFK_SensorChange: " & ex.Message)
        End Try
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

    Public Sub OwnTypes()
        Dim oType As OSAEObjectType
        'Added the follow to automatically own relevant Base types that have no owner.
        'This should become the standard in plugins to try and avoid ever having to manually set the owners
        oType = OSAEObjectTypeManager.ObjectTypeLoad("PHIDGET ANALOG INPUT")
        Log.Info("Checking on the PHIDGET ANALOG INPUT Object Type.")
        If oType.OwnedBy = "" Then
            If gDebug Then Log.Debug("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info(pName & " Plugin took ownership of the PHIDGET ANALOG INPUT Object Type.")
        End If
        oType = OSAEObjectTypeManager.ObjectTypeLoad("PHIDGET DIGITAL INPUT")
        Log.Info("Checking on the PHIDGET DIGITAL INPUT Object Type.")
        If oType.OwnedBy = "" Then
            If gDebug Then Log.Debug("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info(pName & " Plugin took ownership of the PHIDGET DIGITAL INPUT Object Type.")
        End If
        oType = OSAEObjectTypeManager.ObjectTypeLoad("PHIDGET DIGITAL INPUT OC")
        Log.Info("Checking on the PHIDGET DIGITAL INPUT OC Object Type.")
        If oType.OwnedBy = "" Then
            If gDebug Then Log.Debug("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info(pName & " Plugin took ownership of the PHIDGET DIGITAL INPUT OC Object Type.")
        End If
        oType = OSAEObjectTypeManager.ObjectTypeLoad("PHIDGET DIGITAL OUTPUT")
        Log.Info("Checking on the PHIDGET DIGITAL OUTPUT Object Type.")
        If oType.OwnedBy = "" Then
            If gDebug Then Log.Debug("ObjectTypeUpdate(" & oType.Name & ", " & oType.Name & ", " & oType.Description & ", " & pName & ", " & oType.BaseType & ", 0, 0, 0, " & IIf(oType.HideRedundant, 1, 0) & ")")
            OSAEObjectTypeManager.ObjectTypeUpdate(oType.Name, oType.Name, oType.Description, pName, oType.BaseType, 0, 0, 0, IIf(oType.HideRedundant, 1, 0))
            Log.Info(pName & " Plugin took ownership of the PHIDGET DIGITAL OUTPUT Object Type.")
        End If
    End Sub

End Class
