namespace SneezeBoardClient
{
    partial class SneezeBoardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SneezeBoardForm));
            this.btn_add_sneeze = new System.Windows.Forms.Button();
            this.btn_change_color = new System.Windows.Forms.Button();
            this.lbl_sneeze_display = new System.Windows.Forms.Label();
            this.lbl_display_color = new System.Windows.Forms.Label();
            this.lbl_final_sneeze = new System.Windows.Forms.Label();
            this.lbl_apocalypse = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_sneezers = new System.Windows.Forms.ComboBox();
            this.txtbx_ip = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_connect = new System.Windows.Forms.Button();
            this.pnl_scroller = new System.Windows.Forms.Panel();
            this.scroll_horizontal = new System.Windows.Forms.HScrollBar();
            this.txtbx_commentary = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnu_Restore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnu_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btn_settings = new System.Windows.Forms.Button();
            this.lbl_we_win = new System.Windows.Forms.Label();
            this.tmr_updateApocalypse = new System.Windows.Forms.Timer(this.components);
            this.btn_stats = new System.Windows.Forms.Button();
            this.pnl_scroller.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_add_sneeze
            // 
            this.btn_add_sneeze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_add_sneeze.Font = new System.Drawing.Font("Microsoft Sans Serif", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_add_sneeze.Location = new System.Drawing.Point(12, 212);
            this.btn_add_sneeze.Name = "btn_add_sneeze";
            this.btn_add_sneeze.Size = new System.Drawing.Size(276, 104);
            this.btn_add_sneeze.TabIndex = 0;
            this.btn_add_sneeze.Text = "I Sneezed";
            this.btn_add_sneeze.UseVisualStyleBackColor = false;
            this.btn_add_sneeze.Click += new System.EventHandler(this.btn_add_sneeze_Click);
            // 
            // btn_change_color
            // 
            this.btn_change_color.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_change_color.Location = new System.Drawing.Point(418, 357);
            this.btn_change_color.Name = "btn_change_color";
            this.btn_change_color.Size = new System.Drawing.Size(94, 37);
            this.btn_change_color.TabIndex = 1;
            this.btn_change_color.Text = "Change Color";
            this.btn_change_color.UseVisualStyleBackColor = true;
            this.btn_change_color.Click += new System.EventHandler(this.btn_change_color_Click);
            // 
            // lbl_sneeze_display
            // 
            this.lbl_sneeze_display.BackColor = System.Drawing.Color.White;
            this.lbl_sneeze_display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_sneeze_display.Font = new System.Drawing.Font("Ink Free", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_sneeze_display.Location = new System.Drawing.Point(0, 0);
            this.lbl_sneeze_display.Name = "lbl_sneeze_display";
            this.lbl_sneeze_display.Size = new System.Drawing.Size(503, 159);
            this.lbl_sneeze_display.TabIndex = 2;
            this.lbl_sneeze_display.Paint += new System.Windows.Forms.PaintEventHandler(this.lbl_sneeze_display_Paint);
            this.lbl_sneeze_display.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbl_sneeze_display_MouseClick);
            this.lbl_sneeze_display.MouseLeave += new System.EventHandler(this.lbl_sneeze_display_MouseLeave);
            this.lbl_sneeze_display.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbl_sneeze_display_MouseMove);
            // 
            // lbl_display_color
            // 
            this.lbl_display_color.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_display_color.BackColor = System.Drawing.Color.DarkGray;
            this.lbl_display_color.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_display_color.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lbl_display_color.Location = new System.Drawing.Point(418, 325);
            this.lbl_display_color.Name = "lbl_display_color";
            this.lbl_display_color.Size = new System.Drawing.Size(94, 23);
            this.lbl_display_color.TabIndex = 3;
            // 
            // lbl_final_sneeze
            // 
            this.lbl_final_sneeze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_final_sneeze.AutoSize = true;
            this.lbl_final_sneeze.Location = new System.Drawing.Point(9, 374);
            this.lbl_final_sneeze.Name = "lbl_final_sneeze";
            this.lbl_final_sneeze.Size = new System.Drawing.Size(146, 13);
            this.lbl_final_sneeze.TabIndex = 4;
            this.lbl_final_sneeze.Text = "Estimated Final Sneeze Date:";
            // 
            // lbl_apocalypse
            // 
            this.lbl_apocalypse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbl_apocalypse.AutoSize = true;
            this.lbl_apocalypse.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_apocalypse.Location = new System.Drawing.Point(161, 368);
            this.lbl_apocalypse.Name = "lbl_apocalypse";
            this.lbl_apocalypse.Size = new System.Drawing.Size(26, 29);
            this.lbl_apocalypse.TabIndex = 5;
            this.lbl_apocalypse.Text = "#";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(344, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Sneezer:";
            // 
            // cmb_sneezers
            // 
            this.cmb_sneezers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_sneezers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_sneezers.FormattingEnabled = true;
            this.cmb_sneezers.Location = new System.Drawing.Point(347, 282);
            this.cmb_sneezers.Name = "cmb_sneezers";
            this.cmb_sneezers.Size = new System.Drawing.Size(165, 21);
            this.cmb_sneezers.TabIndex = 7;
            this.cmb_sneezers.SelectedIndexChanged += new System.EventHandler(this.cmb_sneezers_SelectedIndexChanged);
            // 
            // txtbx_ip
            // 
            this.txtbx_ip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtbx_ip.Location = new System.Drawing.Point(321, 233);
            this.txtbx_ip.Name = "txtbx_ip";
            this.txtbx_ip.Size = new System.Drawing.Size(110, 20);
            this.txtbx_ip.TabIndex = 8;
            this.txtbx_ip.TextChanged += new System.EventHandler(this.txtbx_ip_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(318, 217);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Server IP:";
            // 
            // btn_connect
            // 
            this.btn_connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_connect.Location = new System.Drawing.Point(437, 231);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 10;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // pnl_scroller
            // 
            this.pnl_scroller.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl_scroller.AutoScroll = true;
            this.pnl_scroller.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnl_scroller.Controls.Add(this.lbl_sneeze_display);
            this.pnl_scroller.Controls.Add(this.scroll_horizontal);
            this.pnl_scroller.Location = new System.Drawing.Point(7, 12);
            this.pnl_scroller.Name = "pnl_scroller";
            this.pnl_scroller.Size = new System.Drawing.Size(505, 178);
            this.pnl_scroller.TabIndex = 11;
            // 
            // scroll_horizontal
            // 
            this.scroll_horizontal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scroll_horizontal.LargeChange = 100;
            this.scroll_horizontal.Location = new System.Drawing.Point(0, 159);
            this.scroll_horizontal.Name = "scroll_horizontal";
            this.scroll_horizontal.Size = new System.Drawing.Size(503, 17);
            this.scroll_horizontal.SmallChange = 10;
            this.scroll_horizontal.TabIndex = 3;
            this.scroll_horizontal.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scroll_horizontal_Scroll);
            // 
            // txtbx_commentary
            // 
            this.txtbx_commentary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtbx_commentary.Location = new System.Drawing.Point(125, 325);
            this.txtbx_commentary.Multiline = true;
            this.txtbx_commentary.Name = "txtbx_commentary";
            this.txtbx_commentary.Size = new System.Drawing.Size(163, 34);
            this.txtbx_commentary.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 328);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Sneeze Commentary:";
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Sneeze Countdown";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnu_Restore,
            this.toolStripSeparator1,
            this.mnu_Exit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(119, 54);
            // 
            // mnu_Restore
            // 
            this.mnu_Restore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnu_Restore.Name = "mnu_Restore";
            this.mnu_Restore.Size = new System.Drawing.Size(118, 22);
            this.mnu_Restore.Text = "Restore";
            this.mnu_Restore.Click += new System.EventHandler(this.mnu_Restore_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(115, 6);
            // 
            // mnu_Exit
            // 
            this.mnu_Exit.Name = "mnu_Exit";
            this.mnu_Exit.Size = new System.Drawing.Size(118, 22);
            this.mnu_Exit.Text = "Exit";
            this.mnu_Exit.Click += new System.EventHandler(this.mnu_Exit_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 15000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // btn_settings
            // 
            this.btn_settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_settings.Image = ((System.Drawing.Image)(resources.GetObject("btn_settings.Image")));
            this.btn_settings.Location = new System.Drawing.Point(486, 198);
            this.btn_settings.Name = "btn_settings";
            this.btn_settings.Size = new System.Drawing.Size(25, 25);
            this.btn_settings.TabIndex = 15;
            this.toolTip.SetToolTip(this.btn_settings, "Settings");
            this.btn_settings.UseVisualStyleBackColor = true;
            this.btn_settings.Click += new System.EventHandler(this.btn_settings_Click);
            // 
            // lbl_we_win
            // 
            this.lbl_we_win.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_we_win.Font = new System.Drawing.Font("Arial Black", 50.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_we_win.ForeColor = System.Drawing.Color.Red;
            this.lbl_we_win.Location = new System.Drawing.Point(0, 193);
            this.lbl_we_win.Name = "lbl_we_win";
            this.lbl_we_win.Size = new System.Drawing.Size(523, 215);
            this.lbl_we_win.TabIndex = 14;
            this.lbl_we_win.Text = "Come, Lord Jesus come!";
            this.lbl_we_win.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tmr_updateApocalypse
            // 
            this.tmr_updateApocalypse.Enabled = true;
            this.tmr_updateApocalypse.Interval = 5000;
            this.tmr_updateApocalypse.Tick += new System.EventHandler(this.tmr_updateApocalypse_Tick);
            // 
            // btn_stats
            // 
            this.btn_stats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_stats.Image = ((System.Drawing.Image)(resources.GetObject("btn_stats.Image")));
            this.btn_stats.Location = new System.Drawing.Point(449, 198);
            this.btn_stats.Name = "btn_stats";
            this.btn_stats.Size = new System.Drawing.Size(31, 25);
            this.btn_stats.TabIndex = 16;
            this.btn_stats.UseVisualStyleBackColor = true;
            this.btn_stats.Click += new System.EventHandler(this.btn_stats_Click);
            // 
            // SneezeBoardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 406);
            this.Controls.Add(this.lbl_we_win);
            this.Controls.Add(this.btn_stats);
            this.Controls.Add(this.btn_settings);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbx_commentary);
            this.Controls.Add(this.pnl_scroller);
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtbx_ip);
            this.Controls.Add(this.cmb_sneezers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_apocalypse);
            this.Controls.Add(this.lbl_final_sneeze);
            this.Controls.Add(this.lbl_display_color);
            this.Controls.Add(this.btn_change_color);
            this.Controls.Add(this.btn_add_sneeze);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(540, 445);
            this.Name = "SneezeBoardForm";
            this.Text = "Sneeze Countdown";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SneezeBoardForm_FormClosed);
            this.SizeChanged += new System.EventHandler(this.SneezeBoardForm_SizeChanged);
            this.Resize += new System.EventHandler(this.SneezeBoardForm_Resize);
            this.pnl_scroller.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_add_sneeze;
        private System.Windows.Forms.Button btn_change_color;
        private System.Windows.Forms.Label lbl_sneeze_display;
        private System.Windows.Forms.Label lbl_display_color;
        private System.Windows.Forms.Label lbl_final_sneeze;
        private System.Windows.Forms.Label lbl_apocalypse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_sneezers;
        private System.Windows.Forms.TextBox txtbx_ip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Panel pnl_scroller;
        private System.Windows.Forms.TextBox txtbx_commentary;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnu_Restore;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnu_Exit;
        private System.Windows.Forms.HScrollBar scroll_horizontal;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lbl_we_win;
        private System.Windows.Forms.Button btn_settings;
        private System.Windows.Forms.Timer tmr_updateApocalypse;
        private System.Windows.Forms.Button btn_stats;
    }
}

