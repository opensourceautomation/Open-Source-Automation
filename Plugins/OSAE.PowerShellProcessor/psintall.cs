/// <summary>
/// Create the Windows PowerShell snap-in for this sample.
/// </summary>
/// 

namespace OSAPS
{
    using System.Management.Automation;
    using System.ComponentModel;  // Windows PowerShell assembly

    [RunInstaller(true)]
    public class GetProcPSSnapIn01 : PSSnapIn
    {
        /// <summary>
        /// Initializes a new instance of the GetProcPSSnapIn01 class.
        /// </summary>
        public GetProcPSSnapIn01()
            : base()
        {
        }

        /// <summary>
        /// Get a name for the snap-in. This name is used to register
        /// the snap-in.
        /// </summary>
        public override string Name
        {
            get
            {
                return "OSA";
            }
        }

        /// <summary>
        /// Get the name of the vendor of the snap-in.
        /// </summary>
        public override string Vendor
        {
            get
            {
                return "Open Source Automation";
            }
        }

        /// <summary>
        /// Get the resource information for vendor. This is a string of format: 
        /// resourceBaseName,resourceName. 
        /// </summary>
        public override string VendorResource
        {
            get
            {
                return "OSA,Open Source Automation";
            }
        }

        /// <summary>
        /// Get a description the snap-in.
        /// </summary>
        public override string Description
        {
            get
            {
                return "This is a PowerShell snap-in that includes the OSA cmdlets.";
            }
        }
    }
}
