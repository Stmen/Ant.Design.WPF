﻿#pragma warning disable 1591, 618
namespace Standard
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;

    internal static class DpiHelper
    {
        [ThreadStatic]
        private static Matrix _transformToDevice;
        [ThreadStatic]
        private static Matrix _transformToDip;

        /// <summary>
        /// Convert a point in device independent pixels (1/96") to a point in the system coordinates.
        /// </summary>
        /// <param name="logicalPoint">A point in the logical coordinate system.</param>
        /// <returns>Returns the parameter converted to the system's coordinates.</returns>
        public static Point LogicalPixelsToDevice(Point logicalPoint, double dpiScaleX, double dpiScaleY)
        {
            _transformToDevice = Matrix.Identity;
            _transformToDevice.Scale(dpiScaleX, dpiScaleY);
            return _transformToDevice.Transform(logicalPoint);
        }

        /// <summary>
        /// Convert a point in system coordinates to a point in device independent pixels (1/96").
        /// </summary>
        /// <param name="logicalPoint">A point in the physical coordinate system.</param>
        /// <returns>Returns the parameter converted to the device independent coordinate system.</returns>
        public static Point DevicePixelsToLogical(Point devicePoint, double dpiScaleX, double dpiScaleY)
        {
            _transformToDip = Matrix.Identity;
            _transformToDip.Scale(1d / dpiScaleX, 1d / dpiScaleY);
            return _transformToDip.Transform(devicePoint);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Rect LogicalRectToDevice(Rect logicalRectangle, double dpiScaleX, double dpiScaleY)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top), dpiScaleX, dpiScaleY);
            Point bottomRight = LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom), dpiScaleX, dpiScaleY);

            return new Rect(topLeft, bottomRight);
        }

        public static Rect DeviceRectToLogical(Rect deviceRectangle, double dpiScaleX, double dpiScaleY)
        {
            Point topLeft = DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top), dpiScaleX, dpiScaleY);
            Point bottomRight = DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom), dpiScaleX, dpiScaleY);

            return new Rect(topLeft, bottomRight);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Size LogicalSizeToDevice(Size logicalSize, double dpiScaleX, double dpiScaleY)
        {
            Point pt = LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height), dpiScaleX, dpiScaleY);

            return new Size { Width = pt.X, Height = pt.Y };
        }

        public static Size DeviceSizeToLogical(Size deviceSize, double dpiScaleX, double dpiScaleY)
        {
            Point pt = DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height), dpiScaleX, dpiScaleY);

            return new Size(pt.X, pt.Y);
        }

        public static Thickness LogicalThicknessToDevice(Thickness logicalThickness, double dpiScaleX, double dpiScaleY)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalThickness.Left, logicalThickness.Top), dpiScaleX, dpiScaleY);
            Point bottomRight = LogicalPixelsToDevice(new Point(logicalThickness.Right, logicalThickness.Bottom), dpiScaleX, dpiScaleY);

            return new Thickness(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        #region Per monitor dpi support

        public static DpiScale GetDpi(Visual visual)
        {
#if NET40 || NET45
            var source = PresentationSource.FromVisual(visual);

            if (source?.CompositionTarget == null)
            {
                return new DpiScale(1.0, 1.0);
            }

            var device = source.CompositionTarget.TransformToDevice;
            return new DpiScale(device.M11, device.M22);
#else
            return VisualTreeHelper.GetDpi(visual);
#endif
        }

        internal static DpiScale GetDpi(this Window window)
        {
            return GetDpi((Visual)window);
        }

        #endregion Per monitor dpi support
    }

#if NET40 || NET45
    /// <summary>Stores DPI information from which a <see cref="T:System.Windows.Media.Visual" /> or <see cref="T:System.Windows.UIElement" /> is rendered.</summary>
    public struct DpiScale
    {
        private readonly double _dpiScaleX;
        private readonly double _dpiScaleY;

        /// <summary>Gets the DPI scale on the X axis.</summary>
        /// <returns>The DPI scale for the X axis.</returns>
        public double DpiScaleX
        {
            get
            {
                return this._dpiScaleX;
            }
        }

        /// <summary>Gets the DPI scale on the Yaxis.</summary>
        /// <returns>The DPI scale for the Y axis.</returns>
        public double DpiScaleY
        {
            get
            {
                return this._dpiScaleY;
            }
        }

        /// <summary>Get or sets the PixelsPerDip at which the text should be rendered.</summary>
        /// <returns>The current <see cref="P:System.Windows.DpiScale.PixelsPerDip" /> value.</returns>
        public double PixelsPerDip
        {
            get
            {
                return this._dpiScaleY;
            }
        }

        /// <summary>Gets the DPI along X axis.</summary>
        /// <returns>The DPI along the X axis.</returns>
        public double PixelsPerInchX
        {
            get
            {
                return 96.0 * this._dpiScaleX;
            }
        }

        /// <summary>Gets the DPI along Y axis.</summary>
        /// <returns>The DPI along the Y axis.</returns>
        public double PixelsPerInchY
        {
            get
            {
                return 96.0 * this._dpiScaleY;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.DpiScale" /> structure.</summary>
        /// <param name="dpiScaleX">The DPI scale on the X axis.</param>
        /// <param name="dpiScaleY">The DPI scale on the Y axis. </param>
        public DpiScale(double dpiScaleX, double dpiScaleY)
        {
            this._dpiScaleX = dpiScaleX;
            this._dpiScaleY = dpiScaleY;
        }
    }
#endif
}
