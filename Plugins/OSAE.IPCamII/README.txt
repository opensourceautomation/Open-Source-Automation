IPCamII IP Camera Plugin for OSA

Adds PTZ controls and snapshot function to existing IP Camera object.

INSTALLATION:
==============
1) Using the manager's plugin installer, install the plugin as normal.

2) Using the settings GUI, open "Objects".

3) Create a new object for the PlugIn as normal.

4) Now, go to "Object types".

5) Chage the Default ownership for the "IP CAMERA" object to belong to "IPCamII" and save your changes.

6) Now, go back to "Objects",

7) Create a new "IP CAMERA" object for your camera as normal.

8) Now select your newly created IP CAMERA Object.

9) Now set each property with the correct information: (Please see NOTES at bottom of page)
    Do Not Include: http:// or www. See Renaming System below

	camSnapShot - URL to Invoke the camera to take a Snapshot.
	Degrees - This is the default degrees used in Up, Down, Left and Right commands. (Default is 5) (Only required when using the renaming system. see NOTES)
	IP Address - This is the IP address for the IP Camera. (ie. 192.168.0.100) (Only required when using the renaming system. see NOTES)
	Password - The password used to access this IP Camera. (Only required when using the renaming system. see NOTES)
	Port - This is the port number used to access the IP Camera. (ie. 80, 81) (Only required when using the renaming system. see NOTES)
	ptzDOWN - URL to invoke the Camera to Move Down.
	ptzFOCUSIN - URL to invoke the Camera to Focus In.
	ptzFOCUSOUT - URL to invoke the Camera to Focus Out.
	ptzIN - URL to invoke the Camera to Zoom In.
	ptzLEFT - URL to invoke the Camera to Move to the Left.
	ptzOUT - URL to invoke the Camera to Zoom Out.
	ptzPRESET1 - URL to invoke the Camera to Move to Preset 1.
	ptzPRESET2 - URL to invoke the Camera to Move to Preset 2.
	ptzPRESET3 - URL to invoke the Camera to Move to Preset 3.
	ptzPRESET4 - URL to invoke the Camera to Move to Preset 4.
	ptzPRESET5 - URL to invoke the Camera to Move to Preset 5.
	ptzPRESET6 - URL to invoke the Camera to Move to Preset 6.
	ptzRIGHT - URL to invoke the Camera to Move to the Right.
	ptzUP - URL to invoke the Camera to Move Up.
	Save Location - This is the folder location where snapshots and future recording will be saved.
	Stream Address - The URL to obtain the video stream from the camera.
	Username - The username used to access this IP Camera. (Only required when using the renaming system. see NOTES)

10) Open the Manager and stop the service. Add a check mark for the IPCamII plugin, and re-start the service.

NOTES:
============================================================================================
The IPCamII.xml file Is no longer used with version 2.2 of this plugin.
All properties are now saved in the database, instead of an external xml file.
This removes the requirement to have the plugin folder with the XML file on remote computers running the screens app.
All the properties can now be retrieved directly from the database through the OSAE.API.

* If your camera only has the streaming function, enter the URL information for "Stream Address" only.

* If your camera has PTZ functions, and you have the correct URL's, enter the URL information for the PTZ commands.

* "Preset1" thru "Preset6" will only send the camera to that location. (You can not set these in OSA, yet !!)
  You must use the camera's software to set the preset locations before IPCamII can use any PRESET functions.

* Currently the Snapshop function uses a "FileDownload" call to the URL you provide. So your camera's snapshot URL function MUST returns an image.
   For example: when you type the URL directly in a browser, it returns a screen with ONLY a picture on it.
============================================================================================

IPCamII Renaming system:
---------------------------------------------------------------------------------------------------------------------------------------------------------
When entering the URL's, there are a few options available here.

OPTION 1: You can type the URL directly with all the values.
	Example 1: 192.168.1.100:80/stream.cgi?user=username&pwd=password
	Example 2: 192.168.1.100:80/camera control.cgi?cmd=right&deg=10&user=username&pwd=password
	Example 2: 192.168.1.100:80/snapshot.cgi?user=username&pwd=password

OPTION 2: Renaming system.
               To use this system, 1st make sure you have filled in the properties for Username, Password, IP Address, Port and Degrees.
               Then enter the URL's replacing the values with these:[username], [password], [address], [port], [degrees].
	Example 1: [address]:[port]/stream.cgi?user=[username]&pwd=[password]
	Example 2: [address]:[port]/camera control.cgi?cmd=right&deg=[degrees]&user=[username]&pwd=[password]
	Example 2: [address]:[port]/snapshot.cgi?user=username&pwd=password