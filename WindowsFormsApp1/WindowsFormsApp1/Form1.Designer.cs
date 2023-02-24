namespace WindowsFormsApp1
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Rdo_MSAA = new System.Windows.Forms.RadioButton();
            this.Rdo_SSAA = new System.Windows.Forms.RadioButton();
            this.Rdo_NoneAA = new System.Windows.Forms.RadioButton();
            this.Chk_ShowNormal = new System.Windows.Forms.CheckBox();
            this.Lbl_Status = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.Chk_ShowShadowMap = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(0, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1440, 720);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // timer1
            // 
            this.timer1.Interval = 2;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1446, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "加载线框模式";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1446, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "Phong光照模型";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1446, 128);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(106, 32);
            this.button3.TabIndex = 3;
            this.button3.Text = "PBR光照模型";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.Rdo_MSAA);
            this.GroupBox1.Controls.Add(this.Rdo_SSAA);
            this.GroupBox1.Controls.Add(this.Rdo_NoneAA);
            this.GroupBox1.Location = new System.Drawing.Point(1446, 218);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(106, 100);
            this.GroupBox1.TabIndex = 5;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "抗锯齿";
            // 
            // Rdo_MSAA
            // 
            this.Rdo_MSAA.AutoSize = true;
            this.Rdo_MSAA.Location = new System.Drawing.Point(11, 64);
            this.Rdo_MSAA.Name = "Rdo_MSAA";
            this.Rdo_MSAA.Size = new System.Drawing.Size(47, 16);
            this.Rdo_MSAA.TabIndex = 9;
            this.Rdo_MSAA.TabStop = true;
            this.Rdo_MSAA.Text = "MSAA";
            this.Rdo_MSAA.UseVisualStyleBackColor = true;
            this.Rdo_MSAA.CheckedChanged += new System.EventHandler(this.Rdo_MSAA_CheckedChanged);
            // 
            // Rdo_SSAA
            // 
            this.Rdo_SSAA.AutoSize = true;
            this.Rdo_SSAA.Location = new System.Drawing.Point(11, 42);
            this.Rdo_SSAA.Name = "Rdo_SSAA";
            this.Rdo_SSAA.Size = new System.Drawing.Size(47, 16);
            this.Rdo_SSAA.TabIndex = 8;
            this.Rdo_SSAA.TabStop = true;
            this.Rdo_SSAA.Text = "SSAA";
            this.Rdo_SSAA.UseVisualStyleBackColor = true;
            this.Rdo_SSAA.CheckedChanged += new System.EventHandler(this.Rdo_SSAA_CheckedChanged);
            // 
            // Rdo_NoneAA
            // 
            this.Rdo_NoneAA.AutoSize = true;
            this.Rdo_NoneAA.Location = new System.Drawing.Point(11, 20);
            this.Rdo_NoneAA.Name = "Rdo_NoneAA";
            this.Rdo_NoneAA.Size = new System.Drawing.Size(47, 16);
            this.Rdo_NoneAA.TabIndex = 7;
            this.Rdo_NoneAA.TabStop = true;
            this.Rdo_NoneAA.Text = "关闭";
            this.Rdo_NoneAA.UseVisualStyleBackColor = true;
            this.Rdo_NoneAA.CheckedChanged += new System.EventHandler(this.Rdo_NoneAA_CheckedChanged);
            // 
            // Chk_ShowNormal
            // 
            this.Chk_ShowNormal.AutoSize = true;
            this.Chk_ShowNormal.Location = new System.Drawing.Point(1446, 325);
            this.Chk_ShowNormal.Name = "Chk_ShowNormal";
            this.Chk_ShowNormal.Size = new System.Drawing.Size(72, 16);
            this.Chk_ShowNormal.TabIndex = 6;
            this.Chk_ShowNormal.Text = "显示法线";
            this.Chk_ShowNormal.UseVisualStyleBackColor = true;
            this.Chk_ShowNormal.CheckedChanged += new System.EventHandler(this.Chk_ShowNormal_CheckedChanged);
            // 
            // Lbl_Status
            // 
            this.Lbl_Status.AutoSize = true;
            this.Lbl_Status.Location = new System.Drawing.Point(1461, 368);
            this.Lbl_Status.Name = "Lbl_Status";
            this.Lbl_Status.Size = new System.Drawing.Size(0, 12);
            this.Lbl_Status.TabIndex = 7;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1446, 172);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(106, 32);
            this.button4.TabIndex = 8;
            this.button4.Text = "阴影场景";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Chk_ShowShadowMap
            // 
            this.Chk_ShowShadowMap.AutoSize = true;
            this.Chk_ShowShadowMap.Location = new System.Drawing.Point(1446, 347);
            this.Chk_ShowShadowMap.Name = "Chk_ShowShadowMap";
            this.Chk_ShowShadowMap.Size = new System.Drawing.Size(192, 16);
            this.Chk_ShowShadowMap.TabIndex = 9;
            this.Chk_ShowShadowMap.Text = "显示阴影图（仅阴影场景有效）";
            this.Chk_ShowShadowMap.UseVisualStyleBackColor = true;
            this.Chk_ShowShadowMap.CheckedChanged += new System.EventHandler(this.Chk_ShowShadowMap_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1630, 723);
            this.Controls.Add(this.Chk_ShowShadowMap);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.Lbl_Status);
            this.Controls.Add(this.Chk_ShowNormal);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.RadioButton Rdo_NoneAA;
        private System.Windows.Forms.RadioButton Rdo_MSAA;
        private System.Windows.Forms.RadioButton Rdo_SSAA;
        private System.Windows.Forms.CheckBox Chk_ShowNormal;
        private System.Windows.Forms.Label Lbl_Status;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox Chk_ShowShadowMap;
    }
}

