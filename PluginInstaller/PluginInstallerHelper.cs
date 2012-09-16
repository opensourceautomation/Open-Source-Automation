using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Xml;
using System.Data;

namespace PluginInstaller
{
    internal class PluginInstallerHelper
    {
        public static void InstallPlugin(string filepath)
        {
            string ErrorText = string.Empty;
            bool proceed = true;
            
            Gui_Install gi = new Gui_Install(filepath);
            DialogResult dr = gi.ShowDialog();
            proceed = (dr == DialogResult.OK);
            
            if (proceed)
            {

                if (!InstallPlugin(filepath, ref ErrorText))
                {
                    MessageBox.Show("Package was not successfully installed.");
                }

                else if (!string.IsNullOrEmpty(ErrorText))
                {
                    MessageBox.Show("Package installed.");
                }

                else
                {
                    MessageBox.Show("Package installed.");
                }
            }
        }

        public static bool InstallPlugin(string PluginPackagePath, ref string ErrorText)
        {
            OSAE.OSAE osae = new OSAE.OSAE("Plugin Installer");
            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            if (Directory.Exists(exePath + "/tempDir/"))
            {
                Directory.Delete(exePath + "/tempDir/", true);
            }

            PluginDescription desc;
            string tempfolder = exePath + "/tempDir/";
            string zipFileName = Path.GetFullPath(PluginPackagePath);
            string DescPath = null;

            bool NoError = true;

            FastZip fastZip = new FastZip();
            //try
            //{
                fastZip.ExtractZip(zipFileName, tempfolder, null);
                // find all included plugin descriptions and install the plugins
                List<string> osapdFiles = new List<string>();
                List<string> sqlFiles = new List<string>();

                string[] pluginFile = Directory.GetFiles(tempfolder, "*.osapd", SearchOption.TopDirectoryOnly);
                osapdFiles.AddRange(pluginFile);
                string[] sqlFile = Directory.GetFiles(tempfolder, "*.sql", SearchOption.TopDirectoryOnly);
                sqlFiles.AddRange(sqlFile);

                if (osapdFiles.Count == 0)
                {
                    MessageBox.Show("No plugin description files found.");
                    return false;
                }

                if (osapdFiles.Count > 1)
                {
                    MessageBox.Show("More than one plugin description file found.");
                    return false;
                }
                if (osapdFiles.Count == 1)
                {

                    DescPath = osapdFiles[0];
                }


                if (!string.IsNullOrEmpty(DescPath))
                {
                    desc = Deserialize(DescPath);

                    //NoError = desc.VerifyInstall(ref ErrorText);

                    //uninstall previous plugin and delete the folder
                    bool u = UninstallPlugin(desc);

                    // get the plugin folder path
                    string pluginFolder = desc.DestFolder;
                    if (!string.IsNullOrEmpty(pluginFolder))  //only extract valid plugins
                    {
                        //Directory.CreateDirectory(exePath + "/Plugins/" + pluginFolder);

                        string[] files = System.IO.Directory.GetFiles(tempfolder);

                        // Copy the files and overwrite destination files if they already exist.
                        //foreach (string s in files)
                        //{
                        //    string fileName = System.IO.Path.GetFileName(s);
                        //    if (desc.AdditionalAssemblies.Contains(fileName))
                        //    {
                        //        string destFile = System.IO.Path.Combine(exePath + "/", fileName);
                        //        System.IO.File.Copy(s, destFile, true);
                        //    }
                        //}

                        string ConnectionString = string.Format("Uid={0};Pwd={1};Server={2};Port={3};Database={4};allow user variables=true",
                            osae.DBUsername, osae.DBPassword, osae.DBConnection, osae.DBPort, osae.DBName);
                        MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
                        connection.Open();
                        foreach (string s in sqlFile)
                        {
                            try
                            {

                                MySql.Data.MySqlClient.MySqlScript script = new MySql.Data.MySqlClient.MySqlScript(connection,File.ReadAllText(s));
                                script.Execute();
                            }
                            catch (Exception ex)
                            {
                                osae.AddToLog("Error running sql script: " + s + " | " + ex.Message, true);
                            }
                        }

                        System.IO.Directory.Move(tempfolder, exePath + "/AddIns/" + pluginFolder);

                        //Check if we are running a x64 bit architecture (This is a silly way to do it since I am not sure if every 64 bit machine has this directory...)
                        bool is64bit = Environment.Is64BitOperatingSystem;

                        //Do a check for any x64 assemblies, and prompt the user to install them if they are running a 64 bit machine
                        if (is64bit && (desc.x64Assemblies.Count > 0))
                        {
                            /* x64 assemblies generally have the same name as their x32 counterparts when referenced by the OSA app
                             * however they are packaged as "filename.ext.x64" so we will replace the 32bit file which is installed by
                             * default with the 64bit versioin with the same filename.ext
                             */

                            if (MessageBox.Show(
                                "You are running an x64 architecture and this plugin has specific assemblies built for 64bit machines." +
                                " It is highly recommended that you install the 64bit versions to ensure proper compatibility",
                                "Install 64bit Assemblies?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                //Install the 64bit assemblies over the 32 bit ones...
                                string[] x64files = System.IO.Directory.GetFiles(exePath + "/AddIns/" + pluginFolder, "*.x64");

                                foreach (string str in x64files)
                                {
                                    string destFile = System.IO.Path.Combine(exePath + "/AddIns/" + pluginFolder + "/", System.IO.Path.GetFileNameWithoutExtension(str));
                                    //Copy it to the new destination overwriting the old file if it exists
                                    System.IO.File.Copy(str, destFile, true);
                                }
                            }
                        }
                        
                        //Delete all the files with .x64 extensions since they aren't needed anymore
                        string[] delfiles = System.IO.Directory.GetFiles(exePath + "/AddIns/" + pluginFolder, "*.x64");
                        foreach (string str in delfiles)
                            System.IO.File.Delete(str);


                    }

                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("catch: " + ex.Message);
            //    return false;
            //}
                if (Directory.Exists(exePath + "/tempDir/"))
                {
                    deleteFolder(exePath + "/tempDir/");
                }

                osae.MethodQueueAdd("SERVICE-" + osae.ComputerName, "RELOAD PLUGINS", "", "");
            return NoError;
        }

        public static bool UninstallPlugin(PluginDescription desc)
        {
            bool returnValue = false;

            string exePath = Path.GetDirectoryName(Application.ExecutablePath);
            string pluginFolder = exePath + "/AddIns/" + desc.DestFolder;
            if (Directory.Exists(pluginFolder))
            {
                deleteFolder(pluginFolder);
                returnValue = true;

                if (Directory.Exists(pluginFolder))
                    returnValue = false;
            }
            return returnValue;
        }

        public static PluginDescription Deserialize(string file)
        {
            PluginDescription desc = new PluginDescription();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(ReadFromFile(file));
            desc.PluginName = xml.SelectSingleNode("//plugin-name").InnerText;
            desc.PluginID = xml.SelectSingleNode("//plugin-id").InnerText;
            desc.PluginVersion = xml.SelectSingleNode("//plugin-version").InnerText;
            desc.PluginState = xml.SelectSingleNode("//plugin-state").InnerText;
            desc.PluginAuthor = xml.SelectSingleNode("//author").InnerText;
            desc.Description = xml.SelectSingleNode("//short-description").InnerText;
            desc.DestFolder = xml.SelectSingleNode("//destination-folder").InnerText;
            desc.MainFile = xml.SelectSingleNode("//main-file").InnerText;
            desc.WikiUrl = xml.SelectSingleNode("//wiki-url").InnerText;

            XmlNode temp = xml.SelectSingleNode("//additional-assemblies");
            XmlNodeList childNodes = temp.ChildNodes;

            foreach (XmlNode t in childNodes)
                desc.AdditionalAssemblies.Add(t.InnerText);

            //Check if this plugin supports any x64 Specific Assemblies
            if ((temp = xml.SelectSingleNode("//x64-additional-assemblies")) != null)
            {
                childNodes = temp.ChildNodes;

                foreach (XmlNode t in childNodes)
                    desc.x64Assemblies.Add(t.InnerText);
            }
            

            return desc;

        }

        static string ReadFromFile(string filename)
        {
            StreamReader SR;
            string S;
            SR = File.OpenText(filename);
            S = SR.ReadToEnd();
            SR.Close();
            return S;
        }

        public static void deleteFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);
            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                deleteFolder(di.FullName);

            }

            dir.Delete(true);
        }
    }

    class PluginDescription
    {
        private string _pluginName;
        public string PluginName
        {
            set { _pluginName = value; }
            get { return _pluginName; }
        }

        private string _pluginID;
        public string PluginID
        {
            set { _pluginID = value; }
            get { return _pluginID; }
        }

        private string _pluginVersion;
        public string PluginVersion
        {
            set { _pluginVersion = value; }
            get { return _pluginVersion; }
        }

        private string _pluginState;
        public string PluginState
        {
            set { _pluginState = value; }
            get { return _pluginState; }
        }

        private string _pluginAuthor;
        public string PluginAuthor
        {
            set { _pluginAuthor = value; }
            get { return _pluginAuthor; }
        }

        private string _wikiUrl;
        public string WikiUrl
        {
            set { _wikiUrl = value; }
            get { return _wikiUrl; }
        }

        private string _description;
        public string Description
        {
            set { _description = value; }
            get { return _description; }
        }

        private string _destFolder;
        public string DestFolder
        {
            set { _destFolder = value; }
            get { return _destFolder; }
        }

        private string _mainFile;
        public string MainFile
        {
            set { _mainFile = value; }
            get { return _mainFile; }
        }

        private List<string> _additionalAssemblies = new List<string>();
        public List<string> AdditionalAssemblies
        {
            set { _additionalAssemblies = value; }
            get { return _additionalAssemblies; }
        }

        //jwelch -- added for the option of x64 bit specific assemblies
        private List<string> _x64Assemblies = new List<string>();
        public List<string> x64Assemblies
        {
            set { _x64Assemblies = value; }
            get { return _x64Assemblies; }
        }


    }
}
