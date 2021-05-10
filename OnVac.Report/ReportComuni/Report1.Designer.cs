namespace Onit.OnAssistnet.OnVac.Report.ReportComuni
{
    partial class Report1
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule5 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule6 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule7 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.sqlDataSource1 = new Telerik.Reporting.SqlDataSource();
            this.labelsGroupHeader = new Telerik.Reporting.GroupHeaderSection();
            this.labelsGroupFooter = new Telerik.Reporting.GroupFooterSection();
            this.labelsGroup = new Telerik.Reporting.Group();
            this.vAC_CODICECaptionTextBox = new Telerik.Reporting.TextBox();
            this.vAC_DESCRIZIONECaptionTextBox = new Telerik.Reporting.TextBox();
            this.vAC_OBBLIGATORIACaptionTextBox = new Telerik.Reporting.TextBox();
            this.pageHeader = new Telerik.Reporting.PageHeaderSection();
            this.reportNameTextBox = new Telerik.Reporting.TextBox();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.currentTimeTextBox = new Telerik.Reporting.TextBox();
            this.pageInfoTextBox = new Telerik.Reporting.TextBox();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.titleTextBox = new Telerik.Reporting.TextBox();
            this.reportFooter = new Telerik.Reporting.ReportFooterSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.vAC_CODICEDataTextBox = new Telerik.Reporting.TextBox();
            this.vAC_DESCRIZIONEDataTextBox = new Telerik.Reporting.TextBox();
            this.vAC_OBBLIGATORIADataTextBox = new Telerik.Reporting.TextBox();
            this.sqlDataSource2 = new Telerik.Reporting.SqlDataSource();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionString = "Onit.OnAssistnet.OnVac.Report.ReportComuni.Properties.Settings.sviora11g";
            this.sqlDataSource1.Name = "sqlDataSource1";
            this.sqlDataSource1.SelectCommand = "select *\r\nfrom t_ana_vaccinazioni";
            // 
            // labelsGroupHeader
            // 
            this.labelsGroupHeader.Height = new Telerik.Reporting.Drawing.Unit(0.71437495946884155D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.labelsGroupHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.vAC_CODICECaptionTextBox,
            this.vAC_DESCRIZIONECaptionTextBox,
            this.vAC_OBBLIGATORIACaptionTextBox});
            this.labelsGroupHeader.Name = "labelsGroupHeader";
            this.labelsGroupHeader.PrintOnEveryPage = true;
            // 
            // labelsGroupFooter
            // 
            this.labelsGroupFooter.Height = new Telerik.Reporting.Drawing.Unit(0.71437495946884155D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.labelsGroupFooter.Name = "labelsGroupFooter";
            this.labelsGroupFooter.Style.Visible = false;
            // 
            // labelsGroup
            // 
            this.labelsGroup.GroupFooter = this.labelsGroupFooter;
            this.labelsGroup.GroupHeader = this.labelsGroupHeader;
            this.labelsGroup.Name = "labelsGroup";
            // 
            // vAC_CODICECaptionTextBox
            // 
            this.vAC_CODICECaptionTextBox.CanGrow = true;
            this.vAC_CODICECaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_CODICECaptionTextBox.Name = "vAC_CODICECaptionTextBox";
            this.vAC_CODICECaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(5.2008333206176758D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_CODICECaptionTextBox.StyleName = "Caption";
            this.vAC_CODICECaptionTextBox.Value = "VAC_CODICE";
            // 
            // vAC_DESCRIZIONECaptionTextBox
            // 
            this.vAC_DESCRIZIONECaptionTextBox.CanGrow = true;
            this.vAC_DESCRIZIONECaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.3066668510437012D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_DESCRIZIONECaptionTextBox.Name = "vAC_DESCRIZIONECaptionTextBox";
            this.vAC_DESCRIZIONECaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(5.2008333206176758D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_DESCRIZIONECaptionTextBox.StyleName = "Caption";
            this.vAC_DESCRIZIONECaptionTextBox.Value = "VAC_DESCRIZIONE";
            // 
            // vAC_OBBLIGATORIACaptionTextBox
            // 
            this.vAC_OBBLIGATORIACaptionTextBox.CanGrow = true;
            this.vAC_OBBLIGATORIACaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(10.560417175292969D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_OBBLIGATORIACaptionTextBox.Name = "vAC_OBBLIGATORIACaptionTextBox";
            this.vAC_OBBLIGATORIACaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(5.2008333206176758D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_OBBLIGATORIACaptionTextBox.StyleName = "Caption";
            this.vAC_OBBLIGATORIACaptionTextBox.Value = "VAC_OBBLIGATORIA";
            // 
            // pageHeader
            // 
            this.pageHeader.Height = new Telerik.Reporting.Drawing.Unit(0.71437495946884155D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.pageHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.reportNameTextBox});
            this.pageHeader.Name = "pageHeader";
            // 
            // reportNameTextBox
            // 
            this.reportNameTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.reportNameTextBox.Name = "reportNameTextBox";
            this.reportNameTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(15.708333015441895D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.reportNameTextBox.StyleName = "PageInfo";
            this.reportNameTextBox.Value = "Report1";
            // 
            // pageFooter
            // 
            this.pageFooter.Height = new Telerik.Reporting.Drawing.Unit(0.71437495946884155D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.pageFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.currentTimeTextBox,
            this.pageInfoTextBox});
            this.pageFooter.Name = "pageFooter";
            // 
            // currentTimeTextBox
            // 
            this.currentTimeTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.currentTimeTextBox.Name = "currentTimeTextBox";
            this.currentTimeTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.8277082443237305D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.currentTimeTextBox.StyleName = "PageInfo";
            this.currentTimeTextBox.Value = "=NOW()";
            // 
            // pageInfoTextBox
            // 
            this.pageInfoTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(7.9335417747497559D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.pageInfoTextBox.Name = "pageInfoTextBox";
            this.pageInfoTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(7.8277082443237305D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.pageInfoTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageInfoTextBox.StyleName = "PageInfo";
            this.pageInfoTextBox.Value = "=PageNumber";
            // 
            // reportHeader
            // 
            this.reportHeader.Height = new Telerik.Reporting.Drawing.Unit(2.0529167652130127D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.titleTextBox});
            this.reportHeader.Name = "reportHeader";
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(15.814167022705078D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.titleTextBox.StyleName = "Title";
            this.titleTextBox.Value = "Report1";
            // 
            // reportFooter
            // 
            this.reportFooter.Height = new Telerik.Reporting.Drawing.Unit(0.71437495946884155D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.reportFooter.Name = "reportFooter";
            // 
            // detail
            // 
            this.detail.Height = new Telerik.Reporting.Drawing.Unit(2.8574001789093018D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.vAC_CODICEDataTextBox,
            this.vAC_DESCRIZIONEDataTextBox,
            this.vAC_OBBLIGATORIADataTextBox,
            this.table1});
            this.detail.Name = "detail";
            // 
            // vAC_CODICEDataTextBox
            // 
            this.vAC_CODICEDataTextBox.CanGrow = true;
            this.vAC_CODICEDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_CODICEDataTextBox.Name = "vAC_CODICEDataTextBox";
            this.vAC_CODICEDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(5.2008333206176758D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_CODICEDataTextBox.StyleName = "Data";
            this.vAC_CODICEDataTextBox.Value = "=Fields.VAC_CODICE";
            // 
            // vAC_DESCRIZIONEDataTextBox
            // 
            this.vAC_DESCRIZIONEDataTextBox.CanGrow = true;
            this.vAC_DESCRIZIONEDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(5.3066668510437012D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_DESCRIZIONEDataTextBox.Name = "vAC_DESCRIZIONEDataTextBox";
            this.vAC_DESCRIZIONEDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(5.2008333206176758D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_DESCRIZIONEDataTextBox.StyleName = "Data";
            this.vAC_DESCRIZIONEDataTextBox.Value = "=Fields.VAC_DESCRIZIONE";
            // 
            // vAC_OBBLIGATORIADataTextBox
            // 
            this.vAC_OBBLIGATORIADataTextBox.CanGrow = true;
            this.vAC_OBBLIGATORIADataTextBox.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(10.560417175292969D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.052916664630174637D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_OBBLIGATORIADataTextBox.Name = "vAC_OBBLIGATORIADataTextBox";
            this.vAC_OBBLIGATORIADataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(5.2008333206176758D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.60000002384185791D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.vAC_OBBLIGATORIADataTextBox.StyleName = "Data";
            this.vAC_OBBLIGATORIADataTextBox.Value = "=Fields.VAC_OBBLIGATORIA";
            // 
            // sqlDataSource2
            // 
            this.sqlDataSource2.ConnectionString = "Onit.OnAssistnet.OnVac.Report.ReportComuni.Properties.Settings.sviora11g";
            this.sqlDataSource2.Name = "sqlDataSource2";
            this.sqlDataSource2.SelectCommand = "select *\r\nfrom t_ana_cicli";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(new Telerik.Reporting.Drawing.Unit(0.5D, Telerik.Reporting.Drawing.UnitType.Cm)));
            this.table1.Body.SetCellContent(0, 0, this.textBox3);
            this.table1.Body.SetCellContent(0, 1, this.textBox4);
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.ReportItem = this.textBox2;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.DataSource = this.sqlDataSource2;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(new Telerik.Reporting.Drawing.Unit(1.6000000238418579D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(1.2183336019515991D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.table1.Name = "table1";
            tableGroup3.Groupings.AddRange(new Telerik.Reporting.Data.Grouping[] {
            new Telerik.Reporting.Data.Grouping("")});
            tableGroup3.Name = "Detail";
            this.table1.RowGroups.Add(tableGroup3);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(4D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(1D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.table1.StyleName = "Normal.TableNormal";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.5D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.textBox1.StyleName = "Normal.TableHeader";
            this.textBox1.Value = "CIC_CODICE";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.5D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.textBox2.StyleName = "Normal.TableHeader";
            this.textBox2.Value = "CIC_DESCRIZIONE";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.5D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.textBox3.StyleName = "Normal.TableBody";
            this.textBox3.Value = "=Fields.CIC_CODICE";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(new Telerik.Reporting.Drawing.Unit(2D, Telerik.Reporting.Drawing.UnitType.Cm), new Telerik.Reporting.Drawing.Unit(0.5D, Telerik.Reporting.Drawing.UnitType.Cm));
            this.textBox4.StyleName = "Normal.TableBody";
            this.textBox4.Value = "=Fields.CIC_DESCRIZIONE";
            // 
            // Report1
            // 
            this.DataSource = this.sqlDataSource1;
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            this.labelsGroup});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.labelsGroupHeader,
            this.labelsGroupFooter,
            this.pageHeader,
            this.pageFooter,
            this.reportHeader,
            this.reportFooter,
            this.detail});
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins.Bottom = new Telerik.Reporting.Drawing.Unit(2.5399999618530273D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.PageSettings.Margins.Left = new Telerik.Reporting.Drawing.Unit(2.5399999618530273D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.PageSettings.Margins.Right = new Telerik.Reporting.Drawing.Unit(2.5399999618530273D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.PageSettings.Margins.Top = new Telerik.Reporting.Drawing.Unit(2.5399999618530273D, Telerik.Reporting.Drawing.UnitType.Cm);
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule1.Style.Color = System.Drawing.Color.Black;
            styleRule1.Style.Font.Bold = true;
            styleRule1.Style.Font.Italic = false;
            styleRule1.Style.Font.Name = "Tahoma";
            styleRule1.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(20D, Telerik.Reporting.Drawing.UnitType.Point);
            styleRule1.Style.Font.Strikeout = false;
            styleRule1.Style.Font.Underline = false;
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Tahoma";
            styleRule2.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(11D, Telerik.Reporting.Drawing.UnitType.Point);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule3.Style.Font.Name = "Tahoma";
            styleRule3.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(11D, Telerik.Reporting.Drawing.UnitType.Point);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule4.Style.Font.Name = "Tahoma";
            styleRule4.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(11D, Telerik.Reporting.Drawing.UnitType.Point);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule5.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), "Normal.TableNormal")});
            styleRule5.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule5.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule5.Style.BorderWidth.Default = new Telerik.Reporting.Drawing.Unit(1D, Telerik.Reporting.Drawing.UnitType.Pixel);
            styleRule5.Style.Color = System.Drawing.Color.Black;
            styleRule5.Style.Font.Name = "Tahoma";
            styleRule5.Style.Font.Size = new Telerik.Reporting.Drawing.Unit(8D, Telerik.Reporting.Drawing.UnitType.Point);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableHeader")});
            styleRule6.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule6.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule6.Style.BorderWidth.Default = new Telerik.Reporting.Drawing.Unit(1D, Telerik.Reporting.Drawing.UnitType.Pixel);
            styleRule6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableBody")});
            styleRule7.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule7.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule7.Style.BorderWidth.Default = new Telerik.Reporting.Drawing.Unit(1D, Telerik.Reporting.Drawing.UnitType.Pixel);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4,
            styleRule5,
            styleRule6,
            styleRule7});
            this.Width = new Telerik.Reporting.Drawing.Unit(15.814167022705078D, Telerik.Reporting.Drawing.UnitType.Cm);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.SqlDataSource sqlDataSource1;
        private Telerik.Reporting.GroupHeaderSection labelsGroupHeader;
        private Telerik.Reporting.TextBox vAC_CODICECaptionTextBox;
        private Telerik.Reporting.TextBox vAC_DESCRIZIONECaptionTextBox;
        private Telerik.Reporting.TextBox vAC_OBBLIGATORIACaptionTextBox;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooter;
        private Telerik.Reporting.Group labelsGroup;
        private Telerik.Reporting.PageHeaderSection pageHeader;
        private Telerik.Reporting.TextBox reportNameTextBox;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.TextBox currentTimeTextBox;
        private Telerik.Reporting.TextBox pageInfoTextBox;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.TextBox titleTextBox;
        private Telerik.Reporting.ReportFooterSection reportFooter;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox vAC_CODICEDataTextBox;
        private Telerik.Reporting.TextBox vAC_DESCRIZIONEDataTextBox;
        private Telerik.Reporting.TextBox vAC_OBBLIGATORIADataTextBox;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.SqlDataSource sqlDataSource2;

    }
}