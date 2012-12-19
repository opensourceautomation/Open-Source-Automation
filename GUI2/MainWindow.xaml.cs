using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using System.Windows.Threading;
//using System.Drawing;
namespace GUI2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OSAE.OSAE OSAEApi = new OSAE.OSAE("GUI");
        MySqlConnection CN = new MySqlConnection();
        MySqlConnection CN2 = new MySqlConnection();
        String gAppName = "";
        String gCurrentScreen = "";
        //ScreenObject aScreenObject(100) = new ScreenObject;
        List<ScreenObject> aScreenObject = new List<ScreenObject>();
        List<Image> aControlStateImage = new List<Image>();
     DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CN.ConnectionString = OSAE.API.Common.ConnectionString;
            CN2.ConnectionString = OSAE.API.Common.ConnectionString;
           
            Load_App_Name();
            gCurrentScreen = OSAEApi.GetObjectProperty(gAppName, "Default Screen");
        if (gCurrentScreen == "")
        {
            Set_Default_Screen();
        }
        Load_Screen(gCurrentScreen);
        timer.Interval = TimeSpan.FromSeconds(5);
        timer.Tick += new EventHandler(timer_Tick);
        timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Update_Objects(gCurrentScreen);
            timer.Start();
        }

        private void Load_Screen(String sScreen)
        {
        String sPath = "";
        Int32 iOldHeight = 0;
        gCurrentScreen = sScreen;
        OSAEApi.ObjectPropertySet(gAppName, "Current Screen", sScreen);
        sPath = OSAEApi.APIpath + OSAEApi.GetObjectProperty(sScreen, "Background Image");
        if (gCurrentScreen != sScreen)
            { 
                gCurrentScreen = sScreen;
            }
        if (File.Exists(sPath))
            {
                byte[] byteArray = File.ReadAllBytes(sPath);
                var imageStream = new MemoryStream(byteArray);
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
                canGUI.Background = new ImageBrush(bitmapImage);
                canGUI.Height = bitmapImage.Height + 34;
                canGUI.Width = bitmapImage.Width + 12;
            }
            ///Controls_Clear()
        Load_Objects(gCurrentScreen);
            //Load_User_Controls(gCurrentScreen)
        }


        private void Load_Objects(String sScreen)
        {
            MySqlCommand CMD = new MySqlCommand();
         //   MySqlDataReader myReader = new MySqlDataReader();
        String sState = "";
        String sStateMatch = "";
        String sImage = ""; 


        CMD.Connection = CN;
        CMD.CommandType = CommandType.Text;

        //COUNT **ALL** control objects for this screen
        //CMD.CommandText = "SELECT COUNT(*) as Results FROM osae_v_screen_object WHERE screen_name=?pscreen";
        //CMD.Parameters.AddWithValue("?pscreen", sScreen);
        //try
        //{
        //    CN.Open();
        //    iObjectCount = Convert.ToInt32(CMD.ExecuteScalar());
        //    CN.Close();
        //}
        //catch (Exception myerror)
        //{
        //    MessageBox.Show("GUI Error Load_Objects 1: " + myerror.Message);
        //    CN.Close();
        //}
        CMD.Parameters.Clear();
        //Select **ALL** control objects for this screen
        CMD.CommandText = "SELECT * FROM osae_v_object_property WHERE object_id IN(SELECT control_id FROM osae_v_screen_object WHERE screen_name=?pscreen) AND property_name='ZOrder' ORDER BY property_value";
        CMD.Parameters.AddWithValue("?pscreen", sScreen);
        try
        {
             CN.Open();
            MySqlDataReader myReader = CMD.ExecuteReader();
            while (myReader.Read()) 
            {
                ScreenObject so = new ScreenObject(); 
                so.Control_Name = Convert.ToString(myReader.GetString("object_name"));
                so.Control_Type = Convert.ToString(myReader.GetString("object_type"));
                aScreenObject.Add(so);
            }
            CN.Close();
        }
        catch (MySqlException myerror)
        {
            MessageBox.Show("GUI Error Load_Objects 2: " + myerror.Message);
            CN.Close();
        }
        CMD.Parameters.Clear();
        foreach (ScreenObject dso in aScreenObject)
        {
            if (dso.Control_Type == "CONTROL STATE IMAGE")
            {
                dso.Object_Name = OSAEApi.GetObjectProperty(dso.Control_Name, "Object Name");
                Image dsi = new Image();
                dsi.Tag = dso.Object_Name;
                dsi.MouseLeftButtonDown += new MouseButtonEventHandler(State_Image_MouseLeftButtonDown);
                sState = OSAEApi.GetObjectState(dso.Object_Name);
                CMD.Parameters.Clear();
                CMD.CommandText = "SELECT COALESCE(last_state_change,NOW()) FROM osae_v_object WHERE object_name=?ObjectName";
                CMD.Parameters.AddWithValue("?ObjectName", dso.Object_Name);
                try
                {
                    CN.Open();
                    dso.Object_State_Time = Convert.ToString(CMD.ExecuteScalar());
                    CN.Close();
                }
                catch (Exception myerror)
                {
                    MessageBox.Show("GUI Error Load_Objects 2.5: " + myerror.Message);
                    CN.Close();
                }
                CMD.Parameters.Clear();
                CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName AND property_value=?pstate";
                CMD.Parameters.AddWithValue("?ObjectName", dso.Control_Name);
                CMD.Parameters.AddWithValue("?pstate", sState);
                try
                {
                    CN.Open();
                    sStateMatch = Convert.ToString(CMD.ExecuteScalar());
                    CN.Close();
                    if (sStateMatch != "")
                    {
                        sStateMatch = sStateMatch.Substring(0, 7);
                    }
                    sImage = OSAEApi.GetObjectProperty(dso.Control_Name, sStateMatch + " Image");
                    if (File.Exists(OSAEApi.APIpath + sImage))
                    {
                        sImage = OSAEApi.APIpath + sImage;
                    }

                    OSAE.ObjectProperty pZOrder = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "ZOrder");
                    OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, sStateMatch + " X");
                    OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, sStateMatch + " Y");
                    dso.Object_State = sState;
                    dso.ScreenImage = dsi;
                    if (File.Exists(sImage))
                    {
                        canGUI.Children.Add(dsi);
                        
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(dsi, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(dsi, dY);
                        
                        byte[] byteArray = File.ReadAllBytes(sImage);
                        var imageStream = new MemoryStream(byteArray);
                        var bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = imageStream;
                        bitmapImage.EndInit();
                        //canGUI.Background = new ImageBrush(bitmapImage);

                        dsi.Source = bitmapImage;

                        if (pZOrder.Value == "0")
                        {
                            // dsi.SendToBack();
                        }
                        else
                        {
                            // dsi.BringToFront();
                        }
                        dsi.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        dsi.Source = null;
                        dsi.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                catch (Exception myerror)
                {
                    MessageBox.Show("GUI Error Load_Objects 3: " + myerror.Message);
                    CN.Close();
                }
            }
            else if (dso.Control_Type == "CONTROL PROPERTY LABEL")
            {
                dso.Object_Name = OSAEApi.GetObjectProperty(dso.Control_Name, "Object Name");
                String sPropertyName = OSAEApi.GetObjectProperty(dso.Control_Name, "Property Name");
                dso.Property_Name = sPropertyName;
                String sPropertyValue = OSAEApi.GetObjectProperty(dso.Object_Name, sPropertyName);
                String sBackColor = OSAEApi.GetObjectProperty(dso.Control_Name, "Back Color");
                String sForeColor = OSAEApi.GetObjectProperty(dso.Control_Name, "Fore Color");
                String sPrefix = OSAEApi.GetObjectProperty(dso.Control_Name, "Prefix");
                String sSuffix = OSAEApi.GetObjectProperty(dso.Control_Name, "Suffix");
                String iFontSize = OSAEApi.GetObjectProperty(dso.Control_Name, "Font Size");
                String sFontName = OSAEApi.GetObjectProperty(dso.Control_Name, "Font Name");
                Label dpl = new Label();
                dpl.Tag = dso.Object_Name;
                OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "X");
                OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Y");
                canGUI.Children.Add(dpl);
                dso.ScreenLabel = dpl;
                Double dX = Convert.ToDouble(pX.Value);
                Canvas.SetLeft(dpl, dX);
                Double dY = Convert.ToDouble(pY.Value);
                Canvas.SetTop(dpl, dY);
                if (sPropertyValue != "")
                {
                    if (sBackColor != "")
                    {
                        try
                        {
                            BrushConverter conv = new BrushConverter();
                            SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                            dpl.Background = brush;
                        }
                        catch (Exception myerror)
                        {
                        }
                    }
                    if (sForeColor != "")
                    {
                        try
                        {
                            BrushConverter conv = new BrushConverter();
                            SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                            dpl.Foreground = brush;
                        }
                        catch (Exception myerror)
                        {
                        }
                    }
                    if (iFontSize != "")
                    {
                        try
                        {
                            dpl.FontSize = Convert.ToDouble(iFontSize);
                        }
                        catch (Exception myerror)
                        {
                        }
                    }
                    dpl.Content = sPrefix + sPropertyValue + sSuffix;
                    dso.Object_State = "";
                }
                else
                {
                    dpl.Content = "";
                }
            }
            else if (dso.Control_Type == "CONTROL STATIC LABEL") 
            {
                dso.Object_Name = OSAEApi.GetObjectProperty(dso.Control_Name, "Object Name");
                String sPropertyValue = OSAEApi.GetObjectProperty(dso.Object_Name, "Value");
                String sBackColor = OSAEApi.GetObjectProperty(dso.Control_Name, "Back Color");
                String sForeColor = OSAEApi.GetObjectProperty(dso.Control_Name, "Fore Color");
                Label dsl = new Label();
                dsl.Tag = dso.Object_Name;
                OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "X");
                OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Y");
                canGUI.Children.Add(dsl);
                Double dX = Convert.ToDouble(pX.Value);
                Canvas.SetLeft(dsl, dX);
                Double dY = Convert.ToDouble(pY.Value);
                Canvas.SetTop(dsl, dY);
                if (sPropertyValue != "")
                {
                    if (sBackColor != "")
                    {
                        try
                        {
                            BrushConverter conv = new BrushConverter();
                            SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                            dsl.Background = brush;
                        }
                        catch (Exception myerror)
                        {
                        }
                    }
                    if (sForeColor != "")
                    {
                        try
                        {
                            BrushConverter conv = new BrushConverter();
                            SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                            dsl.Foreground = brush;
                        }
                        catch (Exception myerror)
                        {
                        }
                    }
                    dsl.Content = sPropertyValue;
                }      
                else
                {
                    dsl.Content = "";
                }
            }
        
            //    else if aScreenObject(iLoop).Control_Type = "CONTROL TIMER LABEL" Then
            //        iTimerLabelCount = iTimerLabelCount + 1
            //        aScreenObject(iLoop).Control_Index = iTimerLabelCount
            //        aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Object Name")
            //        sPropertyName = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Type")
            //        aScreenObject(iLoop).Property_Name = sPropertyName
            //        sPropertyValue = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Object_Name, "OFF Timer")
            //        aScreenObject(iLoop).Property_Value = sPropertyValue
            //        sBackColor = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Back Color")
            //        sForeColor = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Fore Color")
            //        aControlTimerLabel(iTimerLabelCount).Tag = iLoop
            //        sState = OSAEApi.GetObjectState(aScreenObject(iLoop).Object_Name)
            //        aScreenObject(iLoop).Object_State = sState
            //        CMD.Parameters.Clear()

            //        CMD.CommandText = "SELECT COALESCE(last_updated,NOW()) FROM osae_v_object WHERE object_name=?ObjectName"
            //        CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name)
            //        try
            //        {
            //            CN.Open();
            //            aScreenObject(iLoop).Object_Last_Updated = CMD.ExecuteScalar;
            //            CN.Close();
            //        }
            //        catch (MySqlException myerror)
            //        {
            //            MessageBox.Show("GUI Error Load_Objects 2.7: " & myerror.Message);
            //            CN.Close();
            //        }
            //        CMD.Parameters.Clear();
            //        CMD.CommandText = "SELECT COALESCE(last_state_change,NOW()) FROM osae_v_object WHERE object_name=?ObjectName";
            //        CMD.Parameters.AddWithValue("?ObjectName", aScreenObject(iLoop).Object_Name);
            //        try
            //        {
            //            CN.Open();
            //            aScreenObject(iLoop).Object_State_Time = CMD.ExecuteScalar;
            //            CN.Close();
            //        }
            //        catch (MySqlException myerror)
            //        {
            //            MessageBox.Show("GUI Error Load_Objects 666: " & myerror.Message);
            //            CN.Close();
            //        }
            //        iX = Val("" & OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "X"));
            //        iY = Val("" & OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Y"));
            //        if (sBackColor <> "")
            //        {
            //            try
            //            {
            //                aControlTimerLabel(aScreenObject(iLoop).Control_Index).BackColor = Color.FromName(sBackColor);
            //            }
            //            catch (MySqlException myerror)
            //            {}
            //        }
            //        if (sForeColor <> "")
            //                    {
            //            try
            //            {
            //                aControlTimerLabel(aScreenObject(iLoop).Control_Index).ForeColor = Color.FromName(sForeColor)
            //            }
            //        catch (MySqlException myerror)
            //        {}
            //            }
            //        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Text = sPropertyValue;
            //        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Width = sPropertyValue.Length * 7;
            //        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Left = iX;
            //        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Top = iY;
            //        aControlTimerLabel(aScreenObject(iLoop).Control_Index).BringToFront();
            //        aControlTimerLabel(aScreenObject(iLoop).Control_Index).Visible = True;
            //    else if (aScreenObject(iLoop).Control_Type = "CONTROL METHOD IMAGE")
            //        iMethodImageCount = iMethodImageCount + 1
            //        aScreenObject(iLoop).Object_Name = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Object Name")
            //        aScreenObject(iLoop).Control_Index = iMethodImageCount
            //        aControlMethodImage(aControlMethodImage.Count).Tag = iLoop
            //        g_toolTip.SetToolTip(aControlMethodImage(iMethodImageCount), aScreenObject(iLoop).Object_Name)
            //        CMD.Parameters.Clear()
            //        try
            //        {
            //            sImage = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Image");
            //            sImage = sImage.Replace(".\", "\");
            //            If File.Exists(gAppPath & sImage) Then sImage = gAppPath & sImage
            //            iZOrder = Val(OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "ZOrder"));
            //            iX = Val("" & OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "X"));
            //            iY = Val("" & OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Y"));
            //            if (File.Exists(sImage))
            //            {
            //                aControlMethodImage(aScreenObject(iLoop).Control_Index).Image = Image.FromFile(sImage);
            //                aScreenObject(iLoop).Object_State = ""
            //                aControlMethodImage(aScreenObject(iLoop).Control_Index).Left = iX;
            //                aControlMethodImage(aScreenObject(iLoop).Control_Index).Top = iY;
            //                aControlMethodImage(aScreenObject(iLoop).Control_Index).Visible = True;
            //            }
            //            else
            //            {
            //                aControlMethodImage(aScreenObject(iLoop).Control_Index).Image = Nothing;
            //                aControlMethodImage(aScreenObject(iLoop).Control_Index).Visible = False;
            //            }
            //        }
            //         catch (MySqlException myerror)
            //        {
            //            MessageBox.Show("GUI Error Load_Objects 4: " + myerror.Message);
            //            CN.Close();
            //         }
            else if (dso.Control_Type == "CONTROL NAVIGATION IMAGE")
            {
                    dso.Object_Name = OSAEApi.GetObjectProperty(dso.Control_Name, "Screen");
                    dso.Object_State = "";
                    Image dni = new Image();
                    dni.Tag = dso.Object_Name;
                    //aControlNavImage(iNavImageCount).Tag = iLoop
                   // g_toolTip.SetToolTip(aControlNavImage(iNavImageCount), aScreenObject(iLoop).Object_Name)
                    CMD.Parameters.Clear();
                    try
                    {
                        sImage = OSAEApi.GetObjectProperty(dso.Control_Name, "Image");
                        if (File.Exists(OSAEApi.APIpath + sImage))
                        {
                            sImage = OSAEApi.APIpath + sImage;
                        }
                        OSAE.ObjectProperty pZOrder = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "ZOrder");
                        OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "X");
                        OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Y");
                        if (File.Exists(sImage))
                        {
                            canGUI.Children.Add(dni);
                            Double dX = Convert.ToDouble(pX.Value);
                            Canvas.SetLeft(dni, dX);
                            Double dY = Convert.ToDouble(pY.Value);
                            Canvas.SetTop(dni, dY);

                            byte[] byteArray = File.ReadAllBytes(sImage);
                            var imageStream = new MemoryStream(byteArray);
                            var bitmapImage = new BitmapImage();

                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = imageStream;
                            bitmapImage.EndInit();
                        //canGUI.Background = new ImageBrush(bitmapImage);

                            dni.Source = bitmapImage;

                            if (pZOrder.Value == "0")
                            {
                                // dsi.SendToBack();
                            }
                            else
                            {
                                // dsi.BringToFront();
                            }
                     }
                   }
                    catch (MySqlException myerror)
                    {
                        MessageBox.Show("GUI Error Load_Objects 5: " + myerror.Message);
                        CN.Close();
                    }
            }
            
            //    ElseIf aScreenObject(iLoop).Control_Type = "USER CONTROL" Then
            //        iUserControlCount += 1
            //        Dim sUCType As String = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Control Type")
            //                 If sUCType = "USER CONTROL WEATHER" Then
            //            Me.Controls.Add(New ucWeather)
            //            aScreenObject(iLoop).Control_Index = Me.Controls.Count - 1
            //            Me.Controls(aScreenObject(iLoop).Control_Index).Top = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "Y")
            //            Me.Controls(aScreenObject(iLoop).Control_Index).Left = OSAEApi.GetObjectProperty(aScreenObject(iLoop).Control_Name, "X")
            //            Me.Controls(aScreenObject(iLoop).Control_Index).BringToFront()

            //            '
            //        End If
            //    End If
            //If iStateImageList.EndsWith(",") Then iStateImageList = iStateImageList.Substring(0, iStateImageList.Length - 1)
            //Timer1.Enabled = True
        }
        }


        private void Update_Objects(String sScreen)
        {
            MySqlCommand CMD = new MySqlCommand();
            MySqlCommand CMD2 = new MySqlCommand();

            CMD.Connection = CN;
            CMD.CommandType = CommandType.Text;
            CMD2.Connection = CN2;
            CMD2.CommandType = CommandType.Text;
            CMD2.CommandText = "SELECT object_name,control_type,state_name,last_state_change FROM osae_v_screen_updates WHERE screen_name=?ScreenName AND last_updated > subtime(now(),'00:00:05')";
            CMD2.Parameters.AddWithValue("?ScreenName", gCurrentScreen);
            try
            {
                CN2.Open();
                MySqlDataReader myReader = CMD2.ExecuteReader();
                while (myReader.Read())
                {
                    foreach (ScreenObject dso in aScreenObject)
                    {
                        if (dso.Object_Name == myReader.GetString("object_name"))
                        {
                            if (myReader.GetString("control_type") == "CONTROL STATE IMAGE")
                            {
                                String sState = Convert.ToString(myReader.GetString("state_name"));
                                dso.Object_State_Time = myReader.GetString("last_state_change");

                                // '            sPropertyBlock = Read_Properties(aScreenObject(iLoop).Object_Name)
                                // ' g_toolTip.SetToolTip(aControlStateImage(aScreenObject(iLoop).Control_Index), aScreenObject(iLoop).Object_Name & " " & sState)

                                if (sState != dso.Object_State)
                                {
                                    dso.Object_State = sState;
                                    CMD.Parameters.Clear();
                                    CMD.CommandText = "SELECT property_name FROM osae_v_object_property WHERE object_name=?ObjectName AND property_value=?pstate";
                                    CMD.Parameters.AddWithValue("?ObjectName", dso.Control_Name);
                                    CMD.Parameters.AddWithValue("?pstate", sState);
                                    try
                                    {
                                        CN.Open();
                                        String sStateMatch = Convert.ToString(CMD.ExecuteScalar());
                                        CN.Close();
                                        if (sStateMatch != "")
                                        {
                                            sStateMatch = sStateMatch.Substring(0, 7);
                                        }
                                        String sImage = OSAEApi.GetObjectProperty(dso.Control_Name, sStateMatch + " Image");
                                        if (File.Exists(OSAEApi.APIpath + sImage))
                                        {
                                            sImage = OSAEApi.APIpath + sImage;
                                        }
                                        OSAE.ObjectProperty pZOrder = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "ZOrder");
                                        OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, sStateMatch + " X");
                                        OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, sStateMatch + " Y");
                                        dso.Object_State = sState;
                                        if (File.Exists(sImage))
                                        {
                                            Double dX = Convert.ToDouble(pX.Value);
                                            Canvas.SetLeft(dso.ScreenImage, dX);
                                            Double dY = Convert.ToDouble(pY.Value);
                                            Canvas.SetTop(dso.ScreenImage, dY);
                                            //dsi.Tag = canGUI.Children.Count;
                                            byte[] byteArray = File.ReadAllBytes(sImage);
                                            var imageStream = new MemoryStream(byteArray);
                                            var bitmapImage = new BitmapImage();

                                            bitmapImage.BeginInit();
                                            bitmapImage.StreamSource = imageStream;
                                            bitmapImage.EndInit();
                                            //canGUI.Background = new ImageBrush(bitmapImage);

                                            dso.ScreenImage.Source = bitmapImage;
                                        }
                                        if (pZOrder.Value == "0")
                                        {
                                            // dsi.SendToBack();
                                        }
                                        else
                                        {
                                            // dsi.BringToFront();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("GUI Error Update_Objects 1: " + ex.Message);
                                        CN.Close();
                                    }
                                }
                            }
                            else if (dso.Control_Type == "CONTROL PROPERTY LABEL")
                            {
                                dso.Object_Name = OSAEApi.GetObjectProperty(dso.Control_Name, "Object Name");
                                String sPropertyName = OSAEApi.GetObjectProperty(dso.Control_Name, "Property Name");
                                dso.Property_Name = sPropertyName;
                                String sPropertyValue = OSAEApi.GetObjectProperty(dso.Object_Name, sPropertyName);
                                String sBackColor = OSAEApi.GetObjectProperty(dso.Control_Name, "Back Color");
                                String sForeColor = OSAEApi.GetObjectProperty(dso.Control_Name, "Fore Color");
                                String sPrefix = OSAEApi.GetObjectProperty(dso.Control_Name, "Prefix");
                                String sSuffix = OSAEApi.GetObjectProperty(dso.Control_Name, "Suffix");
                                String iFontSize = OSAEApi.GetObjectProperty(dso.Control_Name, "Font Size");
                                String sFontName = OSAEApi.GetObjectProperty(dso.Control_Name, "Font Name");
                                OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "X");
                                OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Y");
                                Double dX = Convert.ToDouble(pX.Value);
                                Canvas.SetLeft(dso.ScreenLabel, dX);
                                Double dY = Convert.ToDouble(pY.Value);
                                Canvas.SetTop(dso.ScreenLabel, dY);
                                try
                                {
                                    if (sPropertyValue != "")
                                    {
                                        if (sBackColor != "")
                                        {
                                            BrushConverter conv = new BrushConverter();
                                            SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                                            dso.ScreenLabel.Background = brush;
                                        }
                                        if (sForeColor != "")
                                        {
                                            BrushConverter conv = new BrushConverter();
                                            SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                                            dso.ScreenLabel.Foreground = brush;
                                        }
                                        if (iFontSize != "")
                                        {
                                            dso.ScreenLabel.FontSize = Convert.ToDouble(iFontSize);
                                        }
                                        dso.ScreenLabel.Content = sPrefix + sPropertyValue + sSuffix;
                                        dso.Object_State = "";
                                    }
                                    else
                                    {
                                        dso.ScreenLabel.Content = "";
                                    }
                                }
                                catch (Exception myerror)
                                {
                                }
                            }
                        }
                    }
                }
            CN2.Close();
            }
            catch (MySqlException ex)
            {
                    MessageBox.Show("GUI Error Update_Objects 2: " + ex.Message);
                    CN2.Close();
            }
        }

        private void Load_App_Name()
        {
            MySqlCommand CMD = new MySqlCommand();
            CMD.Connection = CN;
            CMD.CommandType = System.Data.CommandType.Text;
            CMD.CommandText = "SELECT object_name FROM osae_v_object_property WHERE object_type='GUI CLIENT' AND property_name='Computer Name' AND property_value='" + OSAEApi.ComputerName + "'";
            try
            {
                CN.Open();
                gAppName = "" + (String)CMD.ExecuteScalar();
                CN.Close();
                if (gAppName == "")
                {
                    gAppName = "GUI CLIENT-" + OSAEApi.ComputerName;
                    OSAEApi.ObjectAdd(gAppName, gAppName, "GUI CLIENT", "", "SYSTEM", true);
                    OSAEApi.ObjectPropertySet(gAppName, "Computer Name", OSAEApi.ComputerName);
                }
                ///AddToLog("found my Object Name: " + gAppName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error messaging host: " + ex.Message);
            }
        }


        private void Set_Default_Screen()
        {
            MySqlCommand CMD = new MySqlCommand();
            CMD.Connection = CN;
            CMD.CommandType = CommandType.Text;
            CMD.CommandText = "SELECT object_name FROM osae_v_object WHERE base_type='SCREEN' LIMIT 1";
            try
            {
                CN.Open();
                gCurrentScreen = (String)CMD.ExecuteScalar();
                OSAEApi.ObjectPropertySet(gAppName, "Default Screen", gCurrentScreen);
                CN.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error Set_Default_Screen(): " + ex.Message);
                CN.Close();
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void canvas1_MouseDown(object sender, RoutedEventArgs e)
        {
        }

        private void canvas1_RightButtonDown(object sender, RoutedEventArgs e)
        {
            //mnuMain. = true;
        }

        private void State_Image_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Image iImage = (Image)sender;
            MySqlCommand CMD = new MySqlCommand();
            CMD.Connection = CN;
            CMD.CommandType = CommandType.Text;
            //outputStatus = DirectCast(sender, PictureBox).Tag
            foreach (ScreenObject dso in aScreenObject)
            {
                if (dso.Object_Name == iImage.Tag)
                {
                    if (dso.Object_State == "ON")
                    {
                        CMD.CommandText = "SELECT COUNT(*) FROM osae_v_object_method WHERE object_name='" + dso.Object_Name + "' AND method_name='OFF'";
                        try
                        {
                            CN.Open();
                            Int32 iResults = Convert.ToInt32(CMD.ExecuteScalar());
                            CN.Close();
                            if (iResults > 0)
                            {
                                OSAEApi.MethodQueueAdd(dso.Object_Name, "OFF", "", "");
                            }
                            else
                            {
                                OSAEApi.ObjectStateSet(dso.Object_Name, "OFF");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error ClickControlStateImage: " + ex.Message);
                            CN.Close();
                        }
                    }
                    else
                    {
                        CMD.CommandText = "SELECT COUNT(*) FROM osae_v_object_method WHERE object_name='" + dso.Object_Name + "' AND method_name='ON'";
                        try
                        {
                            CN.Open();
                            Int32 iResults = Convert.ToInt32(CMD.ExecuteScalar());
                            CN.Close();
                            if (iResults > 0)
                            {
                                OSAEApi.MethodQueueAdd(dso.Object_Name, "ON", "", "");
                            }
                            else
                            {
                                OSAEApi.ObjectStateSet(dso.Object_Name, "ON");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error ClickControlStateImage: " + ex.Message);
                            CN.Close();
                        }
                    }
                }
            }
        }

    }

    public class ScreenObject
    {
        public string Control_Name { get; set; }
        public string Control_Type { get; set; }
        public string Object_Name { get; set; }
        public string Property_Name { get; set; }
        public string Property_Value { get; set; }
        public string Object_State { get; set; }
        public string Object_State_Time { get; set; }
        public string Object_Last_Updated { get; set; }
        public Image ScreenImage { get; set; }
        public Label ScreenLabel { get; set; }
    }
}
