Public Class PhidgetRFID
    Inherits OSAEPluginBase
    Private WithEvents phidgetRFID As Phidgets.RFID
    Private pName As String = ""
    Private gAttached As Boolean
    Private gName As String = ""
    Private gSerial As String = ""
    Private gVersion As String = ""
    Private gAntenna As Boolean
    Private gLED As Boolean
    Private gOutput1 As Boolean
    Private gOutput2 As Boolean
    Private Log As OSAE.General.OSAELog

    Private Sub phidgetRFID_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetRFID.Attach
        gAttached = sender.Attached.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        Log.Info("RFID Controller Attached = " & gAttached)

        gName = sender.Name
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Name", gName, pName)
        Log.Info("RFID Controller Nam = " & gName)

        gSerial = sender.SerialNumber.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Serial", gSerial, pName)
        Log.Info("Serial Number = " & gSerial)

        gVersion = sender.Version.ToString()
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Version", gVersion, pName)
        Log.Info("Version Number = " & gVersion)
        ' outputsTxt.Text = sender.outputs.Count.ToString()
        ' antennaChk.Checked = True
        'phidgetRFID.Antenna = True
    End Sub

    Private Sub phidgetRFID_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetRFID.Detach
        gAttached = sender.Attached.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        Log.Info("RFID Controller Attached = " & gAttached)
        gName = sender.Name
    End Sub

    Private Sub phidgetRFID_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetRFID.Error
        Log.Error("phidgetRFID_Error: " & e.Description)
    End Sub

    Private Sub phidgetRFID_Tag(ByVal sender As Object, ByVal e As Phidgets.Events.TagEventArgs) Handles phidgetRFID.Tag
        Dim oObject As OSAEObject
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Last Tag Read", e.Tag, pName)
        Log.Info("Read Tag = " & e.Tag)
        Log.Debug("GetObjectByAddress: " & e.Tag)
        Try
            oObject = OSAEObjectManager.GetObjectByAddress(e.Tag)
            If IsNothing(oObject) Then
                Log.Info("Adding new RFID Tag: " & e.Tag)
                OSAEObjectManager.ObjectAdd("RFID-" & e.Tag, "RFID-" & e.Tag, "Unknown RFID Tag", "PHIDGET RFID TAG", e.Tag, "", 30, True)
            End If
            oObject = OSAEObjectManager.GetObjectByAddress(e.Tag)
            OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", pName)
            Log.Info("Detected: " & oObject.Name)
        Catch ex As Exception
            Log.Error("Object Not Found for: " & e.Tag)
            Log.Error("Error Msg: " & ex.Message)
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

    Public Overrides Sub ProcessCommand(ByVal method As OSAEMethod)
        'Dim sAddress As String, iState As Integer
        'Try
        '    sAddress = row("address").ToString.Replace((gSerial & "-DO"), "")
        '    If row("method_name").ToString = "ON" Then iState = 1
        '    If row("method_name").ToString = "OFF" Then iState = 0
        '    phidgetIFK.outputs(sAddress) = iState
        '    OSAEObjectStateManager.ObjectStateSet(row("object_name").ToString, row("method_name").ToString)
        '    logging.AddToLog("Executed " & row("object_name").ToString & " " & row("method_name").ToString & "(" & row("address").ToString & " " & row("method_name").ToString & ")")
        'Catch ex As Exception
        '    logging.AddToLog("Error ProcessCommand - " & ex.Message)
        'End Try
    End Sub

    Public Overrides Sub RunInterface(ByVal pluginName As String)
        Try
            pName = pluginName
            Log = New General.OSAELog(pName)
            gAntenna = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Antenna Enabled").Value)
            Log.Info("Antenna Enabled = " & gAntenna)
            gLED = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "LED Enabled").Value)
            Log.Info("LED Enabled = " & gLED)
            gOutput1 = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Output 1 ON").Value)
            Log.Info("Output 1 ON = " & gOutput1)
            gOutput2 = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Output 2 ON").Value)
            Log.Info("Output 2 ON = " & gOutput2)
            phidgetRFID = New Phidgets.RFID
            phidgetRFID.open()
        Catch ex As Exception
            Log.Error("Error in InitializePlugin: " & ex.Message)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        phidgetRFID.close()
    End Sub
End Class
