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
    Private Shared logging As Logging = logging.GetLogger("Script Processor")

    Private Sub phidgetRFID_Attach(ByVal sender As Object, ByVal e As Phidgets.Events.AttachEventArgs) Handles phidgetRFID.Attach
        gAttached = sender.Attached.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        logging.AddToLog("RFID Controller Attached = " & gAttached, True)

        gName = sender.Name
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Name", gName, pName)
        logging.AddToLog("RFID Controller Nam = " & gName, True)

        gSerial = sender.SerialNumber.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Serial", gSerial, pName)
        logging.AddToLog("Serial Number = " & gSerial, True)

        gVersion = sender.Version.ToString()
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Version", gVersion, pName)
        logging.AddToLog("Version Number = " & gVersion, True)
        ' outputsTxt.Text = sender.outputs.Count.ToString()
        ' antennaChk.Checked = True
        'phidgetRFID.Antenna = True
    End Sub

    Private Sub phidgetRFID_Detach(ByVal sender As Object, ByVal e As Phidgets.Events.DetachEventArgs) Handles phidgetRFID.Detach
        gAttached = sender.Attached.ToString
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Attached", gAttached, pName)
        logging.AddToLog("RFID Controller Attached = " & gAttached, True)
        gName = sender.Name
    End Sub

    Private Sub phidgetRFID_Error(ByVal sender As Object, ByVal e As Phidgets.Events.ErrorEventArgs) Handles phidgetRFID.Error
        logging.AddToLog("phidgetRFID_Error: " & e.Description, True)
    End Sub

    Private Sub phidgetRFID_Tag(ByVal sender As Object, ByVal e As Phidgets.Events.TagEventArgs) Handles phidgetRFID.Tag
        Dim oObject As OSAEObject
        OSAEObjectPropertyManager.ObjectPropertySet(pName, "Last Tag Read", e.Tag, pName)
        logging.AddToLog("Read Tag = " & e.Tag, True)
        logging.AddToLog("GetObjectByAddress: " & e.Tag, True)
        Try
            oObject = OSAEObjectManager.GetObjectByAddress(e.Tag)
            If IsNothing(oObject) Then
                logging.AddToLog("Adding new RFID Tag: " & e.Tag, True)
                OSAEObjectManager.ObjectAdd("RFID-" & e.Tag, "Unknown RFID Tag", "PHIDGET RFID TAG", e.Tag, "", True)
            End If
            oObject = OSAEObjectManager.GetObjectByAddress(e.Tag)
            OSAEObjectStateManager.ObjectStateSet(oObject.Name, "ON", pName)
            logging.AddToLog("Detected: " & oObject.Name, True)
        Catch ex As Exception
            logging.AddToLog("Object Not Found for: " & e.Tag, True)
            logging.AddToLog("Error Msg: " & ex.Message, True)
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
            logging.AddToLog("Found my Object: " & pName, True)
            gAntenna = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Antenna Enabled").Value)
            logging.AddToLog("Antenna Enabled = " & gAntenna, True)
            gLED = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "LED Enabled").Value)
            logging.AddToLog("LED Enabled = " & gLED, True)
            gOutput1 = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Output 1 ON").Value)
            logging.AddToLog("Output 1 ON = " & gOutput1, True)
            gOutput2 = Val(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Output 2 ON").Value)
            logging.AddToLog("Output 2 ON = " & gOutput2, True)
            phidgetRFID = New Phidgets.RFID
            phidgetRFID.open()
        Catch ex As Exception
            logging.AddToLog("Error in InitializePlugin: " & ex.Message, True)
        End Try
    End Sub

    Public Overrides Sub Shutdown()
        phidgetRFID.close()
    End Sub
End Class
