<?php

//try { $testWMISupport = new COM("WbemScripting.SWbemLocator"); unset($testWMISupport); define('WMI_COM_AVAILABLE', TRUE); }
//catch (Exception $e) {}

class WindowsRegistry 
{
	private $WbemLocator;
	private $WbemServices;
	private $RegObject;

	const HKEY_CLASSES_ROOT = 0x80000000;
	const HKEY_CURRENT_USER = 0x80000001;
	const HKEY_LOCAL_MACHINE = 0x80000002;
	const HKEY_USERS = 0x80000003;
	const HKEY_CURRENT_CONFIG = 0x80000005;

	const REG_SZ = 1;
	const REG_EXPAND_SZ = 2;
	const REG_BINARY = 3;
	const REG_DWORD = 4;
	const REG_MULTI_SZ = 7;

	public function __construct()
	{
		$this->WbemLocator = new COM("WbemScripting.SWbemLocator");
		$this->WbemServices = $this->WbemLocator->ConnectServer("", "\\root\\default");
		$this->WbemServices->Security_->ImpersonationLevel = 0x3;
		$this->RegObject = $this->WbemServices->Get("StdRegProv");
	}

	public function __destruct()
	{
		unset($this->RegObject);
		unset($this->WbemServices);
		unset($this->WbemLocator);
	}

	public function ReadValue($keyPath, $valueName, $asString = TRUE)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return NULL;

		$valueType = $this->GetValueType($hKey, $subKey, $valueName);
		if ($valueType == -1)
			return NULL;

		$valueContents = new VARIANT();
		$result = -1;
		switch ($valueType)
		{
			case self::REG_SZ:
				$result = $this->RegObject->GetStringValue($hKey, $subKey, $valueName, $valueContents);
				$resultValue = strval($valueContents);

				break;
			case self::REG_EXPAND_SZ:
				$result = $this->RegObject->GetExpandedStringValue($hKey, $subKey, $valueName, $valueContents);
				$resultValue = strval($valueContents);

				break;
			case self::REG_BINARY:
				$result = $this->RegObject->GetBinaryValue($hKey, $subKey, $valueName, $valueContents);

				if ($result == 0)
				{
					if ($asString)
					{
						$resultValue = "";
						foreach ($valueContents as $value)
							$resultValue .= dechex($value) . " ";
						$resultValue = trim($resultValue);
					}
					else
					{
						$resultValue = array();
						foreach ($valueContents as $value)
							$resultValue[] = intval($value);
					}
				}

				break;
			case self::REG_DWORD:
				$result = $this->RegObject->GetDWORDValue($hKey, $subKey, $valueName, $valueContents);

				if ($result == 0)
				{
					if ($asString)
						$resultValue = strval($valueContents);
					else
						$resultValue = intval($valueContents);
				}

				break;
			case self::REG_MULTI_SZ:
				$result = $this->RegObject->GetMultiStringValue($hKey, $subKey, $valueName, $valueContents);

				if ($result == 0)
				{
					if ($asString)
					{
						$resultValue = "";
						foreach ($valueContents as $value)
							$resultValue .= $value . "\n";
						$resultValue = trim($resultValue);
					}
					else
					{
						$resultValue = array();
						foreach ($valueContents as $value)
							$resultValue[] = strval($value);
					}
				}

				break;
			default:
				$result = $this->RegObject->GetStringValue($hKey, $subKey, $valueName, $valueContents);
				$resultValue = strval($valueContents);

				break;
		}

		if ($result != 0)
			return NULL;

		return $resultValue;
	}

	public function WriteValue($keyPath, $valueName, $valueContents, $strictValueTypes = TRUE)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return FALSE;

		if (!$this->CreateKey($keyPath))
			return FALSE;

		if ($strictValueTypes)
			$valueType = $this->GetValueType($hKey, $subKey, $valueName);
		else
			$valueType =  -1;

		$result = -1;
		if (is_string($valueContents))
		{
			if (is_bool(strpos($valueContents, "%")) && ($valueType == -1 || $valueType == self::REG_SZ)) // Check for presence of %
				$result = $this->RegObject->SetStringValue($hKey, $subKey, $valueName, $valueContents);
			else if ($valueType == -1 || $valueType == self::REG_EXPAND_SZ)// If % is present in string make REG_EXPAND_SZ
				$result = $this->RegObject->SetExpandedStringValue($hKey, $subKey, $valueName, $valueContents);
		}
		else if (is_int($valueContents) && ($valueType == -1 || $valueType == self::REG_DWORD)) // Make a DWORD value of an integer is specified
			$result = $this->RegObject->SetDWORDValue($hKey, $subKey, $valueName, $valueContents);
		else if (is_array($valueContents))
		{
			if (is_string($valueContents[0]) && ($valueType == -1 || $valueType == self::REG_MULTI_SZ)) // If an array of strings is specified make a REG_MULTI_SZ
				$result = $this->RegObject->SetMultiStringValue($hKey, $subKey, $valueName, $valueContents);
			else if (is_int($valueContents[0]) && ($valueType == -1 || $valueType == self::REG_BINARY)) // If an array of integers is specified make a REG_BINARY
				$result = $this->RegObject->SetBinaryValue($hKey, $subKey, $valueName, $valueContents);
		}

		if ($result == 0)
			return TRUE;
		else
			return FALSE;
	}

	public function DeleteValue($keyPath, $valueName)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return FALSE;

		$result = -1;
		$result = $this->RegObject->DeleteValue($hKey, $subKey, $valueName);

		if ($result == 0)
			return TRUE;
		else
			return FALSE;
	}

	public function CreateKey($keyPath)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return FALSE;

		$result = -1;
		$result = $this->RegObject->CreateKey($hKey, $subKey);

		if ($result == 0)
			return TRUE;
		else
			return FALSE;
	}

	public function DeleteKey($keyPath)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return FALSE;

		$result = -1;
		$result = $this->RegObject->DeleteKey($hKey, $subKey);

		if ($result == 0)
			return TRUE;
		else
			return FALSE;
	}

	public function GetSubKeys($keyPath)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return FALSE;

		$_keyNames = new VARIANT();
		$result = -1;
		$result = $this->RegObject->EnumKey($hKey, $subKey, $_keyNames);

		if ($result != 0)
			return NULL;

		$keyNames = array();
		if (variant_get_type($_keyNames) == (VT_VARIANT | VT_ARRAY)) // Check if this is an array of variants
		{
			foreach($_keyNames as $index => $keyName)
				$keyNames[] = strval($keyName);
		}

		return $keyNames;
	}

	public function GetValueNames($keyPath, $includeTypes = FALSE)
	{
		$hKey = 0;
		$subKey = "";
		if (!$this->SplitKeyPath($keyPath, $hKey, $subKey))
			return FALSE;

		$_valueNames = new VARIANT();
		$_valueTypes = new VARIANT();
		$result = -1;
		$result = $this->RegObject->EnumValues($hKey, $subKey, $_valueNames, $_valueTypes);

		if ($result != 0)
			return NULL;

		$valueNames = array();
		if (variant_get_type($_valueNames) == (VT_VARIANT | VT_ARRAY)) // Check if this is an array of variants
		{
			if ($includeTypes)
				foreach($_valueNames as $index => $valueName)
					$valueNames[] = array(strval($valueName), intval($_valueTypes[$index]));
			else
				foreach($_valueNames as $index => $valueName)
					$valueNames[] = strval($valueName);
		}
		else
			return NULL;

		return $valueNames;
	}

	// PRIVATE FUNCTIONS
	// ================================
	private function SplitKeyPath($keyPath, &$hKeyResult, &$subKeyResult) // Split up a keypath into a HKEY and subkey
	{
		$hKeyResult = self::HKEY_LOCAL_MACHINE;
		$subKeyResult = "Software";

		$splitted = explode("\\", $keyPath, 2);

		if (is_bool($splitted))
			return FALSE;
		else if (count($splitted) == 1)
			$splitted[1] = "";

		$subKeyResult = $splitted[1];

		switch ($splitted[0])
		{
			case "HKEY_CLASSES_ROOT":
				$hKeyResult = self::HKEY_CLASSES_ROOT;
				break;
			case "HKEY_CURRENT_USER":
				$hKeyResult = self::HKEY_CURRENT_USER;
				break;
			case "HKEY_LOCAL_MACHINE":
				$hKeyResult = self::HKEY_LOCAL_MACHINE;
				break;
			case "HKEY_USERS":
				$hKeyResult = self::HKEY_USERS;
				break;
			case "HKEY_CURRENT_CONFIG":
				$hKeyResult = self::HKEY_CURRENT_CONFIG;
				break;
			default:
				unset($splitted);
				return FALSE;
		}

		unset($splitted);

		return TRUE;
	}

	private function GetValueType($hKey, $subKey, $valueName) // Find out the value type of a registry value
	{
		$valueType = -1;

		$valueNames = new VARIANT();
		$valueTypes = new VARIANT();
		$result = -1;
		$result = $this->RegObject->EnumValues($hKey, $subKey, $valueNames, $valueTypes);
		if ($result != 0)
			return $valueType;

		if (variant_get_type($valueNames) == (VT_VARIANT | VT_ARRAY)) // Check if this is an array of variants
		{
			foreach($valueNames as $index => $_valueName)
				if ($_valueName == $valueName)
					$valueType = intval($valueTypes[$index]);
		}

		return $valueType;
	}
}

?>