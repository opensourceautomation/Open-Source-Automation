Imports OSAE

Public Class PhidgetServo
    Inherits OSAEPluginBase
    Private Log As OSAE.General.OSAELog
    Private pName As String = ""
    Private gAttached As Boolean
    Private gName As String = ""
    Private gSerial As String = ""
    Private gVersion As String = ""
    Private gPosition As Integer
    Private gMin(4) As Integer
    Private gMax(4) As Integer
    Private gDefaultPos(4) As Integer
    Dim WithEvents phidgetServo As Phidgets.Servo

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            pName = pluginName
            Log = New General.OSAELog(pName)
            Log.Info("Found my Object: " & pName)
            phidgetServo = New Phidgets.Servo
            'gPosition = Val(OSAEApi.GetObjectProperty(gAppName, "Default Position"))
            'logging.AddToLog("Initial Position: " & gPosition)
            phidgetServo.open()
        Catch ex As Exception
            Log.Error("Error in InitializePlugin: " & ex.Message)
        End Try
    End Sub

    Private Sub phidgetServo_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetServo.Attach
        Dim bExists As Boolean
        Dim sServoName As String = ""
        Dim oObject As OSAEObject
        Try
            gAttached = "TRUE"
            OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
            Log.Info("Attached set to: " & gAttached)

            gSerial = (Str(phidgetServo.SerialNumber))
            OSAEObjectPropertyManager.ObjectPropertySet(pName, "Serial", gSerial, pName)
            Log.Info("Serial set to: " & gSerial)

            Dim i As Integer
            For i = 0 To sender.servos.Count - 1

                bExists = OSAEObjectManager.ObjectExists(gSerial & "-" & i)
                If bExists = False Then
                    OSAEObjectManager.ObjectAdd("New Servo - " & gSerial & "-" & i, "", "New Servo - " & gSerial & "-" & i, "PHIDGET SERVO", gSerial & "-" & i, "", 30, True)
                End If
                Try
                    oObject = OSAEObjectManager.GetObjectByAddress(gSerial & "-" & i)
                    sServoName = oObject.Name.ToString()
                    Log.Info("-- Configuring Servo " & i & ": " & sServoName)
                    gMin(i) = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(sServoName, "Min Position").Value)
                    Log.Info("-- Minimum Position: " & gMin(i))
                    gMax(i) = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(sServoName, "Max Position").Value)
                    Log.Info("-- Maximum Position: " & gMax(i))
                    gDefaultPos(i) = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(sServoName, "Default Position").Value)
                    'logging.AddToLog("-- Default Position: " & gDefaultPos(i), True)
                    'phidgetServo.servos(i).Position = gDefaultPos(i)
                Catch ex As Exception
                    Log.Error("Error in phidgetServo_Attach Get Object: " & ex.Message)
                End Try

                ' If oObject = Nothing Then
                ' End If
                'servoNumCombo.Items.Add(i)
                'phidgetServo.servos(i).Position = 90
            Next i
            ' gMin = phidgetServo.servos(0).PositionMin + 1
            'logging.AddToLog("Minimum Position: " & gMin)
            'gMax = phidgetServo.servos(0).PositionMax
            'OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Max Position", gMax)
            'logging.AddToLog("Maximum Position: " & gMax)
        Catch ex As Exception
            Log.Error("Error in phidgetServo_Attach: " & ex.Message)
        End Try
    End Sub

    Private Sub phidgetServo_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetServo.Detach
        gAttached = "FALSE"
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        Log.Info("Attached set to: " & gAttached)
    End Sub

    Private Sub phidgetServo_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetServo.Error
        Log.Info("phidgetServo_Error: " & e.Description)
    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim sAddress As String, iCurrentPosition As Integer, iNewPosition As Integer
        Try
            sAddress = method.Address.Replace(gSerial & "-", "")
            iCurrentPosition = phidgetServo.servos(sAddress).Position
            If method.MethodName = "POSITION" Then
                'Determine if it is Relitive Positioning and if so, calculate the new position
                If method.Parameter1.StartsWith("-") Or method.Parameter1.StartsWith("+") Then
                    iNewPosition = iCurrentPosition + Val(method.Parameter1)
                Else
                    iNewPosition = Val(method.Parameter1)
                End If
                'Now check to see if the New Position is within the Min/Max range
                If iNewPosition >= gMax(sAddress) Then
                    iNewPosition = gMax(sAddress)
                ElseIf iNewPosition <= gMin(sAddress) Then
                    iNewPosition = gMin(sAddress)
                End If

                'phidgetServo.servos(sAddress).Engaged = True
                phidgetServo.servos(sAddress).Position = iNewPosition
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Position", iNewPosition.ToString(), pName)
                Log.Info("Servo " & sAddress & " (" & method.ObjectName & ") set to Position: " & iNewPosition.ToString() & " (" & gMin(sAddress) & "-" & gMax(sAddress) & ")")
            ElseIf method.MethodName = "ENGAGE" Then
                phidgetServo.servos(sAddress).Engaged = True
                Log.Info("Servo " & sAddress & " (" & method.ObjectName & ") Engaged")
            ElseIf method.MethodName = "DISENGAGE" Then
                phidgetServo.servos(sAddress).Engaged = False
                Log.Info("Servo " & sAddress & " (" & method.ObjectName & ") Disengaged")
            End If
        Catch ex As Exception
            Log.Error("Error in ProcessCommand: " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        If phidgetServo.Attached Then phidgetServo.close()
    End Sub

End Class
