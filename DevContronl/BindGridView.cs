using DevExpress.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foo
{
    public class RulesDto
    {
        /// <summary>
        /// 类型
        /// </summary>
        public SummaryItemType ItemType { get; set; }

        /// <summary>
        /// 列字段
        /// </summary>
        public string Col { get; set; }

        /// <summary>
        /// 描述内容【若是为表格页尾统计时可空】
        /// </summary>
        public string Desc { get; set; }
    }

    public class ColorCellRulesDto
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Col { get; set; }

        /// <summary>
        /// 条件：如colName='xxxx'
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// 单元格显示颜色
        /// </summary>
        public Color CellColor { get; set; }
    }

    public class BindGridView
    {
        /// <summary>
        /// 组内分组合计
        /// </summary>
        public List<RulesDto> rulesDtos = new List<RulesDto>();

        /// <summary>
        /// 右键菜单
        /// </summary>
        public System.Windows.Forms.ContextMenuStrip contextMenuStrip;

        /// <summary>
        /// 行双击事件
        /// </summary>
        public EventHandler DoubleClickHandler { get; set; }

        /// <summary>
        /// 表格页尾统计类型
        /// </summary>
        public List<RulesDto> CalculationDto { get; set; } = new List<RulesDto>();

        /// <summary>
        /// 数据源
        /// </summary>
        public DataTable DataSource;

        /// <summary>
        /// 满足指定条件时单元格显示指定颜色
        /// </summary>
        public List<ColorCellRulesDto> colorCellRules = new List<ColorCellRulesDto>();

        /// <summary>
        /// GridView配置信息
        /// </summary>
        public OptionsView Options = new OptionsView();

        /// <summary>
        /// 列配置信息
        /// </summary>
        public List<ColumnConfig> ColumnsConfig = new List<ColumnConfig>();

    }
    /// <summary>
    /// GridView配置信息
    /// </summary>
    public class OptionsView
    {
        /// <summary>
        /// 可否编辑
        /// </summary>
        public bool AbleEdit { get; set; }

        /// <summary>
        /// 偶行颜色
        /// </summary>
        public Color EvenRowBackColor { get; set; } = Color.FromArgb(255, 255, 255);

        /// <summary>
        /// 奇行颜色
        /// </summary>
        public Color OddRowBackColor { get; set; } = Color.FromArgb(255, 255, 255);


        /// <summary>
        /// 焦点行颜色
        /// </summary>
        public Color FocusedRowBackColor { get; set; } = Color.FromArgb(0, 153, 255);

        /// <summary>
        /// 是否开启左侧选择按钮
        /// </summary>
        public bool ShowCheckBoxSelectColumnHeader { get; set; }

        /// <summary>
        /// 是否显示页脚
        /// </summary>
        public bool ShowGridViewFooter { get; set; } = false;

        /// <summary>
        /// 是否行展示按照单个字段分颜色
        /// </summary>
        public bool IsAbleRowShow { get; set; }

        /// <summary>
        /// 是否列宽度自扩展适应表格
        /// </summary>
        public bool ColumnWindthAuto { get; set; } = true;

    }

    public class OrderDetails
    {
        [GridColumnSet(Caption = "主键", FieldName = "Id")]
        public string Id { get; set; }
    }
    /// <summary>
    /// GridViewBind说明特性
    /// </summary>
    public class GridColumnSetAttribute : Attribute
    {
        /// <summary>
        /// 列头名称
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 绑定数据字段
        /// </summary>
        public string FieldName { get; set; }

        public GridColumnSetAttribute() { }

        public GridColumnSetAttribute(string _caption, string _fieldName)
        {
            this.Caption = _caption;
            this.FieldName = _fieldName;
        }
    }
    /// <summary>
    /// 列配置,需要自定义配置
    /// </summary>
    public class ColumnConfig
    {
        /// <summary>
        /// 列字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 列宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visiable { get; set; } = true;
    }
}
