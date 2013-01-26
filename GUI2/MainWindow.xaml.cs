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
        List<ScreenObject> aScreenObject = new List<ScreenObject>();

        List<OSAE.UI.Controls.StateImage> stateImages = new List<OSAE.UI.Controls.StateImage>();
        List<OSAE.UI.Controls.NavigationImage> navImages = new List<OSAE.UI.Controls.NavigationImage>();
        List<OSAE.UI.Controls.VideoStreamViewer> cameraViewers = new List<OSAE.UI.Controls.VideoStreamViewer>();

        private Point _startPoint;
        DragAdorner _adorner = null;
        AdornerLayer _layer;
        private bool _dragHasLeftScope = false;

        private bool _isDragging;
        public bool IsDragging
        {
            get { return _isDragging; }
            set { _isDragging = value; }
        }

        FrameworkElement _dragScope;
        public FrameworkElement DragScope
        {
            get { return _dragScope; }
            set { _dragScope = value; }
        }
        private dynamic beingDragged;
        private List<Type> controlTypes = new List<Type>();

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

            this.canGUI.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
            this.canGUI.Drop += new DragEventHandler(DragSource_Drop);
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
                    OSAE.OSAEObject osaObject = OSAEApi.GetObjectByName(obj.Property("Object Name").Value);
                    OSAE.UI.Controls.StateImage stateImageControl = new OSAE.UI.Controls.StateImage(obj, osaObject);
                    
                    foreach (OSAE.ObjectProperty p in obj.Properties)
                    {
                        if (p.Value.ToLower() == osaObject.State.Value.ToLower())
                        {
                            sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                        }
                    }
                    OSAE.ObjectProperty pZOrder = obj.Property("ZOrder");
                    OSAE.ObjectProperty pX = obj.Property(sStateMatch + " X");
                    OSAE.ObjectProperty pY = obj.Property(sStateMatch + " Y");

                    canGUI.Children.Add(stateImageControl);

                    Double dX = Convert.ToDouble(pX.Value);
                    Canvas.SetLeft(stateImageControl, dX);
                    Double dY = Convert.ToDouble(pY.Value);
                    Canvas.SetTop(stateImageControl, dY);
                    int dZ = Convert.ToInt32(pZOrder.Value);
                    Canvas.SetZIndex(stateImageControl, dZ);

                    stateImageControl.Location.X = dX;
                    stateImageControl.Location.Y = dY;
                    stateImages.Add(stateImageControl);
                    controlTypes.Add(typeof(OSAE.UI.Controls.StateImage));
                    stateImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
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
                    try
                    {
                        OSAE.UI.Controls.NavigationImage navImageControl = new OSAE.UI.Controls.NavigationImage(obj.Property("Screen").Value, obj.Property("Image").Value, obj);
                        navImageControl.MouseLeftButtonUp += new MouseButtonEventHandler(Navigaton_Image_MouseLeftButtonUp); 
                        canGUI.Children.Add(navImageControl);
                        
                        OSAE.ObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.ObjectProperty pX = obj.Property("X");
                        OSAE.ObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(navImageControl, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(navImageControl, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(navImageControl, dZ);
                        navImageControl.Location.X = dX;
                        navImageControl.Location.Y = dY;
                        navImages.Add(navImageControl);
                        controlTypes.Add(typeof(OSAE.UI.Controls.NavigationImage));
                        navImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    {
                        MessageBox.Show("GUI Error Load Navigation Image: " + myerror.Message);
                    }
                }
                #endregion

                #region CONTROL CAMERA VIEWER
                else if (obj.Type == "CONTROL CAMERA VIEWER")
                {
                    try
                    {
                        string stream = OSAEApi.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
                        OSAE.UI.Controls.VideoStreamViewer vsv = new OSAE.UI.Controls.VideoStreamViewer(stream, obj);
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
                        vsv.Location.X = dX;
                        vsv.Location.Y = dY;
                        cameraViewers.Add(vsv);
                        controlTypes.Add(typeof(OSAE.UI.Controls.VideoStreamViewer));
                        vsv.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
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
                        OSAE.UI.Controls.Weather wc = new OSAE.UI.Controls.Weather(obj);
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

                        wc.Location.X = dX;
                        wc.Location.Y = dY;
                        controlTypes.Add(typeof(OSAE.UI.Controls.Weather));
                        wc.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                }
                #endregion
                
                //If iStateImageList.EndsWith(",") Then iStateImageList = iStateImageList.Substring(0, iStateImageList.Length - 1)
                //Timer1.Enabled = True
            }
        }


        private void Update_Objects(String sScreen)
        {
            //List<OSAE.OSAEScreenControl> controls = OSAEApi.GetScreenControls(sScreen);

            foreach (OSAE.OSAEScreenControl ctrl in controls)
            {
                #region CONTROL STATE IMAGE
                if (ctrl.Control_Type == "CONTROL STATE IMAGE")
                {
                    foreach (OSAE.UI.Controls.StateImage sImage in stateImages)
                    {
                        if (ctrl.Object_Name == sImage.ObjectName)
                        {
                            if (ctrl.Object_State.ToLower() != sImage.ObjectState.ToLower())
                            {
                                sImage.ObjectState = ctrl.Object_State;
                                sImage.Update();
                                String sStateMatch = "";

                                foreach (OSAE.ObjectProperty p in sImage.screenObject.Properties)
                                {
                                    if (p.Value.ToLower() == ctrl.Object_State.ToLower())
                                    {
                                        sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                                    }
                                }
                                OSAE.ObjectProperty pZOrder = sImage.screenObject.Property("ZOrder");
                                OSAE.ObjectProperty pX = sImage.screenObject.Property(sStateMatch + " X");
                                OSAE.ObjectProperty pY = sImage.screenObject.Property(sStateMatch + " Y");

                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    Double dX = Convert.ToDouble(pX.Value);
                                    Canvas.SetLeft(sImage, dX);
                                    Double dY = Convert.ToDouble(pY.Value);
                                    Canvas.SetTop(sImage, dY);
                                    int dZ = Convert.ToInt32(pZOrder.Value);
                                    Canvas.SetZIndex(sImage, dZ);
                                    sImage.Location.X = dX;
                                    sImage.Location.Y = dY;
                                }));
                            }
                        }
                    }
                }
                #endregion


            }
            

                //#region CONTROL PROPERTY LABEL
                //else if (dso.Control_Type == "CONTROL PROPERTY LABEL")
                //{
                //    dso.Object_Name = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Object Name").Value;
                //    String sPropertyName = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Property Name").Value;
                //    dso.Property_Name = sPropertyName;
                //    String sPropertyValue = OSAEApi.GetObjectPropertyValue(dso.Object_Name, sPropertyName).Value;
                //    String sBackColor = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Back Color").Value;
                //    String sForeColor = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Fore Color").Value;
                //    String sPrefix = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Prefix").Value;
                //    String sSuffix = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Suffix").Value;
                //    String iFontSize = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Font Size").Value;
                //    String sFontName = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Font Name").Value;
                //    OSAE.ObjectProperty pX = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "X");
                //    OSAE.ObjectProperty pY = OSAEApi.GetObjectPropertyValue(dso.Control_Name, "Y");
                //    Double dX = Convert.ToDouble(pX.Value);
                //    Canvas.SetLeft(dso.ScreenLabel, dX);
                //    Double dY = Convert.ToDouble(pY.Value);
                //    Canvas.SetTop(dso.ScreenLabel, dY);
                //    try
                //    {
                //        if (sPropertyValue != "")
                //        {
                //            if (sBackColor != "")
                //            {
                //                BrushConverter conv = new BrushConverter();
                //                SolidColorBrush brush = conv.ConvertFromString(sBackColor) as SolidColorBrush;
                //                dso.ScreenLabel.Background = brush;
                //            }
                //            if (sForeColor != "")
                //            {
                //                BrushConverter conv = new BrushConverter();
                //                SolidColorBrush brush = conv.ConvertFromString(sForeColor) as SolidColorBrush;
                //                dso.ScreenLabel.Foreground = brush;
                //            }
                //            if (iFontSize != "")
                //            {
                //                dso.ScreenLabel.FontSize = Convert.ToDouble(iFontSize);
                //            }
                //            dso.ScreenLabel.Content = sPrefix + sPropertyValue + sSuffix;
                //            dso.Object_State = "";
                //        }
                //        else
                //        {
                //            dso.ScreenLabel.Content = "";
                //        }
                //    }
                //    catch (Exception myerror)
                //    {
                //    }
                //}
                //#endregion


            
            
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

        private void canvas1_RightButtonDown(object sender, RoutedEventArgs e)
        {
            //mnuMain. = true;
        }

        private void Navigaton_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            OSAE.UI.Controls.NavigationImage navCtrl = (OSAE.UI.Controls.NavigationImage)sender;
            gCurrentScreen = navCtrl.screenName;
            aScreenObject.Clear();
            canGUI.Children.Clear();
            Load_Screen(gCurrentScreen);
        }

        #region Drag Events
        void DragSource_Drop(dynamic sender, DragEventArgs e)
        {
            IDataObject data = e.Data;

            if (data.GetDataPresent(DataFormats.Text))
            {
                int newX = Convert.ToInt32(e.GetPosition(DragScope).X - (beingDragged.ActualWidth / 2));
                int newY = Convert.ToInt32(e.GetPosition(DragScope).Y - (beingDragged.ActualHeight / 2));

                Canvas.SetLeft(beingDragged, newX);
                Canvas.SetTop(beingDragged, newY);

                beingDragged.Location = new Point(newX, newY);
                
                if(beingDragged.GetType() == typeof(OSAE.UI.Controls.StateImage))
                {
                    string stateMatch = "";

                    foreach (OSAE.ObjectProperty p in beingDragged.screenObject.Properties)
                    {
                        if (p.Value.ToLower() == beingDragged.ObjectState.ToLower())
                        {
                            stateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                        }
                        double a;

                        OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, stateMatch + " X", newX.ToString());
                        OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, stateMatch + " Y", newY.ToString());
                    }
                }
                else if (beingDragged.GetType() == typeof(OSAE.UI.Controls.NavigationImage))
                {
                    OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, "X", newX.ToString());
                    OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, "Y", newY.ToString());
                }
                else if (beingDragged.GetType() == typeof(OSAE.UI.Controls.VideoStreamViewer))
                {
                    OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, "X", newX.ToString());
                    OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, "Y", newY.ToString());
                }
                else if (beingDragged.GetType() == typeof(OSAE.UI.Controls.Weather))
                {
                    OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, "X", newX.ToString());
                    OSAEApi.ObjectPropertySet(beingDragged.screenObject.Name, "Y", newY.ToString());
                }
            }
        }

        void DragSource_PreviewMouseMove(dynamic sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                foreach (Type t in controlTypes)
                {
                    if (t == sender.GetType())
                    {
                        Point position = e.GetPosition(null);

                        double width = sender.ActualWidth;
                        double height = sender.ActualHeight;
                        double x = sender.Location.X;
                        double y = sender.Location.Y;
                        
                        if (
                                (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance) 
                            &&
                                (sender.Location.X < _startPoint.X && _startPoint.X < (sender.Location.X + sender.ActualWidth)) 
                            &&
                                (sender.Location.Y < _startPoint.Y && _startPoint.Y < (sender.Location.Y + sender.ActualHeight))
                           )
                        {
                            beingDragged = (UIElement)sender;
                            StartDragInProcAdorner(e, sender);
                        }
                    }
                }
            }
        }

        void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(DragScope);
        }

        private void StartDragInProcAdorner(MouseEventArgs e, dynamic sender)
        {

            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            DragScope = this.canGUI as FrameworkElement;
            System.Diagnostics.Debug.Assert(DragScope != null);

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;

            // Let's wire our usual events.. 
            // GiveFeedback just tells it to use no standard cursors..  

            //GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
            //this.DragSource.GiveFeedback += feedbackhandler;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(Window1_DragOver);
            DragScope.DragOver += draghandler;

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;

            // QueryContinue Drag goes with drag leave... 
            QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            DragScope.QueryContinueDrag += queryhandler;

            //Here we create our adorner.. 
            _adorner = new DragAdorner(DragScope, (UIElement)sender, true, 0.5);
            _layer = AdornerLayer.GetAdornerLayer(DragScope as Visual);
            _layer.Add(_adorner);


            IsDragging = true;
            _dragHasLeftScope = false;
            //Finally lets drag drop 
            DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            DragDropEffects de = DragDrop.DoDragDrop(this.canGUI, data, DragDropEffects.Move);

            // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;
            AdornerLayer.GetAdornerLayer(DragScope).Remove(_adorner);
            _adorner = null;

            //DragSource.GiveFeedback -= feedbackhandler;
            DragScope.DragLeave -= dragleavehandler;
            DragScope.QueryContinueDrag -= queryhandler;


            IsDragging = false;
        }

        void Window1_DragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                _adorner.LeftOffset = args.GetPosition(DragScope).X /* - _startPoint.X */ ;
                _adorner.TopOffset = args.GetPosition(DragScope).Y /* - _startPoint.Y */ ;
            }
        }

        void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            if (e.OriginalSource == DragScope)
            {
                Point p = e.GetPosition(DragScope);
                Rect r = VisualTreeHelper.GetContentBounds(DragScope);
                if (!r.Contains(p))
                {
                    this._dragHasLeftScope = true;
                    e.Handled = true;
                }
            }

        }

        void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this._dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }

        }

        #endregion
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
