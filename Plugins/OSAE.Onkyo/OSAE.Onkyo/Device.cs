using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;


namespace OSAE.Onkyo
{
	public class Device
	{
		string _OriginalOnkyoOutput;
		string _ip;
		int _port;
		int _deviceType;
		string _modelName;
		string _region;
		string _mac;
		Boolean _loaded;

		public Device()
		{
			Reset();
		}

		public void Reset()
		{
			// Load defaults
			_loaded = false;
			_ip = "0.0.0.0";
			_port = 0;
			_deviceType = 0;
			_modelName = "VOID";
			_region = "VOID";
			_mac = "0000000000000000";
		}

		// --------------------------------------------------------------------------------- //
		// Example Onkyo TX-NR609 AVR: ISCP&!1ECNTX-NR609/60128/XX/0009B0C36316/192.168.1.36 //
		// --------------------------------------------------------------------------------- //
		public void LoadDevice(string strOnkyo)
		{
			_OriginalOnkyoOutput = strOnkyo;
			// Load the Onkyo search reply string to oblect
			if (!ISCPValidate(strOnkyo)) { throw new InvalidOnkyoStringException(); }
			if (!LoadVars(strOnkyo)) { throw new ParseVarsException(); }
			
			_loaded = true;
		}

		private Boolean ISCPValidate(string sOnkyo)
		{
			Boolean retBool = true;
			try 
			{
				if (sOnkyo.Substring(0, 4).ToUpper() != "ISCP") { throw new Exception(); }
			}   
			catch (Exception)
			{		
				retBool = false;
			}
			return retBool;
		}

		// --------------------------------------------------------------------------------- //
		// Example Onkyo TX-NR609 AVR: ISCP&!1ECNTX-NR609/60128/XX/0009B0C36316/192.168.1.36 //
		// -------------------------------------------------------------------------------- //
		private Boolean LoadVars(string newOnkyoStr)
		{
			Boolean retBool = true;
			try
			{
				newOnkyoStr = newOnkyoStr.Substring(6); // Drop ISCP&!
				_deviceType = int.Parse( newOnkyoStr.Substring(0, 1));
				newOnkyoStr = newOnkyoStr.Substring(4); // Drop 1ECN
				string[] tmp = newOnkyoStr.Split(new string[] { "/" }, StringSplitOptions.None);
				_modelName = tmp[0];
				_port = int.Parse( tmp[1]);
				_region = tmp[2];
				_mac  = tmp[3];
				_ip = tmp[4];
 
			}
			catch (Exception)
			{
				retBool = false;
			}
			return retBool;
		}

		public Boolean Loaded
		{
			get { return _loaded; }
		}

		public string IP
		{
			get { return _ip; }
		}

		public int Port
		{
			get { return _port; }
		}
		
		public int DeviceType
		{
			get { return _deviceType; }
		}
		
		public string ModelName
		{
			get { return _modelName; }
		}
		
		public string Region
		{
			get { return _region; }
		}

		public string Mac
		{
			get { return _mac; }
		}

		public string OriginalOnkyoOutput
		{
			get { return _OriginalOnkyoOutput; }
		}
	}
}
