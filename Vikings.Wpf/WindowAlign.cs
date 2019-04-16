using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public WindowAlign()
        {
            ItemsSource = Enum.GetValues(typeof(Location));
        }

        enum Location { LeftTop, RightTop, LeftBottom, RightBottom };

        private static object GetDefaultItemsPanelTemplate()
        {
            ItemsPanelTemplate itemsPanelTemplate = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(UniformGrid)));
            itemsPanelTemplate.Seal();
            return itemsPanelTemplate;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count != 1 || e.RemovedItems.Count > 1) return;
            //CodeSite.SendCollection("RemovedItems", e.RemovedItems);
            //CodeSite.SendCollection("AddedItems", e.AddedItems);
            var locationNew = (Location)e.AddedItems[0];
            if (e.RemovedItems.Count == 0 || (e.RemovedItems[0] is Location locationOld && (int)locationNew + (int)locationOld == 3))
            {
                //CodeSite.Send("1");
                //SelectedItem = null;
                return;
            }
            //CodeSite.Send("2");
            locationOld = (Location)e.RemovedItems[0];
            var area = SystemParameters.WorkArea;

            Matrix matrix = Matrix.Identity;
            if (locationOld == Location.RightTop || locationOld == Location.RightBottom) matrix.ScaleAt(-1, 1, area.Width / 2, area.Height / 2);//水平翻转
            if (locationOld == Location.LeftBottom || locationOld == Location.RightBottom) matrix.ScaleAt(1, -1, area.Width / 2, area.Height / 2);//垂直翻转
            if (Math.Abs(locationNew - locationOld) == 2) { matrix.Rotate(90); matrix.Scale(-1, 1); }//横纵转换

            Rect item = CaleLocation(EnumWindows(), area, Window.RestoreBounds, matrix);

            if (IsMouseCaptured) Mouse.Capture(null);
            Window.Left = item.Left;
            Window.Top = item.Top;
            SelectedItem = null;
        }

        List<Rect> EnumWindows()
        {
            var handleWindow = new WindowInteropHelper(Window).Handle;
            var list = new List<Rect>();
            User32_Gdi.EnumWindows((hWnd, _) =>
            {
                if (User32_Gdi.IsWindowVisible(hWnd) && hWnd != handleWindow && User32_Gdi.GetWindowRect(hWnd, out var lpRect))
                {
                    var sb = new StringBuilder(256);
                    User32_Gdi.GetClassName(hWnd, sb, 255);
                    if (!IgnoreWindow.Contains(sb.ToString())) list.Add(new Rect(lpRect.X, lpRect.Y, lpRect.Width, lpRect.Height));
                }
                return true;
            }, IntPtr.Zero);
            return list;
        }

        static Rect CaleLocation(IEnumerable<Rect> listWindow, Rect area, Rect window, Matrix matrix)
        {
            listWindow = listWindow.Select(f => Rect.Intersect(f, area)).Where(f => f.Width > 0 && f.Height > 0 && f != area);
            listWindow = listWindow.Select(f => Rect.Transform(f, matrix)).ToList();
            area.Transform(matrix);
            var move = Rect.Transform(window, matrix);
            matrix.Invert();
            move.Location = new Point(0, 0);
            while (move.Bottom < area.Bottom)
            {
                double minY = area.Bottom;
                while (move.Right < area.Right)
                {
                    var first = listWindow.FirstOrDefault(f => f.IntersectsWith(move));
                    if (first == default(Rect)) return Rect.Transform(move, matrix);
                    move.X = first.Right + 1;
                    minY = Math.Min(minY, first.Bottom + 1);
                }
                move.X = 0;
                move.Y = minY;
            }
            return window;
        }
    }
}
