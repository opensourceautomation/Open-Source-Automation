namespace GUI2
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
        Logging logging = Logging.GetLogger("GUI");
        
        String gAppName = "";
        String gCurrentScreen = "";

        List<StateImage> stateImages = new List<StateImage>();
        List<NavigationImage> navImages = new List<NavigationImage>();
        List<MethodImage> methodImages = new List<MethodImage>();
        List<VideoStreamViewer> cameraViewers = new List<VideoStreamViewer>();
        List<PropertyLabel> propLabels = new List<PropertyLabel>();
        List<StaticLabel> staticLabels = new List<StaticLabel>();
        List<TimerLabel> timerLabels = new List<TimerLabel>();
        List<dynamic> userControls = new List<dynamic>();

        bool loadingScreen = true;
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
                logging.AddToLog("Error starting GUI: " + ex.Message,true);
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
            gCurrentScreen = OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Default Screen").Value;
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
                stateImages.Clear();
                propLabels.Clear();
                navImages.Clear();
                methodImages.Clear();
                cameraViewers.Clear();
                canGUI.Children.Clear(); 
                
                loadingScreen = true;
                logging.AddToLog("Loading screen: " + sScreen, false);
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
                Thread thread = new Thread(() => Load_Objects(sScreen));
                thread.Start();

                logging.AddToLog("Loading screen complete: " + sScreen, false);
            }
            catch (Exception ex)
            {
                logging.AddToLog("Failed to load screen: " + sScreen, true);
            }
        }

        private void Load_Objects(String sScreen)
        {
            OSAEObjectCollection screenObjects = OSAEObjectManager.GetObjectsByContainer(sScreen);

            foreach (OSAE.OSAEObject obj in screenObjects)
            {
                LoadControl(obj);
            }
            loadingScreen = false;
        }

        private void Update_Objects()
        {
            while (!closing)
            {
                if (!loadingScreen)
                {
                    bool oldCtrl = false;
                    logging.AddToLog("Entering Update_Objects", false);
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
                                        logging.AddToLog("Updating:  " + newCtrl.ControlName, false);
                                        sImage.LastUpdated = newCtrl.LastUpdated;
                                        sImage.Update();

                                        this.Dispatcher.Invoke((Action)(() =>
                                        {
                                            Canvas.SetLeft(sImage, sImage.Location.X);
                                            Canvas.SetTop(sImage, sImage.Location.Y);
                                        }));
                                        logging.AddToLog("Complete:  " + newCtrl.ControlName, false);
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
                                    if (newCtrl.LastUpdated != pl.LastUpdated)
                                    {
                                        logging.AddToLog("Updating:  " + newCtrl.ControlName, false);
                                        pl.LastUpdated = newCtrl.LastUpdated;
                                        pl.Update();
                                        logging.AddToLog("Complete:  " + newCtrl.ControlName, false);
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
                                if (newCtrl.ControlName == tl.screenObject.Name)
                                {
                                    if (newCtrl.LastUpdated != tl.LastUpdated)
                                    {
                                        logging.AddToLog("Updating:  " + newCtrl.ControlName, false);
                                        tl.LastUpdated = newCtrl.LastUpdated;
                                        tl.Update();
                                        logging.AddToLog("Complete:  " + newCtrl.ControlName, false);
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

                        #region CONTROL NAVIGATION IMAGE
                        else if (newCtrl.ControlType == "CONTROL METHOD IMAGE")
                        {
                            foreach (OSAE.UI.Controls.MethodImage method in methodImages)
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
                        
                        if (!oldCtrl)
                        {
                            OSAE.OSAEObject obj = OSAEObjectManager.GetObjectByName(newCtrl.ControlName);
                            logging.AddToLog("Load new control: " + newCtrl.ControlName, false);
                            LoadControl(obj);
                        }
                    }
                    logging.AddToLog("Leaving Update_Objects", false);
                }
                System.Threading.Thread.Sleep(1000);
            }
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
                        if (p.Value.ToLower() == stateImageControl.CurState.ToLower())
                        {
                            sStateMatch = p.Name.Substring(0, p.Name.LastIndexOf(' '));
                        }
                    }
                    int dZ = Int32.Parse(obj.Property("ZOrder").Value);
                    stateImageControl.Location.X = Double.Parse(obj.Property(sStateMatch + " X").Value);
                    stateImageControl.Location.Y = Double.Parse(obj.Property(sStateMatch + " Y").Value);
                    canGUI.Children.Add(stateImageControl);
                    Canvas.SetLeft(stateImageControl, stateImageControl.Location.X);
                    Canvas.SetTop(stateImageControl, stateImageControl.Location.Y);
                    Canvas.SetZIndex(stateImageControl, dZ);
                    stateImages.Add(stateImageControl);
                    controlTypes.Add(typeof(StateImage));
                    stateImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                }
                #endregion

                #region CONTROL PROPERTY LABEL
                else if (obj.Type == "CONTROL PROPERTY LABEL")
                {
                    logging.AddToLog("Loading PropertyLabelControl: " + obj.Name, false);
                    PropertyLabel pl = new PropertyLabel(obj);
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
                #endregion

                #region CONTROL STATIC LABEL
                else if (obj.Type == "CONTROL STATIC LABEL")
                {
                    logging.AddToLog("Loading PropertyLabelControl: " + obj.Name, false);
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
                #endregion

                #region CONTROL TIMER LABEL
                else if (obj.Type == "CONTROL TIMER LABEL")
                {
                    logging.AddToLog("Loading PropertyLabelControl: " + obj.Name, false);
                    OSAE.UI.Controls.TimerLabel tl = new OSAE.UI.Controls.TimerLabel(obj);
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
                #endregion

                #region CONTROL METHOD IMAGE
                else if (obj.Type == "CONTROL METHOD IMAGE")
                {
                    try
                    {
                        MethodImage methodImageControl = new MethodImage(obj);
                        canGUI.Children.Add(methodImageControl);

                        OSAE.OSAEObjectProperty pZOrder = obj.Property("ZOrder");
                        OSAE.OSAEObjectProperty pX = obj.Property("X");
                        OSAE.OSAEObjectProperty pY = obj.Property("Y");
                        Double dX = Convert.ToDouble(pX.Value);
                        Canvas.SetLeft(methodImageControl, dX);
                        Double dY = Convert.ToDouble(pY.Value);
                        Canvas.SetTop(methodImageControl, dY);
                        int dZ = Convert.ToInt32(pZOrder.Value);
                        Canvas.SetZIndex(methodImageControl, dZ);
                        methodImageControl.Location.X = dX;
                        methodImageControl.Location.Y = dY;
                        methodImages.Add(methodImageControl);
                        controlTypes.Add(typeof(MethodImage));
                        methodImageControl.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                    catch (MySqlException myerror)
                    {
                        MessageBox.Show("GUI Error Load Navigation Image: " + myerror.Message);
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
        }

        private void Navigaton_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            NavigationImage navCtrl = (NavigationImage)sender;
            gCurrentScreen = navCtrl.screenName;
            
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

        private void menuAddMethodImage_Click(object sender, RoutedEventArgs e)
        {
            AddControl addControl = new AddControl();
            AddControlMethodImage cmi = new AddControlMethodImage(gCurrentScreen);
            addControl.Content = cmi;
            addControl.Width = cmi.Width + 80;
            addControl.Height = cmi.Height + 80;
            addControl.Show();
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
            CreateScreen addControl = new CreateScreen(this);
            addControl.Show();
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
