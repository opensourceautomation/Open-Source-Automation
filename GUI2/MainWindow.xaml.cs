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
using System.Threading;
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
        String gAppName = "";
        String gCurrentScreen = "";
        //ScreenObject aScreenObject(100) = new ScreenObject;
        List<ScreenObject> aScreenObject = new List<ScreenObject>();
        List<Image> aControlStateImage = new List<Image>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_App_Name();
            gCurrentScreen = OSAEApi.GetObjectPropertyValue(gAppName, "Default Screen").Value;
            if (gCurrentScreen == "")
            {
                Set_Default_Screen();
            }
            Load_Screen(gCurrentScreen);

            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //Update_Objects(gCurrentScreen);
            Thread thread = new Thread(() => Update_Objects(gCurrentScreen));
            thread.Start();
        }

        private void Load_Screen(String sScreen)
        {
        String sPath = "";
        Int32 iOldHeight = 0;
        gCurrentScreen = sScreen;
        OSAEApi.ObjectPropertySet(gAppName, "Current Screen", sScreen);
        sPath = OSAEApi.APIpath + OSAEApi.GetObjectPropertyValue(sScreen, "Background Image").Value;
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
            String sStateMatch = "";
            String sImage = "";


            List<OSAE.OSAEObject> screenObjects = OSAEApi.GetObjectsByContainer(sScreen);

            foreach (OSAE.OSAEObject obj in screenObjects)
            {
                ScreenObject sObj = new ScreenObject();
                #region CONTROL STATE IMAGE
                if (obj.Type == "CONTROL STATE IMAGE")
                {
                    sObj.Object_Name = obj.Property("Object Name").Value;
                    sObj.Control_Name = obj.Name;
                    sObj.Control_Type = obj.Type;
                    OSAE.OSAEObject osaObject = OSAEApi.GetObjectByName(sObj.Object_Name);
                    Image dsi = new Image();
                    dsi.Tag = sObj.Object_Name;
                    dsi.MouseLeftButtonUp += new MouseButtonEventHandler(State_Image_MouseLeftButtonUp);

                    sObj.Object_State_Time = obj.LastUpd;

                    foreach (OSAE.ObjectProperty p in obj.Properties)
                    {
                        if (p.Value.ToLower() == osaObject.State.Value.ToLower())
                        {
                            sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                        }
                    }

                    sImage = OSAEApi.APIpath + obj.Property(sStateMatch + " Image").Value;


                    OSAE.ObjectProperty pZOrder = obj.Property("ZOrder");
                    OSAE.ObjectProperty pX = obj.Property(sStateMatch + " X");
                    OSAE.ObjectProperty pY = obj.Property(sStateMatch + " Y");
                    sObj.Object_State = osaObject.State.Value;
                    sObj.ScreenImage = dsi;
                    aScreenObject.Add(sObj);
                    if (File.Exists(sImage))
                    {
                        canGUI.Children.Add(dsi);

                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(dsi, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(dsi, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(dsi, dZ);

                        byte[] byteArray = File.ReadAllBytes(sImage);
                        var imageStream = new MemoryStream(byteArray);
                        var bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = imageStream;
                        bitmapImage.EndInit();

                        dsi.Source = bitmapImage;

                        dsi.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        dsi.Source = null;
                        dsi.Visibility = System.Windows.Visibility.Hidden;
                    }
                    aScreenObject.Add(sObj);
                }
                #endregion

                #region CONTROL PROPERTY LABEL
                else if (obj.Type == "CONTROL PROPERTY LABEL")
                {
                    sObj.Object_Name = obj.Property("Object Name").Value;
                    String sPropertyName = obj.Property("Property Name").Value;
                    sObj.Property_Name = sPropertyName;
                    String sPropertyValue = OSAEApi.GetObjectPropertyValue(sObj.Object_Name, sPropertyName).Value;
                    String sBackColor = obj.Property("Back Color").Value;
                    String sForeColor = obj.Property("Fore Color").Value;
                    String sPrefix = obj.Property("Prefix").Value;
                    String sSuffix = obj.Property("Suffix").Value;
                    String iFontSize = obj.Property("Font Size").Value;
                    String sFontName = obj.Property("Font Name").Value;
                    Label dpl = new Label();
                    dpl.Tag = sObj.Object_Name;
                    OSAE.ObjectProperty pX = obj.Property("X");
                    OSAE.ObjectProperty pY = obj.Property("Y");
                    canGUI.Children.Add(dpl);
                    sObj.ScreenLabel = dpl;
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
                        sObj.Object_State = "";
                    }
                    else
                    {
                        dpl.Content = "";
                    }
                    aScreenObject.Add(sObj);
                }
                #endregion

                #region CONTROL STATIC LABEL
                else if (obj.Type == "CONTROL STATIC LABEL")
                {
                    String sPropertyValue = obj.Property("Value").Value;
                    String sBackColor = obj.Property("Back Color").Value;
                    String sForeColor = obj.Property("Fore Color").Value;
                    Label dsl = new Label();
                    OSAE.ObjectProperty pX = obj.Property("X");
                    OSAE.ObjectProperty pY = obj.Property("Y");
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
                #endregion

                #region CONTROL TIMER LABEL
                else if (obj.Type == "CONTROL TIMER LABEL")
                {
                    sObj.Object_Name = obj.Property("Object Name").Value;
                    OSAE.OSAEObject timerObject = OSAEApi.GetObjectByName(sObj.Object_Name);
                    String sBackColor = obj.Property("Back Color").Value;
                    String sForeColor = obj.Property("Font Color").Value;
                    Label dtl = new Label();
                    OSAE.ObjectProperty pX = obj.Property("X");
                    OSAE.ObjectProperty pY = obj.Property("Y");
                    canGUI.Children.Add(dtl);
                    Double dX = Convert.ToDouble(pX.Value);
                    Canvas.SetLeft(dtl, dX);
                    Double dY = Convert.ToDouble(pY.Value);
                    Canvas.SetTop(dtl, dY);

                    sObj.Object_State_Time = Convert.ToInt32(timerObject.State.TimeInState).ToString();
                    sObj.Object_Last_Updated = timerObject.LastUpd;

                    if (sBackColor != "")
                    {
                        try
                        {
                            BrushConverter conv = new BrushConverter();
                            SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                            dtl.Background = brush;
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
                            dtl.Foreground = brush;
                        }
                        catch (Exception myerror)
                        {
                        }
                    }

                }
                #endregion

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

                #region CONTROL NAVIGATION IMAGE
                else if (obj.Type == "CONTROL NAVIGATION IMAGE")
                {
                    sObj.Object_Name = obj.Property("Screen").Value;
                    sObj.Object_State = "";
                    Image dni = new Image();
                    dni.Tag = sObj.Object_Name;
                    dni.MouseLeftButtonUp += new MouseButtonEventHandler(Navigaton_Image_MouseLeftButtonUp);
                    try
                    {
                        sImage = obj.Property("Image").Value;
                        if (File.Exists(OSAEApi.APIpath + sImage))
                        {
                            sImage = OSAEApi.APIpath + sImage;
                        }
                        OSAE.ObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.ObjectProperty pX = obj.Property("X");
                        OSAE.ObjectProperty pY = obj.Property("Y");
                        if (File.Exists(sImage))
                        {
                            canGUI.Children.Add(dni);
                            Double dX = Convert.ToDouble(pX.Value);
                            Canvas.SetLeft(dni, dX);
                            Double dY = Convert.ToDouble(pY.Value);
                            Canvas.SetTop(dni, dY);
                            int dZ = Convert.ToInt32(pZOrder.Value);
                            Canvas.SetZIndex(dni, dZ);

                            byte[] byteArray = File.ReadAllBytes(sImage);
                            var imageStream = new MemoryStream(byteArray);
                            var bitmapImage = new BitmapImage();

                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = imageStream;
                            bitmapImage.EndInit();
                            dni.Source = bitmapImage;
                        }
                    }
                    catch (MySqlException myerror)
                    {
                        MessageBox.Show("GUI Error Load_Objects 5: " + myerror.Message);
                    }
                }
                #endregion

                #region CONTROL CAMERA VIEWER
                else if (obj.Type == "CONTROL CAMERA VIEWER")
                {
                    try
                    {
                        string stream = OSAEApi.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
                        OSAE.UI.Controls.VideoStreamViewer vsv = new OSAE.UI.Controls.VideoStreamViewer(stream);
                        canGUI.Children.Add(vsv);
                        OSAE.ObjectProperty pZOrder =obj.Property("ZOrder");
                        OSAE.ObjectProperty pX = obj.Property("X");
                        OSAE.ObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(vsv, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(vsv, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(vsv, dZ);

                    }
                    catch (MySqlException myerror)
                    {
                        MessageBox.Show("GUI Error Load Camera Viewer: " + myerror.Message);
                    }
                }
                #endregion

                #region USER CONTROL
                else if (obj.Type == "USER CONTROL")
                {
                    string sUCType = obj.Property("Control Type").Value;
                    if (sUCType == "USER CONTROL WEATHER")
                    {
                        OSAE.UI.Controls.Weather wc = new OSAE.UI.Controls.Weather();
                        canGUI.Children.Add(wc);
                        OSAE.ObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.ObjectProperty pX = obj.Property("X");
                        OSAE.ObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(wc, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(wc, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(wc, dZ);
                    }
                }
                #endregion
                
                //If iStateImageList.EndsWith(",") Then iStateImageList = iStateImageList.Substring(0, iStateImageList.Length - 1)
                //Timer1.Enabled = True
            }
        }


        private void Update_Objects(String sScreen)
        {
            List<OSAE.OSAEScreenControl> controls = OSAEApi.GetScreenControls(sScreen);

            foreach (ScreenObject dso in aScreenObject)
            {
                #region CONTROL STATE IMAGE
                if (dso.Control_Type == "CONTROL STATE IMAGE")
                {   
                    foreach (OSAE.OSAEScreenControl ctrl in controls)
                    {
                        if (ctrl.Object_Name == dso.Object_Name)
                        {
                            dso.Object_Last_Updated = ctrl.Object_Last_Updated;

                            if (ctrl.Object_State.ToLower() != dso.Object_State.ToLower())
                            {
                                dso.Object_State = ctrl.Object_State;
                                string sStateMatch = "";
                                OSAE.OSAEObject screenObject = OSAEApi.GetObjectByName(dso.Control_Name);
                                foreach (OSAE.ObjectProperty p in screenObject.Properties)
                                {
                                    if (p.Value.ToLower() == ctrl.Object_State.ToLower())
                                    {
                                        sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                                    }
                                }
                                String sImage = screenObject.Property(sStateMatch + " Image").Value;
                                if (File.Exists(OSAEApi.APIpath + sImage))
                                {
                                    sImage = OSAEApi.APIpath + sImage;
                                }
                                OSAE.ObjectProperty pZOrder = screenObject.Property("ZOrder");
                                OSAE.ObjectProperty pX = screenObject.Property(sStateMatch + " X");
                                OSAE.ObjectProperty pY = screenObject.Property(sStateMatch + " Y");

                                if (File.Exists(sImage))
                                {
                                    Double dX = Convert.ToDouble(pX.Value);
                                    Double dY = Convert.ToDouble(pY.Value);
                                    int dZ = Convert.ToInt32(pZOrder.Value);
                                    byte[] byteArray = File.ReadAllBytes(sImage);
                                    var imageStream = new MemoryStream(byteArray);


                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        Canvas.SetLeft(dso.ScreenImage, dX);
                                        Canvas.SetTop(dso.ScreenImage, dY);
                                        Canvas.SetZIndex(dso.ScreenImage, dZ);
                                        var bitmapImage = new BitmapImage();
                                        bitmapImage.BeginInit();
                                        bitmapImage.StreamSource = imageStream;
                                        bitmapImage.EndInit();
                                        dso.ScreenImage.Source = bitmapImage;
                                    }));
                                }
                            }
                        }
                    }
                }
                #endregion

                #region CONTROL PROPERTY LABEL
                else if (dso.Control_Type == "CONTROL PROPERTY LABEL")
                {
                    dso.Object_Name = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Object Name").Value;
                    String sPropertyName = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Property Name").Value;
                    dso.Property_Name = sPropertyName;
                    String sPropertyValue = OSAEApi.GetObjectPropertyValue(dso.Object_Name, sPropertyName).Value;
                    String sBackColor = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Back Color").Value;
                    String sForeColor = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Fore Color").Value;
                    String sPrefix = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Prefix").Value;
                    String sSuffix = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Suffix").Value;
                    String iFontSize = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Font Size").Value;
                    String sFontName = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Font Name").Value;
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
                #endregion


            }
            
        }

        private void Load_App_Name()
        {
            
            List<OSAE.OSAEObject> screens = OSAEApi.GetObjectsByType("GUI CLIENT");
            foreach (OSAE.OSAEObject obj in screens)
            {
                if (obj.Property("Computer Name").Value == OSAEApi.ComputerName)
                    gAppName = obj.Name;
            }
            if (gAppName == "")
            {
                gAppName = "GUI CLIENT-" + OSAEApi.ComputerName;
                OSAEApi.ObjectAdd(gAppName, gAppName, "GUI CLIENT", "", "SYSTEM", true);
                OSAEApi.ObjectPropertySet(gAppName, "Computer Name", OSAEApi.ComputerName);
            }
        }


        private void Set_Default_Screen()
        {
            List<OSAE.OSAEObject> screens = OSAEApi.GetObjectsByType("SCREEN");
            if (screens.Count > 0)
            {
                gCurrentScreen = screens[0].Name;
                OSAEApi.ObjectPropertySet(gAppName, "Default Screen", gCurrentScreen);
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

        private void State_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            Image iImage = (Image)sender;
            bool iResults = false;

            foreach (ScreenObject dso in aScreenObject)
            {
                if (dso.Object_Name == iImage.Tag)
                {
                    if (dso.Object_State.ToUpper() == "ON")
                    {
                        List<string> methods = OSAEApi.GetObjectByName(dso.Object_Name).Methods;
                        foreach(string m in methods)
                        {
                            if(m == "ON")
                                iResults = true;
                        }
                        if (iResults)
                            OSAEApi.MethodQueueAdd(dso.Object_Name, "OFF", "", "");
                        else
                            OSAEApi.ObjectStateSet(dso.Object_Name, "OFF");
                    }
                    else
                    {
                        List<string> methods = OSAEApi.GetObjectByName(dso.Object_Name).Methods;
                        foreach (string m in methods)
                        {
                            if (m == "OFF")
                                iResults = true;
                        }
                        if (iResults)
                            OSAEApi.MethodQueueAdd(dso.Object_Name, "ON", "", "");
                        else
                            OSAEApi.ObjectStateSet(dso.Object_Name, "ON");
                    }
                }
            }
        }

        private void Navigaton_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            Image iImage = (Image)sender;
            gCurrentScreen = iImage.Tag.ToString();
            aScreenObject.Clear();
            canGUI.Children.Clear();
            Load_Screen(iImage.Tag.ToString());
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
