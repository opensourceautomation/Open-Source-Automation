using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OSAE.UI.Controls
{
    /// <summary>
    /// Interaction logic for ClickImage.xaml
    /// </summary>
    public partial class ClickImage : UserControl
    {
        public Point Location;
        public OSAEObject screenObject { get; set; }
        private string gAppName = "";
        private string currentUser;
        private string PressObjectName = "";
        private string PressMethodName = "";
        private string PressMethodParam1 = "";
        private string PressMethodParam2 = "";
        private string PressScriptName = "";
        private string PressScriptParam1 = "";
        private string PressScriptParam2 = "";
        private string PressX = "";
        private string PressY = "";
        public double ImageWidth;
        public double ImageHeight;

        private string ReleaseObjectName = "";
        private string ReleaseMethodName = "";
        private string ReleaseMethodParam1 = "";
        private string ReleaseMethodParam2 = "";
        private string ReleaseScriptName = "";
        private string ReleaseScriptParam1 = "";
        private string ReleaseScriptParam2 = "";
        private string XPos = "";
        private string YPos = "";

        private OSAEImageManager imgMgr = new OSAEImageManager();

        public ClickImage(OSAEObject sObj, string appName, string user)

        {
            InitializeComponent();
            screenObject = sObj;
            gAppName = appName;
            currentUser = user;
            PressObjectName = screenObject.Property("Press Object Name").Value;
            PressMethodName = screenObject.Property("Press Method Name").Value;
            PressMethodParam1 = screenObject.Property("Press Method Param 1").Value;
            PressMethodParam2 = screenObject.Property("Press Method Param 2").Value;
            PressScriptName = screenObject.Property("Press Script Name").Value;
            PressScriptParam1 = screenObject.Property("Press Script Param 1").Value;
            PressScriptParam2 = screenObject.Property("Press Script Param 2").Value;

            ReleaseObjectName = screenObject.Property("Release Object Name").Value;
            ReleaseMethodName = screenObject.Property("Release Method Name").Value;
            ReleaseMethodParam1 = screenObject.Property("Release Method Param 1").Value;
            ReleaseMethodParam2 = screenObject.Property("Release Method Param 2").Value;
            ReleaseScriptName = screenObject.Property("Release Script Name").Value;
            ReleaseScriptParam1 = screenObject.Property("Release Script Param 1").Value;
            ReleaseScriptParam2 = screenObject.Property("Release Script Param 2").Value;

            XPos = screenObject.Property("X").Value;
            YPos = screenObject.Property("Y").Value;

            Image.Tag = screenObject.Name;
            Image.ToolTip = Image.Tag;
            Image.MouseLeftButtonUp += new MouseButtonEventHandler(Click_Image_MouseLeftButtonUp);
            Image.MouseLeftButtonDown += new MouseButtonEventHandler(Click_Image_MouseLeftButtonDown);
            string imgName = screenObject.Property("Normal Image").Value;
            OSAEImage img = imgMgr.GetImage(imgName);

            if (img != null)
            {
                var imageStream = new MemoryStream(img.Data);
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();

                ImageWidth = bitmapImage.Width;
                ImageHeight = bitmapImage.Height;
                Image.Source = bitmapImage;
                Image.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Image.Source = null;
                Image.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void Click_Image_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            string currentUser = OSAE.OSAEObjectPropertyManager.GetObjectPropertyValue(gAppName, "Current User").Value;
            if (currentUser == "") return;

            string imgName = screenObject.Property("Pressed Image").Value;
            OSAEImage img = imgMgr.GetImage(imgName);

            if (img != null)
            {
                var imageStream = new MemoryStream(img.Data);
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
                Image.Source = bitmapImage;
                Image.Visibility = System.Windows.Visibility.Visible;
            }
           // else
          //  {
          //      Image.Source = null;
          //      Image.Visibility = System.Windows.Visibility.Hidden;
         //   }
            if (PressMethodName != "")
            {
                if (PressMethodParam1 == "[ASK]" | PressMethodParam2 == "[ASK]")
                {
                    ParamInput addControl = new ParamInput("Method", screenObject,currentUser);
                    string cppX = screenObject.Property("X").Value;
                    string cppY = screenObject.Property("Y").Value;
                    double cpp_X = Convert.ToDouble(cppX);
                    double cpp_Y = Convert.ToDouble(cppY);
                    if (cpp_X < 320) { cpp_X = cpp_X + 200; }

                    addControl.Left = cpp_X;
                    addControl.Top = cpp_Y;
                    addControl.Show();
                }
                else
                {
                    OSAEMethodManager.MethodQueueAdd(PressObjectName, PressMethodName, PressMethodParam1, PressMethodParam2, currentUser);
                }
            }
            if (PressScriptName != "")
            {
                if (PressMethodParam1 == "[ASK]" | PressMethodParam2 == "[ASK]")
                {
                    ParamInput addControl = new ParamInput("Method", screenObject, currentUser);
                    string cppX = screenObject.Property("X").Value;
                    string cppY = screenObject.Property("Y").Value;
                    double cpp_X = Convert.ToDouble(cppX);
                    double cpp_Y = Convert.ToDouble(cppY);
                    if (cpp_X < 320) { cpp_X = cpp_X + 200; }

                    addControl.Left = cpp_X;
                    addControl.Top = cpp_Y;
                    addControl.Show();
                }
                else
                {
                    OSAEMethodManager.MethodQueueAdd(PressObjectName, PressMethodName, PressMethodParam1, PressMethodParam2, currentUser);
                }
            }
        }


        private void Click_Image_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            string imgName = screenObject.Property("Normal Image").Value;
            OSAEImage img = imgMgr.GetImage(imgName);

            if (img.Data != null)
            {
                var imageStream = new MemoryStream(img.Data);
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = imageStream;
                bitmapImage.EndInit();
                Image.Source = bitmapImage;
                Image.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Image.Source = null;
                Image.Visibility = System.Windows.Visibility.Hidden;
            }
            if (ReleaseMethodName != "")
            {
                if (ReleaseMethodParam1 == "[ASK]" | ReleaseMethodParam2 == "[ASK]")
                {
                    ParamInput addControl = new ParamInput("Method", screenObject, currentUser);
                    string cppX = screenObject.Property("X").Value;
                    string cppY = screenObject.Property("Y").Value;
                    double cpp_X = Convert.ToDouble(cppX);
                    double cpp_Y = Convert.ToDouble(cppY);
                    if (cpp_X < 320) { cpp_X = cpp_X + 200; }

                    addControl.Left = cpp_X;
                    addControl.Top = cpp_Y;
                    addControl.Show();
                }
                else
                {
                    OSAEMethodManager.MethodQueueAdd(ReleaseObjectName, ReleaseMethodName, ReleaseMethodParam1, ReleaseMethodParam2, currentUser);
                }
            }
            if (PressScriptName != "")
            {
                if (PressScriptParam1 == "[ASK]" | PressScriptParam2 == "[ASK]")
                {
                    ParamInput addControl = new ParamInput("Method", screenObject,currentUser);
                    string cppX = screenObject.Property("X").Value;
                    string cppY = screenObject.Property("Y").Value;
                    double cpp_X = Convert.ToDouble(cppX);
                    double cpp_Y = Convert.ToDouble(cppY);
                    if (cpp_X < 320) { cpp_X = cpp_X + 200; }

                    addControl.Left = cpp_X;
                    addControl.Top = cpp_Y;
                    addControl.Show();
                }
                else
                {
                    OSAEMethodManager.MethodQueueAdd(ReleaseObjectName, ReleaseMethodName, ReleaseMethodParam1, ReleaseMethodParam2, currentUser);
                }
            }
        }     
    }
}
