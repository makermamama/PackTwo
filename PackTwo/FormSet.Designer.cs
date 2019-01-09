namespace PackTwo
{
    partial class FormSet
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServiceAddress = new System.Windows.Forms.TextBox();
            this.txtRFID_IP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRFID_Port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAntenna = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.Font = new System.Drawing.Font("宋体", 17F);
            this.button1.Location = new System.Drawing.Point(112, 238);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.Font = new System.Drawing.Font("宋体", 17F);
            this.button2.Location = new System.Drawing.Point(263, 238);
            this.button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 35);
            this.button2.TabIndex = 1;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(19, 87);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "服务器地址:";
            // 
            // txtServiceAddress
            // 
            this.txtServiceAddress.Font = new System.Drawing.Font("宋体", 12F);
            this.txtServiceAddress.Location = new System.Drawing.Point(106, 85);
            this.txtServiceAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtServiceAddress.Name = "txtServiceAddress";
            this.txtServiceAddress.Size = new System.Drawing.Size(546, 26);
            this.txtServiceAddress.TabIndex = 3;
            // 
            // txtRFID_IP
            // 
            this.txtRFID_IP.Font = new System.Drawing.Font("宋体", 12F);
            this.txtRFID_IP.Location = new System.Drawing.Point(106, 156);
            this.txtRFID_IP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtRFID_IP.Name = "txtRFID_IP";
            this.txtRFID_IP.Size = new System.Drawing.Size(146, 26);
            this.txtRFID_IP.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(21, 158);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "采集器IP:";
            // 
            // txtRFID_Port
            // 
            this.txtRFID_Port.Font = new System.Drawing.Font("宋体", 12F);
            this.txtRFID_Port.Location = new System.Drawing.Point(349, 156);
            this.txtRFID_Port.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtRFID_Port.Name = "txtRFID_Port";
            this.txtRFID_Port.Size = new System.Drawing.Size(72, 26);
            this.txtRFID_Port.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(256, 158);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "采集器端口:";
            // 
            // txtAntenna
            // 
            this.txtAntenna.Font = new System.Drawing.Font("宋体", 12F);
            this.txtAntenna.Location = new System.Drawing.Point(537, 155);
            this.txtAntenna.Margin = new System.Windows.Forms.Padding(2);
            this.txtAntenna.Name = "txtAntenna";
            this.txtAntenna.Size = new System.Drawing.Size(115, 26);
            this.txtAntenna.TabIndex = 9;
            this.txtAntenna.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAntenna_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(425, 159);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "读取功率设置:";
            // 
            // FormSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 322);
            this.Controls.Add(this.txtAntenna);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRFID_Port);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtRFID_IP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServiceAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FormSet";
            this.Text = "FormSet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServiceAddress;
        private System.Windows.Forms.TextBox txtRFID_IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRFID_Port;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAntenna;
        private System.Windows.Forms.Label label4;
    }
}