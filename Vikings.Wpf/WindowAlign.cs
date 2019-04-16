using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Vanara.PInvoke;

namespace Vikings.Wpf
{
    public class WindowAlign : ListBox
    {
        public string[] IgnoreWindow { get; set; } = new string[] { "EdgeUiInputTopWndClass", "Windows.UI.Core.CoreWindow", "ApplicationFrameWindow" };

        Window Window => Window.GetWindow(this);

        static WindowAlign()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowAlign), new FrameworkPropertyMetadata(typeof(WindowAlign)));
            ItemsPanelProperty.OverrideMetadata(typeof(WindowAlign), new FrameworkPropertyMetadata(GetDefaultItemsPanelTemplate()));
        }

        static object GetDefaultItemsPanelTemplate()
        {
            ItemsPanelTemplate itemsPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(UniformGrid)));
            itemsPanelTemplate.Seal();
            return itemsPanelTemplate;
        }

        public WindowAlign()
        {
            ItemsSource = new List<Dock>
            {
                Dock.Left  | Dock.Top,
                Dock.Right | Dock.Top,
                Dock.Left  | Dock.Bottom,
                Dock.Right | Dock.Bottom
            };
        }

        [Flags]
        public enum Dock { None = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 };

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count != 1 || e.RemovedItems.Count > 1) return;
            Dock dockFrom = e.RemovedItems.Count == 1 ? (Dock)e.RemovedItems[0] : Dock.None;
            Dock dockTo = (Dock)e.AddedItems[0];
            if (Align(dockFrom, dockTo, out var location))
            {
                if (IsMouseCaptured) Mouse.Capture(null);
                Window.Left = location.X;
                Window.Top = location.Y;
                SelectedItem = null;
            }
        }

        public bool Align(Dock dockFrom, Dock dockTo, out Point location)
        {
            var area = SystemParameters.WorkArea;
            var window = Window.RestoreBounds;
            var listWindow = EnumWindows().Select(f => Rect.Intersect(f, area)).Where(f => f.Width > 0 && f.Height > 0 && f != area);
            if (dockFrom == Dock.None || (dockFrom & dockTo) == Dock.None)
            {
                window.X = (dockTo & Dock.Left) == Dock.Left ? 0 : area.Right - window.Width;
                window.Y = (dockTo & Dock.Top) == Dock.Top ? 0 : area.Bottom - window.Height;
                location = window.Location;
                return !listWindow.Any(f => f.IntersectsWith(window));
            }

            Matrix matrix = Matrix.Identity;
            if ((dockFrom & Dock.Right) == Dock.Right) matrix.ScaleAt(-1, 1, area.Width / 2, area.Height / 2);//水平翻转
            if ((dockFrom & Dock.Bottom) == Dock.Bottom) matrix.ScaleAt(1, -1, area.Width / 2, area.Height / 2);//垂直翻转
            if (((dockTo & dockFrom) & (Dock.Left | Dock.Right)) != Dock.None) { matrix.Rotate(90); matrix.Scale(-1, 1); }//横纵转换
            return CaleLocation(listWindow, area, window, matrix, out location);
        }

        protected virtual List<Rect> EnumWindows()
        {
            var handleWindow = new WindowInteropHelper(Window).Handle;
            var list = new List<Rect>();
            User32_Gdi.EnumWindows((hWnd, _) =>
            {
                if (hWnd != handleWindow && User32_Gdi.IsWindowVisible(hWnd) && User32_Gdi.GetWindowRect(hWnd, out var lpRect))
                {
                    var sb = new StringBuilder(256);
                    User32_Gdi.GetClassName(hWnd, sb, 255);
                    if (!IgnoreWindow.Contains(sb.ToString())) list.Add(new Rect(lpRect.X, lpRect.Y, lpRect.Width, lpRect.Height));
                }
                return true;
            }, IntPtr.Zero);
            return list;
        }

        static bool CaleLocation(IEnumerable<Rect> listWindow, Rect area, Rect window, Matrix matrix, out Point location)
        {
            location = window.Location;
            listWindow = listWindow.Select(f => Rect.Transform(f, matrix)).ToList();
            area.Transform(matrix);
            window.Transform(matrix);
            matrix.Invert();//反转
            window.Location = new Point(0, 0);
            while (window.Bottom < area.Bottom)
            {
                double minY = area.Bottom;
                while (window.Right < area.Right)
                {
                    var first = listWindow.FirstOrDefault(f => f.IntersectsWith(window));
                    if (first == default(Rect))
                    {
                        window.Transform(matrix);
                        location = window.Location;
                        return true;
                    }
                    window.X = first.Right + 1;
                    minY = Math.Min(minY, first.Bottom);
                }
                window.X = 0;
                window.Y = minY + 1;
            }
            return false;
        }
    }
}
