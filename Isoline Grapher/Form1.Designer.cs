using System.Collections.Generic;

namespace Isoline_Grapher
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.drawButton = new System.Windows.Forms.Button();
            this.dataDrawBox = new System.Windows.Forms.PictureBox();
            this.lineDrawBox = new System.Windows.Forms.PictureBox();
            this.generateDataButton = new System.Windows.Forms.Button();
            this.fillSampleButton = new System.Windows.Forms.Button();
            this.triangulateButton = new System.Windows.Forms.Button();
            this.approxGridButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataDrawBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineDrawBox)).BeginInit();
            this.SuspendLayout();
            // 
            // drawButton
            // 
            this.drawButton.Location = new System.Drawing.Point(22, 146);
            this.drawButton.Name = "drawButton";
            this.drawButton.Size = new System.Drawing.Size(100, 23);
            this.drawButton.TabIndex = 0;
            this.drawButton.Text = "Draw isolines";
            this.drawButton.UseVisualStyleBackColor = true;
            this.drawButton.Click += new System.EventHandler(this.drawButton_Click);
            // 
            // dataDrawBox
            // 
            this.dataDrawBox.Location = new System.Drawing.Point(146, 7);
            this.dataDrawBox.Name = "dataDrawBox";
            this.dataDrawBox.Size = new System.Drawing.Size(310, 310);
            this.dataDrawBox.TabIndex = 3;
            this.dataDrawBox.TabStop = false;
            // 
            // lineDrawBox
            // 
            this.lineDrawBox.Location = new System.Drawing.Point(476, 7);
            this.lineDrawBox.Name = "lineDrawBox";
            this.lineDrawBox.Size = new System.Drawing.Size(310, 310);
            this.lineDrawBox.TabIndex = 2;
            this.lineDrawBox.TabStop = false;
            // 
            // generateDataButton
            // 
            this.generateDataButton.Location = new System.Drawing.Point(22, 41);
            this.generateDataButton.Name = "generateDataButton";
            this.generateDataButton.Size = new System.Drawing.Size(100, 23);
            this.generateDataButton.TabIndex = 4;
            this.generateDataButton.Text = "Generate Data";
            this.generateDataButton.UseVisualStyleBackColor = true;
            this.generateDataButton.Click += new System.EventHandler(this.generateDataButton_Click);
            // 
            // fillSampleButton
            // 
            this.fillSampleButton.Location = new System.Drawing.Point(22, 12);
            this.fillSampleButton.Name = "fillSampleButton";
            this.fillSampleButton.Size = new System.Drawing.Size(100, 23);
            this.fillSampleButton.TabIndex = 5;
            this.fillSampleButton.Text = "Fill Sample Data";
            this.fillSampleButton.UseVisualStyleBackColor = true;
            this.fillSampleButton.Click += new System.EventHandler(this.FillSampleButton_Click);
            // 
            // triangulateButton
            // 
            this.triangulateButton.Location = new System.Drawing.Point(22, 70);
            this.triangulateButton.Name = "triangulateButton";
            this.triangulateButton.Size = new System.Drawing.Size(100, 23);
            this.triangulateButton.TabIndex = 6;
            this.triangulateButton.Text = "Trinagulate";
            this.triangulateButton.UseVisualStyleBackColor = true;
            this.triangulateButton.Click += new System.EventHandler(this.triangulateButton_Click);
            // 
            // approxGridButton
            // 
            this.approxGridButton.Location = new System.Drawing.Point(22, 99);
            this.approxGridButton.Name = "approxGridButton";
            this.approxGridButton.Size = new System.Drawing.Size(100, 41);
            this.approxGridButton.TabIndex = 7;
            this.approxGridButton.Text = "Approximate a grid";
            this.approxGridButton.UseVisualStyleBackColor = true;
            this.approxGridButton.Click += new System.EventHandler(this.approxGridButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 328);
            this.Controls.Add(this.approxGridButton);
            this.Controls.Add(this.triangulateButton);
            this.Controls.Add(this.fillSampleButton);
            this.Controls.Add(this.generateDataButton);
            this.Controls.Add(this.dataDrawBox);
            this.Controls.Add(this.lineDrawBox);
            this.Controls.Add(this.drawButton);
            this.Name = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataDrawBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineDrawBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button drawButton;
        private System.Windows.Forms.PictureBox dataDrawBox;
        private System.Windows.Forms.PictureBox lineDrawBox;
        private System.Windows.Forms.Button generateDataButton;
        private System.Windows.Forms.Button fillSampleButton;
        private System.Windows.Forms.Button triangulateButton;
        private System.Windows.Forms.Button approxGridButton;
    }
}

