To add the template to visual studio copy the Zip to:

C:\Users\<User Name>\Documents\Visual Studio 2012\Templates\ProjectTemplates\

Upack the Zip file. When adding a new project "Open Source Automation - Plugin" should be available.

NOTES:

do not name your project OSAE.PluginName as this will affect the project creation. Simply call the project the name of the plugin e.g. Zwave.

Ensure you place the project in the Plugin Folder else the output paths will not be correct due to the number of ..\..\

Do not submit the Solution with the new plugin into GIT, only the core plugins are included in the solution. You can however leave project added in your local solution.
