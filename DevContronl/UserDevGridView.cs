using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
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
    public partial class UserDevGridView : UserControl
    {
        public UserDevGridView()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
        }

        Dictionary<string, Button> keyControl = new Dictionary<string, Button>();

        private BindGridView _bindGridView;

        int _height = 87;

        int _px = 10;
        string _ColorRecordStr = "";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bindGridView"></param>
        public void SetInit(BindGridView bindGridView)
        {
            //某些页面已经开了多线程,在这种情况下,用dev原生封装的等待悬浮提示可能会造成阻塞,可以使用Task.run自定义的loading

            //SplashScreenManager.ShowDefaultWaitForm("请稍等", "加载数据中");

            _bindGridView = bindGridView;

            contextPopMenu = _bindGridView.contextMenuStrip;

            //样式
            GridViewConfigView(gridView1);
            GridViewConfigTurn(gridView2);

            Init();

            // 关闭等待界面
            //SplashScreenManager.CloseForm(false);
        }

        private void Init()
        {
            InitLoadGrid(_bindGridView);

            this.gridView1.PopupMenuShowing += gridViewDetail_PopupMenuShowing;

            gridView1.EndGrouping += EndGroupEvent;//分组结束事件

            gridView1.KeyDown += KeyDownViewEvent;//按键事件

            gridView1.RowClick += RowClickEvent;//点击行事件用于处理展开所有分组

            gridView2.EndGrouping += EndGroupViewEvent;//分组结束事件

            gridView2.StartGrouping += StartGroupViewEvent;//分组开始事件

            gridView1.CustomDrawGroupRow += CustomDrawGroupRowEvent;//自定义分组显示文本

            gridView2.ColumnPositionChanged += ColumnPositeionChangeedEvent;//排序变化事件

            gridView2.ColumnWidthChanged += ColumnWidthChangedEvent;//列宽度变化事件

            gridView2.FocusedRowChanged += RowFocusedChangedEvent;//行焦点变化事件

            gridView2.ColumnFilterChanged += ColumnFilterChangedEvent;//列头筛选条件变化事件

            gridView2.CellValueChanged += CellValueChangedEvent;//单元格值变化事件

            gridView2.SelectionChanged += SelectionChangedEvent;//多选框变化事件

            gridView2.CustomColumnDisplayText += CustomColumnDisplayText;

            AttachHScrollBarEvent();//同步滑动事件

            groupBox1.Paint += GroupBox_Paint;

            if (_bindGridView.contextMenuStrip != null)
                gdcFlts.ContextMenuStrip = _bindGridView.contextMenuStrip;

            if (_bindGridView.DoubleClickHandler != null)
                this.gridView1.DoubleClick += _bindGridView.DoubleClickHandler;

            //设置组内合计
            SetGroupSummary(_bindGridView.rulesDtos);

            //设置页脚统计
            SetFooterSummary(_bindGridView.CalculationDto);

            //设置条件指定单元格显示背景颜色
            SetCellStyleByColorCellRules(gridView1, _bindGridView.colorCellRules);

            //设置是否开启多选
            SetCheckBoxSelect();

            // 禁用分组功能
            gridView2.OptionsCustomization.AllowGroup = false;
            gridView2.OptionsView.ShowGroupPanel = false;

            //设置列头字体颜色
            for (int i = 0; i < this.gridView2.Columns.Count; i++)
            {
                this.gridView2.Columns[i].AppearanceHeader.ForeColor = Color.FromArgb(0, 0, 0);
            }

            // 启用分组页脚
            gridView1.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
            gridView1.OptionsView.ShowGroupedColumns = true;

            gridView1.OptionsView.GroupDrawMode = GroupDrawMode.Standard;
            //设置分组前距
            gridView1.LevelIndent = 5;

            //设置表格列是否根据当前表格宽度自适应
            gridView1.OptionsView.ColumnAutoWidth = _bindGridView.Options.ColumnWindthAuto;
            gridView2.OptionsView.ColumnAutoWidth = _bindGridView.Options.ColumnWindthAuto;
        }

        private void RowClickEvent(object sender, RowClickEventArgs e)
        {
            if (!_bindGridView.Options.ExpandAllChildGroupsAuto) return;

            GridView view = sender as GridView;

            // 检查当前点击的行是否是分组行
            if (view.IsGroupRow(e.RowHandle))
            {
                if (view.GetRowExpanded(e.RowHandle))
                {
                    view.ExpandGroupRow(e.RowHandle);
                    ExpandAllChildGroups(view, e.RowHandle); // 展开所有子分组
                }
            }
        }
        // 递归展开所有子分组
        private void ExpandAllChildGroups(GridView view, int rowHandle)
        {
            int childRowCount = view.GetChildRowCount(rowHandle);
            for (int i = 0; i < childRowCount; i++)
            {
                int childRowHandle = view.GetChildRowHandle(rowHandle, i);
                if (view.IsGroupRow(childRowHandle))
                {
                    view.ExpandGroupRow(childRowHandle);
                    ExpandAllChildGroups(view, childRowHandle); // 递归展开子分组
                }
            }
        }
        private void AttachHScrollBarEvent()
        {
            // 查找水平滚动条
            var hScrollBar = this.gridView1.GridControl.Controls[0] as DevExpress.XtraGrid.Scrolling.HCrkScrollBar;
            if (hScrollBar != null)
            {
                // 监听滚动事件
                hScrollBar.Scroll += HScrollBar_Scroll;
            }
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // 同步目标 GridView 的水平滚动条位置
            var targetHScrollBar = gridView2.GridControl.Controls[0] as DevExpress.XtraGrid.Scrolling.HCrkScrollBar;
            if (targetHScrollBar != null)
            {
                gridView2.LeftCoord = e.NewValue;
            }
        }

        private void GroupBox_Paint(object sender, PaintEventArgs e)
        {
            // 获取 Graphics 对象
            Graphics g = e.Graphics;

            // 设置线条颜色和宽度
            Pen pen = new Pen(Color.Black, 2);

            List<Button> btns = keyControl.Values.Where(s => s != null).ToList();

            for (int i = 0; i < btns.Count; i++)
            {
                if (i != btns.Count - 1)
                {//绘制连接关系
                    Point center1 = new Point(btns[i].Left + btns[i].Width / 2, btns[i].Top + btns[i].Height / 2);
                    Point center2 = new Point(btns[i + 1].Left + btns[i + 1].Width / 2, btns[i + 1].Top + btns[i + 1].Height / 2);

                    // 绘制连接按钮的直线
                    g.DrawLine(pen, center1, center2);
                }
            }
        }

        private void CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {

        }

        /// <summary>
        /// 是否开启多选
        /// </summary>
        /// <param name="bindGridView"></param>
        private void SetCheckBoxSelect()
        {
            if (_bindGridView.Options.ShowCheckBoxSelectColumnHeader)
            {
                gridView1.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
                gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                gridView1.OptionsSelection.MultiSelect = true;

                gridView2.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.False;
                gridView2.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                gridView2.OptionsSelection.MultiSelect = true;

                gridView2.VisibleColumns[0].Caption = "选择";

                gridView1.VisibleColumns[0].OptionsColumn.AllowSize = true;

                gridView2.VisibleColumns[0].OptionsColumn.AllowSize = true;

                gridView1.VisibleColumns[0].MinWidth = 30;
                gridView2.VisibleColumns[0].MinWidth = 30;

                gridView1.VisibleColumns[0].Width = 75;
                gridView2.VisibleColumns[0].Width = 75;
            }
        }
        /// <summary>
        /// 多选同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            if (_bindGridView.Options.ShowCheckBoxSelectColumnHeader)
            {
                bool _action = e.Action == CollectionChangeAction.Add ? true : false;

                gridView1.BeginInit();

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    gridView1.SetRowCellValue(i, gridView1.VisibleColumns[0], _action);
                }

                gridView1.EndInit();
            }
        }

        private void KeyDownViewEvent(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                var gridView = sender as GridView;
                if (gridView != null)
                {
                    var cellValue = gridView.GetFocusedDisplayText();

                    Clipboard.Clear();

                    Clipboard.SetText(cellValue);
                }
            }
        }

        private void CustomDrawGroupRowEvent(object sender, RowObjectCustomDrawEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo GridGroupRowInfo = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;

            GridGroupRowInfo.GroupText += "";

            // 获取当前的分组级别数
            int groupLevelCount = gridView1.GroupCount;

            for (int i = 0; i < groupLevelCount; i++)
            {

            }
        }

        private void SetCellStyleByColorCellRules(GridView gdv, List<ColorCellRulesDto> colorCellRules)
        {
            if (colorCellRules.IsNull()) return;

            List<string> cols = gdv.Columns.Select(s => s.FieldName).ToList();
            //筛选已有列
            List<ColorCellRulesDto> _colorCellRules = colorCellRules.Where(s => cols.Contains(s.Col)).ToList();

            foreach (ColorCellRulesDto cellRulesDto in _colorCellRules) gdv.FormatConditions.Add(SetStyleFormatRowCondition(gridView1.Columns[cellRulesDto.Col], cellRulesDto.CellColor, cellRulesDto.Expression));
        }

        private void StartGroupViewEvent(object sender, EventArgs e)
        {
        }

        private void CellValueChangedEvent(object sender, CellValueChangedEventArgs e)
        {
            var data = gridView2.GridControl.DataSource as DataTable;

            foreach (DataRow item in data.Rows)
            {
                item[e.Column.FieldName] = e.Value.ToString();
            }

            DataRow _row = data.Rows[0];

            List<string> rowFilter = new List<string>();
            //筛选操作
            foreach (DataColumn item in data.Columns)
            {
                if (_row[item.ColumnName] != null && _row[item.ColumnName].ToString() != "")
                {
                    if (_row[item.ColumnName].ToString().Contains(","))
                    {

                        List<string> filterList = _row[item.ColumnName].ToString().Split(',').ToList();

                        List<string> _filter = new List<string>();

                        foreach (string filterstr in filterList)
                        {
                            if (filterstr != "")
                                _filter.Add($" CONVERT({item.ColumnName}, System.String) = '{filterstr.Trim()}' ");
                        }

                        string result = $"({string.Join(" or ", _filter)})";

                        rowFilter.Add(result);
                    }
                    else
                    {
                        //rowFilter.Add($"{item.ColumnName} = '{_row[item.ColumnName]}'");

                        //rowFilter.Add($"CONVERT({item.ColumnName}, System.String) LIKE '%{_row[item.ColumnName]}%'");
                        rowFilter.Add($"CONVERT({item.ColumnName}, System.String) LIKE '%{_row[item.ColumnName]}%'");
                    }
                }
            }

            string filter = String.Join(" and ", rowFilter);

            DataTable sourceData = _bindGridView.DataSource as DataTable;

            if (sourceData != null)
            {
                DataView dv = new DataView(sourceData);

                dv.RowFilter = filter;

                DataTable _SourceData = dv.ToTable();

                SetTableDataSource(_SourceData);
            }

            foreach (DataRow item in data.Rows)
            {
                item[e.Column.FieldName] = e.Value.ToString().Trim(',');
            }
        }

        private void ColumnFilterChangedEvent(object sender, EventArgs e)
        {
            var filteredDataView = new DataView(gridControl1.DataSource as DataTable);

            gridView1.ActiveFilterCriteria = gridView2.ActiveFilterCriteria;

            filteredDataView.RowFilter = DevExpress.Data.Filtering.CriteriaToWhereClauseHelper.GetDataSetWhere(gridView1.ActiveFilterCriteria);

            var filterRecords = filteredDataView.ToTable();
        }

        private void RowFocusedChangedEvent(object sender, FocusedRowChangedEventArgs e)
        {
            if (gridView2.FocusedRowHandle != -1)
            {
                //var rowHandle = 0;

                //gridView2.ClearSelection();

                //gridView2.FocusedRowHandle = rowHandle;// 设置焦点行
                //gridView2.SelectRow(rowHandle);

            }
        }

        private void ColumnWidthChangedEvent(object sender, ColumnEventArgs e)
        {
            //新排宽度
            foreach (GridColumn item in gridView2.Columns)
            {
                var col = gridView1.Columns.FirstOrDefault(s => s.FieldName == item.FieldName);

                if (col != null)
                    col.Width = item.Width;
            }

            foreach (GridColumn item in gridView2.VisibleColumns)
            {
                var col = gridView1.VisibleColumns.FirstOrDefault(s => s.FieldName == item.FieldName);

                if (col != null)
                    col.Width = item.Width;
            }
        }
        #region 事件处理
        private void ColumnPositeionChangeedEvent(object sender, EventArgs e)
        {
           List<string> fields =  _bindGridView.ColumnsConfig.Where(s => s.Visiable == false).Select(s => s.Field).ToList();

            //重新排序
            foreach (GridColumn item in gridView2.Columns)
            {
                var col = gridView1.Columns.FirstOrDefault(s => s.FieldName == item.FieldName);

                if (col != null)
                {

                    //如果设置了隐藏不操作该列
                    if (fields.Contains(col.FieldName)) continue;

                    if (item.Visible == false)
                    {

                        if (!keyControl.Keys.Contains(item.Caption))
                        {
                            keyControl.Add(item.Caption, null);
                        }

                        item.Visible = true;

                        col.Group();
                    }

                    col.Visible = true;
                    col.VisibleIndex = item.VisibleIndex;
                }
            }
            List<string> keys = keyControl.Where(s => s.Value == null).Select(v => v.Key).ToList();

            //写入分组样式
            foreach (string key in keys)
            {
                if (keyControl[key] == null)
                {

                    Button btn = new Button()
                    {
                        Text = key,
                        Name = key,
                        TextAlign = ContentAlignment.MiddleLeft,
                        //Image = Properties.Resources.关闭2,
                        ImageAlign = ContentAlignment.MiddleRight,
                        Font = new Font("宋体", 10, FontStyle.Regular),
                    };

                    btn.Location = new Point(_px, 20);

                    _px += 100;

                    btn.Click += (object _sender, EventArgs _e) =>
                    {
                        groupBox1.Controls.Remove(btn);

                        var col = gridView1.Columns.FirstOrDefault(s => s.Caption == btn.Name);
                        if (col != null)
                        {
                            col.UnGroup();

                            keyControl.Remove(col.Caption);

                            _px = _px - 100;
                        }

                        ColumnWidthChangedEvent(null, null);

                        groupBox1.Invalidate();
                    };

                    keyControl[key] = btn;

                    groupBox1.Controls.Add(btn);

                }
            }

            groupBox1.Invalidate();
        }

        private void EndGroupViewEvent(object sender, EventArgs e)
        {
            var groupcol = gridView2.GroupedColumns;

            gridView1.BeginInit();

            gridView1.ClearGrouping();

            foreach (GridColumn item in groupcol)
            {
                gridView1.Columns[item.FieldName].Group();
            }

            if (groupcol.Count() > 0)
            {
                gridView2.GridControl.Height = _height + groupcol.Count() * 40;
            }
            else
            {
                gridView2.GridControl.Height = _height;
            }

            gridView1.EndInit();


        }
        private void EndGroupEvent(object sender, EventArgs e)
        {
            //// 获取当前的分组级别数
            //int groupLevelCount = gridView1.GroupCount;

            //// 默认开启第一层级
            //for (int i = 0; i < groupLevelCount; i++)
            //{
            //    if (i == 0)
            //    {
            //        gridView1.ExpandGroupLevel(i, false); ;
            //    }
            //    else
            //    {
            //        gridView1.CollapseGroupLevel(i, false);
            //    }
            //}
        }
        #endregion
        //初始化gridview列信息,加载数据
        private void InitLoadGrid(BindGridView bindGridView)
        {

            //DataTable dt = bindGridView.DataSource as DataTable;
            DataTable dt = InitDataTable(bindGridView.DataSource);
            //添加一列选择项

            foreach (DataColumn item in dt.Columns)
            {
                GridColumn col = new GridColumn();

                col.Caption = item.Caption;
                col.FieldName = item.ColumnName;
                col.Name = item.ColumnName;
                col.Visible = true;
                col.Width = 100;


                GridColumn _col = new GridColumn();

                _col.Caption = item.Caption;
                _col.FieldName = item.ColumnName;
                _col.Name = item.ColumnName;
                _col.Visible = true;
                _col.Width = 100;
                //设置筛选方式
                _col.OptionsFilter.AllowAutoFilter = false;
                _col.OptionsFilter.AllowFilter = false;
                _col.OptionsFilter.ImmediateUpdateAutoFilter = false;

                //存在某些定制配置
                ColumnConfig config = bindGridView.ColumnsConfig.FirstOrDefault(s => s.Field == item.ColumnName);

                if (config != null)
                {
                    _col.Width = config.Width;
                    _col.Visible = config.Visiable;

                    col.Width = config.Width;
                    col.Visible = config.Visiable;
                }

                AddFilterToColumnHeader(_col, dt);

                this.gridView1.Columns.Add(col);
                this.gridView2.Columns.Add(_col);

                DataTable _dt = dt.Clone();

                _dt.Rows.InsertAt(_dt.NewRow(), 0);

                this.gridView2.GridControl.DataSource = _dt;

            }

            this.gridView1.GridControl.DataSource = bindGridView.DataSource;
        }

        private void AddFilterToColumnHeader(GridColumn column, DataTable data)
        {
            RepositoryItemCheckedComboBoxEdit checkedComboBox = new RepositoryItemCheckedComboBoxEdit();

            List<string> comboBoxdataList = new List<string>();


            DataView dv = data.DefaultView;
            DataTable distTable = dv.ToTable("Dist", true, column.FieldName);


            foreach (DataRow row in distTable.Rows)
            {
                comboBoxdataList.Add(row[column.FieldName].ToString());
            }

            checkedComboBox.Items.AddRange(comboBoxdataList.ToArray());

            checkedComboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            checkedComboBox.EditValueType = EditValueTypeCollection.CSV;

            checkedComboBox.CustomDisplayText += (s, e) =>
            {
                // 获取当前的 CheckedComboBoxEdit
                CheckedComboBoxEdit edit = s as CheckedComboBoxEdit;

                if (edit != null)
                {
                    // 获取用户选择的值
                    var selectedValues = edit.Properties.GetCheckedItems();

                    if (selectedValues != null && selectedValues.ToString() != "")
                    {
                        e.DisplayText = selectedValues.ToString() + ",";
                    }

                }
            };

            // 绑定到列
            column.ColumnEdit = checkedComboBox;
        }

        private DataTable InitDataTable(DataTable data)
        {
            DataTable dt = new DataTable();

            foreach (DataColumn item in data.Columns)
            {
                DataColumn col = new DataColumn();
                col.ColumnName = item.ColumnName;
                col.Caption = item.Caption;
                col.DataType = typeof(string);

                dt.Columns.Add(col);
            }

            foreach (DataRow row in data.Rows)
            {
                DataRow dataRow = dt.NewRow();

                foreach (DataColumn col in data.Columns)
                {
                    if (col.DataType == typeof(DateTime) && row[col.ColumnName] != null && row[col.ColumnName].ToString().Trim() != "")
                    {
                        dataRow[col.ColumnName] = Convert.ToDateTime(row[col.ColumnName]).ToString("d");
                    }
                    else
                    {
                        dataRow[col.ColumnName] = row[col.ColumnName].ToString();
                    }
                }

                dt.Rows.Add(dataRow);
            }

            return dt;

        }

        private void gridViewDetail_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            gdcFlts.ContextMenuStrip = contextPopMenu;
        }

        #region 功能方法
        public void SetAllChecked(bool chencked)
        {
            if (_bindGridView.Options.ShowCheckBoxSelectColumnHeader)
            {
                gridView1.BeginInit();

                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    gridView1.SetRowCellValue(i, gridView1.VisibleColumns[0], chencked);
                }

                gridView1.EndInit();
            }
        }

        /// <summary>
        /// 设置组内合计
        /// </summary>
        /// <param name="rulesDtos"></param>
        private void SetGroupSummary(List<RulesDto> rulesDtos)
        {
            if (rulesDtos.IsNull()) return;

            List<string> jumdentOptions = this.gridView1.Columns.Select(s => s.FieldName).ToList();

            foreach (var item in rulesDtos)
            {
                if (jumdentOptions.Contains(item.Col))
                    this.gridView1.GroupSummary.Add(item.ItemType, item.Col, gridView1.Columns[item.Col], item.Desc);
            }
        }

        /// <summary>
        /// 设置页脚统计类型
        /// </summary>
        /// <param name="rulesDtos"></param>
        private void SetFooterSummary(List<RulesDto> rulesDtos)
        {
            if (rulesDtos.IsNull()) return;

            List<string> jumdentOptions = this.gridView1.Columns.Select(s => s.FieldName).ToList();

            foreach (var item in rulesDtos)
            {
                if (jumdentOptions.Contains(item.Col))
                {
                    this.gridView1.Columns[item.Col].SummaryItem.SummaryType = item.ItemType;

                    if (item.Desc != null && item.Desc.Trim() != "")
                    {
                        this.gridView1.Columns[item.Col].SummaryItem.DisplayFormat = item.Desc;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前焦点所处值与列名
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetSelectedCell()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (gridView1.FocusedRowHandle < 0)
                return dic;

            string value = gridView1.FocusedValue.ToString();

            string key = gridView1.FocusedColumn.FieldName;

            dic.Add(key, value);

            return dic;
        }

        /// <summary>
        /// 获取当前GridView所有数据
        /// </summary>
        /// <returns></returns>
        public object GetDataSource() => gridView1.DataSource;

        /// <summary>
        /// 获取选中行数据信息
        /// </summary>
        /// <returns></returns>
        public object GetSelectedRows()
        {
            int[] indexArr = gridView1.GetSelectedRows();

            if (indexArr != null && indexArr.Length > 0)
            {
                List<object> list = new List<object>();

                foreach (var index in indexArr)
                {
                    list.Add(gridView1.GetRow(index));
                }
                return list;
            }

            return new List<object>();
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="tableDataSource">table数据源</param>
        public void SetTableDataSource(DataTable tableDataSource)
        {
            //SplashScreenManager.ShowDefaultWaitForm("请稍等", "加载数据中");

            this.gridView1.GridControl.DataSource = tableDataSource;

            //SplashScreenManager.CloseForm(false);
        }
        /// <summary>
        /// 获取所有选中行的索引
        /// </summary>
        /// <returns></returns>
        public int[] GetContronlSelectFoued()
        {
            return gridView1.GetSelectedRows();
        }

        /// <summary>
        /// gridView样式
        /// </summary>
        /// <param name="gdv"></param>
        private void GridViewConfigView(DevExpress.XtraGrid.Views.Grid.GridView gdv)
        {
            #region GridView属性设置
            //gridView1.OptionsView.ShowFooter = _bindGridView.Options.ShowGridViewFooter;

            gdv.ScrollStyle = ScrollStyleFlags.None;
            //行号所在列的宽度
            gdv.IndicatorWidth = 50;
            //顶部面板 可用于分组
            gdv.OptionsView.ShowGroupPanel = false;
            //显示底部面板 可用于展示统计
            gdv.OptionsView.ShowFooter = true;
            //显示底部面板 可用于展示统计
            gdv.OptionsView.ShowColumnHeaders = false;
            //奇数行的效果设置是否可用
            gdv.OptionsView.EnableAppearanceEvenRow = true;
            //失去焦点时 是否保留行选中效果
            gdv.OptionsSelection.EnableAppearanceHideSelection = false;
            //是否显示焦点单元格样式
            gdv.OptionsSelection.EnableAppearanceFocusedCell = false;
            //只读
            gdv.OptionsBehavior.ReadOnly = !_bindGridView.Options.AbleEdit;
            //不可编辑 若设置不可编辑 会导致表格中增加的按钮的单击事件不可用
            gdv.OptionsBehavior.Editable = _bindGridView.Options.AbleEdit;
            //启用偶数行定义颜色
            gdv.OptionsView.EnableAppearanceOddRow = true;
            //行选中
            gdv.FocusRectStyle = DrawFocusRectStyle.CellFocus;
            //边框
            //gdv.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            //关闭列右键菜单
            gdv.OptionsMenu.EnableColumnMenu = true;
            //列字体对齐方式
            gdv.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //列字体设置
            gdv.Appearance.HeaderPanel.Font = new System.Drawing.Font("微软雅黑", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            //行字体对齐方式
            gdv.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //奇数行背景色
            gdv.Appearance.EvenRow.BackColor = _bindGridView.Options.EvenRowBackColor;
            //偶数行背景色
            gdv.Appearance.OddRow.BackColor = _bindGridView.Options.OddRowBackColor;
            //焦点行背景色
            gdv.Appearance.FocusedRow.BackColor = _bindGridView.Options.FocusedRowBackColor;
            //焦点行字体颜色
            gdv.Appearance.FocusedRow.ForeColor = Color.White;
            //FooterPanel字体对齐方式
            gdv.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //行字体
            gdv.Appearance.Row.Font = new System.Drawing.Font("微软雅黑", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            //导出相关设置
            gdv.AppearancePrint.Row.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gdv.OptionsPrint.AutoWidth = false;
            gdv.AppearancePrint.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            #endregion

            #region 行号显示
            gdv.CustomDrawRowIndicator += (s, e) =>
            {
                e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                e.Appearance.Font = new System.Drawing.Font("微软雅黑", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {

                    e.Info.DisplayText = Convert.ToString(e.RowHandle + 1);
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "";//"G" + e.RowHandle.ToString();
                }
            };
            #endregion

            #region 当表格内容为空时显示
            gdv.CustomDrawEmptyForeground += (s, e) =>
            {
                if (gdv.RowCount == 0)
                {
                    string str = "未查询数据";
                    Font font = new Font("微软雅黑", 10F, FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                    Rectangle rectangle = new Rectangle(e.Bounds.Left + 5, e.Bounds.Top + 5, e.Bounds.Width - 5, e.Bounds.Height - 5);
                    e.Graphics.DrawString(str, font, Brushes.Black, rectangle);
                }
            };

            #endregion
        }

        /// <summary>
        /// gridView样式
        /// </summary>
        /// <param name="gdv"></param>
        private void GridViewConfigTurn(GridView gdv)
        {
            #region GridView属性设置
            gridView2.OptionsBehavior.AutoExpandAllGroups = true;//展开所有分组

            gdv.GridControl.ContextMenuStrip = contextPopMenu1;

            gdv.OptionsView.ShowGroupPanelColumnsAsSingleRow = true;

            //行号所在列的宽度
            gdv.IndicatorWidth = 50;
            //顶部面板 可用于分组
            gdv.OptionsView.ShowGroupPanel = true;
            //显示底部面板 可用于展示统计
            gdv.OptionsView.ShowFooter = false;
            //奇数行的效果设置是否可用
            gdv.OptionsView.EnableAppearanceEvenRow = true;
            //失去焦点时 是否保留行选中效果
            gdv.OptionsSelection.EnableAppearanceHideSelection = false;
            //是否显示焦点单元格样式
            gdv.OptionsSelection.EnableAppearanceFocusedCell = false;
            //只读
            gdv.OptionsBehavior.ReadOnly = false;
            //不可编辑 若设置不可编辑 会导致表格中增加的按钮的单击事件不可用
            gdv.OptionsBehavior.Editable = true;
            //行选中
            gdv.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.CellFocus;
            //关闭列右键菜单
            gdv.OptionsMenu.EnableColumnMenu = true;
            //列字体对齐方式
            gdv.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //列字体设置
            gdv.Appearance.HeaderPanel.Font = new System.Drawing.Font("微软雅黑", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            //行字体对齐方式
            gdv.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gdv.Appearance.FocusedRow.ForeColor = Color.Black;
            //FooterPanel字体对齐方式
            gdv.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //行字体
            gdv.Appearance.Row.Font = new System.Drawing.Font("微软雅黑", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            //导出相关设置
            gdv.AppearancePrint.Row.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gdv.OptionsPrint.AutoWidth = false;
            gdv.AppearancePrint.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            #endregion

            #region 行号显示
            gdv.CustomDrawRowIndicator += (s, e) =>
            {
                e.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                e.Appearance.Font = new System.Drawing.Font("微软雅黑", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {

                    e.Info.DisplayText = Convert.ToString(e.RowHandle);
                }
                else if (e.RowHandle < 0 && e.RowHandle > -1000)
                {
                    e.Info.Appearance.BackColor = System.Drawing.Color.AntiqueWhite;
                    e.Info.DisplayText = "G" + e.RowHandle.ToString();
                }
            };
            #endregion
        }
        #endregion
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DataTable dataTable = _bindGridView.DataSource as DataTable;

            DataTable dt = dataTable.Clone();

            dt.Rows.Add(dt.NewRow());

            gridView2.GridControl.DataSource = dt;

            SetTableDataSource(dataTable);

        }

        /// <summary>
        /// 设置单元格样式(色块)
        /// </summary>
        /// <param name="gc">Devexpress GridView </param>
        /// <param name="color">颜色</param>
        /// <param name="expression">表达式(条件)</param>
        /// <returns></returns>
        protected virtual StyleFormatCondition SetStyleFormatCellCondition(GridColumn gc, Color color, string expression)
        {
            StyleFormatCondition condition1 = new StyleFormatCondition();
            condition1.Column = gc;
            condition1.Appearance.BackColor = color;
            condition1.Appearance.Options.UseBackColor = true;
            condition1.Condition = FormatConditionEnum.Expression;
            condition1.Expression = expression;
            return condition1;
        }
        /// <summary>
        /// 设置行样式(色块)
        /// </summary>
        /// <param name="gc">Devexpress GridView </param>
        /// <param name="color">颜色</param>
        /// <param name="expression">表达式(条件)</param>
        /// <returns></returns>
        protected virtual StyleFormatCondition SetStyleFormatRowCondition(GridColumn gc, Color color, string expression)
        {
            StyleFormatCondition condition1 = new StyleFormatCondition();
            condition1.Column = gc;
            condition1.Appearance.BackColor = color;
            condition1.Appearance.Options.UseBackColor = true;
            condition1.Condition = FormatConditionEnum.Expression;
            condition1.Expression = expression;
            condition1.ApplyToRow = true;
            return condition1;
        }

        /// <summary>
        /// 设置单元格(字体颜色)
        /// </summary>
        /// <param name="gc">Devexpress GridView</param>
        /// <param name="color">颜色</param>
        /// <param name="expression">表达式(条件)</param>
        /// <returns></returns>
        protected virtual StyleFormatCondition SetStyleFormatFontCellCondition(GridColumn gc, Color color, string expression)
        {
            StyleFormatCondition condition1 = new StyleFormatCondition();
            condition1.Column = gc;
            condition1.Appearance.ForeColor = color;
            condition1.Appearance.Options.UseForeColor = true;
            condition1.Appearance.Options.UseTextOptions = true;
            condition1.Condition = FormatConditionEnum.Expression;
            condition1.Expression = expression;
            return condition1;
        }

        private void 清除所有分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridView2.ClearGrouping();

            gridView1.ClearGrouping();

            foreach (Button btn in keyControl.Values) groupBox1.Controls.Remove(btn);

            this.keyControl = new Dictionary<string, Button>();

            this._px = 10;

            ColumnWidthChangedEvent(null, null);

            groupBox1.Invalidate();
        }

    }
}
