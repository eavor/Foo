using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
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

            //指定某些列开启表格页尾合计
            bindGridView.CalculationDto.AddRange(
                new List<RulesDto>()
                {
                    new RulesDto() { Col = "col_7", ItemType = DevExpress.Data.SummaryItemType.Sum,Desc="合计: {0}" },
                });

            //指定某些列开启组内合计
            bindGridView.rulesDtos.AddRange(new List<RulesDto>()
            {
                new RulesDto { ItemType = DevExpress.Data.SummaryItemType.Sum, Col = "col_7", Desc = "合计：{0}" },
            });

            string expression = "col_0='第一组数据'";

            //指定条件显示不同颜色
            bindGridView.colorCellRules.Add(
                new ColorCellRulesDto() { CellColor = Color.FromArgb(192, 255, 255), Col = "col_0", Expression = expression }
                );

            //是否开启左侧选择按钮
            bindGridView.Options.ShowCheckBoxSelectColumnHeader = true;

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
