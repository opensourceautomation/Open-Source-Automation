Imports OSAE

Public Class PhidgetServo
    Inherits OSAEPluginBase
    Private Shared logging As Logging = logging.GetLogger("Phidget-Servo")
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
            logging.AddToLog("Found my Object: " & pName, True)
            phidgetServo = New Phidgets.Servo
            'gPosition = Val(OSAEApi.GetObjectProperty(gAppName, "Default Position"))
            'logging.AddToLog("Initial Position: " & gPosition)
            phidgetServo.open()
        Catch ex As Exception
            logging.AddToLog("Error in InitializePlugin: " & ex.Message, True)
        End Try
    End Sub

    Private Sub phidgetServo_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetServo.Attach
        Dim bExists As Boolean
        Dim sServoName As String = ""
        Dim oObject As OSAEObject
        Try
            gAttached = "TRUE"
            OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
            Logging.AddToLog("Attached set to: " & gAttached, True)

            gSerial = (Str(phidgetServo.SerialNumber))
            OSAEObjectPropertyManager.ObjectPropertySet(pName, "Serial", gSerial, pName)
            Logging.AddToLog("Serial set to: " & gSerial, True)

            Dim i As Integer
            For i = 0 To sender.servos.Count - 1

                bExists = OSAEObjectManager.ObjectExists(gSerial & "-" & i)
                If bExists = False Then
                    OSAEObjectManager.ObjectAdd("New Servo - " & gSerial & "-" & i, "New Servo - " & gSerial & "-" & i, "PHIDGET SERVO", gSerial & "-" & i, "", True)
                End If
                Try
                    oObject = OSAEObjectManager.GetObjectByAddress(gSerial & "-" & i)
                    sServoName = oObject.Name.ToString()
                    Logging.AddToLog("-- Configuring Servo " & i & ": " & sServoName, True)
                    gMin(i) = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(sServoName, "Min Position").Value)
                    logging.AddToLog("-- Minimum Position: " & gMin(i), True)
                    gMax(i) = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(sServoName, "Max Position").Value)
                    logging.AddToLog("-- Maximum Position: " & gMax(i), True)
                    gDefaultPos(i) = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(sServoName, "Default Position").Value)
                    'logging.AddToLog("-- Default Position: " & gDefaultPos(i), True)
                    'phidgetServo.servos(i).Position = gDefaultPos(i)
                Catch ex As Exception
                    Logging.AddToLog("Error in phidgetServo_Attach Get Object: " & ex.Message, True)
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
            Logging.AddToLog("Error in phidgetServo_Attach: " & ex.Message, True)
        End Try
    End Sub

    Private Sub phidgetServo_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetServo.Detach
        gAttached = "FALSE"
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        Logging.AddToLog("Attached set to: " & gAttached, True)
    End Sub

    Private Sub phidgetServo_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetServo.Error
        Logging.AddToLog("phidgetServo_Error: " & e.Description, True)
    End Sub

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        Dim iLevel As Integer, sAddress As String
        Try
            sAddress = method.Address.Replace(gSerial & "-", "")
            If method.MethodName = "POSITION" Then
                If Val(method.Parameter1) >= gMax(sAddress) Then
                    iLevel = gMax(sAddress)
                ElseIf Val(method.Parameter1) <= gMin(sAddress) Then
                    iLevel = gMin(sAddress)
                Else
                    iLevel = Val(method.Parameter1)
                End If
                'phidgetServo.servos(sAddress).Engaged = True
                phidgetServo.servos(sAddress).Position = iLevel
                OSAEObjectPropertyManager.ObjectPropertySet(method.ObjectName, "Position", iLevel.ToString, pName)
                logging.AddToLog("Servo " & sAddress & "(" & method.ObjectName & ") set to Position: " & iLevel.ToString & " (" & gMin(sAddress) & "-" & gMax(sAddress) & ")", True)
            ElseIf method.MethodName = "ENGAGE" Then
                phidgetServo.servos(sAddress).Engaged = True
                logging.AddToLog("Servo " & sAddress & "(" & method.ObjectName & ") Engaged", True)
            ElseIf method.MethodName = "DISENGAGE" Then
                phidgetServo.servos(sAddress).Engaged = False
                logging.AddToLog("Servo " & sAddress & "(" & method.ObjectName & ") Disengaged", True)
            End If
        Catch ex As Exception
            logging.AddToLog("Error in ProcessCommand: " & ex.Message, True)
        End Try
    End Sub



    Public Overrides Sub Shutdown()
        If phidgetServo.Attached Then phidgetServo.close()
    End Sub

End Class
