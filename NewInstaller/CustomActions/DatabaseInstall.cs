namespace OSAInstallCustomActions
{
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Windows.Forms;


    public partial class DatabaseInstall : Form
    {
        MySqlConnection connection; 
        string current = "0.1.0";
        string newVersion = "0.4.3";
        string ConnectionStringRoot;
        string ConnectionStringOSAE;
        string directory;
        string machine = "";

        public DatabaseInstall(string dir, string mach)
        {
            directory = dir;
            machine = mach;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OSAE.ModifyRegistry myRegistry = new OSAE.ModifyRegistry();
            myRegistry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";
            
            string pass = myRegistry.Read("DBPASSWORD");
            string user = myRegistry.Read("DBUSERNAME");
            string port = myRegistry.Read("DBPORT");
            string server = myRegistry.Read("DBCONNECTION");

            if(pass != "")
                txbPassword.Text = myRegistry.Read("DBPASSWORD");
            if (user != "")
                txbUsername.Text = myRegistry.Read("DBUSERNAME");
            if (port != "") 
                txbPort.Text = myRegistry.Read("DBPORT");
            if (server != "") 
                txbServer.Text = myRegistry.Read("DBCONNECTION");


            if (machine == "Client")
            {
                lb_Progress.Visible = false;
                ProgressBar1.Visible = false;
                txbUsername.Visible = false;
                txbPassword.Visible = false;
                lbl1.Visible = false;
                lbl2.Visible = false;
                label1.Text = "Please enter the server address and MySql port of your Open Source Automation server.";
            }
            else
            {
                lb_Progress.Visible = true;
                ProgressBar1.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!btnClose.Visible && btnInstall.Text != "Close")
            {
                MessageBox.Show("Database did not install successfully.  Please make sure your MySql instance is running and re-run the Open Source Automation installer again.");
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (machine == "Client")
            {
                ConnectionStringRoot = string.Format("Uid=osae;Password=osaePass;Server={0};Port={1};", txbServer.Text, txbPort.Text);

                connection = new MySqlConnection(ConnectionStringRoot);
                try
                {
                    connection.Open();

                }
                catch (Exception ex)
                {
                    lblConnectResult.ForeColor = System.Drawing.Color.Red;
                    lblConnectResult.Text = "Connection failed. \nPlease make sure the OSA server running.";
                    return;
                }
                lblConnectResult.ForeColor = System.Drawing.Color.Green;
                lblConnectResult.Text = "Connection Successful.";
                btnInstall.Text = "Close";
                btnInstall.Visible = true;
                btnConnect.Enabled = false;
                OSAE.ModifyRegistry myRegistry = new OSAE.ModifyRegistry();
                myRegistry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";
                myRegistry.Write("DBUSERNAME", "osae");
                myRegistry.Write("DBPASSWORD", "osaePass");
                myRegistry.Write("DBCONNECTION", txbServer.Text);
                myRegistry.Write("DBPORT", txbPort.Text);

            }
            else
            {
                ConnectionStringRoot = string.Format("Uid={0};Password={1};Server={2};Port={3};", txbUsername.Text, txbPassword.Text, txbServer.Text, txbPort.Text);

                connection = new MySqlConnection(ConnectionStringRoot);
                try
                {
                    connection.Open();

                }
                catch (Exception ex)
                {
                    lblConnectResult.ForeColor = System.Drawing.Color.Red;
                    lblConnectResult.Text = "Connection failed. \nPlease check the settings \nand make sure MySql is running.";
                    return;
                }

                try
                {
                    MySqlCommand command;
                    MySqlDataAdapter adapter;
                    DataSet dataset = new DataSet();
                    command = new MySqlCommand("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'osae'", connection);
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dataset);
                    connection.Close();
                    int count = dataset.Tables[0].Rows.Count;

                    if (count == 1)
                    {
                        // DB found.  Need to upgrade.  First find out current version.
                        addToLog("Found OSA database.  Checking to see if we need to upgrade.");
                        dataset = new DataSet();
                        ConnectionStringOSAE = string.Format("Uid={0};Pwd={1};Server={2};Port={3};Database={4};allow user variables=true", "osae", "osaePass", txbServer.Text, txbPort.Text, "osae");
                        connection = new MySqlConnection(ConnectionStringOSAE);
                        connection.Open();
                        command = new MySqlCommand("select property_value from osae_object_property p inner join osae_object_type_property tp on p.object_type_property_id = tp.property_id inner join osae_object o on o.object_id = p.object_id where object_name = 'SYSTEM' and property_name = 'DB Version'", connection);
                        adapter = new MySqlDataAdapter(command);
                        adapter.Fill(dataset);
                        if (dataset.Tables[0].Rows[0][0].ToString() == "")
                            current = "0.1.0";
                        else
                            current = dataset.Tables[0].Rows[0][0].ToString();
                        if (current == newVersion)
                        {
                            lblConnectResult.ForeColor = System.Drawing.Color.Green;
                            lblConnectResult.Text = "Connection Successful. \nDatabase is up to date.";
                            btnInstall.Text = "Close";
                            btnInstall.Visible = true;
                            btnConnect.Enabled = false;
                        }
                        else
                        {
                            lblConnectResult.ForeColor = System.Drawing.Color.Green;
                            lblConnectResult.Text = "Connection Successful. \nClick to upgrade.";
                            btnInstall.Text = "Upgrade";
                            btnInstall.Visible = true;
                            btnConnect.Enabled = false;
                        }

                    }
                    else
                    {
                        lblConnectResult.ForeColor = System.Drawing.Color.Green;
                        lblConnectResult.Text = "Connection Successful. \nClick to install.";
                        btnInstall.Text = "Install";
                        btnInstall.Visible = true;
                        btnConnect.Enabled = false;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (btnInstall.Text == "Install")
            {
                try
                {
                    // DB not found.  Need to install.
                    addToLog("No OSA database found.  We need to install it.");

                    MySqlCommand command;
                    MySqlDataAdapter adapter;
                    DataSet dataset = new DataSet();
                    MySqlScript script;
                    command = new MySqlCommand("select User from mysql.user where User = 'osae'", connection);
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(dataset);
                    connection.Close();
                    int count = dataset.Tables[0].Rows.Count;
                    ProgressBar1.Value = 25;

                    if (count == 0)
                    {
                        script = new MySqlScript(connection, "CREATE USER `osae`@`%` IDENTIFIED BY 'osaePass';");
                        script.Execute();
                    }
                    ProgressBar1.Value = 50;
                    script = new MySqlScript(connection, "GRANT ALL ON osae.* TO `osae`@`%`;GRANT ALL ON osae.* TO `osae`@`%`;");
                    script.Execute();
                    ProgressBar1.Value = 75;
                    script = new MySqlScript(connection, File.ReadAllText(directory + "\\osae.sql"));
                    script.Execute();
                    connection.Close();

                    ProgressBar1.Style = ProgressBarStyle.Blocks;
                    ProgressBar1.Value = 100;

                    lblError.ForeColor = System.Drawing.Color.Green;
                    lblError.Text = "Success!";
                    btnClose.Visible = true;

                    OSAE.ModifyRegistry myRegistry = new OSAE.ModifyRegistry();
                    myRegistry.SubKey = "SOFTWARE\\OSAE\\DBSETTINGS";
                    myRegistry.Write("DBUSERNAME", txbUsername.Text);
                    myRegistry.Write("DBPASSWORD", txbPassword.Text);
                    myRegistry.Write("DBCONNECTION", txbServer.Text);
                    myRegistry.Write("DBPORT", txbPort.Text);
                }
                catch (Exception ex)
                {
                    lblError.ForeColor = System.Drawing.Color.Red;
                    lblError.Text = "Error installing database!";
                    ProgressBar1.Style = ProgressBarStyle.Blocks;
                }
            }
            else if (btnInstall.Text == "Upgrade")
            {
                upgrade();
            }
            else if (btnInstall.Text == "Close")
            {
                this.Close();
            }
        }
        
        private void addToLog(string log)
        {
            StreamWriter sw = File.AppendText("install_log.log");
            sw.WriteLine(System.DateTime.Now.ToString() + " - " + log);
            sw.Close();
        }

        private void txbServer_KeyDown(object sender, KeyEventArgs e)
        {
            btnInstall.Visible = false;
            btnConnect.Enabled = true;
        }

        private void txbPort_KeyDown(object sender, KeyEventArgs e)
        {
            btnInstall.Visible = false;
            btnConnect.Enabled = true;
        }

        private void txbUsername_KeyDown(object sender, KeyEventArgs e)
        {
            btnInstall.Visible = false;
            btnConnect.Enabled = true;
        }

        private void txbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            btnInstall.Visible = false;
            btnConnect.Enabled = true;
        }

        private void upgrade()
        {
            addToLog("Upgrading...");
            string[] version = current.Split('.');
            int major = Int32.Parse(version[0]);
            int minor = Int32.Parse(version[1]);
            int bug = Int32.Parse(version[2]);

            string[] updateScripts = Directory.GetFiles(directory, "*.sql", SearchOption.TopDirectoryOnly);
            List<string> scripts = new List<string>();
            
            foreach (string s in updateScripts)
            {
                if (s.Substring(directory.Length + 1).Contains("-"))
                {
                    string[] vers = s.Substring(directory.Length + 1).Split('-');
                    string[] nums = vers[0].Split('.');

                    if (Int32.Parse(nums[0]) >= major)
                    {
                        if (Int32.Parse(nums[1]) >= minor)
                        {
                            if (Int32.Parse(nums[2]) >= bug)
                            {
                                scripts.Add(s.Substring(directory.Length + 1));
                                addToLog("Found upgrade script: " + s.Substring(directory.Length + 1));
                            }
                        }
                    }
                }
            }
            scripts.Sort();
            int max = scripts.Count;
            int count = 1;
            foreach (string s in scripts)
            {
                try
                {
                    MySqlScript script = new MySqlScript(connection, File.ReadAllText(directory + "\\" + s));
                    //script.Delimiter = "$$";
                    script.Execute();
                    decimal percent = count / max;
                    ProgressBar1.Value = (int)Math.Round(percent, 0);
                    count++;
                }
                catch (Exception ex)
                {
                    addToLog("Upgrade script failed: " + ex.Message);
                }
            }
            connection.Close();
            lblError.Text = "Database upgraded successfully.";
            btnClose.Visible = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
