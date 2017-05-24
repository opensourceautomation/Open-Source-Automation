/* Plugin to interface J-Works USB products to Open Source Automation.
 * Currently handles JSB38x, JSB34x, JSB39X and any others using the same DLLs
 * http://www.j-works.com
 * http://www.opensourceautomation.com
 * 
 * Jim Kearney (jkearneyma@gmail.com), 2012
 */
namespace OSAE.JWorks
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Timers;

    public class jworks : OSAEPluginBase
    {
        // Sadly, we have to use classic Interop to access the J-Works DLLs.  They have managed DLLs, but they are
        // mixed-mode .Net 2.0 that won't load in OSA, even with the activation policy hack.
        #region JSB34x
        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xFlashLed")]
        public static extern void Jsb34xFlashLed([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xNumberOfModules")]
        public static extern short Jsb34xNumberOfModules();

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xSerialNumber")]
        public static extern void Jsb34xSerialNumber(int nModule, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int iSize);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xDllVersion")]
        public static extern void Jsb34xDllVersion([MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int iSize);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xDriverVersion")]
        public static extern void Jsb34xDriverVersion([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int iSize);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xFirmwareVersion")]
        public static extern void Jsb34xFirmwareVersion([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int iSize);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xMaxInputs")]
        public static extern byte Jsb34xMaxInputs([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xMaxRelays")]
        public static extern byte Jsb34xMaxRelays([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xOpenRelay")]
        public static extern void Jsb34xOpenRelay([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule, byte ucRelayNumber);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xCloseRelay")]
        public static extern void Jsb34xCloseRelay([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule, byte ucRelayNumber);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xIsRelayClosed")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool Jsb34xIsRelayClosed([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule, byte ucRelayNumber);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xInput")]
        public static extern byte Jsb34xInput([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xCloseAllRelays")]
        public static extern void Jsb34xCloseAllRelays([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xOpenAllRelays")]
        public static extern void Jsb34xOpenAllRelays([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule);

        [DllImportAttribute("Jsb34x.dll", EntryPoint = "Jsb34xIsInputOn")]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        public static extern bool Jsb34xIsInputOn([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strModule, byte ucInputNumber);
        #endregion

        #region JSB383 / JSB39x
        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383NumberOfModules")]
        public static extern short Jsb383NumberOfModules();

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383DllVersion")]
        public static extern void Jsb383DllVersion([MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int nSize);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383DriverVersion")]
        public static extern void Jsb383DriverVersion([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strSerialNum, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int nSize);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383FirmwareVersion")]
        public static extern void Jsb383FirmwareVersion([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strSerialNum, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int nSize);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383SerialNumber")]
        public static extern void Jsb383SerialNumber(int nModuleNumber, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder strReturn, int nSize);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383FlashLed")]
        public static extern void Jsb383FlashLed([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strSerialNum);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383MaxInputs")]
        public static extern byte Jsb383MaxInputs([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strSerialNumber);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383IsInputOn")]
        [return: MarshalAsAttribute(UnmanagedType.I1)]
        public static extern bool Jsb383IsInputOn([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strSerialNum, byte ucInput);

        [DllImportAttribute("Jsb383.dll", EntryPoint = "Jsb383Input")]
        public static extern ushort Jsb383Input([InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string strSerialNum);
        #endregion

        #region implementation variables
        //Logging logging = Logging.GetLogger("JWorks");
        private OSAE.General.OSAELog Log;
        private string pName;
        enum Direction { Input, Output };
        private List<string> JSB383s = new List<string>(), JSB34Xs = new List<string>();
        private List<ushort> JSB383State = new List<ushort>(), JSB34XState = new List<ushort>();
        private Timer timer = new Timer();
        #endregion

        #region properties
        UInt32 pollInterval = 250; // 4x a second
        #endregion

        private void AddIO(string address, Direction dir, string serial, byte id)
        {
            if (OSAEObjectManager.GetObjectByAddress(address) == null)
            {
                if (dir == Direction.Input)
                    OSAEObjectManager.ObjectAdd("J-Works-I" + address,"", "J -Works input", "JWORKS INPUT", address, "", 30, true);
                else
                    OSAEObjectManager.ObjectAdd("J-Works-0" + address,"", "J-Works output", "JWORKS OUTPUT", address, "", 30, true);

                OSAEObjectPropertyManager.ObjectPropertySet(address, "Serial", serial, pName);
                OSAEObjectPropertyManager.ObjectPropertySet(address, "Id", Convert.ToString(id),pName);
                OSAEObjectPropertyManager.ObjectPropertySet(address, "Invert", "FALSE", pName);
            }

            OSAEObject obj = OSAEObjectManager.GetObjectByAddress(address);
            bool invert = (OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Invert").Value == "TRUE");
            // initial state is 0; interpret that in light of Invert setting
            OSAEObjectStateManager.ObjectStateSet(obj.Name, invert ? "ON" : "OFF", pName);
        }

        private void StateChange(string serial, Direction dir, uint n, int state)
        {
            string address = serial + ((dir == Direction.Input) ? "_I" : "_O") + n;
            //osae.AddToLog("state: " + address + " = " + state, true);
            OSAEObject obj = OSAEObjectManager.GetObjectByAddress(address);
            if (obj != null)
            {
                bool invert = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Name, "Invert").Value == "TRUE";
                bool on = (state != 0) ^ invert;
                OSAEObjectStateManager.ObjectStateSet(obj.Name, on ? "ON" : "OFF", pName);
                Log.Debug("State change: " + obj.Name + " (" + address + ")" + " changed to " + OSAEObjectStateManager.GetObjectStateValue(obj.Name).Value);
            }
        }

        private void Poll(object sender, EventArgs args)
        {
            try
            {
                // get state of all inputs and outputs and report changes.
                // N.b. that setting of an output and reporting the new state
                // are independent.
                for (int i = 0; i < JSB34Xs.Count; ++i)
                {
                    string serial = JSB34Xs[i];
                    ushort state = Jsb34xInput(serial);
                    byte max = Jsb34xMaxRelays(serial);
                    for (byte relay = max; relay >= 1; --relay)
                    {
                        state <<= 1;
                        if (Jsb34xIsRelayClosed(serial, relay)) state |= 1;
                    }
                    if (state != JSB34XState[i])
                    {
                        ushort changes = (ushort)(state ^ JSB34XState[i]);
                        JSB34XState[i] = state;

                        for (byte relay = 1; relay <= max; ++relay)
                        {
                            if (0 != (changes & 1))  StateChange(serial, Direction.Output, relay, state & 1);

                            state >>= 1;
                            changes >>= 1;
                        }
                        for (byte input = 1; changes != 0; ++input)
                        {
                            if (0 != (changes & 1)) StateChange(serial, Direction.Input, input, state & 1);

                            state >>= 1;
                            changes >>= 1;
                        }
                    }
                }
                for (int i = 0; i < JSB383s.Count; ++i)
                {
                    string serial = JSB383s[i];
                    ushort state = Jsb383Input(serial);
                    if (state != JSB383State[i])
                    {
                        ushort changes = (ushort)(state ^ JSB383State[i]);
                        JSB383State[i] = state;
                        for (byte input = 1; changes != 0; ++input)
                        {
                            if (0 != (changes & 1))  StateChange(serial, Direction.Input, input, state & 1);

                            state >>= 1;
                            changes >>= 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            { Log.Error("Polling error!", ex); }
        }

        public override void ProcessCommand(OSAEMethod method)
        {
            try
            {               
                string command = method.MethodName;
                string address = method.Address;
                string name =  method.ObjectName;
                if (address.Length > 0)
                {
                    string serial = OSAEObjectPropertyManager.GetObjectPropertyValue(name, "Serial").Value;
                    string id = OSAEObjectPropertyManager.GetObjectPropertyValue(name, "Id").Value;
                    ushort n = UInt16.Parse(id);
                    if (command == "ON")
                    {
                        Jsb34xCloseRelay(serial, (byte)n);
                        Log.Info("Set state: " + name + " (" + address + ")" + " to ON");
                    }
                    else if (command == "OFF")
                    {
                        Jsb34xOpenRelay(serial, (byte)n);
                        Log.Info("Set state: " + name  + " (" + address + ")" + " to OFF");
                    }
                }
                else if (command == "POLL")
                    Poll(this, new EventArgs());
            }
            catch (Exception ex)
            { Log.Error("Error processing command!", ex); }
        }

        public override void RunInterface(string pluginName)
        {

         
            pName = pluginName;
            Log = new General.OSAELog(pName);
            Log.Info("Starting J-Works plugin");
            if (OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Polling Inverval").Value != "")
                pollInterval = UInt32.Parse(OSAEObjectPropertyManager.GetObjectPropertyValue(pName, "Polling Interval").Value);

            // point subobject classes at this instance (I think)
            OSAEObjectTypeManager.ObjectTypeUpdate("JWORKS INPUT", "JWORKS INPUT", "J-Works Input", pName, "JWORKS INPUT", false, false, false, true, "J-Works Input");
            OSAEObjectTypeManager.ObjectTypeUpdate("JWORKS OUTPUT", "JWORKS OUTPUT", "J-Works Output", pName, "JWORKS OUTPUT", false, false, false, true, "J-Works Output");

            // enumerate JSB34x inputs and outputs
            short numDevs = Jsb34xNumberOfModules();
            StringBuilder sb = new StringBuilder(32);
            Jsb34xDllVersion(sb, 32);
            string DllVersion = sb.ToString();
            for (short dev = 1; dev <= numDevs; ++dev)
            {
                sb.Clear();
                Jsb34xSerialNumber(dev, sb, 32);
                String serial = sb.ToString();
                sb.Clear();
                Jsb34xFirmwareVersion(serial, sb, 32);
                string firmwareVersion = sb.ToString();
                sb.Clear();
                Jsb34xDriverVersion(serial, sb, 32);
                string driverVersion = sb.ToString();

                Jsb34xOpenAllRelays(serial);
                JSB34Xs.Add(serial);
                JSB34XState.Add(0);

                byte numInputs = Jsb34xMaxInputs(serial);
                byte numOutputs = Jsb34xMaxRelays(serial);
                Log.Info("Found JSB34x device " + serial + " " + driverVersion + " " + DllVersion + " " + firmwareVersion + " with " + numInputs + " inputs, " + numOutputs + " outputs.");
                for (byte inp = 1; inp <= numInputs; ++inp)
                    AddIO(serial + "_I" + inp, Direction.Input, serial, inp);
                for (byte outp = 1; outp <= numOutputs; ++outp)
                    AddIO(serial + "_O" + outp, Direction.Output, serial, outp);
            }

            // enumerate JSB38x /39x inputs
            numDevs = Jsb383NumberOfModules();
            sb.Clear();
            Jsb383DllVersion(sb, 32);
            DllVersion = sb.ToString();
            for (short dev = 1; dev <= numDevs; ++dev)
            {
                sb.Clear();
                Jsb383SerialNumber(dev, sb, 32);
                String serial = sb.ToString();
                sb.Clear();
                Jsb383FirmwareVersion(serial, sb, 32);
                string firmwareVersion = sb.ToString();
                sb.Clear();
                Jsb383DriverVersion(serial, sb, 32);
                string driverVersion = sb.ToString();

                JSB383s.Add(serial);
                JSB383State.Add(0);

                byte numInputs = Jsb383MaxInputs(serial);
                Log.Info("Found JSB38x / JSB39x device " + serial + " " + driverVersion + " " + DllVersion + " " + firmwareVersion + " with " + numInputs + " inputs.");
                for (byte inp = 1; inp <= numInputs; ++inp)
                    AddIO(serial + "_I" + inp, Direction.Input, serial, inp);
            }

            if (pollInterval > 0)
            {
                timer.Interval = pollInterval;
                timer.Elapsed += new ElapsedEventHandler(Poll);
                timer.Start();
            }
        }

        public override void Shutdown()
        {
            Log.Info("Stopping J-Works plugin");
            timer.Stop();
        }
    }
}
