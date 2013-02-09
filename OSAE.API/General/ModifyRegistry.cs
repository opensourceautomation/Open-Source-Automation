namespace OSAE
{
    using System;
    using Microsoft.Win32;

    /// <summary>
    /// Class for modifying OSAE registry keys
    /// </summary>
    public class ModifyRegistry
    {
        public string SubKey { get; set; }        

        public RegistryKey BaseRegistryKey { get; set; }       

        /// <summary>
        /// Default constructor
        /// </summary>
        public ModifyRegistry()
        {
            if (Environment.Is64BitOperatingSystem == true)
            {
                BaseRegistryKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                BaseRegistryKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
            }
        }

        /// <summary>
        /// Reads a registry key
        /// </summary>
        /// <param name="KeyName">The key to read from</param>
        /// <returns>The value from the registry key</returns>
        public string Read(string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = BaseRegistryKey;

            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(SubKey);

            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    return (string)sk1.GetValue(KeyName.ToUpper());
                }
                catch (Exception)
                {                   
                    return null;
                }
            }
        }        

        /// <summary>
        /// Writes a registry key
        /// </summary>
        /// <param name="KeyName">The key to write to</param>
        /// <param name="Value">The value to write to the registry key</param>
        /// <returns>True on success false otherwise</returns>
        public bool Write(string KeyName, object Value)
        {
            try
            {
                // Setting
                RegistryKey rk = BaseRegistryKey;
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                RegistryKey sk1 = rk.CreateSubKey(SubKey);
                // Save the value
                sk1.SetValue(KeyName.ToUpper(), Value);

                return true;
            }
            catch (Exception)
            {             
                return false;
            }
        }
    }
}
