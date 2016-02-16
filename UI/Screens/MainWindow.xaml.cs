namespace Screens
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Timers;
    using MySql.Data.MySqlClient;
    using OSAE;
    using OSAE.UI.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OSAE.General.OSAELog Log;
        public string gAppName = "";
        public int gAppMinTrust = 50;
        public string gAppLocation = "";
        public string gCurrentScreen = "";
        public string gCurrentUser = "";
        public int gCurrentUserTrust = 0;
        public bool gDebug = false;

        private UserSelector userSelectorControl;
        private ScreenObjectList screenObjectControl;

        List<StateImage> stateImages = new List<StateImage>();
        List<NavigationImage> navImages = new List<NavigationImage>();
        List<ClickImage> clickImages = new List<ClickImage>();
        List<VideoStreamViewer> cameraViewers = new List<VideoStreamViewer>();
        List<PropertyLabel> propLabels = new List<PropertyLabel>();
        List<StaticLabel> staticLabels = new List<StaticLabel>();
        List<TimerLabel> timerLabels = new List<TimerLabel>();
        List<dynamic> userControls = new List<dynamic>();
        List<dynamic> browserFrames = new List<dynamic>();
        
        bool loadingScreen = true;
        bool updatingScreen = false;
        bool editMode = false;
        //bool closing = false;
        System.Timers.Timer _timer;
        
        #region drag and drop properties
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
        #endregion

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                GlobalUserControls.OSAEUserControls.FindPlugins(OSAE.Common.ApiPath + @"\UserControls");
            }
            catch (Exception ex)
            { MessageBox.Show("Error Loading User Controls!"); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_App_Name();
            Log = new OSAE.General.OSAELog(gAppName);
            Uri uri = new Uri("pack://siteoforigin:,,,/OSA.png");
            var bitmapImage = new BitmapImage(uri);
            
            canGUI.Background = new ImageBrush(bitmapImage);
            canGUI.Height = bitmapImage.Height;
            canGUI.Width = bitmapImage.Width;

            menuEditMode.IsChecked = false;
            menuEditMode.IsEnabled = false;
            menuShowControlList.IsChecked = false;
            menuShowControlList.IsEnabled = false;
            menuCreateScreen.IsEnabled = false;
            menuAddControl.IsEnabled = false;
         
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true; // Enable it

            gCurrentScreen = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Default Screen").Value;
            gDebug = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Debug").Value);
            if (gCurrentScreen == "") Set_Default_Screen();

            Load_Screen(gCurrentScreen);

            OSAE.OSAEObjectStateManager.ObjectStateSet(gAppName, "ON", gAppName);

            canGUI.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
            canGUI.Drop += new DragEventHandler(DragSource_Drop);
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Thread t888 = new Thread(() => Update_Objects());
            t888.Start();
        }

        public void Load_Screen(string sScreen)
        {
            _timer.Stop();
            try
            {
                loadingScreen = true;
                if (updatingScreen)
                {
                    System.Threading.Thread.Sleep(1000);
                    updatingScreen = false;
                }
                stateImages.Clear();
                propLabels.Clear();
                navImages.Clear();
                clickImages.Clear();
                cameraViewers.Clear();
                canGUI.Children.Clear();
                browserFrames.Clear();
                controlTypes.Clear();
                string titl = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Title").Value;
                Title = titl + " - " + sScreen;
                gCurrentScreen = sScreen;
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Current Screen", sScreen, gCurrentUser);
                OSAE.OSAEImageManager imgMgr = new OSAE.OSAEImageManager();
                string imgID = OSAEObjectPropertyManager.GetObjectPropertyValue(sScreen, "Background Image").Value;
                OSAE.OSAEImage img = imgMgr.GetImage(imgID);

                if (img.Data != null)
                {
                    var imageStream = new MemoryStream(img.Data);
                    var bitmapImage = new BitmapImage();

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = imageStream;
                    bitmapImage.EndInit();
                    canGUI.Background = new ImageBrush(bitmapImage);
                    canGUI.Height = bitmapImage.Height;
                    canGUI.Width = bitmapImage.Width;
                }

                Load_Objects(sScreen);
                loadingScreen = false;
              //  tvControls
            }
            catch (Exception ex)
            { Log.Error("Failed to load screen: " + sScreen, ex); }

            Log.Info("Loaded Screen:  " + sScreen);
            _timer.Start();
        }

        private void Load_Objects(String sScreen)
        {
            bool FoundUserControl = OSAEObjectManager.ObjectExists(sScreen + " - User Selector");
            if (!FoundUserControl)
            {
                OSAEObjectManager.ObjectAdd(sScreen + " - User Selector", "", sScreen + " - User Selector", "CONTROL USER SELECTOR", "", sScreen, 50, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sScreen + " - User Selector", "X", (canGUI.Width - userSelectorControl.Width).ToString(),gAppName);
                OSAEObjectPropertyManager.ObjectPropertySet(sScreen + " - User Selector", "Y", "0", gAppName);
            }

            bool FoundObjectList = OSAEObjectManager.ObjectExists(sScreen + " - Screen Objects");
            if (!FoundObjectList)
            {
                OSAEObjectManager.ObjectAdd(sScreen + " - Screen Objects", "", sScreen + " - Screen Objects", "CONTROL SCREEN OBJECTS", "", sScreen, 50, true);
                OSAEObjectPropertyManager.ObjectPropertySet(sScreen + " - Screen Objects", "X", "0", gAppName);
                OSAEObjectPropertyManager.ObjectPropertySet(sScreen + " - Screen Objects", "Y", "0", gAppName);
            }

            OSAEObjectCollection screenObjects = OSAEObjectManager.GetObjectsByContainer(sScreen);
            foreach (OSAE.OSAEObject obj in screenObjects)
                LoadControl(obj);
        }

        private void Update_Objects()
        {
            try
            {
                if (loadingScreen || updatingScreen)
                {
                    updatingScreen = false;
                    return;
                }
                updatingScreen = true;
                if (gDebug) Log.Debug("Checking for updates on:  " + gCurrentScreen);
                bool oldCtrl = false;

                Dispatcher.Invoke((Action)(() =>
                {
                    if (screenObjectControl.Visibility == System.Windows.Visibility.Hidden && menuShowControlList.IsChecked) menuShowControlList.IsChecked = false;
                }));

                if (gCurrentUser != userSelectorControl._CurrentUser)
                {
                    gCurrentUser = userSelectorControl._CurrentUser;
                    gCurrentUserTrust = userSelectorControl._CurrentUserTrust;
                    Dispatcher.Invoke((Action)(() =>
                    {
                        if (gCurrentUser == "" || gCurrentUserTrust < gAppMinTrust)
                        {
                            menuEditMode.IsChecked = false;
                            menuEditMode.IsEnabled = false;
                            menuShowControlList.IsChecked = false;
                            menuShowControlList.IsEnabled = false;
                            menuCreateScreen.IsEnabled = false;
                            menuAddControl.IsEnabled = false;
                        }
                        else
                        {
                            menuEditMode.IsEnabled = true;
                            menuShowControlList.IsEnabled = true;
                            menuCreateScreen.IsEnabled = true;
                            menuAddControl.IsEnabled = true;
                        }
                       
                    }));
                }
                List<OSAE.OSAEScreenControl> controls = OSAEScreenControlManager.GetScreenControls(gCurrentScreen);
                foreach (OSAE.OSAEScreenControl newCtrl in controls)
                {
                    oldCtrl = false;

                    #region CONTROL STATE IMAGE
                    if (newCtrl.ControlType == "CONTROL STATE IMAGE")
                    {
                        foreach (StateImage sImage in stateImages)
                        {
                            if (newCtrl.ControlName == sImage.screenObject.Name)
                            {
                                if (newCtrl.LastUpdated != sImage.LastUpdated)
                                {
                                    sImage.LastUpdated = newCtrl.LastUpdated;
                                    try
                                    {
                                        sImage.Update();
                                    }
                                    catch
                                    { }
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        Canvas.SetLeft(sImage, sImage.Location.X);
                                        Canvas.SetTop(sImage, sImage.Location.Y);
                                        sImage.Opacity = Convert.ToDouble(sImage.LightLevel) / 100.00;
                                    }));
                                    if (gDebug) Log.Debug("Updated:  " + newCtrl.ControlName);
                                }
                                if (newCtrl.PropertyLastUpdated != sImage.PropertyLastUpdated)
                                {
                                    sImage.PropertyLastUpdated = newCtrl.PropertyLastUpdated;
                                    sImage.Update();
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        Canvas.SetLeft(sImage, sImage.Location.X);
                                        Canvas.SetTop(sImage, sImage.Location.Y);
                                        sImage.Opacity = Convert.ToDouble(sImage.LightLevel) / 100.00;
                                    }));
                                }
                                oldCtrl = true;
                            }
                        }
                    }
                    #endregion

                    #region CONTROL PROPERTY LABEL
                    else if (newCtrl.ControlType == "CONTROL PROPERTY LABEL")
                    {
                        foreach (PropertyLabel pl in propLabels)
                        {
                            if (newCtrl.ControlName == pl.screenObject.Name)
                            {
                                if (newCtrl.PropertyLastUpdated >= pl.LastUpdated)
                                {
                                    pl.LastUpdated = newCtrl.PropertyLastUpdated;
                                    pl.Update("Full");
                                    if (gDebug) Log.Debug("Updated:  " + newCtrl.ControlName);
                                    oldCtrl = true;
                                }
                                else
                                {
                                    pl.Update("Refresh");
                                    oldCtrl = true;
                                }
                            }
                        }
                    }
                    #endregion

                    #region CONTROL TIMER LABEL
                    else if (newCtrl.ControlType == "CONTROL TIMER LABEL")
                    {
                        foreach (OSAE.UI.Controls.TimerLabel tl in timerLabels)
                        {
                            if (newCtrl.ControlName == tl.screenObject.Name)
                            {
                                if (newCtrl.LastUpdated != tl.LastUpdated)
                                {
                                    tl.LastUpdated = newCtrl.LastUpdated;
                                    tl.Update();
                                    if (gDebug) Log.Debug("Updated:  " + newCtrl.ControlName);
                                }
                                oldCtrl = true;
                            }
                        }
                    }
                    #endregion

                    #region CONTROL STATIC LABEL
                    else if (newCtrl.ControlType == "CONTROL STATIC LABEL")
                    {
                        foreach (OSAE.UI.Controls.StaticLabel sl in staticLabels)
                        {
                            if (newCtrl.ControlName == sl.screenObject.Name) oldCtrl = true;
                        }
                    }
                    #endregion

                    #region CONTROL NAVIGATION IMAGE
                    else if (newCtrl.ControlType == "CONTROL NAVIGATION IMAGE")
                    {
                        foreach (OSAE.UI.Controls.NavigationImage nav in navImages)
                        {
                            if (newCtrl.ControlName == nav.screenObject.Name) oldCtrl = true;
                        }
                    }
                    #endregion

                    #region CONTROL CLICK IMAGE
                    else if (newCtrl.ControlType == "CONTROL METHOD IMAGE")
                    {
                        foreach (OSAE.UI.Controls.ClickImage method in clickImages)
                        {
                            if (newCtrl.ControlName == method.screenObject.Name) oldCtrl = true;
                        }
                    }
                    #endregion

                    #region CONTROL CAMERA VIEWER
                    else if (newCtrl.ControlType == "CONTROL CAMERA VIEWER")
                    {
                        foreach (OSAE.UI.Controls.VideoStreamViewer vsv in cameraViewers)
                        {
                            if (newCtrl.ControlName == vsv.screenObject.Name) oldCtrl = true;
                        }
                    }
                    #endregion

                    #region CONTROL USER CONTROL
                    else if (newCtrl.ControlType.Contains("USER CONTROL"))
                    {
                        foreach (dynamic obj in userControls)
                        {
                            if (newCtrl.ControlName == obj.screenObject.Name)
                            {
                                if (newCtrl.LastUpdated != obj.LastUpdated)
                                {
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        obj.LastUpdated = newCtrl.LastUpdated;
                                        obj.Update();
                                    }));
                                }
                                oldCtrl = true;
                            }
                        }
                    }
                    #endregion

                    #region CONTROL BROWSER
                    else if (newCtrl.ControlType == "CONTROL BROWSER")
                    {
                        foreach (BrowserFrame oBrowser in browserFrames)
                        {
                           if (newCtrl.ControlName == oBrowser.screenObject.Name)
                            {
                             //   if (newCtrl.LastUpdated != oBrowser.LastUpdated)
                            //    {
                             //       this.Log.Debug("Updating:  " + newCtrl.ControlName);
                                    //sImage.LastUpdated = newCtrl.LastUpdated;
                             //       try
                             //       {
                                    // sImage.Update();
                             ///       }
                              //      catch (Exception ex)
                               //     { }
                               //     this.Dispatcher.Invoke((Action)(() =>
                              //          {
                              //              Canvas.SetLeft(oBrowser, oBrowser.Location.X);
                              //              Canvas.SetTop(oBrowser, oBrowser.Location.Y);
                               //         }));
                               //         this.Log.Debug("Complete:  " + newCtrl.ControlName);
                               //     }
                                oldCtrl = true;
                            }
                        }
                    }
                    #endregion

                    if (!oldCtrl)
                    {
                        OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(newCtrl.ControlName);
                        if (gDebug) Log.Debug("Load new control: " + newCtrl.ControlName);
                        LoadControl(obj);
                    }
                }
            }
            catch (Exception ex)
            { Log.Error("Error in Update!", ex); }
            updatingScreen = false;
        }

        private void LoadControl(OSAE.OSAEObject obj)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                string sStateMatch = "";

                #region USER SELECTOR
                if (obj.Type == "CONTROL USER SELECTOR")
                {
                    userSelectorControl = new UserSelector(obj, gAppName);
                  //  userSelectorControl.MouseRightButtonDown += new MouseButtonEventHandler(State_Image_MouseRightButtonDown);
                    userSelectorControl.Location.X = Double.Parse(obj.Property("X").Value);
                    userSelectorControl.Location.Y = Double.Parse(obj.Property("Y").Value);
                    userSelectorControl._ScreenLocation = gAppLocation;
                    canGUI.Children.Add(userSelectorControl);
                    Canvas.SetLeft(userSelectorControl, userSelectorControl.Location.X);
                    Canvas.SetTop(userSelectorControl, userSelectorControl.Location.Y);
                    Canvas.SetZIndex(userSelectorControl, 5);
                    controlTypes.Add(typeof(UserSelector));
                    userSelectorControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                }
                #endregion

                #region SCREEN OBJECTS
                if (obj.Type == "CONTROL SCREEN OBJECTS")
                {
                    screenObjectControl = new ScreenObjectList(obj,gCurrentScreen,gCurrentUser);
              //      screenObjectControl.MouseRightButtonDown += new MouseButtonEventHandler(State_Image_MouseRightButtonDown);
                    screenObjectControl.Location.X = Double.Parse(obj.Property("X").Value);
                    screenObjectControl.Location.Y = Double.Parse(obj.Property("Y").Value);
                    screenObjectControl._ScreenLocation = gAppLocation;
                    screenObjectControl.Visibility = System.Windows.Visibility.Hidden;
                    canGUI.Children.Add(screenObjectControl);
                    Canvas.SetLeft(screenObjectControl, screenObjectControl.Location.X);
                    Canvas.SetTop(screenObjectControl, screenObjectControl.Location.Y);
                    Canvas.SetZIndex(screenObjectControl, 5);
                    controlTypes.Add(typeof(ScreenObjectList));
                    screenObjectControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                }
                #endregion

                #region CONTROL STATE IMAGE
                if (obj.Type == "CONTROL STATE IMAGE")
                {
                    StateImage stateImageControl = new StateImage(obj, gAppName);

                    foreach (OSAE.OSAEObjectProperty p in obj.Properties)
                    {
                        try
                        {
                            if (p.Value.ToLower() == stateImageControl.CurState.ToLower())
                                sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Error finding object ", ex);
                            return;
                        }
                    }
                    try
                    {
                        int dZ = Int32.Parse(obj.Property("ZOrder").Value);
                        stateImageControl.MouseRightButtonDown += new MouseButtonEventHandler(State_Image_MouseRightButtonDown);
                        stateImageControl.Location.X = Double.Parse(obj.Property(sStateMatch + " X").Value);
                        stateImageControl.Location.Y = Double.Parse(obj.Property(sStateMatch + " Y").Value);
                        double dOpacity = Convert.ToDouble(stateImageControl.LightLevel) / 100.00;
                        //Opacity is new and unknow in 044
                        stateImageControl.Opacity = dOpacity;
                        canGUI.Children.Add(stateImageControl);
                        Canvas.SetLeft(stateImageControl, stateImageControl.Location.X);
                        Canvas.SetTop(stateImageControl, stateImageControl.Location.Y);
                        Canvas.SetZIndex(stateImageControl, dZ);
                        stateImages.Add(stateImageControl);
                        controlTypes.Add(typeof(StateImage));
                        stateImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error updating screenObject", ex);
                        return;
                    }
                }
                #endregion

                #region CONTROL PROPERTY LABEL
                else if (obj.Type == "CONTROL PROPERTY LABEL")
                {
                    if (gDebug) Log.Debug("Loading PropertyLabelControl: " + obj.Name);
                    try
                    {
                        PropertyLabel pl = new PropertyLabel(obj);
                        pl.MouseRightButtonDown += new MouseButtonEventHandler(Property_Label_MouseRightButtonDown);
                        canGUI.Children.Add(pl);
                        int dZ = Int32.Parse(obj.Property("ZOrder").Value);
                        pl.Location.X = Double.Parse(obj.Property("X").Value);
                        pl.Location.Y = Double.Parse(obj.Property("Y").Value);
                        Canvas.SetLeft(pl, pl.Location.X);
                        Canvas.SetTop(pl, pl.Location.Y);
                        Canvas.SetZIndex(pl, dZ);
                        propLabels.Add(pl);
                        controlTypes.Add(typeof(PropertyLabel));
                        pl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error updating PropertyLabelControl", ex);
                        return;
                    }
                }
                #endregion

                #region CONTROL STATIC LABEL
                else if (obj.Type == "CONTROL STATIC LABEL")
                {
                    if (gDebug) Log.Debug("Loading PropertyLabelControl: " + obj.Name);
                    try
                    {

                        OSAE.UI.Controls.StaticLabel sl = new OSAE.UI.Controls.StaticLabel(obj);
                        canGUI.Children.Add(sl);
                        int dZ = Int32.Parse(obj.Property("ZOrder").Value);
                        sl.Location.X = Double.Parse(obj.Property("X").Value);
                        sl.Location.Y = Double.Parse(obj.Property("Y").Value);
                        Canvas.SetLeft(sl, sl.Location.X);
                        Canvas.SetTop(sl, sl.Location.Y);
                        Canvas.SetZIndex(sl, dZ);
                        staticLabels.Add(sl);
                        controlTypes.Add(typeof(OSAE.UI.Controls.StaticLabel));
                        sl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }

                    catch (Exception ex)
                    {
                            Log.Error("Error updating PropertyLabelControl", ex);
                            return;
                    }
                }
                #endregion

                #region CONTROL TIMER LABEL
                else if (obj.Type == "CONTROL TIMER LABEL")
                {
                    if (gDebug) Log.Debug("Loading PropertyTimerControl: " + obj.Name);
                    try
                    {

                        OSAE.UI.Controls.TimerLabel tl = new OSAE.UI.Controls.TimerLabel(obj);
                        tl.MouseRightButtonDown += new MouseButtonEventHandler(Timer_Label_MouseRightButtonDown);
                        canGUI.Children.Add(tl);
                        int dZ = Int32.Parse(obj.Property("ZOrder").Value);
                        tl.Location.X = Double.Parse(obj.Property("X").Value);
                        tl.Location.Y = Double.Parse(obj.Property("Y").Value);
                        Canvas.SetLeft(tl, tl.Location.X);
                        Canvas.SetTop(tl, tl.Location.Y);
                        Canvas.SetZIndex(tl, dZ);
                        timerLabels.Add(tl);
                        controlTypes.Add(typeof(OSAE.UI.Controls.TimerLabel));
                        tl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error updating PropertyTimerControl", ex);
                        return;
                    }
                }
                #endregion

                #region CONTROL CLICK IMAGE
                else if (obj.Type == "CONTROL CLICK IMAGE")
                {
                    try
                    {
                        ClickImage ClickImageControl = new ClickImage(obj, gAppName,gCurrentUser);
                        ClickImageControl.MouseRightButtonDown += new MouseButtonEventHandler(Click_Image_MouseRightButtonDown);
                        canGUI.Children.Add(ClickImageControl);

                        OSAE.OSAEObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.OSAEObjectProperty pX = obj.Property("X");
                        OSAE.OSAEObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(ClickImageControl, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(ClickImageControl, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(ClickImageControl, dZ);
                        ClickImageControl.Location.X = dX;
                        ClickImageControl.Location.Y = dY;
                        clickImages.Add(ClickImageControl);
                        controlTypes.Add(typeof(ClickImage));
                        ClickImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    { MessageBox.Show("GUI Error Load Click Image: " + myerror.Message); }
                }
                #endregion

                #region CONTROL NAVIGATION IMAGE
                else if (obj.Type == "CONTROL NAVIGATION IMAGE")
                {
                    try
                    {
                        NavigationImage navImageControl = new NavigationImage(obj.Property("Screen").Value, obj);
                        navImageControl.MouseLeftButtonUp += new MouseButtonEventHandler(Navigaton_Image_MouseLeftButtonUp);
                        navImageControl.MouseRightButtonDown += new MouseButtonEventHandler(Navigaton_Image_MouseRightButtonDown);
                        
                        canGUI.Children.Add(navImageControl);

                        OSAE.OSAEObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.OSAEObjectProperty pX = obj.Property("X");
                        OSAE.OSAEObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(navImageControl, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(navImageControl, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(navImageControl, dZ);
                        navImageControl.Location.X = dX;
                        navImageControl.Location.Y = dY;
                        navImages.Add(navImageControl);
                        controlTypes.Add(typeof(NavigationImage));
                        navImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    { MessageBox.Show("GUI Error Load Navigation Image: " + myerror.Message); }
                }
                #endregion

                #region CONTROL CAMERA VIEWER
                else if (obj.Type == "CONTROL CAMERA VIEWER")
                {
                    try
                    {
                        string stream = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
                        VideoStreamViewer vsv = new VideoStreamViewer(stream, obj);
                        vsv.MouseRightButtonDown += new MouseButtonEventHandler(VideoStreamViewer_MouseRightButtonDown);
                        canGUI.Children.Add(vsv);
                        OSAE.OSAEObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.OSAEObjectProperty pX = obj.Property("X");
                        OSAE.OSAEObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(vsv, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(vsv, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(vsv, dZ);
                        vsv.Location.X = dX;
                        vsv.Location.Y = dY;
                        cameraViewers.Add(vsv);
                        controlTypes.Add(typeof(VideoStreamViewer));
                        vsv.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    { MessageBox.Show("GUI Error Load Camera Viewer: " + myerror.Message); }
                }
                #endregion

                #region USER CONTROL
                else if (obj.BaseType == "USER CONTROL")
                {
                    string sUCType = obj.Property("Control Type").Value;
                    sUCType = sUCType.Replace("USER CONTROL ", "");
                    OSAE.Types.AvailablePlugin selectedPlugin = GlobalUserControls.OSAEUserControls.AvailablePlugins.Find(sUCType);
                    selectedPlugin.Instance.InitializeMainCtrl(obj, gAppName, gCurrentUser);
                    dynamic uc = new UserControl();
                    uc = selectedPlugin.Instance.mainCtrl;
                    uc.MouseRightButtonDown += new MouseButtonEventHandler(UserControl_MouseRightButtonDown);
                    uc.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);                 
                    OSAE.OSAEObjectProperty pZOrder = obj.Property("ZOrder");
                    OSAE.OSAEObjectProperty pX = obj.Property("X");
                    OSAE.OSAEObjectProperty pY = obj.Property("Y");
                    Double dX = Convert.ToDouble(pX.Value);
                    Canvas.SetLeft(uc, dX);
                    Double dY = Convert.ToDouble(pY.Value);
                    Canvas.SetTop(uc, dY);
                    int dZ = Convert.ToInt32(pZOrder.Value);
                    Canvas.SetZIndex(uc, dZ);
                    uc.setLocation(dX, dY);
                    canGUI.Children.Add(uc);
                    userControls.Add(uc);
                    controlTypes.Add(uc.GetType());
                }
                #endregion

                #region CONTROL BROWSER
                else if (obj.Type == "CONTROL BROWSER")
                {
                    if (gDebug) Log.Debug("Loading BrowserControl: " + obj.Name);
                    try
                    {
                        OSAE.UI.Controls.BrowserFrame bf = new OSAE.UI.Controls.BrowserFrame(obj);
                        bf.MouseRightButtonDown += new MouseButtonEventHandler(Broswer_Control_MouseRightButtonDown);
                        //OSAE.UI.Controls.StaticLabel sl = new OSAE.UI.Controls.StaticLabel(obj);
                        canGUI.Children.Add(bf);
                        int dZ = Int32.Parse(obj.Property("ZOrder").Value);
                        bf.Location.X = Double.Parse(obj.Property("X").Value);
                        bf.Location.Y = Double.Parse(obj.Property("Y").Value);
                        bf.Width = Double.Parse(obj.Property("Width").Value);
                        bf.Height = Double.Parse(obj.Property("Height").Value);
                        Canvas.SetLeft(bf, bf.Location.X);
                        Canvas.SetTop(bf, bf.Location.Y);
                        Canvas.SetZIndex(bf, dZ);
                        browserFrames.Add(bf);
                        controlTypes.Add(typeof(OSAE.UI.Controls.BrowserFrame));
                        //
                        bf.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error updating BrowserControl", ex);
                        return;
                    }
                }
                #endregion
            }));
        }

        private void Load_App_Name()
        {
            gAppName = "GUI CLIENT-" + Common.ComputerName;
            gAppLocation = Common.ComputerName;

            OSAEObject obj = OSAEObjectManager.GetObjectByName("GUI CLIENT-" + Common.ComputerName);
            if (obj == null) OSAEObjectManager.ObjectAdd(gAppName, "", gAppName, "GUI CLIENT", "", Common.ComputerName, 50, true);
            else OSAEObjectManager.ObjectUpdate(obj.Name, gAppName, "", gAppName, "GUI CLIENT", "", Common.ComputerName, 50, true);
        }

        private void Set_Default_Screen()
        {
            OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("SCREEN");
            if (screens.Count > 0)
            {
                gCurrentScreen = screens[0].Name;
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Default Screen", gCurrentScreen, gCurrentUser);
            }
            else menuCreateScreen.IsEnabled = true;
        }

        #region CONTROL CLICK EVENTS
        private void Click_Image_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            ClickImage navCtrl = (ClickImage)sender;
            //MessageBox.Show(navCtrl.screenObject.Name);
            AddControl addControl = new AddControl();
            AddControlClickImage cmi = new AddControlClickImage(gCurrentScreen, gCurrentUser, navCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void State_Image_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            StateImage navCtrl = (StateImage)sender;
            //MessageBox.Show(navCtrl.screenObject.Name);
            AddControl addControl = new AddControl();
            AddControlStateImage cmi = new AddControlStateImage(gCurrentScreen, gCurrentUser, navCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Broswer_Control_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            BrowserFrame bfCtrl = (BrowserFrame)sender;
            AddControl addControl = new AddControl();
            AddControlBrowser cmi = new AddControlBrowser(gCurrentScreen, gCurrentUser, bfCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Navigaton_Image_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            NavigationImage navCtrl = (NavigationImage)sender;
            AddControl addControl = new AddControl();
            AddControlNavigationImage cmi = new AddControlNavigationImage(gCurrentScreen, gCurrentUser, navCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Property_Label_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            PropertyLabel propLabel = (PropertyLabel)sender;
            AddControl addControl = new AddControl();
            AddControlPropertyLabel cmi = new AddControlPropertyLabel(gCurrentScreen, gCurrentUser, propLabel.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Navigaton_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            _timer.Stop();
            NavigationImage navCtrl = (NavigationImage)sender;
            gCurrentScreen = navCtrl.screenName;
            Load_Screen(gCurrentScreen);
        }

        private void Timer_Label_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            TimerLabel tmrLbl = (TimerLabel)sender;
            AddControl addControl = new AddControl();
            AddControlTimerLabel cmi = new AddControlTimerLabel(gCurrentScreen, gCurrentUser, tmrLbl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void VideoStreamViewer_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            _timer.Stop();
            VideoStreamViewer vidviewr = (VideoStreamViewer)sender;
            AddControl addControl = new AddControl();
            AddNewCameraViewer cmi = new AddNewCameraViewer(gCurrentScreen, vidviewr.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false || gCurrentUser == "") return;
            dynamic item = sender;
            string sUCType = item._controlname;
            string screenObject = item.screenObject.Name;
            OSAE.Types.AvailablePlugin selectedPlugin = GlobalUserControls.OSAEUserControls.AvailablePlugins.Find(sUCType);
            AddControl addControl = new AddControl();
            selectedPlugin.Instance.InitializeAddCtrl(gCurrentScreen, selectedPlugin.Instance.Name, gCurrentUser, screenObject);
            UserControl cmi = selectedPlugin.Instance.CtrlInterface;
            addControl.Content = cmi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }
        #endregion

        #region DRAG EVENTS
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

                if (beingDragged.GetType() == typeof(StateImage))
                {
                    Thread thread = new Thread(() => updateObjectCoordsStateImg(beingDragged.screenObject, beingDragged.StateMatch, newX.ToString(), newY.ToString()));
                    thread.Start();
                }
                else
                {
                    Thread thread = new Thread(() => updateObjectCoords(beingDragged.screenObject, newX.ToString(), newY.ToString()));
                    thread.Start();
                }
            }
        }

        void DragSource_PreviewMouseMove(dynamic sender, MouseEventArgs e)
        {
            if (editMode)
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
                                && (sender.Location.X < _startPoint.X && _startPoint.X < (sender.Location.X + sender.ActualWidth))
                                && (sender.Location.Y < _startPoint.Y && _startPoint.Y < (sender.Location.Y + sender.ActualHeight))
                               )
                            {
                                beingDragged = (UIElement)sender;
                                StartDragInProcAdorner(e, sender);
                            }
                        }
                    }
                }
            }
        }

        void DragSource_PreviewMouseLeftButtonDown(dynamic sender, MouseButtonEventArgs e)
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
                    _dragHasLeftScope = true;
                    e.Handled = true;
                }
            }
        }

        void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (_dragHasLeftScope)
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }
        }

        private void updateObjectCoords(OSAE.OSAEObject obj, string X, string Y)
        {
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "X", X, gCurrentUser);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Y", Y, gCurrentUser);

            obj.Property("X").Value = X;
            obj.Property("Y").Value = Y;
        }

        private void updateObjectCoordsStateImg(OSAE.OSAEObject obj, string state, string X, string Y)
        {
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, state + " X", X, gCurrentUser);
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, state + " Y", Y, gCurrentUser);

            obj.Property(state + " X").Value = X; 
            obj.Property(state + " Y").Value = Y;
        }

       
        #endregion
               
        #region MENU EVENTS

        private void menuEditMode_Checked(object sender, RoutedEventArgs e)
        {
            editMode = true;
        }

        private void menuEditMode_Unchecked(object sender, RoutedEventArgs e)
        {
            editMode = false;
        }

        private void menuAddStateImage_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlStateImage csi = new AddControlStateImage(gCurrentScreen,gCurrentUser);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddPropertyLabel_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlPropertyLabel csi = new AddControlPropertyLabel(gCurrentScreen, gCurrentUser);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddNavImage_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlNavigationImage cni = new AddControlNavigationImage(gCurrentScreen, gCurrentUser);
            addControl.Content = cni;
            addControl.Width = cni.Width + 80;
            addControl.Height = cni.Height + 80;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddClickImage_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlClickImage cmi = new AddControlClickImage(gCurrentScreen, gCurrentUser);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddTimerLabel_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlTimerLabel csi = new AddControlTimerLabel(gCurrentScreen, gCurrentUser);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddCameraViewer_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddNewCameraViewer csi = new AddNewCameraViewer(gCurrentScreen, gCurrentUser);
            addControl.Width = csi.Width + 40;
            addControl.Height = csi.Height + 40;
            addControl.Content = csi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddWebBrowser_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlBrowser csi = new AddControlBrowser(gCurrentScreen, gCurrentUser);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddUserControl_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlUserControl csi = new AddControlUserControl(gCurrentScreen, gCurrentUser);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuCreateScreen_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            AddControl addControl = new AddControl();
            AddControlScreen cscr = new AddControlScreen(gCurrentScreen, gCurrentUser);
            addControl.Width = cscr.Width + 80;
            addControl.Height = cscr.Height + 80;
            addControl.Content = cscr;
            addControl.ShowDialog();
            Load_Screen(cscr.currentScreen);
        }

        private void menuInstallUserControl_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = string.Empty; // Default file name 
            dlg.DefaultExt = ".osauc"; // Default file extension 
            dlg.Filter = "Open Source Automation User Control Pakages (.osauc)|*.osauc"; // Filter files by extension 

            // Show open file dialog box 
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                Log.Info("Plugin file selected: " + dlg.FileName + ".  Installing...");
                // Open Plugin Package
                GUI.UserControlInstallerHelper pInst = new GUI.UserControlInstallerHelper();
                pInst.InstallPlugin(dlg.FileName);
                GlobalUserControls.OSAEUserControls.FindPlugins(OSAE.Common.ApiPath + @"\UserControls");
            }
        }

        private void menuShowControlList_Checked(object sender, RoutedEventArgs e)
        {
            screenObjectControl.Visibility = System.Windows.Visibility.Visible;
        }

        private void menuShowControlList_Unchecked(object sender, RoutedEventArgs e)
        {
            screenObjectControl.Visibility = System.Windows.Visibility.Hidden;
        }

        private void menuChangeScreen_Click(object sender, RoutedEventArgs e)
        {
            ChangeScreen chgScrn = new ChangeScreen(this);
            chgScrn.Show();
        }

        private void menuFrameShow_Click(object sender, RoutedEventArgs e)     
        {
            ResizeMode = System.Windows.ResizeMode.CanResize;
            WindowStyle = System.Windows.WindowStyle.ToolWindow;
            ResizeMode = System.Windows.ResizeMode.NoResize;    
        }

        private void menuFrameHide_Click(object sender, RoutedEventArgs e)
        {
            WindowStyle = System.Windows.WindowStyle.None;
            ResizeMode = System.Windows.ResizeMode.CanResize;
            Width = canGUI.Width;
            Height = canGUI.Height;
            ResizeMode = System.Windows.ResizeMode.NoResize;        
        }

        private void menuFrameTopLeft_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            Left = desktopWorkingArea.Left;
            Top = desktopWorkingArea.Top;
        }

        private void menuFrameBottomRight_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - this.Width;
            Top = desktopWorkingArea.Bottom - this.Height;

        }

        private void menuFrameCentre_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            Left = (desktopWorkingArea.Width / 2) - (this.Width /2);
            Top = (desktopWorkingArea.Height / 2) - (this.Height / 2);
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //closing = true;
            OSAE.OSAEObjectStateManager.ObjectStateSet(gAppName, "OFF", gAppName);
            string remUser = OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "LogOut on Close").Value;
            bool loUser = Convert.ToBoolean(remUser);
            if (loUser == true) OSAE.OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Current User", "", "SYSTEM");
            _timer.Stop();
        }
    }
}
