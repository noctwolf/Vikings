using System.Windows.Forms;

namespace Vikings.Swf
{
    partial class UCFormAlign
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tlPanel = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tlPanel
            // 
            this.tlPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlPanel.ColumnCount = 1;
            this.tlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlPanel.Location = new System.Drawing.Point(0, 0);
            this.tlPanel.Name = "tlPanel";
            this.tlPanel.RowCount = 1;
            this.tlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlPanel.Size = new System.Drawing.Size(23, 23);
            this.tlPanel.TabIndex = 0;
            this.tlPanel.Layout += new System.Windows.Forms.LayoutEventHandler(this.TlPanel_Layout);
            // 
            // UCFormAlign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlPanel);
            this.Name = "UCFormAlign";
            this.Size = new System.Drawing.Size(23, 23);
            this.VisibleChanged += new System.EventHandler(this.UCFormAlign_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tlPanel;
    }
}
