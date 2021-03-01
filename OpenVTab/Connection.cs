using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Windows.Foundation;
using Windows.UI.Input;

namespace OpenVTab
{
    public enum PointerEventType
    {
        PointerCanceled = 0,
        PointerCaptureLost = 1,
        PointerEntered = 2,
        PointerExited = 3,
        PointerMoved = 4,
        PointerPressed = 5,
        PointerReleased = 6,
        PointerWheelChanged = 7
    }

    public enum EventType
    {
        Pointer = 0
    }

    [Flags]
    public enum PointerPropertiesFlags
    {
        None = 0,
        TouchConfidence = 1 << 0,
        LeftButtonPressed = 1 << 1,
        RightButtonPressed = 1 << 2,
        MiddleButtonPressed = 1 << 3,
        Inverted = 1 << 4,
        Eraser = 1 << 5,
        HorizontalMouseWheel = 1 << 6,
        Primary = 1 << 7,
        InRange = 1 << 8,
        Canceled = 1 << 9,
        BarrelButtonPressed = 1 << 10,
        XButton1Pressed = 1 << 11,
        XButton2Pressed = 1 << 12
    }

    internal class Connection
    {
        private readonly Socket _socket;
        public Size CanvasSize { get; set; }

        public Connection(string hostUri, short port)
        {
            _socket = GetHostSocket(hostUri, port);

            if (IsConnected()) return;
            Debug.WriteLine("Cannot connect to socket");
        }


        public bool IsConnected()
        {
            return _socket.Connected;
        }

        private int SendPointerPointProperties(PointerPointProperties properties)
        {
            var flags = PointerPropertiesFlags.None;
            if (properties.TouchConfidence)
                flags |= PointerPropertiesFlags.TouchConfidence;
            if (properties.IsLeftButtonPressed)
                flags |= PointerPropertiesFlags.LeftButtonPressed;
            if (properties.IsRightButtonPressed)
                flags |= PointerPropertiesFlags.RightButtonPressed;
            if (properties.IsMiddleButtonPressed)
                flags |= PointerPropertiesFlags.MiddleButtonPressed;
            if (properties.IsInverted)
                flags |= PointerPropertiesFlags.Inverted;
            if (properties.IsEraser)
                flags |= PointerPropertiesFlags.Eraser;
            if (properties.IsHorizontalMouseWheel)
                flags |= PointerPropertiesFlags.HorizontalMouseWheel;
            if (properties.IsPrimary)
                flags |= PointerPropertiesFlags.Primary;
            if (properties.IsInRange)
                flags |= PointerPropertiesFlags.InRange;
            if (properties.IsCanceled)
                flags |= PointerPropertiesFlags.Canceled;
            if (properties.IsBarrelButtonPressed)
                flags |= PointerPropertiesFlags.BarrelButtonPressed;
            if (properties.IsXButton1Pressed)
                flags |= PointerPropertiesFlags.XButton1Pressed;
            if (properties.IsXButton2Pressed)
                flags |= PointerPropertiesFlags.XButton2Pressed;

            int rc;
            var total = 0;
            if ((rc = _socket.Send(BitConverter.GetBytes(properties.Pressure))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes(properties.Orientation))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes(properties.XTilt))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes(properties.YTilt))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes(properties.Twist))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes(properties.MouseWheelDelta))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes((UInt16) flags))) < 0)
                return -1;
            total += rc;
            return total;
        }

        private int SendPoint(Point point)
        {
            int rc;
            var total = 0;
            if ((rc = _socket.Send(BitConverter.GetBytes(point.X))) < 0)
                return -1;
            total += rc;
            if ((rc = _socket.Send(BitConverter.GetBytes(point.Y))) < 0)
                return -1;
            total += rc;
            return total;
        }

        private int SendPointerPoint(PointerPoint ptrPt)
        {
            int rc;
            var total = 0;
            if ((rc = SendPoint(new Point(ptrPt.Position.X / CanvasSize.Width * 100,
                ptrPt.Position.Y / CanvasSize.Height * 100))) < 0)
                return -1;
            total += rc;
            if ((rc = SendPointerPointProperties(ptrPt.Properties)) < 0)
                return -1;
            total += rc;
            return total;
        }

        private int SendPointerEventType(PointerEventType eventType)
        {
            return _socket.Send(new[] {(byte) eventType});
        }

        private int SendEventType(EventType eventType)
        {
            return _socket.Send(new[] {(byte) eventType});
        }

        private int SendPointerEvent(PointerEventType eventType, PointerPoint ptrPt)
        {
            int rc;
            var total = 0;
            if ((rc = SendPointerEventType(eventType)) < 0)
                return -1;
            total += rc;
            if ((rc = SendPointerPoint(ptrPt)) < 0)
                return -1;
            total += rc;
            return total;
        }

        public int SendEvent(PointerEventType eventType, PointerPoint ptrPt)
        {
            int rc;
            var total = 0;
            if ((rc = SendEventType(EventType.Pointer)) < 0)
                return -1;
            total += rc;
            if ((rc = SendPointerEvent(eventType, ptrPt)) < 0)
                return -1;
            total += rc;
            return total;
        }

        private static Socket GetHostSocket(string hostUri, short port)
        {
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (IPAddress.TryParse(hostUri, out var ip))
            {
                s.Connect(ip, port);
                return s;
            }

            foreach (var address in Dns.GetHostAddresses(hostUri))
            {
                if (address.AddressFamily != AddressFamily.InterNetwork)
                    continue;
                s.Connect(address, port);
                if (s.Connected)
                    break;
            }

            return s;
        }
    }
}