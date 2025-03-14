using DevExpress.Utils.Extensions;
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

            //绑定双击事件
            bindGridView.DoubleClickHandler += (_s, _e) =>
            {
                //获取选中数据
                var data = userDevGridView1.GetSelectedRows();

                var selected = userDevGridView1.GetSelectedCell();

                MessageBox.Show($"双击事件,点击了列{selected.Keys.FirstOrDefault()}点击值为{selected[selected.Keys.FirstOrDefault()]}");
            };

            //绑定右键菜单
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            // 添加菜单项
            contextMenuStrip.Items.Add("选项1");
            contextMenuStrip.Items.Add("选项2");
            contextMenuStrip.Items.Add("选项3");

            bindGridView.contextMenuStrip = contextMenuStrip;

            //设置col_6列不可见
            bindGridView.ColumnsConfig.Add(new ColumnConfig() { Field = "col_6", Visiable = false});

            //设置col_7列宽度为200
            bindGridView.ColumnsConfig.Add(new ColumnConfig() { Field = "col_7", Visiable = true, Width = 200 });

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
