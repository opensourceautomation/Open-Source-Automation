Imports System.AddIn
Imports OpenSourceAutomation
<AddIn("Phidget-Servo", Version:="1.0.2")>
Public Class PhidgetServo
    Implements OSAEPluginBase
    Private OSAEApi As New OSAE("Phidget-Servo")
    Private gAppName As String = ""
    Private gAttached As Boolean
    Private gName As String = ""
    Private gSerial As String = ""
    Private gVersion As String = ""
    Private gPosition As Integer
    Private gMin(4) As Integer
    Private gMax(4) As Integer
    Private gDefaultPos(4) As Integer
    Dim WithEvents phidgetServo As Phidgets.Servo

    Public Sub RunInterface(ByVal pluginName As String) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.RunInterface
        Try
            gAppName = pluginName
            OSAEApi.AddToLog("Found my Object: " & gAppName, True)
            phidgetServo = New Phidgets.Servo
            'gPosition = Val(OSAEApi.GetObjectProperty(gAppName, "Default Position"))
            'OSAEApi.AddToLog("Initial Position: " & gPosition)
            phidgetServo.open()
        Catch ex As Exception
            OSAEApi.AddToLog("Error in InitializePlugin: " & ex.Message, True)
        End Try
    End Sub

    Private Sub phidgetServo_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetServo.Attach
        Dim bExists As Boolean
        Dim sServoName As String = ""
        Dim oObject As OSAEObject
        Try
            gAttached = "TRUE"
            OSAEApi.ObjectPropertySet(gAppName, "Attached", gAttached)
            OSAEApi.AddToLog("Attached set to: " & gAttached, True)

            gSerial = (Str(phidgetServo.SerialNumber))
            OSAEApi.ObjectPropertySet(gAppName, "Serial", gSerial)
            OSAEApi.AddToLog("Serial set to: " & gSerial, True)

            Dim i As Integer
            For i = 0 To sender.servos.Count - 1

                bExists = OSAEApi.ObjectExists(gSerial & "-" & i)
                If bExists = False Then
                    OSAEApi.ObjectAdd("New Servo - " & gSerial & "-" & i, "New Servo - " & gSerial & "-" & i, "PHIDGET SERVO", gSerial & "-" & i, "", True)
                End If
                Try
                    oObject = OSAEApi.GetObjectByAddress(gSerial & "-" & i)
                    sServoName = oObject.Name.ToString()
                    OSAEApi.AddToLog("-- Configuring Servo " & i & ": " & sServoName, True)
                    gMin(i) = Val(OSAEApi.GetObjectPropertyValue(sServoName, "Min Position").Value)
                    OSAEApi.AddToLog("-- Minimum Position: " & gMin(i), True)
                    gMax(i) = Val(OSAEApi.GetObjectPropertyValue(sServoName, "Max Position").Value)
                    OSAEApi.AddToLog("-- Maximum Position: " & gMax(i), True)
                    gDefaultPos(i) = Val(OSAEApi.GetObjectPropertyValue(sServoName, "Default Position").Value)
                    'OSAEApi.AddToLog("-- Default Position: " & gDefaultPos(i), True)
                    'phidgetServo.servos(i).Position = gDefaultPos(i)
                Catch ex As Exception
                    OSAEApi.AddToLog("Error in phidgetServo_Attach Get Object: " & ex.Message, True)
                End Try

                ' If oObject = Nothing Then
                ' End If
                'servoNumCombo.Items.Add(i)
                'phidgetServo.servos(i).Position = 90
            Next i
            ' gMin = phidgetServo.servos(0).PositionMin + 1
            'OSAEApi.AddToLog("Minimum Position: " & gMin)
            'gMax = phidgetServo.servos(0).PositionMax
            'OSAEApi.ObjectPropertySet(gAppName, "Max Position", gMax)
            'OSAEApi.AddToLog("Maximum Position: " & gMax)
        Catch ex As Exception
            OSAEApi.AddToLog("Error in phidgetServo_Attach: " & ex.Message, True)
        End Try
    End Sub

    Private Sub phidgetServo_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetServo.Detach
        gAttached = "FALSE"
        OSAEApi.ObjectPropertySet(gAppName, "Attached", gAttached)
        OSAEApi.AddToLog("Attached set to: " & gAttached, True)
    End Sub

    Private Sub phidgetServo_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetServo.Error
        OSAEApi.AddToLog("phidgetServo_Error: " & e.Description, True)
    End Sub

    Public Sub ProcessCommand(ByVal table As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        Dim iLevel As Integer, sAddress As String
        Dim row As System.Data.DataRow
        For Each row In table.Rows
            Try
                sAddress = row("address").ToString.Replace(gSerial & "-", "")
                If row("method_name").ToString() = "POSITION" Then
                    If Val(row("parameter_1").ToString) >= gMax(sAddress) Then
                        iLevel = gMax(sAddress)
                    ElseIf Val(row("parameter_1").ToString) <= gMin(sAddress) Then
                        iLevel = gMin(sAddress)
                    Else
                        iLevel = Val(row("parameter_1").ToString)
                    End If
                    'phidgetServo.servos(sAddress).Engaged = True
                    phidgetServo.servos(sAddress).Position = iLevel
                    OSAEApi.ObjectPropertySet(row("object_name").ToString, "Position", iLevel.ToString)
                    OSAEApi.AddToLog("Servo " & sAddress & "(" & row("object_name").ToString & ") set to Position: " & iLevel.ToString & " (" & gMin(sAddress) & "-" & gMax(sAddress) & ")", True)
                ElseIf row("method_name").ToString() = "ENGAGE" Then
                    phidgetServo.servos(sAddress).Engaged = True
                    OSAEApi.AddToLog("Servo " & sAddress & "(" & row("object_name").ToString & ") Engaged", True)
                ElseIf row("method_name").ToString() = "DISENGAGE" Then
                    phidgetServo.servos(sAddress).Engaged = False
                    OSAEApi.AddToLog("Servo " & sAddress & "(" & row("object_name").ToString & ") Disengaged", True)
                End If
            Catch ex As Exception
                OSAEApi.AddToLog("Error in ProcessCommand: " & ex.Message, True)
            End Try
        Next
    End Sub



    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        If phidgetServo.Attached Then phidgetServo.close()
    End Sub

End Class
