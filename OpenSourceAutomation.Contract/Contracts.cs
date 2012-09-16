using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Data;
//using PipelineHints;

//Generally for V1 of your application you'll want to use the same view assembly for both your host and add-in. 
//[assembly: ShareViews]

//You'll also likely want to name the other pipeline assemblies. Uncomment these attributes and supply your own assembly names

//[assembly: SegmentAssemblyName(PipelineSegment.Views, "OpenSourceAutomation.Blank")]
//[assembly: SegmentAssemblyName(PipelineSegment.HostSideAdapter, "OpenSourceAutomation.HostSideAdapter")]
//[assembly: SegmentAssemblyName(PipelineSegment.AddInSideAdapter, "OpenSourceAutomation.AddInSideAdapter")]



//You should change the assembly name and default namespace for this project before you use it. 
namespace OpenSourceAutomation.Contracts
{
    [AddInContract]
    public interface IOpenSourceAutomationAddInContract : IContract
    {
        void RunInterface(string pluginName);

        void Shutdown();

        void ProcessCommand(OSAE.OSAEMethod method);
    }
}
