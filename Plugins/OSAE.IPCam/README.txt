IPCam Plugin for OSA

Adds Custom URL Controls and Snapshot function to the existing IP Camera object.

INSTALLATION:
==============
1) Using the manager's plugin installer, install the plugin as normal.

2) Using the settings GUI, open "Objects".

3) Create a new "IP CAMERA" object for your camera as normal.

4) Now select your newly created IP CAMERA Object.

5) Now set each property with the correct information: (Please see NOTES at bottom of page)
    
	Stream Address - The URL to obtain the video stream from the camera.
	Snap Shot - URL to Invoke the camera to take a Snapshot. (if camera has function)
	Save Location - This is the folder location where snapshots and future recording will be saved. (If camera has function)

6) Select the IPCam object and check Enabled, update.

7) Open Manager.exe and stop the service, and re-start the service.

NOTES:
============================================================================================
* If your camera only has the streaming function, enter the URL information for "Stream Address" only.

* If your camera has PTZ functions, and you have the correct URL's, See How to Add Custom Methods below.

* Currently the Snapshop function uses a "FileDownload" call to the URL you provide. 
   So your camera's snapshot URL function MUST return an image.
   For example: when you type the URL directly in a browser, it returns a screen with ONLY a picture on it.
============================================================================================

HOW TO ADD CUSTOM METHODS
============================================================================================
1) Go to Object Types in the Web UI Settings page.
2) Locate your IP Camera Object, and click on it.
3) At the right you will see several sections. Look at the Methods section.
4) In the Name box, type in the name of the method. For example: LEFT.
5) In the Label box, type a description lable. for example: Left or Pan Left. 
6) Click the Add button.
7) Now look down at the Properties section.
8) In the name box, enter the exact same name you used for the method above. For example: LEFT
9) Type should be set to String.
10) Click the Add button.
11) continue doing steps 3-10 until you have added all your custom commands.
12) Now go to the Objects page in the Web UI.
13) Locate and click on your IP Camera object.
14) At the right you should see your newly added Methods and Properties.
15) Under Properties, click on one of the newly created properties.
16) At the bottom, enter your custom URL command
       Example: 192.168.0.251:80/decoder_control.cgi?command=4&user=myname&pwd=password
17) Click Save.
18) Continue to enter each newly created property following steps 15-17.

To test or execute, just run the method that matches the property.
For example: IPcam_office.Run Method.LEFT will execute the URL stored in Property LEFT.

This allows for an unlimited number of methods, and does not require everyone to have the same setup!
THANKS VAUGHN!
============================================================================================

Now, Here is where it gets intresting.....

The new IPCam Renaming system allows users to add properties to each object that can hold URL information so in
the case of changes to addresses, ports, usernames and such, you would only have to change it in 1 place.
---------------------------------------------------------------------------------------------------------------------------------------------------------
HOW TO USE THE RENAMING SYSTEM

As we saw above, a command URL contains many peices of information that is used in every URL command.
	For our example, we will use this command URL: 
		http://192.168.0.251:80/decoder_control.cgi?command=4&user=myname&pwd=password1234

	Now, lets break it down in to peices:
		http://  - used in every web address (Not required to be entered here!)
	  192.168.0.251  - The IP address of the camera (also used in every URL Command)Yours is probably different.
		    :80  - The Port the camera is on. (also used in every URL Command) Yours may be different.
         /decoder_control.cgi  - The CGI command to send to the camera. (also used in most URL Commands)
		              (Your CGI command may look a little different)
	   user=myname  - Username to login to Camera. (also required in every URL Command)
	  pwd=password  - Password to login to Camera. (also required in every URL Command)

OK, now imagine you have 20 or 30 URL commands, and then you have to change the password to the IP camera!!!
Without the renaming system, you would have to edit every Property with the new password.
However, with the new renaming system, you would only have to change 1 property!!!!

Here's How to do it.

1) Go to the Object Types page in the Web UI.
2) Click on your IP Camera Object.
3) At the right look for the Properties Section.
4)  In the name box, enter the name you want to use to store the information. For example: password.
5) Type should be set to String.
6) Click the Add button.
7) Now go to the Objects page in the Web UI.
8) Locate and click on your IP Camera object.
9) At the right you should see the Properties.
10) Click on the newly created password property.
11) At the bottom, enter the information to be stored: For example, password1234 
12) Now when you enter any custom Command URL's, all you do is use the Property Rename Placement Holder!
	Using our above Example:
		192.168.0.251:80/decoder_control.cgi?command=4&user=myname&pwd=[password]

Using this in every Command URL, will automatically add the password to the URL. So now if I change my
      Password, I only have to change the password property once!!

This process can be used for any part of the URL.

Example 2:

1) Create a new property called: path.
2) Store the following information in the property: 192.168.0.251:80/decoder_control.cgi?
3) Then change your URLs like this: [path]command=4&user=myname&pwd=[password]
Now if the path changes, all you edit is the path property.

Example 3:

1) Create a property called: auth.
2) And create another new property called: path.
3) Store the following information in the path property: 192.168.0.251:80/decoder_control.cgi?
2) Store the following information in the auth property: &user=myname&pwd=password1234
3) Then change your URLs like this: [path]command=4[auth]

Now if the path or authentication changes, all you edit is the path or auth property.

You can break it down as many times as needed, or add as many custom holders you need.
Just remember to surrond the property name with square brackets to use in the command URL.

---------------------------------------------------------------------------------------------------------------------------------------------------------
ADDITIONAL CUSTOMIZATION OPTIONS:

The renaming system also allows for custom place holders to be used in scripts or Method Images on the screens application.
A method's Parameter 1 and Parameter 2 can be used to add many additional customizable options.

For this example, we will use this command:  [path]command=4[auth]
assuming we have created the "path" and "auth" properties as explained above in example 3.		

EXAMPLE 4:

1) Go to Object Types and create a new property named: CUSTOM
2) Also create a new Method named: CUSTOM
3) Now, Go to Objects and click on your IP Camera object.
4) In the "CUSTOM" property, store this URL command: [path]command=[cmd][auth]
5) Now, open the Screens app, and go to the screen where your camera viewer is located.
6) Right click and choose Add Control->Method Image
7) Choose the image you want to use.
8) Select your Camera object.
9) Select the CUSTOM method
10) For Parameter 1, type the name of the custom place holder - in this example it is: cmd (Do not include the square brackets)
11) Now for Parameter 2, type: [ASK]
12) Click Add

Once your new Method Image appears, Click on it.
You should get a Popup that is asking for Parameter 2. Now you can enter any valid command.
For example, if I enter 4 and click enter, it will run the method just like our example above.
However, if I enter say 6, it will run command 6 instead.

NOTICE:
This options does not require us to create a property named: cmd.
If you Run this Method with out the parameters, it will cause an error.
So, use special caution when using this. An invalid entery can also cause undesireable results or errors in the log.

To take this option to the next level, I have also provided a way to use custom Properties to replace custom placeholders.

In this example I will use my cameras URL command for Preset locations.
My commands look like this: 
	Preset 1: [path]command=31[auth]
	Preset 2: [path]command=33[auth]
	Preset 3: [path]command=35[auth]
	Preset 4: [path]command=37[auth]
	Preset 5: [path]command=39[auth]
	Preset 6: [path]command=41[auth]

Example 5:

1) Follow steps 1 & 2 in example 4 above.
2) Also, create 6 new properties, named: pre1, pre2, pre3, pre4, pre5 & pre6.
3) Now, Go to Objects and click on your IP Camera object.
4) In the "CUSTOM" property, store this URL command: [path]command=[cmd][auth] 
5) Now go to each of the new Custom Properties and enter the values for each.
    For example: In pre1 property, store the value 31.
                          In pre2 property, store the value 33.
                          In pre3 property, store the value 35.
	And so on...............
6) Now, open the Screens app, and go to the screen where your camera viewer is located.
7) Right click and choose Add Control->Method Image
8) Choose the image you want to use.
9) Select your Camera object.
10) Select the CUSTOM method
11) For Parameter 1, type the name of the custom place holder - in this example it is: cmd (Do not include the square brackets)
12) Now for Parameter 2, type: [ASK]
13) Click Add

Once your new Method Image appears, Click on it.
You should get a Popup that is asking for Parameter 2. 
Now you can enter any of our custom Properties by surrounding it with square brackets.
For example, I can enter [pre2] and it will use this property and turn my camera to Preset 2.
Or I could enter [pre4] and it would go to Preset 4.
This way I can have 1 button on the screen that can take me to any preset!!

Please see the Wiki page for more information, or post any question in the forum.

I also want to thank Vaughn Rupp for his input and help!