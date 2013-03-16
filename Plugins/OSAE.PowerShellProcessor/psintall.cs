/// <summary>
/// Create the Windows PowerShell snap-in for this sample.
/// </summary>
/// 
using System.Management.Automation;
using System.ComponentModel;  // Windows PowerShell assembly

namespace OSAPS
{

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
                return "OSAPS";
            }
        }

        /// <summary>
        /// Get the name of the vendor of the snap-in.
        /// </summary>
        public override string Vendor
        {
            get
            {
                return "IandL";
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
                return "OSAPS,IandL";
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
