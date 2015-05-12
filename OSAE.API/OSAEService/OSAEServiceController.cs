//**********************************************************************************
//*                                                                                *
//*                       GNU LESSER GENERAL PUBLIC LICENSE                        *
//* github.com/opensourceautomation/Open-Source-Automation/blob/master/License.txt *
//*                                                                                *
//**********************************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace OSAE
{
  public partial class OSAEServiceController : Form
  {
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    public enum ServiceState
    {
      Stopped = 0,
      StartPending = 1,
      Running = 2,
      PausePending = 3,
      Paused = 4,
      ContinuePending = 5,
      StopPending = 6
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private ServiceState _serviceState = ServiceState.Stopped;
    private OSAEServiceBase _service = null;

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private void OSAEServiceController_Load(object sender, EventArgs e)
    {
      ProcessServiceState(ServiceState.Stopped);
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private void buttonStart_Click(object sender, EventArgs e)
    {
      buttonStart.Enabled = false;
      ProcessServiceState(ServiceState.StartPending);
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private void buttonPause_Click(object sender, EventArgs e)
    {
      buttonPause.Enabled = false;
      ProcessServiceState(ServiceState.PausePending);
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private void buttonStop_Click(object sender, EventArgs e)
    {
      buttonStop.Enabled = false;
      ProcessServiceState(ServiceState.StopPending);
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private void ProcessServiceState(ServiceState serviceState)
    {
      _serviceState = serviceState;

      //----- Update Button State -----
      UpdateUserInterface();

      switch (_serviceState)
      {
        case ServiceState.Stopped:
          break;

        case ServiceState.StartPending:
          string[] args = null;
          _service.Start(args);
          ProcessServiceState(ServiceState.Running);
          break;

        case ServiceState.Running:
          break;

        case ServiceState.PausePending:
          _service.Pause();
          ProcessServiceState(ServiceState.Paused);
          break;

        case ServiceState.Paused:
          break;

        case ServiceState.ContinuePending:
          _service.Continue();
          ProcessServiceState(ServiceState.Running);
          break;

        case ServiceState.StopPending:
          _service.Stop();
          ProcessServiceState(ServiceState.Stopped);
          break;
      }
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    private void UpdateUserInterface()
    {
      toolStripStatusLabel1.Text = "Service State: " + _serviceState.ToString();

      switch (_serviceState)
      {
        case ServiceState.Stopped:
          buttonStart.Enabled = true;
          buttonPause.Enabled = false;
          buttonStop.Enabled = false;
          break;

        case ServiceState.StartPending:
          buttonStart.Enabled = false;
          buttonPause.Enabled = false;
          buttonStop.Enabled = false;
          break;

        case ServiceState.Running:
          buttonStart.Enabled = false;
          buttonPause.Enabled = true;
          buttonStop.Enabled = true;
          break;

        case ServiceState.PausePending:
          buttonStart.Enabled = false;
          buttonPause.Enabled = false;
          buttonStop.Enabled = false;
          break;

        case ServiceState.Paused:
          buttonStart.Enabled = true;
          buttonPause.Enabled = false;
          buttonStop.Enabled = true;
          break;

        case ServiceState.ContinuePending:
          buttonStart.Enabled = false;
          buttonPause.Enabled = false;
          buttonStop.Enabled = false;
          break;

        case ServiceState.StopPending:
          buttonStart.Enabled = false;
          buttonPause.Enabled = false;
          buttonStop.Enabled = false;
          break;
      }

      Application.DoEvents();
    }

    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------

    public OSAEServiceController(OSAEServiceBase service, string title)
    {
      InitializeComponent();

      this.Text = title;
      _service = service;
    }


    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------
  }
}
