using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageOnTop
{
    public partial class MainWindow : Window
    {
        const int WM_MOUSEHWHEEL = 0x020E;
        int tiltThreshlod = 55;
        int lastTiltValue = 0;
        TimeSpan duration = TimeSpan.FromMilliseconds(150);
        System.Windows.Point lastPoint;
        string uri;
        bool ctrlIsDown = false;
        bool shiftIsDown = false;
        double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        double originalWidth;
        double originalHeight;

        bool attachedHorizontally = true;
        bool attachedVertically = true;
        string horizontalAttach = "right";
        string verticalAttach = "top";

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetImage(string uri)
        {
            this.uri = uri;
            ImageBrush sentImage = new ImageBrush();
            sentImage.ImageSource = new BitmapImage(new Uri(uri, UriKind.Absolute));
            this.Background = sentImage;
            double width = sentImage.ImageSource.Width;
            double height = sentImage.ImageSource.Height;

            if (width > (screenWidth - 10) * 2 / 5 || height > (screenHeight - 75))
            {
                if (width / ((screenWidth - 10) * 2 / 5) > height / (screenHeight - 75))
                {
                    originalWidth = (screenWidth - 10) * 2 / 5;
                    originalHeight = (screenWidth - 10) * 2 / 5 * (height / width);
                }
                else
                {
                    originalHeight = screenHeight - 75;
                    originalWidth = (screenHeight - 75) * (width / height);
                }
            }
            else
            {
                originalWidth = width;
                originalHeight = height;
            }
            this.Width = originalWidth;
            this.Height = originalHeight;
            this.Top = 30;
            this.Left = screenWidth - this.Width - 5;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(this);
            double x = p.X;
            double y = p.Y;
            lastPoint = new System.Windows.Point(x, y);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            this.BeginAnimation(MainWindow.LeftProperty, null);
            this.BeginAnimation(MainWindow.TopProperty, null);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                this.attachedHorizontally = false;
                this.attachedVertically = false;

                System.Windows.Point p = e.GetPosition(this);
                double x = p.X;
                double y = p.Y;

                this.Left += x - lastPoint.X;
                this.Top += y - lastPoint.Y;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.BeginAnimation(MainWindow.LeftProperty, null);
            this.BeginAnimation(MainWindow.TopProperty, null);

            if (e.Key == Key.Escape)
            {
                System.Diagnostics.Debug.WriteLine("Escape pressed. Closing the app");
                this.Close();
            }
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                this.ctrlIsDown = true;
            }
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                this.shiftIsDown = true;
            }
            if (e.Key == Key.Space)
            {
                resize(originalWidth, originalHeight);
            }

            if (e.Key == Key.Left)
            {
                if (this.ctrlIsDown)
                {
                    MoveSide("left");
                }
                else if (this.shiftIsDown)
                {
                    this.Left -= 100;
                    attachedHorizontally = false;
                }
                else
                {
                    this.Left -= 10;
                    attachedHorizontally = false;
                }
            }
            if (e.Key == Key.Right)
            {
                if (this.ctrlIsDown)
                {
                    MoveSide("right");
                }
                else if (this.shiftIsDown)
                {
                    this.Left += 100;
                    attachedHorizontally = false;
                }
                else
                {
                    this.Left += 10;
                    attachedHorizontally = false;
                }
            }
            if (e.Key == Key.Up)
            {
                if (this.ctrlIsDown)
                {
                    MoveSide("up");
                }
                else if (this.shiftIsDown)
                {
                    this.Top -= 100;
                    attachedVertically = false;
                }
                else
                {
                    this.Top -= 10;
                    attachedVertically = false;
                }
            }
            if (e.Key == Key.Down)
            {
                if (this.ctrlIsDown)
                {
                    MoveSide("bottom");
                }
                else if (this.shiftIsDown)
                {
                    this.Top += 100;
                    attachedVertically = false;
                }
                else
                {
                    this.Top += 10;
                    attachedVertically = false;
                }
            }

            if (e.Key == Key.Add)
            {
                double newWidth = this.Width + this.Width * 0.1;
                double newHeight = this.Height + this.Height * 0.1;

                resize(newWidth, newHeight);
            }
            if (e.Key == Key.Subtract)
            {
                double newWidth = this.Width - this.Width * 0.1;
                double newHeight = this.Height - this.Height * 0.1;

                resize(newWidth, newHeight);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                this.ctrlIsDown = false;
            }
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                this.shiftIsDown = false;
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Double clicked. Closing the app");
            this.Close();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.BeginAnimation(MainWindow.LeftProperty, null);
            this.BeginAnimation(MainWindow.TopProperty, null);

            double delta = e.Delta;

            double newWidth = this.Width + this.Width * delta * 0.001;
            double newHeight = this.Height + this.Height * delta * 0.001;

            double newLeft = this.Left + this.Left * delta * 0.001;
            double newTop = this.Top + this.Top * delta * 0.001;

            resize(newWidth, newHeight);
        }

        private void resize(double newWidth, double newHeight)
        {
            if (newWidth >= 5 && newWidth < (screenWidth - 10) && newHeight >= 5 && newHeight < (screenHeight - 75))
            {
                if (this.attachedHorizontally)
                {
                    if (this.horizontalAttach == "left")
                    {
                        this.Left = 5;
                    }
                    else if (this.horizontalAttach == "right")
                    {
                        this.Left = this.Left + (this.Width - newWidth);
                    }
                }
                else
                {
                    this.Left = this.Left + (this.Width - newWidth) / 2;
                }

                if (this.attachedVertically)
                {
                    if (this.verticalAttach == "top")
                    {
                        this.Top = 30;
                    }
                    else if (this.verticalAttach == "bottom")
                    {
                        this.Top = this.Top + (this.Height - newHeight);
                    }
                }
                else
                {
                    this.Top = this.Top + (this.Height - newHeight) / 2;
                }
                this.Width = newWidth;
                this.Height = newHeight;
            }
        }

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = new System.Drawing.Bitmap(uri);

            IDataObject data = new DataObject();
            data.SetData(image);
            Clipboard.SetDataObject(data, true); // true means copy
        }

        /// Beginning of support of horizontal scrolling
        protected override void OnSourceInitialized(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnTruc");
            var source = PresentationSource.FromVisual(this);
            ((System.Windows.Interop.HwndSource)source)?.AddHook(Hook);
        }

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWHEEL:
                    int tilt = (short)HIWORD(wParam);
                    OnMouseTilt(tilt);
                    return (IntPtr)1;
            }
            return IntPtr.Zero;
        }

        private static int HIWORD(IntPtr ptr)
        {
            var val32 = ptr.ToInt32();
            return ((val32 >> 16) & 0xFFFF);
        }

        private static int LOWORD(IntPtr ptr)
        {
            var val32 = ptr.ToInt32();
            return (val32 & 0xFFFF);
        }

        private void OnMouseTilt(int tilt)
        {
            if (Math.Abs(tilt - lastTiltValue) < 100)
            {
                System.Diagnostics.Debug.WriteLine(tilt);
                if (tilt < -tiltThreshlod)
                {
                    System.Diagnostics.Debug.WriteLine("Tilt right");
                    MoveSide("right");
                }
                else if (tilt > tiltThreshlod)
                {
                    System.Diagnostics.Debug.WriteLine("Tilt left");
                    MoveSide("left");
                }
            }

            lastTiltValue = tilt;
        }
        /// End of functions made for the horizontal tilt

        private void MoveSide(string side)
        {
            System.Diagnostics.Debug.WriteLine("In move side");
            System.Diagnostics.Debug.WriteLine(side);

            if (side == "left" || side == "right")
            {
                double newLeft = 5;
                if (side == "right")
                {
                    newLeft = screenWidth - this.Width - 5;
                }
                this.attachedHorizontally = true;
                this.horizontalAttach = side;

                System.Windows.Media.Animation.DoubleAnimation animation = new System.Windows.Media.Animation.DoubleAnimation(newLeft, duration);
                this.BeginAnimation(MainWindow.LeftProperty, animation);
            }
            else
            {
                double newTop = 30;
                if (side == "bottom")
                {
                    newTop = screenHeight - this.Height - 45;
                }
                this.attachedVertically = true;
                this.verticalAttach = side;

                System.Windows.Media.Animation.DoubleAnimation animation = new System.Windows.Media.Animation.DoubleAnimation(newTop, duration);
                this.BeginAnimation(MainWindow.TopProperty, animation);
            }
        }
    }
}
