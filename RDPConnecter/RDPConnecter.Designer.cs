namespace RDPConnecter
{
    partial class RDPConnecter
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
            this.server = new System.Windows.Forms.Label();
            this.serverText = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.Label();
            this.usernameText = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.Label();
            this.passwordText = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.Label();
            this.portText = new System.Windows.Forms.TextBox();
            this.width = new System.Windows.Forms.Label();
            this.widthText = new System.Windows.Forms.TextBox();
            this.height = new System.Windows.Forms.Label();
            this.heightText = new System.Windows.Forms.TextBox();
            this.isFullScreen = new System.Windows.Forms.CheckBox();
            this.connect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // server
            // 
            this.server.AutoSize = true;
            this.server.Location = new System.Drawing.Point(35, 21);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(41, 12);
            this.server.TabIndex = 0;
            this.server.Text = "server";
            // 
            // serverText
            // 
            this.serverText.Location = new System.Drawing.Point(82, 19);
            this.serverText.Name = "serverText";
            this.serverText.Size = new System.Drawing.Size(155, 21);
            this.serverText.TabIndex = 1;
            // 
            // username
            // 
            this.username.AutoSize = true;
            this.username.Location = new System.Drawing.Point(80, 56);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(53, 12);
            this.username.TabIndex = 2;
            this.username.Text = "username";
            // 
            // usernameText
            // 
            this.usernameText.Location = new System.Drawing.Point(148, 53);
            this.usernameText.Name = "usernameText";
            this.usernameText.Size = new System.Drawing.Size(155, 21);
            this.usernameText.TabIndex = 3;
            // 
            // password
            // 
            this.password.AutoSize = true;
            this.password.Location = new System.Drawing.Point(80, 91);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(53, 12);
            this.password.TabIndex = 4;
            this.password.Text = "password";
            // 
            // passwordText
            // 
            this.passwordText.Location = new System.Drawing.Point(148, 88);
            this.passwordText.Name = "passwordText";
            this.passwordText.PasswordChar = '*';
            this.passwordText.Size = new System.Drawing.Size(155, 21);
            this.passwordText.TabIndex = 5;
            // 
            // port
            // 
            this.port.AutoSize = true;
            this.port.Location = new System.Drawing.Point(261, 21);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(29, 12);
            this.port.TabIndex = 6;
            this.port.Text = "port";
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(296, 18);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(72, 21);
            this.portText.TabIndex = 7;
            // 
            // width
            // 
            this.width.AutoSize = true;
            this.width.Location = new System.Drawing.Point(98, 125);
            this.width.Name = "width";
            this.width.Size = new System.Drawing.Size(35, 12);
            this.width.TabIndex = 8;
            this.width.Text = "width";
            // 
            // widthText
            // 
            this.widthText.Location = new System.Drawing.Point(148, 122);
            this.widthText.Name = "widthText";
            this.widthText.Size = new System.Drawing.Size(155, 21);
            this.widthText.TabIndex = 9;
            // 
            // height
            // 
            this.height.AutoSize = true;
            this.height.Location = new System.Drawing.Point(92, 160);
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(41, 12);
            this.height.TabIndex = 10;
            this.height.Text = "height";
            // 
            // heightText
            // 
            this.heightText.Location = new System.Drawing.Point(148, 157);
            this.heightText.Name = "heightText";
            this.heightText.Size = new System.Drawing.Size(155, 21);
            this.heightText.TabIndex = 11;
            // 
            // isFullScreen
            // 
            this.isFullScreen.AutoSize = true;
            this.isFullScreen.Location = new System.Drawing.Point(137, 196);
            this.isFullScreen.Name = "isFullScreen";
            this.isFullScreen.Size = new System.Drawing.Size(90, 16);
            this.isFullScreen.TabIndex = 13;
            this.isFullScreen.Text = "Full Screen";
            this.isFullScreen.UseVisualStyleBackColor = true;
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(246, 190);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(57, 27);
            this.connect.TabIndex = 14;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // RDPConnecter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 295);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.isFullScreen);
            this.Controls.Add(this.heightText);
            this.Controls.Add(this.height);
            this.Controls.Add(this.widthText);
            this.Controls.Add(this.width);
            this.Controls.Add(this.portText);
            this.Controls.Add(this.port);
            this.Controls.Add(this.passwordText);
            this.Controls.Add(this.password);
            this.Controls.Add(this.usernameText);
            this.Controls.Add(this.username);
            this.Controls.Add(this.serverText);
            this.Controls.Add(this.server);
            this.Name = "RDPConnecter";
            this.Text = "RDPConnecter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label server;
        private System.Windows.Forms.TextBox serverText;
        private System.Windows.Forms.Label username;
        private System.Windows.Forms.TextBox usernameText;
        private System.Windows.Forms.Label password;
        private System.Windows.Forms.TextBox passwordText;
        private System.Windows.Forms.Label port;
        private System.Windows.Forms.TextBox portText;
        private System.Windows.Forms.Label width;
        private System.Windows.Forms.TextBox widthText;
        private System.Windows.Forms.Label height;
        private System.Windows.Forms.TextBox heightText;
        private System.Windows.Forms.CheckBox isFullScreen;
        private System.Windows.Forms.Button connect;
    }
}