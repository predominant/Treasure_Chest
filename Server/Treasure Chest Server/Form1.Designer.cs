namespace Treasure_Chest_Server
{
	partial class form_Main
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
			this.status = new System.Windows.Forms.StatusStrip();
			this.rtb_Log = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// status
			// 
			this.status.Location = new System.Drawing.Point(0, 219);
			this.status.Name = "status";
			this.status.Size = new System.Drawing.Size(451, 22);
			this.status.TabIndex = 0;
			this.status.Text = "status";
			// 
			// rtb_Log
			// 
			this.rtb_Log.Location = new System.Drawing.Point(170, 0);
			this.rtb_Log.Name = "rtb_Log";
			this.rtb_Log.ReadOnly = true;
			this.rtb_Log.Size = new System.Drawing.Size(281, 216);
			this.rtb_Log.TabIndex = 1;
			this.rtb_Log.Text = "";
			// 
			// form_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(451, 241);
			this.Controls.Add(this.rtb_Log);
			this.Controls.Add(this.status);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "form_Main";
			this.Text = "TC Server";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip status;
		private System.Windows.Forms.RichTextBox rtb_Log;

	}
}

