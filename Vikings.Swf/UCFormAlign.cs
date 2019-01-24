using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Vikings.Swf
{
    public partial class UCFormAlign : UserControl
    {
        class CellLocation
        {
            internal readonly TableLayoutPanelCellPosition Cell;
            internal readonly Point Location;

            internal CellLocation(TableLayoutPanelCellPosition cell, Point location)
            {
                Cell = cell;
                Location = location;
            }
        }

        Form form;

        Form Form
        {
            get
            {
                var f = FindForm();
                if (form != f)
                {
                    if (form != null)
                    {
                        form.LocationChanged -= Form_LocationSizeChanged;
                        form.SizeChanged -= Form_LocationSizeChanged;
                    }
                    form = f;
                    if (form != null && !form.IsDisposed)
                    {
                        form.LocationChanged += Form_LocationSizeChanged;
                        form.SizeChanged += Form_LocationSizeChanged;
                    }
                }
                return form;
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Size), "3, 3")]
        public Size GridSize
        {
            get
            {
                return new Size(tlPanel.ColumnCount, tlPanel.RowCount);
            }
            set
            {
                tlPanel.SuspendLayout();
                tlPanel.ColumnCount = value.Width;
                tlPanel.RowCount = value.Height;

                while (tlPanel.ColumnStyles.Count < value.Width) tlPanel.ColumnStyles.Add(new ColumnStyle());
                while (tlPanel.RowStyles.Count < value.Height) tlPanel.RowStyles.Add(new RowStyle());

                while (tlPanel.Controls.Count < value.Width * value.Height)
                {
                    RadioButton rb = new RadioButton()
                    {
                        Name = "rb" + tlPanel.Controls.Count,
                        Appearance = Appearance.Button,
                        AutoSize = true,
                        Dock = DockStyle.Fill,
                        FlatStyle = FlatStyle.Flat,
                        Margin = new Padding(0),
                    };
                    rb.FlatAppearance.BorderSize = 0;
                    rb.FlatAppearance.CheckedBackColor = SystemColors.Highlight;
                    rb.CheckedChanged += Rb_CheckedChanged;
                    tlPanel.Controls.Add(rb);
                }
                Form_LocationSizeChanged(Form, EventArgs.Empty);
                tlPanel.ResumeLayout();
            }
        }

        public UCFormAlign()
        {
            InitializeComponent();
            GridSize = new Size(3, 3);
        }

        private void Rb_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.Checked)
            {
                var dict = CellLocations().ToDictionary(f => f.Cell, f => f.Location);
                Form.Location = dict[tlPanel.GetPositionFromControl(rb)];
            }
        }

        private IEnumerable<CellLocation> CellLocations()
        {
            var sizeForm = Form.Size;
            var rectScreen = Screen.FromControl(Form).WorkingArea;
            var width = (rectScreen.Width - sizeForm.Width).Split(tlPanel.ColumnCount - 1);
            var height = (rectScreen.Height - sizeForm.Height).Split(tlPanel.RowCount - 1);

            for (int x = 0; x < tlPanel.ColumnCount; x++)
            {
                for (int y = 0; y < tlPanel.RowCount; y++)
                {
                    Point location = new Point(width.Take(x).Sum(), height.Take(y).Sum());
                    location.Offset(rectScreen.Location);
                    yield return new CellLocation(new TableLayoutPanelCellPosition(x, y), location);
                }
            }
        }

        private void UCFormAlign_VisibleChanged(object sender, EventArgs e)
        {
            if (!DesignMode && Form != null && Visible) Form_LocationSizeChanged(Form, EventArgs.Empty);
        }

        private void Form_LocationSizeChanged(object sender, EventArgs e)
        {
            if (Form == null || Form != sender) return;
            var dict = CellLocations().ToDictionary(f => f.Location, f => f.Cell);
            var location = Form.Location;
            if (dict.ContainsKey(location))
            {
                var cell = dict[location];
                var c = tlPanel.GetControlFromPosition(cell.Column, cell.Row);
                (c as RadioButton).Checked = true;
            }
            else
                tlPanel.All().OfType<RadioButton>().ToList().ForEach(f => f.Checked = false);
        }

        private void TlPanel_Layout(object sender, LayoutEventArgs e)
        {
            var c = tlPanel.ColumnCount;
            if (ClientSize.Width > c)
            {
                var width = (ClientSize.Width - c - 1).Split(c).ToList();
                for (int i = 0; i < tlPanel.ColumnCount; i++)
                    tlPanel.ColumnStyles[i] = new ColumnStyle(SizeType.Absolute, width[i]);
            }
            c = tlPanel.RowCount;
            if (ClientSize.Height > c)
            {
                var height = (ClientSize.Height - c - 1).Split(c).ToList();
                for (int i = 0; i < tlPanel.RowCount; i++)
                    tlPanel.RowStyles[i] = new RowStyle(SizeType.Absolute, height[i]);
            }
        }
    }
}
