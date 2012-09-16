Option Strict Off
Option Explicit On
Module modW800RF
    Public gHouseCodes(25) As String
    Public gFunctions(25) As String

	Structure Device
		Dim Device_Type As String
		Dim House_Code As String
		Dim Device_Code As String
		Dim Current_Command As String
    End Structure

    Structure ByteDetails
        Dim DecimalValue As Short
        Dim HexValue As String
        Dim BinaryValue As String
    End Structure

	Public gDevice As Device
    Public gLastDevice As Device

	Public Enum FunctionCodes
		X10_AllUnitsOff = 0
		X10_AllLightsOn
		X10_On
		X10_Off
		X10_Dim
		X10_Bright
		X10_AllLightsOff
		X10_ExtendedCode
		X10_HailRequest
		X10_HailAcknowledge
		X10_PresetDim1
		X10_PresetDim2
		X10_ExtendedDataTransfer
		X10_StatusOn
		X10_StatusOff
		X10_StatusRequest
    End Enum

	Public Enum DS10Func
		X10_Security_Alert
		X10_Security_Normal
		X10_Security_Unknown
    End Enum

	Public Enum KF574Func
		X10_Security_ArmAway
		X10_Security_Disarm
		X10_Security_Light_On
		X10_Security_Light_Off
    End Enum

	Public strActive_Code As String

	Public Sub Set_Codes()
		gHouseCodes(0) = "M"
		gHouseCodes(1) = "E"
		gHouseCodes(2) = "C"
		gHouseCodes(3) = "K"
		gHouseCodes(4) = "O"
		gHouseCodes(5) = "G"
		gHouseCodes(6) = "A"
		gHouseCodes(7) = "I"
		gHouseCodes(8) = "N"
		gHouseCodes(9) = "F"
		gHouseCodes(10) = "D"
		gHouseCodes(11) = "L"
		gHouseCodes(12) = "P"
		gHouseCodes(13) = "H"
		gHouseCodes(14) = "B"
		gHouseCodes(15) = "J"
    End Sub
End Module