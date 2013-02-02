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
    using OSAE.UI.Controls;   

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OSAE.OSAE OSAEApi = new OSAE.OSAE("GUI");

        OSAE.Logging logging = new OSAE.Logging("GUI");
        
        String gAppName = "";
        String gCurrentScreen = "";

        List<StateImage> stateImages = new List<StateImage>();
        List<NavigationImage> navImages = new List<NavigationImage>();
        List<VideoStreamViewer> cameraViewers = new List<VideoStreamViewer>();
        List<PropertyLabel> propLabels = new List<PropertyLabel>();
        List<OSAE.UI.Controls.StaticLabel> staticLabels = new List<OSAE.UI.Controls.StaticLabel>();
        List<OSAE.UI.Controls.TimerLabel> timerLabels = new List<OSAE.UI.Controls.TimerLabel>();

        bool loadingScreen = true;

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

            Thread thread = new Thread(() => Update_Objects());
            thread.Start();

            this.canGUI.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
            this.canGUI.Drop += new DragEventHandler(DragSource_Drop);
        }

        private void Load_Screen(String sScreen)
        {
            loadingScreen = true;
            logging.AddToLog("Loading screen: " + sScreen, false);
            gCurrentScreen = sScreen;
            String sPath = "";
            OSAEApi.ObjectPropertySet(gAppName, "Current Screen", sScreen);
            sPath = OSAEApi.APIpath + OSAEApi.GetObjectPropertyValue(sScreen, "Background Image").Value;
            if (File.Exists(sPath))
            {
                byte[] byteArray = File.ReadAllBytes(sPath);
                var imageStream = new MemoryStream(byteArray);
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
        }


        private void Load_Objects(String sScreen)
        {
            String sStateMatch = "";
            String sImage = "";


            List<OSAE.OSAEObject> screenObjects = OSAEApi.GetObjectsByContainer(sScreen);

            foreach (OSAE.OSAEObject obj in screenObjects)
            {
                #region CONTROL STATE IMAGE
                if (obj.Type == "CONTROL STATE IMAGE")
                {
                    StateImage stateImageControl = new StateImage(obj);

                    foreach (OSAE.ObjectProperty p in obj.Properties)
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
                #endregion

                #region CONTROL NAVIGATION IMAGE
                else if (obj.Type == "CONTROL NAVIGATION IMAGE")
                {
                    try
                    {
                        NavigationImage navImageControl = new NavigationImage(obj.Property("Screen").Value, obj.Property("Image").Value, obj);
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
                        string stream = OSAEApi.GetObjectPropertyValue(obj.Property("Object Name").Value, "Stream Address").Value;
                        VideoStreamViewer vsv = new VideoStreamViewer(stream);
                        canGUI.Children.Add(vsv);
                        OSAE.ObjectProperty pZOrder = obj.Property("ZOrder");
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
                        controlTypes.Add(typeof(Weather));
                        wc.PreviewMouseMove += new MouseEventHandler(DragSource_PreviewMouseMove);
                    }
                }
                #endregion
                
                //If iStateImageList.EndsWith(",") Then iStateImageList = iStateImageList.Substring(0, iStateImageList.Length - 1)
                //Timer1.Enabled = True
            }
        }

        private void Update_Objects()
        {
            while (true)
            {
                if (!loadingScreen)
                {
                    logging.AddToLog("Entering Update_Objects", false);
                    List<OSAE.OSAEScreenControl> controls = OSAEApi.GetScreenControls(gCurrentScreen);

                    foreach (OSAE.OSAEScreenControl newCtrl in controls)
                    {
                        #region CONTROL STATE IMAGE
                        if (newCtrl.ControlType == "CONTROL STATE IMAGE")
                        {
                            foreach (StateImage sImage in stateImages)
                            {
                                if (newCtrl.ControlName == sImage.screenObject.Name && newCtrl.LastUpdated != sImage.LastUpdated)
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
                            }
                        }
                        #endregion

                        #region CONTROL PROPERTY LABEL
                        else if (newCtrl.ControlType == "CONTROL PROPERTY LABEL")
                        {
                            foreach (PropertyLabel pl in propLabels)
                            {
                                if (newCtrl.ControlName == pl.screenObject.Name && newCtrl.LastUpdated != pl.LastUpdated)
                                {
                                    logging.AddToLog("Updating:  " + newCtrl.ControlName, false);
                                    pl.LastUpdated = newCtrl.LastUpdated; 
                                    pl.Update();
                                    logging.AddToLog("Complete:  " + newCtrl.ControlName, false);
                                }
                            }
                        }
                        #endregion

                        #region CONTROL TIMER LABEL
                        else if (newCtrl.ControlType == "CONTROL TIMER LABEL")
                        {
                            foreach (OSAE.UI.Controls.TimerLabel tl in timerLabels)
                            {
                                if (newCtrl.ControlName == tl.screenObject.Name && newCtrl.LastUpdated != tl.LastUpdated)
                                {
                                    logging.AddToLog("Updating:  " + newCtrl.ControlName, false);
                                    tl.LastUpdated = newCtrl.LastUpdated;
                                    tl.Update();
                                    logging.AddToLog("Complete:  " + newCtrl.ControlName, false);
                                }
                            }
                        }
                        #endregion
                    }
                    logging.AddToLog("Leaving Update_Objects", false);
                }
                System.Threading.Thread.Sleep(1000);
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

        private void canvas1_RightButtonDown(object sender, RoutedEventArgs e)
        {
            //mnuMain. = true;
        }

        private void Navigaton_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            NavigationImage navCtrl = (NavigationImage)sender;
            gCurrentScreen = navCtrl.screenName;
            stateImages.Clear();
            propLabels.Clear();
            navImages.Clear();
            cameraViewers.Clear();
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

                if (beingDragged.GetType() == typeof(StateImage))
                {
                    Thread thread = new Thread(() => updateObjectCoordsStateImg(beingDragged.screenObject.Name, beingDragged.StateMatch, newX.ToString(), newY.ToString()));
                    thread.Start();
                }
                else
                {
                    Thread thread = new Thread(() => updateObjectCoords(beingDragged.screenObject.Name, newX.ToString(), newY.ToString()));
                    thread.Start();
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

        private void updateObjectCoords(string name, string X, string Y)
        {
            OSAEApi.ObjectPropertySet(name, "X", X);
            OSAEApi.ObjectPropertySet(name, "Y", Y);
        }

        private void updateObjectCoordsStateImg(string name, string state, string X, string Y)
        {
            OSAEApi.ObjectPropertySet(name, state + " X", X);
            OSAEApi.ObjectPropertySet(name, state + " Y", Y);
        }
        #endregion
    }
    
}
