
namespace Foo
{
    partial class Foo
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
            this.userDevGridView1 = new UserDevGridView();
            this.SuspendLayout();
            // 
            // userDevGridView1
            // 
            this.userDevGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userDevGridView1.Location = new System.Drawing.Point(0, 0);
            this.userDevGridView1.Name = "userDevGridView1";
            this.userDevGridView1.Size = new System.Drawing.Size(800, 450);
            this.userDevGridView1.TabIndex = 0;
            // 
            // Foo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.userDevGridView1);
            this.Name = "Foo";
            this.Text = "Foo";
            this.Load += new System.EventHandler(this.Foo_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private UserDevGridView userDevGridView1;
    }
}

