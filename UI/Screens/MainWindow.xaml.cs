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
    using MySql.Data.MySqlClient;
    using OSAE;
    using OSAE.UI.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //OSAELog
        private OSAE.General.OSAELog Log = new OSAE.General.OSAELog();
        
        String gAppName = "";
        String gCurrentScreen = "";

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
        bool closing = false;

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
            }
            catch (Exception ex)
            {
                this.Log.Error("Error starting GUI", ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("pack://siteoforigin:,,,/OSA.png");
            var bitmapImage = new BitmapImage(uri);
            
            canGUI.Background = new ImageBrush(bitmapImage);
            canGUI.Height = bitmapImage.Height;
            canGUI.Width = bitmapImage.Width;
            canGUI.Background = new ImageBrush(bitmapImage);
            canGUI.Height = bitmapImage.Height;
            canGUI.Width = bitmapImage.Width;

            Load_App_Name();
            gCurrentScreen = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Default Screenccccc").Value;
            if (gCurrentScreen == "")
            {
                Set_Default_Screen();
            }
            Load_Screen(gCurrentScreen);

            Thread thread = new Thread(() => Update_Objects());
            thread.Start();

            this.canGUI.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
            this.canGUI.Drop += new DragEventHandler(DragSource_Drop);
        }

        public void Load_Screen(string sScreen)
        {
            try
            {
                while (updatingScreen)
                {
                    System.Threading.Thread.Sleep(100);
                }
                loadingScreen = true;
                //this.Log.Debug("Loading screen: " + sScreen);

                stateImages.Clear();
                propLabels.Clear();
                navImages.Clear();
                clickImages.Clear();
                cameraViewers.Clear();
                canGUI.Children.Clear();
                browserFrames.Clear();
                
                gCurrentScreen = sScreen;
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Current Screen", sScreen, "GUI");
                OSAE.OSAEImageManager imgMgr = new OSAE.OSAEImageManager();
                string imgID = OSAEObjectPropertyManager.GetObjectPropertyValue(sScreen, "Background Image").Value;
                OSAE.OSAEImage img = imgMgr.GetImage(imgID);

                //sPath = OSAEApi.APIpath + OSAEApi.GetObjectPropertyValue(sScreen, "Background Image").Value;
                //byte[] byteArray = File.ReadAllBytes(sPath);

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

                //Thread threadLoad = new Thread(() => Load_Objects(sScreen));
                //threadLoad.Start();

                Load_Objects(sScreen);
                loadingScreen = false;

                //this.Log.Debug("Loading screen complete: " + sScreen);
            }
            catch (Exception ex)
            {
                this.Log.Error("Failed to load screen: " + sScreen, ex);
            }
        }

        private void Load_Objects(String sScreen)
        {
            OSAEObjectCollection screenObjects = OSAEObjectManager.GetObjectsByContainer(sScreen);

            foreach (OSAE.OSAEObject obj in screenObjects)
            {
                LoadControl(obj);
            }

            //Thread threadUpdate = new Thread(() => Update_Objects());
            //threadUpdate.Start();
        }

        private void Update_Objects()
        {
         //   try
         //   {
                 
                 while (!closing)
                {
                    while (loadingScreen)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    updatingScreen = true;
                    bool oldCtrl = false;
                    //Log.Debug("Entering Update_Objects");
                    List<OSAE.OSAEScreenControl> controls = OSAEScreenControlManager.GetScreenControls(gCurrentScreen);

                    foreach (OSAE.OSAEScreenControl newCtrl in controls)
                    {
                        if (loadingScreen) return;
                        oldCtrl = false;

                        #region CONTROL STATE IMAGE
                        if (newCtrl.ControlType == "CONTROL STATE IMAGE")
                        {
                            foreach (StateImage sImage in stateImages)
                            {
                                if (loadingScreen) return;
                                if (newCtrl.ControlName == sImage.screenObject.Name)
                                {
                                    if (newCtrl.LastUpdated != sImage.LastUpdated)
                                    {
                                        this.Log.Debug("Updating:  " + newCtrl.ControlName);
                                        sImage.LastUpdated = newCtrl.LastUpdated;
                                        try
                                        {
                                            sImage.Update();
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        this.Dispatcher.Invoke((Action)(() =>
                                        {
                                            Canvas.SetLeft(sImage, sImage.Location.X);
                                            Canvas.SetTop(sImage, sImage.Location.Y);
                                            sImage.Opacity = Convert.ToDouble(sImage.LightLevel) / 100.00;
                                        }));
                                        this.Log.Debug("Complete:  " + newCtrl.ControlName);
                                    }
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL PROPERTY LABEL
                        else if (newCtrl.ControlType == "CONTROL PROPERTY LABEL")
                        {
                            if (loadingScreen) return;
                            foreach (PropertyLabel pl in propLabels)
                            {
                                if (loadingScreen) return;
                                if (newCtrl.ControlName == pl.screenObject.Name)
                                {
                                    if (newCtrl.LastUpdated != pl.LastUpdated)
                                    {
                                        this.Log.Debug("Updating:  " + newCtrl.ControlName);
                                        pl.LastUpdated = newCtrl.LastUpdated;
                                        pl.Update();
                                        this.Log.Debug("Complete:  " + newCtrl.ControlName);
                                        if (loadingScreen) return;
                                    }
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL TIMER LABEL
                        else if (newCtrl.ControlType == "CONTROL TIMER LABEL")
                        {
                            foreach (OSAE.UI.Controls.TimerLabel tl in timerLabels)
                            {
                                if (loadingScreen) return;
                                if (newCtrl.ControlName == tl.screenObject.Name)
                                {
                                    if (newCtrl.LastUpdated != tl.LastUpdated)
                                    {
                                        this.Log.Debug("Updating:  " + newCtrl.ControlName);
                                        tl.LastUpdated = newCtrl.LastUpdated;
                                        tl.Update();
                                        this.Log.Debug("Complete:  " + newCtrl.ControlName);
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
                                if (newCtrl.ControlName == sl.screenObject.Name)
                                {
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL NAVIGATION IMAGE
                        else if (newCtrl.ControlType == "CONTROL NAVIGATION IMAGE")
                        {
                            foreach (OSAE.UI.Controls.NavigationImage nav in navImages)
                            {
                                if (newCtrl.ControlName == nav.screenObject.Name)
                                {
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL CLICK IMAGE
                        else if (newCtrl.ControlType == "CONTROL METHOD IMAGE")
                        {
                            foreach (OSAE.UI.Controls.ClickImage method in clickImages)
                            {
                                if (newCtrl.ControlName == method.screenObject.Name)
                                {
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL CAMERA VIEWER
                        else if (newCtrl.ControlType == "CONTROL CAMERA VIEWER")
                        {
                            foreach (OSAE.UI.Controls.VideoStreamViewer vsv in cameraViewers)
                            {
                                if (newCtrl.ControlName == vsv.screenObject.Name)
                                {
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL USAER CONTROL
                        else if (newCtrl.ControlType == "USER CONTROL")
                        {
                            foreach (dynamic obj in userControls)
                            {
                                if (newCtrl.ControlName == obj.screenObject.Name)
                                {
                                    oldCtrl = true;
                                }
                            }
                        }
                        #endregion

                        #region CONTROL BROWSER
                        if (newCtrl.ControlType == "CONTROL BROWSER")
                        {
                            //    foreach (BrowserFrame oBrowser in browserFrames)
                            //     {
                            //      if (newCtrl.ControlName == oBrowser.screenObject.Name)
                            //      {
                            //  if (newCtrl.LastUpdated != sImage.LastUpdated)
                            //  {
                            //        this.Log.Debug("Updating:  " + newCtrl.ControlName);
                            //  sImage.LastUpdated = newCtrl.LastUpdated;
                            //   try
                            //   {
                            //      sImage.Update();
                            //  }
                            //   catch (Exception ex)
                            //   {

                            //    }
                            //        this.Dispatcher.Invoke((Action)(() =>
                            //        {
                            //           Canvas.SetLeft(oBrowser, oBrowser.Location.X);
                            //           Canvas.SetTop(oBrowser, oBrowser.Location.Y);
                            //           }));
                            //          this.Log.Debug("Complete:  " + newCtrl.ControlName);
                            //      }
                            //       oldCtrl = true;
                            //   }
                            //   }
                        }
                        #endregion

                        if (!oldCtrl)
                        {
                            OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(newCtrl.ControlName);
                            this.Log.Debug("Load new control: " + newCtrl.ControlName);
                            LoadControl(obj);
                        }
                    }
                    updatingScreen = false;
                    System.Threading.Thread.Sleep(500);
                }
               
        //    }
      //      catch (Exception ex)
       //     {
       //         System.Threading.Thread.Sleep(100);
       //     }
        }

        private void LoadControl(OSAE.OSAEObject obj)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                String sStateMatch = "";

                #region CONTROL STATE IMAGE
                if (obj.Type == "CONTROL STATE IMAGE")
                {
                    StateImage stateImageControl = new StateImage(obj);

                    foreach (OSAE.OSAEObjectProperty p in obj.Properties)
                    {
                        try
                        {
                            if (p.Value.ToLower() == stateImageControl.CurState.ToLower())
                            {
                                sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                            }
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
                        this.Log.Error("Error updating screenObject", ex);
                        return;
                    }
                }
                #endregion

                #region CONTROL PROPERTY LABEL
                else if (obj.Type == "CONTROL PROPERTY LABEL")
                {
                    this.Log.Debug("Loading PropertyLabelControl: " + obj.Name);
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
                        this.Log.Error("Error updating PropertyLabelControl", ex);
                        return;
                    }
                }
                #endregion

                #region CONTROL STATIC LABEL
                else if (obj.Type == "CONTROL STATIC LABEL")
                {
                    this.Log.Debug("Loading PropertyLabelControl: " + obj.Name);
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
                            this.Log.Error("Error updating PropertyLabelControl", ex);
                            return;
                    }
                }
                #endregion

                #region CONTROL TIMER LABEL
                else if (obj.Type == "CONTROL TIMER LABEL")
                {
                    this.Log.Debug("Loading PropertyTimerControl: " + obj.Name);
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
                        this.Log.Error("Error updating PropertyTimerControl", ex);
                        return;
                    }
                }
                #endregion

                #region CONTROL CLICK IMAGE
                else if (obj.Type == "CONTROL CLICK IMAGE")
                {
                    try
                    {
                        ClickImage ClickImageControl = new ClickImage(obj);
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
                    {
                        MessageBox.Show("GUI Error Load Click Image: " + myerror.Message);
                    }
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
                        string stream = OSAEObjectPropertyManager.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
                        VideoStreamViewer vsv = new VideoStreamViewer(stream, obj);
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
                        Weather wc = new Weather(obj);
                        canGUI.Children.Add(wc);
                        OSAE.OSAEObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.OSAEObjectProperty pX = obj.Property("X");
                        OSAE.OSAEObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(wc, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(wc, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(wc, dZ);

                        wc.Location.X = dX;
                        wc.Location.Y = dY;
                        userControls.Add(wc);
                        controlTypes.Add(typeof(Weather));
                        wc.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                }
                #endregion

                #region CONTROL BROWSER
                else if (obj.Type == "CONTROL BROWSER")
                {
                    this.Log.Debug("Loading BrowserControl: " + obj.Name);
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
                        this.Log.Error("Error updating BrowserControl", ex);
                        return;
                    }
                }
                #endregion
            }));
        }

        private void Load_App_Name()
        {

            OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("GUI CLIENT");
            foreach (OSAE.OSAEObject obj in screens)
            {
                if (obj.Property("Computer Name").Value == Common.ComputerName)
                    gAppName = obj.Name;
            }
            if (gAppName == "")
            {
                gAppName = "GUI CLIENT-" + Common.ComputerName;
                OSAEObjectManager.ObjectAdd(gAppName, gAppName, "GUI CLIENT", "", "SYSTEM", true);
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Computer Name", Common.ComputerName, "GUI");
            }
        }

        private void Set_Default_Screen()
        {
            OSAEObjectCollection screens = OSAEObjectManager.GetObjectsByType("SCREEN");
            if (screens.Count > 0)
            {
                gCurrentScreen = screens[0].Name;
                OSAEObjectPropertyManager.ObjectPropertySet(gAppName, "Default Screen", gCurrentScreen, "GUI");
            }
        }

        private void canvas1_RightButtonDown(object sender, RoutedEventArgs e)
        {
            //mnuMain. = true;
            //MessageBox.Show(sender.ToString());
        }

        private void Click_Image_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false) return;
            ClickImage navCtrl = (ClickImage)sender;
            //MessageBox.Show(navCtrl.screenObject.Name);
            AddControl addControl = new AddControl();
            AddControlClickImage cmi = new AddControlClickImage(gCurrentScreen, navCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void State_Image_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false) return;
            StateImage navCtrl = (StateImage)sender;
            //MessageBox.Show(navCtrl.screenObject.Name);
            AddControl addControl = new AddControl();
            AddControlStateImage cmi = new AddControlStateImage(gCurrentScreen, navCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Broswer_Control_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false) return;
            ClickImage bfCtrl = (ClickImage)sender;
            AddControl addControl = new AddControl();
            AddControlBrowser cmi = new AddControlBrowser(gCurrentScreen, bfCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Navigaton_Image_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false) return;
            NavigationImage navCtrl = (NavigationImage)sender;
            AddControl addControl = new AddControl();
            AddControlNavigationImage cmi = new AddControlNavigationImage(gCurrentScreen, navCtrl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Property_Label_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false) return;
            PropertyLabel propLabel = (PropertyLabel)sender;
            AddControl addControl = new AddControl();
            AddControlPropertyLabel cmi = new AddControlPropertyLabel(gCurrentScreen, propLabel.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void Navigaton_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            NavigationImage navCtrl = (NavigationImage)sender;
            gCurrentScreen = navCtrl.screenName;
            Load_Screen(gCurrentScreen);
        }

        private void Timer_Label_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (editMode == false) return;
            TimerLabel tmrLbl = (TimerLabel)sender;
            AddControl addControl = new AddControl();
            AddControlTimerLabel cmi = new AddControlTimerLabel(gCurrentScreen, tmrLbl.screenObject.Name);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
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

        private void updateObjectCoords(OSAE.OSAEObject obj, string X, string Y)
        {
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "X", X, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, "Y", Y, "GUI");

            obj.Property("X").Value = X;
            obj.Property("Y").Value = Y;
        }

        private void updateObjectCoordsStateImg(OSAE.OSAEObject obj, string state, string X, string Y)
        {
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, state + " X", X, "GUI");
            OSAEObjectPropertyManager.ObjectPropertySet(obj.Name, state + " Y", Y, "GUI");

            obj.Property(state + " X").Value = X; 
            obj.Property(state + " Y").Value = Y;
        }

       
        #endregion
               
        #region menu events
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
            AddControl addControl = new AddControl();
            AddControlStateImage csi = new AddControlStateImage(gCurrentScreen);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.Show();
        }

        private void menuAddPropertyLabel_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlPropertyLabel csi = new AddControlPropertyLabel(gCurrentScreen);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.Show();
        }

        private void menuAddNavImage_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlNavigationImage cni = new AddControlNavigationImage(gCurrentScreen);
            addControl.Content = cni;
            addControl.Width = cni.Width + 80;
            addControl.Height = cni.Height + 80;
            addControl.Show();
        }

        private void menuAddClickImage_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlClickImage cmi = new AddControlClickImage(gCurrentScreen);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Owner = this;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        private void menuAddTimerLabel_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlTimerLabel csi = new AddControlTimerLabel(gCurrentScreen);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.Show();
        }

        private void menuAddCameraViewer_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddNewCameraViewer csi = new AddNewCameraViewer(gCurrentScreen);
            addControl.Width = csi.Width + 40;
            addControl.Height = csi.Height + 40;
            addControl.Content = csi;
            addControl.Show();
        }

        private void menuAddWebBrowser_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();

            AddControlBrowser csi = new AddControlBrowser(gCurrentScreen);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.Show();
        }

        private void menuAddUserControl_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlUserControl csi = new AddControlUserControl(gCurrentScreen);
            addControl.Width = csi.Width + 80;
            addControl.Height = csi.Height + 80;
            addControl.Content = csi;
            addControl.Show();
        }

        private void menuCreateScreen_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlScreen cscr = new AddControlScreen(gCurrentScreen);
            addControl.Width = cscr.Width + 80;
            addControl.Height = cscr.Height + 80;
            addControl.Content = cscr;
            addControl.ShowDialog();
            Load_Screen(gCurrentScreen);
        }

        #endregion

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closing = true;
        }

        private void menuChangeScreen_Click(object sender, RoutedEventArgs e)
        {
            
            ChangeScreen chgScrn = new ChangeScreen(this);
            chgScrn.Show();
        }

        private void menuFrameShow_Click(object sender, RoutedEventArgs e)     
        {
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;    
        }

        private void menuFrameHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.Width = canGUI.Width;
            this.Height = canGUI.Height;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;        
        }

        private void menuFrameTopLeft_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Left;
            this.Top = desktopWorkingArea.Top;
        }

        private void menuFrameBottomRight_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

        }

        private void menuFrameCentre_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = (desktopWorkingArea.Width / 2) - (this.Width /2);
            this.Top = (desktopWorkingArea.Height / 2) - (this.Height / 2);
        }       
    }
    
}
