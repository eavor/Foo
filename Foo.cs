using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Foo
{
    public partial class Foo : Form
    {
        public Foo()
        {
            InitializeComponent();
        }

        private void Foo_Load(object sender, EventArgs e)
        {
            BindGridView bindGridView = new BindGridView();
            
            //绑定数据源
            bindGridView.DataSource = GetDataTableSource();

            //数据渲染到userDevGridView1控件
            userDevGridView1.SetInit(bindGridView);
        }

        private DataTable GetDataTableSource()
        {
            DataTable dt = new DataTable();
            //初始化两组数据
            for (int i = 0; i < 8; i++)
            {
                dt.Columns.Add(new DataColumn() { ColumnName = $"col_{i}", Caption = $"列_{i}" });
            }

            for (int i = 0; i < 10; i++)
            {
                dt.Rows.Add("第一组数据", $"ContentOne_{i + 1}", $"ContentOne_{i + 2}", $"ContentOne_{i + 3}", 
                    $"ContentOne_{i + 4}", $"ContentOne_{i + 5}", $"ContentOne_{i + 6}", $"7");
            }

            for (int i = 0; i < 10; i++)
            {
                dt.Rows.Add("第二组数据", $"ContentTwo_{i + 1}", $"ContentTwo_{i + 2}", $"ContentTwo_{i + 3}", 
                    $"ContentTwo_{i + 4}", $"ContentTwo_{i + 5}", $"ContentTwo_{i + 6}", $"7");
            }

            return dt;
        }
    }
}
