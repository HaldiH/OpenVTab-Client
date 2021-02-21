using System;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Devices.Input;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Sockets;
using System.Text;
using Windows.ApplicationModel.Core;

namespace OpenVTab
{
    public sealed partial class MainPage : Page
    {
        private readonly Connection _conn;
        private readonly Frame _frame;

        public MainPage()
        {
            _frame = (Frame) Window.Current.Content;
            _conn = new Connection("192.168.2.30", 4444);
            this.InitializeComponent();

            _frame.SizeChanged += new SizeChangedEventHandler(Container_SizeChanged);

            // Declare the pointer event handlers.
            Target.PointerCanceled += new PointerEventHandler(Target_PointerCanceled);
            Target.PointerCaptureLost += new PointerEventHandler(Target_PointerCaptureLost);
            Target.PointerEntered += new PointerEventHandler(Target_PointerEntered);
            Target.PointerExited += new PointerEventHandler(Target_PointerExited);
            Target.PointerMoved += new PointerEventHandler(Target_PointerMoved);
            Target.PointerPressed += new PointerEventHandler(Target_PointerPressed);
            Target.PointerReleased += new PointerEventHandler(Target_PointerReleased);
            Target.PointerWheelChanged += new PointerEventHandler(Target_PointerWheelChanged);

            //_conn.SendConfig(new ConfigData(Target.Width, Target.Height));
            _conn.CanvasSize = Target.RenderSize;
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = _frame.ActualWidth;
            var height = _frame.ActualHeight;
            Target.Width = width;
            Target.Height = height;
            Container.Width = width;
            Container.Height = height;
            _conn.CanvasSize = new Size(width, height);
            //_conn.SendEvent(WindowEventType.WindowSizeChanged, Window.Current);
        }


        private void Target_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerCanceled, e.GetCurrentPoint(Target));
        }

        private void Target_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerCaptureLost, e.GetCurrentPoint(Target));
        }

        private void Target_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerEntered, e.GetCurrentPoint(Target));
        }

        private void Target_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerExited, e.GetCurrentPoint(Target));
        }

        private void Target_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerMoved, e.GetCurrentPoint(Target));
        }

        private void Target_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerPressed, e.GetCurrentPoint(Target));
        }

        private void Target_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerReleased, e.GetCurrentPoint(Target));
        }

        private void Target_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
            _conn.SendEvent(PointerEventType.PointerWheelChanged, e.GetCurrentPoint(Target));
        }
    }
}