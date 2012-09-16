Imports System.AddIn
Imports OpenSourceAutomation
<AddIn("Phidget-RFID", Version:="0.3.0")>
Public Class PhidgetRFID
    Implements IOpenSourceAutomationAddIn
    Private OSAEApi As New OSAE("Phidget-RFID")
    Private WithEvents phidgetRFID As Phidgets.RFID
    Private gAppName As String = ""
    Private gAttached As Boolean
    Private gName As String = ""
    Private gSerial As String = ""
    Private gVersion As String = ""
    Private gAntenna As Boolean
    Private gLED As Boolean
    Private gOutput1 As Boolean
    Private gOutput2 As Boolean

    Private Sub phidgetRFID_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetRFID.Attach
        gAttached = sender.Attached.ToString
        OSAEApi.ObjectPropertySet(gAppName, "Attached", gAttached)
        OSAEApi.AddToLog("RFID Controller Attached = " & gAttached, True)

        gName = sender.Name
        OSAEApi.ObjectPropertySet(gAppName, "Name", gName)
        OSAEApi.AddToLog("RFID Controller Nam = " & gName, True)

        gSerial = sender.SerialNumber.ToString
        OSAEApi.ObjectPropertySet(gAppName, "Serial", gSerial)
        OSAEApi.AddToLog("Serial Number = " & gSerial, True)

        gVersion = sender.Version.ToString()
        OSAEApi.ObjectPropertySet(gAppName, "Version", gVersion)
        OSAEApi.AddToLog("Version Number = " & gVersion, True)
        ' outputsTxt.Text = sender.outputs.Count.ToString()
        ' antennaChk.Checked = True
        'phidgetRFID.Antenna = True
    End Sub

    Private Sub phidgetRFID_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetRFID.Detach
        gAttached = sender.Attached.ToString
        OSAEApi.ObjectPropertySet(gAppName, "Attached", gAttached)
        OSAEApi.AddToLog("RFID Controller Attached = " & gAttached, True)
        gName = sender.Name
    End Sub

    Private Sub phidgetRFID_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetRFID.Error
        OSAEApi.AddToLog("phidgetRFID_Error: " & e.Description, True)
    End Sub

    Private Sub phidgetRFID_Tag(ByVal sender As Object, ByVal e As Phidgets.Events.TagEventArgs) Handles phidgetRFID.Tag
        Dim oObject As OSAEObject
        OSAEApi.ObjectPropertySet(gAppName, "Last Tag Read", e.Tag)
        OSAEApi.AddToLog("Read Tag = " & e.Tag, True)
        OSAEApi.AddToLog("GetObjectByAddress: " & e.Tag, True)
        Try
            oObject = OSAEApi.GetObjectByAddress(e.Tag)
            If IsNothing(oObject) Then
                OSAEApi.AddToLog("Adding new RFID Tag: " & e.Tag, True)
                OSAEApi.ObjectAdd("RFID-" & e.Tag, "Unknown RFID Tag", "PHIDGET RFID TAG", e.Tag, "", True)
            End If
            oObject = OSAEApi.GetObjectByAddress(e.Tag)
            OSAEApi.ObjectStateSet(oObject.Name, "ON")
            OSAEApi.AddToLog("Detected: " & oObject.Name, True)
        Catch ex As Exception
            OSAEApi.AddToLog("Object Not Found for: " & e.Tag, True)
            OSAEApi.AddToLog("Error Msg: " & ex.Message, True)
        End Try
    End Sub

    'Private Sub phidgetRFID_TagLost(ByVal sender As Object, ByVal e As Phidgets.Events.TagEventArgs) Handles phidgetRFID.TagLost
    '    tagTxt.Text = ""
    'End Sub

    ''Enable or disable the RFID antenna by clicking the checkbox
    'Private Sub antennaChk_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles antennaChk.CheckedChanged
    '    phidgetRFID.Antenna = antennaChk.Checked
    'End Sub

    ''turn on and off the onboard LED by clicking the checkox
    'Private Sub ledChk_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ledChk.CheckedChanged
    '    phidgetRFID.LED = ledChk.Checked
    'End Sub

    ''turn on and off output 0, to light a LED for example
    'Private Sub output0Chk_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles output0Chk.CheckedChanged
    '    phidgetRFID.outputs(0) = output0Chk.Checked
    'End Sub

    ''turn on and off output 1, to light a LED for example
    'Private Sub output1chk_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles output1chk.CheckedChanged
    '    phidgetRFID.outputs(1) = output1chk.Checked
    'End Sub

    'When the application is being terminated, close the Phidget.

    Public Sub ProcessCommand(ByVal table As System.Data.DataTable) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.ProcessCommand
        'Dim sAddress As String, iState As Integer
        'Try
        '    sAddress = row("address").ToString.Replace((gSerial & "-DO"), "")
        '    If row("method_name").ToString = "ON" Then iState = 1
        '    If row("method_name").ToString = "OFF" Then iState = 0
        '    phidgetIFK.outputs(sAddress) = iState
        '    OSAEApi.ObjectStateSet(row("object_name").ToString, row("method_name").ToString)
        '    OSAEApi.AddToLog("Executed " & row("object_name").ToString & " " & row("method_name").ToString & "(" & row("address").ToString & " " & row("method_name").ToString & ")")
        'Catch ex As Exception
        '    OSAEApi.AddToLog("Error ProcessCommand - " & ex.Message)
        'End Try
    End Sub

    Public Sub RunInterface(ByVal pluginName As String) Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.RunInterface
        Try
            gAppName = pluginName
            OSAEApi.AddToLog("Found my Object: " & gAppName, True)
            gAntenna = Val(OSAEApi.GetObjectPropertyValue(gAppName, "Antenna Enabled").Value)
            OSAEApi.AddToLog("Antenna Enabled = " & gAntenna, True)
            gLED = Val(OSAEApi.GetObjectPropertyValue(gAppName, "LED Enabled").Value)
            OSAEApi.AddToLog("LED Enabled = " & gLED, True)
            gOutput1 = Val(OSAEApi.GetObjectPropertyValue(gAppName, "Output 1 ON").Value)
            OSAEApi.AddToLog("Output 1 ON = " & gOutput1, True)
            gOutput2 = Val(OSAEApi.GetObjectPropertyValue(gAppName, "Output 2 ON").Value)
            OSAEApi.AddToLog("Output 2 ON = " & gOutput2, True)
            phidgetRFID = New Phidgets.RFID
            phidgetRFID.open()
        Catch ex As Exception
            OSAEApi.AddToLog("Error in InitializePlugin: " & ex.Message, True)
        End Try
    End Sub

    Public Sub Shutdown() Implements OpenSourceAutomation.IOpenSourceAutomationAddIn.Shutdown
        phidgetRFID.close()
    End Sub
End Class
