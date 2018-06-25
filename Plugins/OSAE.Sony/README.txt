Sony plugin for OSA 4.9 or later.

NOTE: This plugin has been compiled using the latest OSAE.API. (This Plugin will not load properly on earlier versions of OSA)

Provides a LAN connection to compatiable Sony Smart devices equiped with a LAN/WiFi and has the IRCC service (See compatability list) allowing Remote control via LAN through the OSA service.

INSTALLATION:
==============
1) Using the manager's plugin installer, install the plugin as normal.

2) Using the settings GUI, open "Objects".

3) Create a new object for the PlugIn as normal.

4) Once the new object is created, set the "Refresh" property to the number of minutes you want the plugin to scan the network for devices that are "Powered ON".

5) Open the Manager, Stop the service.

6) Add a check mark beside the Sony plugin and Re-Start the service.

7) Now, go back to the "Objects" page, and click on the Sony plugin.

8) At the Right, run the "Discover" method. #### NOTE: ALL COMPATIABLE DEVICES MUST BE POWERED ON DURING THIS PROCESS"

	If any compatiable devices are found, The plugin will automatically
	create the new Objects and Object Types for you. (You can see
	what the plugin found by going to Server Logs->OSAE.Sony.Sony

9) Sony devices require a Registration before they will render control through the LAN port. To
    accomplish this, you must follow any instructions included with your device before you can
    successfully register the device. For example, TV's will display a Popup on the screen that
    you have to confirm within 30 seconds before registration will complete. Most receivers,
    DVR or Blue Ray players have to be in Registration Mode, before you can register them.

    #### NOTE: ALL COMPATIABLE DEVICES MUST BE POWERED ON DURING THIS PROCESS"

    Go to the "Objects" page, and click on the newly created Sony Device.
    At the right, run the "Register" method.

10) To check to see if the device was registered, you can check the log at Server Logs->OSAE.Sony.Sony
       Or you can refesh the Objects page and check the Registered property. It should be set to "TRUE".

11) If your device registered successfully, then all you have left to do is run the "Retrieve" method for the device.
      This will communicate with the device, and retrieve all the available commands and create Methods for each.

12) Once this is complete, refresh the screen and you should be able to see all the Methods that were created at the right.

Your device is now setup and ready to use.

#### NOTE: Some Sony devices disable the LAN port when powered off or in standby. These devices will not be available if powered off. So, also this plugin will not be able to power them ON. Check the network settings of your device, as some devices have an optional setting that will leave the LAN port active even when powered off or in standby. If your device has this option, please be aware to set the refresh property to a high number or 0, as this device will always appear to be ONLINE!


ADDITIONAL NOTES:
Special Methods:

This plugin also adds a special method.

The Set Channel method is not a function of the device, but a special function of the plugin.
The Set Channel Methods requires a valid value to be entered in Parameter 1.
The Set Channel Method will ONLY execute successfully with Sont TV's, as other devices do not have a Channel method!

For example: instead of running all 3 methods in a row: Num3, Num0, DOT, Num2 to change the channel to 30.2,
you can use the Set Channel method by setting Parameter 1 to "30.2". (SonyTV.Run Method.Set Channel("30.2","")

The Current_Channel property will only report the correct value when the Set Channel method is used. Channel Up or Down or directly entered will not change this. (Maybe next version)
The Current_Volume property is currently not enabled in the plugin. (Next Version)
