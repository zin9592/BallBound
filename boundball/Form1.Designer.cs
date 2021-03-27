namespace boundball
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.game_panel = new boundball.DoubleBufPanel();
            this.load = new System.Windows.Forms.Label();
            this.connect_panel = new boundball.DoubleBufPanel();
            this.ConnectText = new System.Windows.Forms.Label();
            this.title_panel = new boundball.DoubleBufPanel();
            this.game_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // game_panel
            // 
            this.game_panel.BackColor = System.Drawing.SystemColors.Info;
            this.game_panel.Controls.Add(this.load);
            this.game_panel.Controls.Add(this.connect_panel);
            this.game_panel.Controls.Add(this.ConnectText);
            this.game_panel.Controls.Add(this.title_panel);
            this.game_panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.game_panel.Location = new System.Drawing.Point(0, 0);
            this.game_panel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.game_panel.Name = "game_panel";
            this.game_panel.Size = new System.Drawing.Size(1124, 682);
            this.game_panel.TabIndex = 0;
            this.game_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint_1);
            // 
            // load
            // 
            this.load.AutoSize = true;
            this.load.Font = new System.Drawing.Font("HY견고딕", 20.25F, System.Drawing.FontStyle.Bold);
            this.load.Location = new System.Drawing.Point(476, 340);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(32, 27);
            this.load.TabIndex = 12;
            this.load.Text = "  ";
            this.load.Visible = false;
            // 
            // connect_panel
            // 
            this.connect_panel.Location = new System.Drawing.Point(0, 372);
            this.connect_panel.Name = "connect_panel";
            this.connect_panel.Size = new System.Drawing.Size(612, 318);
            this.connect_panel.TabIndex = 11;
            this.connect_panel.Visible = false;
            this.connect_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.ConnectPanelPaint);
            // 
            // ConnectText
            // 
            this.ConnectText.AutoSize = true;
            this.ConnectText.Font = new System.Drawing.Font("HY견고딕", 21.75F, System.Drawing.FontStyle.Bold);
            this.ConnectText.Location = new System.Drawing.Point(3, 340);
            this.ConnectText.Name = "ConnectText";
            this.ConnectText.Size = new System.Drawing.Size(221, 29);
            this.ConnectText.TabIndex = 10;
            this.ConnectText.Text = "ConnectText";
            this.ConnectText.Visible = false;
            // 
            // title_panel
            // 
            this.title_panel.Location = new System.Drawing.Point(392, 99);
            this.title_panel.Name = "title_panel";
            this.title_panel.Size = new System.Drawing.Size(200, 100);
            this.title_panel.TabIndex = 9;
            this.title_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.title_panel_Paint);
            this.title_panel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.title_panel_MouseClick);
            this.title_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.title_panel_MouseMove);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1124, 682);
            this.Controls.Add(this.game_panel);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.game_panel.ResumeLayout(false);
            this.game_panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufPanel game_panel;
        private DoubleBufPanel title_panel;
        private DoubleBufPanel connect_panel;
        private System.Windows.Forms.Label ConnectText;
        private System.Windows.Forms.Label load;
    }
}

