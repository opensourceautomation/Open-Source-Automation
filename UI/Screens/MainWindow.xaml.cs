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
        public double gCurrentScreenHeight = 1;
        public double gCurrentScreenWidth = 1;
        public double gHeightRatio;
        public double gWidthRatio;
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
            Common.CheckComputerObject(gAppName);
            Uri uri = new Uri("pack://siteoforigin:,,,/OSA.png");
            var bitmapImage = new BitmapImage(uri);
            
            canGUI.Background = new ImageBrush(bitmapImage);
            canGUI.Height = bitmapImage.Height;
            canGUI.Width = bitmapImage.Width;
            this.Height = bitmapImage.Height;
            this.Width = bitmapImage.Width;


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

            // uh maybe this should have code for no screens here, since the resize code went it.  if above fix dont work
            if (gCurrentScreen == "")
            {
                Set_Default_Screen();
                menuCreateScreen.IsEnabled = true;
            }
            else Load_Screen(gCurrentScreen);

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
                    //The next line sucks an should not be done, but it was getting stuck in this loop sometimes
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
                bool isMax = (this.WindowState == WindowState.Maximized);
                //this.WindowState = WindowState.Normal;
                if (img.Data != null)
                {
                    var imageStream = new MemoryStream(img.Data);
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = imageStream;
                    bitmapImage.EndInit();

                    gCurrentScreenHeight = bitmapImage.Height;
                    gCurrentScreenWidth = bitmapImage.Width;

                    canGUI.Background = new ImageBrush(bitmapImage);

                    bool gForcedScreenSettings = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Use Global Screen Settings").Value);
                    if (gForcedScreenSettings)
                    {
                        double gForcedScreenHeight = Convert.ToDouble(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Height").Value);
                        double gForcedScreenWidth = Convert.ToDouble(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Width").Value);
                        canGUI.Height = gForcedScreenHeight;
                        canGUI.Width = gForcedScreenWidth;
                        this.Height = gForcedScreenHeight;
                        this.Width = gForcedScreenWidth;
                        gHeightRatio = this.ActualHeight / gForcedScreenHeight;
                        gWidthRatio = this.ActualWidth / gForcedScreenWidth;

                        bool gForcedScreenShowFrame = Convert.ToBoolean(OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Show Frame").Value);
                        if (gForcedScreenShowFrame)
                        {
                            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                            this.ResizeMode = ResizeMode.CanResize;
                            menuFrameShow.IsChecked = true;
                        }
                        else
                        {
                            WindowStyle = System.Windows.WindowStyle.None;
                            this.ResizeMode = ResizeMode.NoResize;
                            menuFrameShow.IsChecked = false;
                        }
                    }
                    else
                    {
                        if (isMax)
                        {
                            gHeightRatio = this.ActualHeight / gCurrentScreenHeight;
                            gWidthRatio = this.ActualWidth / gCurrentScreenWidth;
                        }
                        else
                        {
                            canGUI.Height = bitmapImage.Height;
                            canGUI.Width = bitmapImage.Width;
                            this.Height = bitmapImage.Height;
                            this.Width = bitmapImage.Width;
                            gHeightRatio = 1;
                            gWidthRatio = 1;
                        }
                    }
                }

                Load_Objects(sScreen);
                if (isMax) this.WindowState = WindowState.Maximized;
                loadingScreen = false;
              //  tvControls
            }
            catch (Exception ex)
            { Log.Error("Failed to load screen: " + sScreen, ex); }

            Log.Debug("Loaded Screen:  " + sScreen);
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

        private void Resize_Objects()
        {
            foreach (System.Type newCtrl in controlTypes)
            {
                if (newCtrl.Name == "UserSelector")
                {
                    Canvas.SetLeft(userSelectorControl, userSelectorControl.Location.X * gWidthRatio);
                    Canvas.SetTop(userSelectorControl, userSelectorControl.Location.Y * gHeightRatio);
                }
                else if (newCtrl.Name == "ScreenObjectList")
                {
                    Canvas.SetLeft(screenObjectControl, screenObjectControl.Location.X * gWidthRatio);
                    Canvas.SetTop(screenObjectControl, screenObjectControl.Location.Y * gHeightRatio);
                    screenObjectControl.Width = screenObjectControl.ControlWidth * gWidthRatio;
                    screenObjectControl.Height = screenObjectControl.ControlHeight * gHeightRatio;
                }
                else if (newCtrl.Name == "StateImage")
                {
                    foreach (StateImage sImage in stateImages)
                    {
                        Canvas.SetLeft(sImage, sImage.Location.X * gWidthRatio);
                        Canvas.SetTop(sImage, sImage.Location.Y * gHeightRatio);
                       // sImage.Opacity = Convert.ToDouble(sImage.LightLevel) / 100.00;
                        sImage.Width = sImage.ImageWidth * gWidthRatio;
                        sImage.Height = sImage.ImageHeight * gHeightRatio;
                    }
                }
                else if (newCtrl.Name == "PropertyLabel")
                {
                    foreach (PropertyLabel pl in propLabels)
                    {
                        Canvas.SetLeft(pl, pl.Location.X * gWidthRatio);
                        Canvas.SetTop(pl, pl.Location.Y * gHeightRatio);
                    }
                }
                else if (newCtrl.Name == "TimerLabel")
                {
                    foreach (OSAE.UI.Controls.TimerLabel tl in timerLabels)
                    {
                        Canvas.SetLeft(tl, tl.Location.X * gWidthRatio);
                        Canvas.SetTop(tl, tl.Location.Y * gHeightRatio);
                    }
                }
                else if (newCtrl.Name == "StaticLabel")
                {
                    foreach (OSAE.UI.Controls.StaticLabel sl in staticLabels)
                    {
                        Canvas.SetLeft(sl, sl.Location.X * gWidthRatio);
                        Canvas.SetTop(sl, sl.Location.Y * gHeightRatio);
                    }
                }
                else if (newCtrl.Name == "NavigationImage")
                {
                    foreach (OSAE.UI.Controls.NavigationImage nav in navImages)
                    {
                        Canvas.SetLeft(nav, nav.Location.X * gWidthRatio);
                        Canvas.SetTop(nav, nav.Location.Y * gHeightRatio);
                        nav.Width = nav.ImageWidth * gWidthRatio;
                        nav.Height = nav.ImageHeight * gHeightRatio;
                    }
                }
                else if (newCtrl.Name == "ClickImage")
                {
                    foreach (OSAE.UI.Controls.ClickImage method in clickImages)
                    {
                        Canvas.SetLeft(method, method.Location.X * gWidthRatio);
                        Canvas.SetTop(method, method.Location.Y * gHeightRatio);
                        method.Width = method.ImageWidth * gWidthRatio;
                        method.Height = method.ImageHeight * gHeightRatio;
                    }
                }
                else if (newCtrl.Name == "CONTROL CAMERA VIEWER")
                {
                    foreach (OSAE.UI.Controls.VideoStreamViewer vsv in cameraViewers)
                    {
                        Canvas.SetLeft(vsv, vsv.Location.X * gWidthRatio);
                        Canvas.SetTop(vsv, vsv.Location.Y * gHeightRatio);
                    }
                }
                else if (newCtrl.Name.Contains("UserControl"))
                {
                    foreach (dynamic obj in userControls)
                    {
                        Canvas.SetLeft(obj, obj.Location.X * gWidthRatio);
                        Canvas.SetTop(obj, obj.Location.Y * gHeightRatio);
                    }
                }
                else if (newCtrl.Name == "BrowserFrame")
                {
                    foreach (BrowserFrame oBrowser in browserFrames)
                    {
                        Canvas.SetLeft(oBrowser, oBrowser.Location.X * gWidthRatio);
                        Canvas.SetTop(oBrowser, oBrowser.Location.Y * gHeightRatio);
                        oBrowser.Width = oBrowser.ControlWidth * gWidthRatio;
                        oBrowser.Height = oBrowser.ControlHeight * gHeightRatio;
                    }
                }
            }
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
                    if (newCtrl.ControlType == "CONTROL STATE IMAGE")
                    {
                        foreach (StateImage sImage in stateImages)
                        {
                            if (newCtrl.ControlName == sImage.screenObject.Name)
                            {
                                if (newCtrl.LastUpdated != sImage.LastUpdated)
                                {
                                    sImage.LastUpdated = newCtrl.LastUpdated;
                                    try { sImage.Update(); }
                                    catch { }
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        Canvas.SetLeft(sImage, sImage.Location.X * gWidthRatio);
                                        Canvas.SetTop(sImage, sImage.Location.Y * gHeightRatio);
                                        sImage.Width = sImage.ImageWidth * gWidthRatio;
                                        sImage.Height = sImage.ImageHeight * gHeightRatio;
                                    }));
                                    if (gDebug) Log.Debug("Updated:  " + newCtrl.ControlName);
                                }
                                if (newCtrl.PropertyLastUpdated != sImage.PropertyLastUpdated)
                                {
                                    sImage.PropertyLastUpdated = newCtrl.PropertyLastUpdated;
                                    sImage.Update();
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        sImage.Opacity = Convert.ToDouble(sImage.LightLevel) / 100.00;
                                    }));
                                }
                                oldCtrl = true;
                            }
                        }
                    }
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
                    /*
                    else if (newCtrl.ControlType == "CONTROL STATIC LABEL")
                    {
                        foreach (OSAE.UI.Controls.StaticLabel sl in staticLabels)
                        {
                            if (newCtrl.ControlName == sl.screenObject.Name)
                            {
                                oldCtrl = true;
                                Dispatcher.Invoke((Action)(() =>
                                {
                                    Canvas.SetLeft(sl, sl.Location.X * gWidthRatio);
                                    Canvas.SetTop(sl, sl.Location.Y * gHeightRatio);
                                }));
                            }
                        }
                    }
                    else if (newCtrl.ControlType == "CONTROL NAVIGATION IMAGE")
                    {
                        foreach (OSAE.UI.Controls.NavigationImage nav in navImages)
                        {
                            if (newCtrl.ControlName == nav.screenObject.Name)
                            {
                                oldCtrl = true;
                                Dispatcher.Invoke((Action)(() =>
                                {
                                    Canvas.SetLeft(nav, nav.Location.X * gWidthRatio);
                                    Canvas.SetTop(nav, nav.Location.Y * gHeightRatio);
                                    nav.Width = nav.ImageWidth * gWidthRatio;
                                    nav.Height = nav.ImageHeight * gHeightRatio;
                                }));
                            }
                        }
                    }
                    else if (newCtrl.ControlType == "CONTROL CLICK IMAGE")
                    {
                        foreach (OSAE.UI.Controls.ClickImage method in clickImages)
                        {
                            if (newCtrl.ControlName == method.screenObject.Name)
                            {
                                oldCtrl = true;
                                Dispatcher.Invoke((Action)(() =>
                                {
                                    Canvas.SetLeft(method, method.Location.X * gWidthRatio);
                                    Canvas.SetTop(method, method.Location.Y * gHeightRatio);
                                    method.Width = method.ImageWidth * gWidthRatio;
                                    method.Height = method.ImageHeight * gHeightRatio;
                                }));
                            }
                        }
                    }
                    */
                    else if (newCtrl.ControlType == "CONTROL CAMERA VIEWER")
                    {
                        foreach (OSAE.UI.Controls.VideoStreamViewer vsv in cameraViewers)
                        {
                            if (newCtrl.ControlName == vsv.screenObject.Name)
                            {
                                oldCtrl = true;
                                if (gWidthRatio > gHeightRatio)
                                {
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        Canvas.SetLeft(vsv, vsv.Location.X * gWidthRatio);
                                        Canvas.SetTop(vsv, vsv.Location.Y * gHeightRatio);
                                        vsv.Width = vsv.ControlWidth * gHeightRatio;
                                        vsv.Height = vsv.ControlHeight * gHeightRatio;
                                    }));
                                }
                                else
                                {
                                    Dispatcher.Invoke((Action)(() =>
                                    {
                                        Canvas.SetLeft(vsv, vsv.Location.X * gWidthRatio);
                                        Canvas.SetTop(vsv, vsv.Location.Y * gHeightRatio);
                                        vsv.Width = vsv.ControlWidth * gWidthRatio;
                                        vsv.Height = vsv.ControlHeight * gWidthRatio;
                                    }));
                                }
                            }
                        }
                    }
                  
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
                           //.     Dispatcher.Invoke((Action)(() =>
                           //     {
                          //         Canvas.SetLeft(obj, obj.Location.X * gWidthRatio);
                           //         Canvas.SetTop(obj, obj.Location.Y * gHeightRatio);
                           //     }));
                            }
                        }
                    }
                    /*
                    else if (newCtrl.ControlType == "CONTROL BROWSER")
                    {
                        foreach (BrowserFrame oBrowser in browserFrames)
                        {
                           if (newCtrl.ControlName == oBrowser.screenObject.Name)
                            {
                                oldCtrl = true;
                                Dispatcher.Invoke((Action)(() =>
                                {
                                    Canvas.SetLeft(oBrowser, oBrowser.Location.X * gWidthRatio);
                                    Canvas.SetTop(oBrowser, oBrowser.Location.Y * gHeightRatio);
                                    oBrowser.Width = oBrowser.ControlWidth * gWidthRatio;
                                    oBrowser.Height = oBrowser.ControlHeight * gHeightRatio;
                                }));
                            }
                        }
                    }
                    */
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

                if (obj.Type == "CONTROL USER SELECTOR")
                {
                    userSelectorControl = new UserSelector(obj, gAppName);
                  //  userSelectorControl.MouseRightButtonDown += new MouseButtonEventHandler(State_Image_MouseRightButtonDown);
                    userSelectorControl.Location.X = Double.Parse(obj.Property("X").Value);
                    userSelectorControl.Location.Y = Double.Parse(obj.Property("Y").Value);
                    userSelectorControl._ScreenLocation = gAppLocation;
                    canGUI.Children.Add(userSelectorControl);
                    Canvas.SetLeft(userSelectorControl, userSelectorControl.Location.X * gWidthRatio);
                    Canvas.SetTop(userSelectorControl, userSelectorControl.Location.Y * gHeightRatio);
                    Canvas.SetZIndex(userSelectorControl, 5);
                    controlTypes.Add(typeof(UserSelector));
                    userSelectorControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                }
                else if (obj.Type == "CONTROL SCREEN OBJECTS")
                {
                    screenObjectControl = new ScreenObjectList(obj,gCurrentScreen,gCurrentUser);
              //      screenObjectControl.MouseRightButtonDown += new MouseButtonEventHandler(State_Image_MouseRightButtonDown);
                    screenObjectControl.Location.X = Double.Parse(obj.Property("X").Value);
                    screenObjectControl.Location.Y = Double.Parse(obj.Property("Y").Value);
                    screenObjectControl._ScreenLocation = gAppLocation;
                    screenObjectControl.Visibility = System.Windows.Visibility.Hidden;

                    canGUI.Children.Add(screenObjectControl);
                    Canvas.SetLeft(screenObjectControl, screenObjectControl.Location.X * gWidthRatio);
                    Canvas.SetTop(screenObjectControl, screenObjectControl.Location.Y * gHeightRatio);
                    Canvas.SetZIndex(screenObjectControl, 5);
                    screenObjectControl.ControlWidth = screenObjectControl.Width;
                    screenObjectControl.ControlHeight = screenObjectControl.Height;
                    controlTypes.Add(typeof(ScreenObjectList));
                    screenObjectControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                }
                else if (obj.Type == "CONTROL STATE IMAGE")
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
                        Canvas.SetLeft(stateImageControl, stateImageControl.Location.X * gWidthRatio);
                        Canvas.SetTop(stateImageControl, stateImageControl.Location.Y * gHeightRatio);
                        Canvas.SetZIndex(stateImageControl, dZ);
                        stateImageControl.Width = stateImageControl.ImageWidth * gWidthRatio;
                        stateImageControl.Height = stateImageControl.ImageHeight * gHeightRatio;
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
                        Canvas.SetLeft(pl, pl.Location.X * gWidthRatio);
                        Canvas.SetTop(pl, pl.Location.Y * gHeightRatio);
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
                        Canvas.SetLeft(sl, sl.Location.X * gWidthRatio);
                        Canvas.SetTop(sl, sl.Location.Y * gHeightRatio);
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
                        Canvas.SetLeft(tl, tl.Location.X * gWidthRatio);
                        Canvas.SetTop(tl, tl.Location.Y * gHeightRatio);
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
                else if (obj.Type == "CONTROL CLICK IMAGE")
                {
                    try
                    {
                        ClickImage ClickImageControl = new ClickImage(obj, gAppName, gCurrentUser);
                        ClickImageControl.MouseRightButtonDown += new MouseButtonEventHandler(Click_Image_MouseRightButtonDown);
                        canGUI.Children.Add(ClickImageControl);
                        double dX = Convert.ToDouble(obj.Property("X").Value);
                        double dY = Convert.ToDouble(obj.Property("Y").Value);
                        int dZ = Convert.ToInt32(obj.Property("ZOrder").Value);
                        Canvas.SetLeft(ClickImageControl, dX * gWidthRatio);
                        Canvas.SetTop(ClickImageControl, dY * gHeightRatio);
                        Canvas.SetZIndex(ClickImageControl, dZ);
                        ClickImageControl.Location.X = dX;
                        ClickImageControl.Location.Y = dY;
                        ClickImageControl.Width = ClickImageControl.ImageWidth * gWidthRatio;
                        ClickImageControl.Height = ClickImageControl.ImageHeight * gHeightRatio;
                        clickImages.Add(ClickImageControl);
                        controlTypes.Add(typeof(ClickImage));
                        ClickImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    { MessageBox.Show("GUI Error Load Click Image: " + myerror.Message); }
                }
                else if (obj.Type == "CONTROL NAVIGATION IMAGE")
                {
                    try
                    {
                        NavigationImage navImageControl = new NavigationImage(obj.Property("Screen").Value, obj);
                        navImageControl.MouseLeftButtonUp += new MouseButtonEventHandler(Navigaton_Image_MouseLeftButtonUp);
                        navImageControl.MouseRightButtonDown += new MouseButtonEventHandler(Navigaton_Image_MouseRightButtonDown);
                        canGUI.Children.Add(navImageControl);
                        double dX = Convert.ToDouble(obj.Property("X").Value);
                        double dY = Convert.ToDouble(obj.Property("Y").Value);
                        int dZ = Convert.ToInt32(obj.Property("ZOrder").Value);
                        Canvas.SetLeft(navImageControl, dX * gWidthRatio);
                        Canvas.SetTop(navImageControl, dY * gHeightRatio);
                        Canvas.SetZIndex(navImageControl, dZ);
                        navImageControl.Location.X = dX;
                        navImageControl.Location.Y = dY;
                        navImageControl.Width = navImageControl.ImageWidth * gWidthRatio;
                        navImageControl.Height = navImageControl.ImageHeight * gHeightRatio;
                        navImages.Add(navImageControl);
                        controlTypes.Add(typeof(NavigationImage));
                        navImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    { MessageBox.Show("GUI Error Load Navigation Image: " + myerror.Message); }
                }
                else if (obj.Type == "CONTROL CAMERA VIEWER")
                {
                    try
                    {
                        string stream = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
                        VideoStreamViewer vsv = new VideoStreamViewer(stream, obj, gAppName);
                        vsv.MouseRightButtonDown += new MouseButtonEventHandler(VideoStreamViewer_MouseRightButtonDown);
                        canGUI.Children.Add(vsv);
                        double dX = Convert.ToDouble(obj.Property("X").Value);
                        double dY = Convert.ToDouble(obj.Property("Y").Value);
                        int dZ = Convert.ToInt32(obj.Property("ZOrder").Value);
                        Canvas.SetLeft(vsv, dX * gWidthRatio);
                        Canvas.SetTop(vsv, dY * gHeightRatio);
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
                    double dX = Convert.ToDouble(obj.Property("X").Value);
                    double dY = Convert.ToDouble(obj.Property("Y").Value);
                    int dZ = Convert.ToInt32(obj.Property("ZOrder").Value);
                    Canvas.SetLeft(uc, dX * gWidthRatio);
                    Canvas.SetTop(uc, dY * gHeightRatio);
                    Canvas.SetZIndex(uc, dZ);
                    uc.setLocation(dX, dY);
                    uc.Width = uc.ControlWidth * gWidthRatio;
                    uc.Height = uc.ControlHeight * gHeightRatio;
                    canGUI.Children.Add(uc);
                    userControls.Add(uc);
                    controlTypes.Add(uc.GetType());
                }
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
                        bf.Width = Double.Parse(obj.Property("Width").Value) * gWidthRatio;
                        bf.Height = Double.Parse(obj.Property("Height").Value) * gHeightRatio;
                        bf.ControlWidth = bf.Width;
                        bf.ControlHeight = bf.Height;
                        Canvas.SetLeft(bf, bf.Location.X * gWidthRatio);
                        Canvas.SetTop(bf, bf.Location.Y * gHeightRatio);
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
            AddNewCameraViewer cmi = new AddNewCameraViewer(gCurrentScreen, vidviewr.screenObject.Name, vidviewr.screenObject.Name);
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

                            if ((Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                                && (sender.Location.X < _startPoint.X && _startPoint.X < (sender.Location.X + sender.ActualWidth)) && (sender.Location.Y < _startPoint.Y && _startPoint.Y < (sender.Location.Y + sender.ActualHeight)))
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
                _adorner.LeftOffset = args.GetPosition(DragScope).X;
                _adorner.TopOffset = args.GetPosition(DragScope).Y;
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
            menuFrameShow.IsEnabled = false;
            this.WindowState = WindowState.Normal;
            this.ResizeMode = ResizeMode.NoResize;
            WindowStyle = System.Windows.WindowStyle.ToolWindow;
            gHeightRatio = 1;
            gWidthRatio = 1;
            this.Width = gCurrentScreenWidth;
            this.Height = gCurrentScreenHeight;
        }

        private void menuEditMode_Unchecked(object sender, RoutedEventArgs e)
        {
            editMode = false;
            menuFrameShow.IsEnabled = true;
            this.ResizeMode = ResizeMode.CanResize;
            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
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
            WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
        }

        private void menuFrameHide_Click(object sender, RoutedEventArgs e)
        {
            ResizeMode = System.Windows.ResizeMode.CanMinimize;
            WindowStyle = System.Windows.WindowStyle.None;

            Width = canGUI.Width;
            Height = canGUI.Height;
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

        private void update_size(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                canGUI.Width = this.Width;
                canGUI.Height = this.Height;
                gHeightRatio = this.Height / gCurrentScreenHeight;
                gWidthRatio = this.Width / gCurrentScreenWidth;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                canGUI.Width = this.ActualWidth;
                canGUI.Height = this.ActualHeight;
                gHeightRatio = this.ActualHeight / gCurrentScreenHeight;
                gWidthRatio = this.ActualWidth / gCurrentScreenWidth;
            }
            Resize_Objects();
        }
    }
}
