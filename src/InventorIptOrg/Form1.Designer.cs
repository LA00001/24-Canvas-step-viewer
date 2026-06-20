namespace InventorIptOrg
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора форм.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить используемые ресурсы.
        /// </summary>
        /// <param name="disposing">true, если управляемые ресурсы нужно освободить; иначе false.</param>
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
        /// Обязательный метод для поддержки конструктора форм — не изменять
        /// содержимое этого метода в редакторе кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageIpt = new System.Windows.Forms.TabPage();
            this.ButtonIptApplyBrowserTreeEdits = new System.Windows.Forms.Button();
            this.ButtonIptSaveBrowserTree = new System.Windows.Forms.Button();
            this.ButtonIptCopyBrowserTree = new System.Windows.Forms.Button();
            this.ButtonIptRefreshBrowserTreeSpatialBase = new System.Windows.Forms.Button();
            this.ButtonIptRefreshBrowserTree = new System.Windows.Forms.Button();
            this.ButtonIptBuildVisibleListTree = new System.Windows.Forms.Button();
            this.dataGridViewIptBrowserTree = new System.Windows.Forms.DataGridView();
            this.ColumnIptTreeDepth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIptTreeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIptTreeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIptTreeX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIptTreeY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnIptTreeZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelIptBrowserTree = new System.Windows.Forms.Label();
            this.labelIptFeatureList = new System.Windows.Forms.Label();
            this.ButtonIptCreateFeatureFolder = new System.Windows.Forms.Button();
            this.ButtonIptCreateFeatureFolder2 = new System.Windows.Forms.Button();
            this.ButtonIptRemoveSelectedFeature = new System.Windows.Forms.Button();
            this.ButtonIptCopyFeatureList = new System.Windows.Forms.Button();
            this.ButtonIptClearFeatureList = new System.Windows.Forms.Button();
            this.listBoxIptFeatures = new System.Windows.Forms.ListBox();
            this.labelIptGroupList = new System.Windows.Forms.Label();
            this.ButtonIptCreateBodyFolder = new System.Windows.Forms.Button();
            this.ButtonIptRemoveSelected = new System.Windows.Forms.Button();
            this.ButtonIptCopyList = new System.Windows.Forms.Button();
            this.ButtonIptClearList = new System.Windows.Forms.Button();
            this.listBoxIptBodies = new System.Windows.Forms.ListBox();
            this.labelIptInfo = new System.Windows.Forms.Label();
            this.ButtonIptToggle = new System.Windows.Forms.Button();
            this.ButtonIptAddInsideBox = new System.Windows.Forms.Button();
            this.ButtonIptAdd = new System.Windows.Forms.Button();
            this.tabPageIam = new System.Windows.Forms.TabPage();
            this.labelIamInfo = new System.Windows.Forms.Label();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageIpt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIptBrowserTree)).BeginInit();
            this.tabPageIam.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageIpt);
            this.tabControl1.Controls.Add(this.tabPageIam);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1184, 821);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageIpt
            // 
            this.tabPageIpt.Controls.Add(this.ButtonIptApplyBrowserTreeEdits);
            this.tabPageIpt.Controls.Add(this.ButtonIptSaveBrowserTree);
            this.tabPageIpt.Controls.Add(this.ButtonIptCopyBrowserTree);
            this.tabPageIpt.Controls.Add(this.ButtonIptRefreshBrowserTreeSpatialBase);
            this.tabPageIpt.Controls.Add(this.ButtonIptRefreshBrowserTree);
            this.tabPageIpt.Controls.Add(this.ButtonIptBuildVisibleListTree);
            this.tabPageIpt.Controls.Add(this.dataGridViewIptBrowserTree);
            this.tabPageIpt.Controls.Add(this.labelIptBrowserTree);
            this.tabPageIpt.Controls.Add(this.labelIptFeatureList);
            this.tabPageIpt.Controls.Add(this.ButtonIptCreateFeatureFolder2);
            this.tabPageIpt.Controls.Add(this.ButtonIptCreateFeatureFolder);
            this.tabPageIpt.Controls.Add(this.ButtonIptRemoveSelectedFeature);
            this.tabPageIpt.Controls.Add(this.ButtonIptCopyFeatureList);
            this.tabPageIpt.Controls.Add(this.ButtonIptClearFeatureList);
            this.tabPageIpt.Controls.Add(this.listBoxIptFeatures);
            this.tabPageIpt.Controls.Add(this.labelIptGroupList);
            this.tabPageIpt.Controls.Add(this.ButtonIptCreateBodyFolder);
            this.tabPageIpt.Controls.Add(this.ButtonIptRemoveSelected);
            this.tabPageIpt.Controls.Add(this.ButtonIptCopyList);
            this.tabPageIpt.Controls.Add(this.ButtonIptClearList);
            this.tabPageIpt.Controls.Add(this.listBoxIptBodies);
            this.tabPageIpt.Controls.Add(this.labelIptInfo);
            this.tabPageIpt.Controls.Add(this.ButtonIptToggle);
            this.tabPageIpt.Controls.Add(this.ButtonIptAddInsideBox);
            this.tabPageIpt.Controls.Add(this.ButtonIptAdd);
            this.tabPageIpt.Location = new System.Drawing.Point(4, 25);
            this.tabPageIpt.Name = "tabPageIpt";
            this.tabPageIpt.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIpt.Size = new System.Drawing.Size(1176, 792);
            this.tabPageIpt.TabIndex = 0;
            this.tabPageIpt.Text = ".ipt";
            this.tabPageIpt.UseVisualStyleBackColor = true;
            // 
            // ButtonIptApplyBrowserTreeEdits
            // 
            this.ButtonIptApplyBrowserTreeEdits.Location = new System.Drawing.Point(980, 722);
            this.ButtonIptApplyBrowserTreeEdits.Name = "ButtonIptApplyBrowserTreeEdits";
            this.ButtonIptApplyBrowserTreeEdits.Size = new System.Drawing.Size(179, 52);
            this.ButtonIptApplyBrowserTreeEdits.TabIndex = 21;
            this.ButtonIptApplyBrowserTreeEdits.Text = "Apply edited\r\nname / XYZ to Inventor";
            this.ButtonIptApplyBrowserTreeEdits.UseVisualStyleBackColor = true;
            this.ButtonIptApplyBrowserTreeEdits.Click += new System.EventHandler(this.ButtonIptApplyBrowserTreeEdits_Click);
            // 
            // ButtonIptSaveBrowserTree
            // 
            this.ButtonIptSaveBrowserTree.Location = new System.Drawing.Point(812, 722);
            this.ButtonIptSaveBrowserTree.Name = "ButtonIptSaveBrowserTree";
            this.ButtonIptSaveBrowserTree.Size = new System.Drawing.Size(155, 52);
            this.ButtonIptSaveBrowserTree.TabIndex = 20;
            this.ButtonIptSaveBrowserTree.Text = "Save tree\r\nJSON file";
            this.ButtonIptSaveBrowserTree.UseVisualStyleBackColor = true;
            this.ButtonIptSaveBrowserTree.Click += new System.EventHandler(this.ButtonIptSaveBrowserTree_Click);
            // 
            // ButtonIptCopyBrowserTree
            // 
            this.ButtonIptCopyBrowserTree.Location = new System.Drawing.Point(665, 722);
            this.ButtonIptCopyBrowserTree.Name = "ButtonIptCopyBrowserTree";
            this.ButtonIptCopyBrowserTree.Size = new System.Drawing.Size(141, 52);
            this.ButtonIptCopyBrowserTree.TabIndex = 19;
            this.ButtonIptCopyBrowserTree.Text = "Copy tree\r\nto clipboard";
            this.ButtonIptCopyBrowserTree.UseVisualStyleBackColor = true;
            this.ButtonIptCopyBrowserTree.Click += new System.EventHandler(this.ButtonIptCopyBrowserTree_Click);
            // 
            // ButtonIptRefreshBrowserTree
            // 
            this.ButtonIptRefreshBrowserTree.Location = new System.Drawing.Point(323, 722);
            this.ButtonIptRefreshBrowserTree.Name = "ButtonIptRefreshBrowserTree";
            this.ButtonIptRefreshBrowserTree.Size = new System.Drawing.Size(165, 52);
            this.ButtonIptRefreshBrowserTree.TabIndex = 18;
            this.ButtonIptRefreshBrowserTree.Text = "Refresh browser tree\r\nlast: -- s";
            this.ButtonIptRefreshBrowserTree.UseVisualStyleBackColor = true;
            this.ButtonIptRefreshBrowserTree.Click += new System.EventHandler(this.ButtonIptRefreshBrowserTree_Click);
            // 
            // ButtonIptRefreshBrowserTreeSpatialBase
            // 
            this.ButtonIptRefreshBrowserTreeSpatialBase.Location = new System.Drawing.Point(494, 722);
            this.ButtonIptRefreshBrowserTreeSpatialBase.Name = "ButtonIptRefreshBrowserTreeSpatialBase";
            this.ButtonIptRefreshBrowserTreeSpatialBase.Size = new System.Drawing.Size(165, 52);
            this.ButtonIptRefreshBrowserTreeSpatialBase.TabIndex = 24;
            this.ButtonIptRefreshBrowserTreeSpatialBase.Text = "Refresh tree #2\r\nfrom spatial BASE";
            this.ButtonIptRefreshBrowserTreeSpatialBase.UseVisualStyleBackColor = true;
            this.ButtonIptRefreshBrowserTreeSpatialBase.Click += new System.EventHandler(this.ButtonIptRefreshBrowserTreeSpatialBase_Click);
            // 
            // ButtonIptBuildVisibleListTree
            // 
            this.ButtonIptBuildVisibleListTree.Location = new System.Drawing.Point(846, 496);
            this.ButtonIptBuildVisibleListTree.Name = "ButtonIptBuildVisibleListTree";
            this.ButtonIptBuildVisibleListTree.Size = new System.Drawing.Size(297, 28);
            this.ButtonIptBuildVisibleListTree.TabIndex = 23;
            this.ButtonIptBuildVisibleListTree.Text = "Build Browser tree from current visible-list";
            this.ButtonIptBuildVisibleListTree.UseVisualStyleBackColor = true;
            this.ButtonIptBuildVisibleListTree.Click += new System.EventHandler(this.ButtonIptBuildVisibleListTree_Click);
            // 
            // dataGridViewIptBrowserTree
            // 
            this.dataGridViewIptBrowserTree.AllowUserToAddRows = false;
            this.dataGridViewIptBrowserTree.AllowUserToDeleteRows = false;
            this.dataGridViewIptBrowserTree.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewIptBrowserTree.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnIptTreeDepth,
            this.ColumnIptTreeType,
            this.ColumnIptTreeName,
            this.ColumnIptTreeX,
            this.ColumnIptTreeY,
            this.ColumnIptTreeZ});
            this.dataGridViewIptBrowserTree.Location = new System.Drawing.Point(323, 530);
            this.dataGridViewIptBrowserTree.MultiSelect = true;
            this.dataGridViewIptBrowserTree.Name = "dataGridViewIptBrowserTree";
            this.dataGridViewIptBrowserTree.RowHeadersWidth = 28;
            this.dataGridViewIptBrowserTree.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewIptBrowserTree.Size = new System.Drawing.Size(820, 180);
            this.dataGridViewIptBrowserTree.TabIndex = 17;
            // 
            // ColumnIptTreeDepth
            // 
            this.ColumnIptTreeDepth.HeaderText = "Level";
            this.ColumnIptTreeDepth.Name = "ColumnIptTreeDepth";
            this.ColumnIptTreeDepth.ReadOnly = true;
            this.ColumnIptTreeDepth.Width = 55;
            // 
            // ColumnIptTreeType
            // 
            this.ColumnIptTreeType.HeaderText = "Type";
            this.ColumnIptTreeType.Name = "ColumnIptTreeType";
            this.ColumnIptTreeType.ReadOnly = true;
            this.ColumnIptTreeType.Width = 105;
            // 
            // ColumnIptTreeName
            // 
            this.ColumnIptTreeName.HeaderText = "Name / имя";
            this.ColumnIptTreeName.Name = "ColumnIptTreeName";
            this.ColumnIptTreeName.Width = 355;
            // 
            // ColumnIptTreeX
            // 
            this.ColumnIptTreeX.HeaderText = "X";
            this.ColumnIptTreeX.Name = "ColumnIptTreeX";
            this.ColumnIptTreeX.Width = 90;
            // 
            // ColumnIptTreeY
            // 
            this.ColumnIptTreeY.HeaderText = "Y";
            this.ColumnIptTreeY.Name = "ColumnIptTreeY";
            this.ColumnIptTreeY.Width = 90;
            // 
            // ColumnIptTreeZ
            // 
            this.ColumnIptTreeZ.HeaderText = "Z";
            this.ColumnIptTreeZ.Name = "ColumnIptTreeZ";
            this.ColumnIptTreeZ.Width = 90;
            // 
            // labelIptBrowserTree
            // 
            this.labelIptBrowserTree.AutoSize = true;
            this.labelIptBrowserTree.Location = new System.Drawing.Point(320, 505);
            this.labelIptBrowserTree.Name = "labelIptBrowserTree";
            this.labelIptBrowserTree.Size = new System.Drawing.Size(269, 16);
            this.labelIptBrowserTree.TabIndex = 16;
            this.labelIptBrowserTree.Text = "Browser tree / дерево браузера Inventor: 0";
            // 
            // labelIptFeatureList
            // 
            this.labelIptFeatureList.AutoSize = true;
            this.labelIptFeatureList.Location = new System.Drawing.Point(320, 263);
            this.labelIptFeatureList.Name = "labelIptFeatureList";
            this.labelIptFeatureList.Size = new System.Drawing.Size(261, 16);
            this.labelIptFeatureList.TabIndex = 13;
            this.labelIptFeatureList.Text = "Features / элементы, связанные с телами";
            // 
            // ButtonIptCreateFeatureFolder
            // 
            this.ButtonIptCreateFeatureFolder.Location = new System.Drawing.Point(762, 440);
            this.ButtonIptCreateFeatureFolder.Name = "ButtonIptCreateFeatureFolder";
            this.ButtonIptCreateFeatureFolder.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptCreateFeatureFolder.TabIndex = 14;
            this.ButtonIptCreateFeatureFolder.Text = "Create feature\r\nbrowser folder";
            this.ButtonIptCreateFeatureFolder.UseVisualStyleBackColor = true;
            this.ButtonIptCreateFeatureFolder.Click += new System.EventHandler(this.ButtonIptCreateFeatureFolder_Click);
            // 
            // ButtonIptCreateFeatureFolder2
            // 
            this.ButtonIptCreateFeatureFolder2.Location = new System.Drawing.Point(908, 440);
            this.ButtonIptCreateFeatureFolder2.Name = "ButtonIptCreateFeatureFolder2";
            this.ButtonIptCreateFeatureFolder2.Size = new System.Drawing.Size(160, 52);
            this.ButtonIptCreateFeatureFolder2.TabIndex = 22;
            this.ButtonIptCreateFeatureFolder2.Text = "Create feature\r\nbrowser folder #2";
            this.ButtonIptCreateFeatureFolder2.UseVisualStyleBackColor = true;
            this.ButtonIptCreateFeatureFolder2.Click += new System.EventHandler(this.ButtonIptCreateFeatureFolder_Click);
            // 
            // ButtonIptRemoveSelectedFeature
            // 
            this.ButtonIptRemoveSelectedFeature.Location = new System.Drawing.Point(616, 440);
            this.ButtonIptRemoveSelectedFeature.Name = "ButtonIptRemoveSelectedFeature";
            this.ButtonIptRemoveSelectedFeature.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptRemoveSelectedFeature.TabIndex = 12;
            this.ButtonIptRemoveSelectedFeature.Text = "Remove selected\r\nfeature";
            this.ButtonIptRemoveSelectedFeature.UseVisualStyleBackColor = true;
            this.ButtonIptRemoveSelectedFeature.Click += new System.EventHandler(this.ButtonIptRemoveSelectedFeature_Click);
            // 
            // ButtonIptCopyFeatureList
            // 
            this.ButtonIptCopyFeatureList.Location = new System.Drawing.Point(470, 440);
            this.ButtonIptCopyFeatureList.Name = "ButtonIptCopyFeatureList";
            this.ButtonIptCopyFeatureList.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptCopyFeatureList.TabIndex = 11;
            this.ButtonIptCopyFeatureList.Text = "Copy features\r\nto clipboard";
            this.ButtonIptCopyFeatureList.UseVisualStyleBackColor = true;
            this.ButtonIptCopyFeatureList.Click += new System.EventHandler(this.ButtonIptCopyFeatureList_Click);
            // 
            // ButtonIptClearFeatureList
            // 
            this.ButtonIptClearFeatureList.Location = new System.Drawing.Point(323, 440);
            this.ButtonIptClearFeatureList.Name = "ButtonIptClearFeatureList";
            this.ButtonIptClearFeatureList.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptClearFeatureList.TabIndex = 10;
            this.ButtonIptClearFeatureList.Text = "Clear feature\r\nlist";
            this.ButtonIptClearFeatureList.UseVisualStyleBackColor = true;
            this.ButtonIptClearFeatureList.Click += new System.EventHandler(this.ButtonIptClearFeatureList_Click);
            // 
            // listBoxIptFeatures
            // 
            this.listBoxIptFeatures.FormattingEnabled = true;
            this.listBoxIptFeatures.HorizontalScrollbar = true;
            this.listBoxIptFeatures.ItemHeight = 16;
            this.listBoxIptFeatures.Location = new System.Drawing.Point(323, 285);
            this.listBoxIptFeatures.Name = "listBoxIptFeatures";
            this.listBoxIptFeatures.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxIptFeatures.Size = new System.Drawing.Size(820, 132);
            this.listBoxIptFeatures.TabIndex = 9;
            this.listBoxIptFeatures.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxIptFeatures_KeyDown);
            // 
            // labelIptGroupList
            // 
            this.labelIptGroupList.AutoSize = true;
            this.labelIptGroupList.Location = new System.Drawing.Point(320, 23);
            this.labelIptGroupList.Name = "labelIptGroupList";
            this.labelIptGroupList.Size = new System.Drawing.Size(279, 16);
            this.labelIptGroupList.TabIndex = 8;
            this.labelIptGroupList.Text = "Bodies in current group / список тел в группе";
            // 
            // ButtonIptCreateBodyFolder
            // 
            this.ButtonIptCreateBodyFolder.Location = new System.Drawing.Point(762, 195);
            this.ButtonIptCreateBodyFolder.Name = "ButtonIptCreateBodyFolder";
            this.ButtonIptCreateBodyFolder.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptCreateBodyFolder.TabIndex = 15;
            this.ButtonIptCreateBodyFolder.Text = "Create body\r\nbrowser folder";
            this.ButtonIptCreateBodyFolder.UseVisualStyleBackColor = true;
            this.ButtonIptCreateBodyFolder.Click += new System.EventHandler(this.ButtonIptCreateBodyFolder_Click);
            // 
            // ButtonIptRemoveSelected
            // 
            this.ButtonIptRemoveSelected.Location = new System.Drawing.Point(616, 195);
            this.ButtonIptRemoveSelected.Name = "ButtonIptRemoveSelected";
            this.ButtonIptRemoveSelected.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptRemoveSelected.TabIndex = 7;
            this.ButtonIptRemoveSelected.Text = "Remove selected\r\nbody";
            this.ButtonIptRemoveSelected.UseVisualStyleBackColor = true;
            this.ButtonIptRemoveSelected.Click += new System.EventHandler(this.ButtonIptRemoveSelected_Click);
            // 
            // ButtonIptCopyList
            // 
            this.ButtonIptCopyList.Location = new System.Drawing.Point(470, 195);
            this.ButtonIptCopyList.Name = "ButtonIptCopyList";
            this.ButtonIptCopyList.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptCopyList.TabIndex = 6;
            this.ButtonIptCopyList.Text = "Copy bodies\r\nto clipboard";
            this.ButtonIptCopyList.UseVisualStyleBackColor = true;
            this.ButtonIptCopyList.Click += new System.EventHandler(this.ButtonIptCopyList_Click);
            // 
            // ButtonIptClearList
            // 
            this.ButtonIptClearList.Location = new System.Drawing.Point(323, 195);
            this.ButtonIptClearList.Name = "ButtonIptClearList";
            this.ButtonIptClearList.Size = new System.Drawing.Size(134, 52);
            this.ButtonIptClearList.TabIndex = 5;
            this.ButtonIptClearList.Text = "Clear ALL\r\nlists/groups";
            this.ButtonIptClearList.UseVisualStyleBackColor = true;
            this.ButtonIptClearList.Click += new System.EventHandler(this.ButtonIptClearList_Click);
            // 
            // listBoxIptBodies
            // 
            this.listBoxIptBodies.FormattingEnabled = true;
            this.listBoxIptBodies.HorizontalScrollbar = true;
            this.listBoxIptBodies.ItemHeight = 16;
            this.listBoxIptBodies.Location = new System.Drawing.Point(323, 50);
            this.listBoxIptBodies.Name = "listBoxIptBodies";
            this.listBoxIptBodies.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxIptBodies.Size = new System.Drawing.Size(820, 132);
            this.listBoxIptBodies.TabIndex = 4;
            this.listBoxIptBodies.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxIptBodies_KeyDown);
            // 
            // labelIptInfo
            // 
            this.labelIptInfo.AutoSize = true;
            this.labelIptInfo.Location = new System.Drawing.Point(28, 23);
            this.labelIptInfo.Name = "labelIptInfo";
            this.labelIptInfo.Size = new System.Drawing.Size(245, 160);
            this.labelIptInfo.TabIndex = 3;
            this.labelIptInfo.Text = "Part mode (.ipt)\r\n1) Add selected: selected bodies.\r\n2) Select Frame: drag window in Inventor.\r\n3) Toggle grouped bodies.\r\n\r\nRight side has editable lists:\r\nBodies + Features + Browser tree.\r\n\r\nSave tree JSON to transfer:\r\nIPT file + tree file.";
            // 
            // ButtonIptToggle
            // 
            this.ButtonIptToggle.Location = new System.Drawing.Point(58, 335);
            this.ButtonIptToggle.Name = "ButtonIptToggle";
            this.ButtonIptToggle.Size = new System.Drawing.Size(186, 56);
            this.ButtonIptToggle.TabIndex = 2;
            this.ButtonIptToggle.Text = "Hide or Show\r\nBodies in Group";
            this.ButtonIptToggle.UseVisualStyleBackColor = true;
            this.ButtonIptToggle.Click += new System.EventHandler(this.ButtonIptToggle_Click);
            // 
            // ButtonIptAddInsideBox
            // 
            this.ButtonIptAddInsideBox.Location = new System.Drawing.Point(58, 245);
            this.ButtonIptAddInsideBox.Name = "ButtonIptAddInsideBox";
            this.ButtonIptAddInsideBox.Size = new System.Drawing.Size(186, 58);
            this.ButtonIptAddInsideBox.TabIndex = 1;
            this.ButtonIptAddInsideBox.Text = "Select Frame + Add\r\nInner/Hidden Bodies";
            this.ButtonIptAddInsideBox.UseVisualStyleBackColor = true;
            this.ButtonIptAddInsideBox.Click += new System.EventHandler(this.ButtonIptAddInsideBox_Click);
            // 
            // ButtonIptAdd
            // 
            this.ButtonIptAdd.Location = new System.Drawing.Point(58, 180);
            this.ButtonIptAdd.Name = "ButtonIptAdd";
            this.ButtonIptAdd.Size = new System.Drawing.Size(186, 50);
            this.ButtonIptAdd.TabIndex = 0;
            this.ButtonIptAdd.Text = "Add selected\r\nBodies to Group";
            this.ButtonIptAdd.UseVisualStyleBackColor = true;
            this.ButtonIptAdd.Click += new System.EventHandler(this.ButtonIptAdd_Click);
            // 
            // tabPageIam
            // 
            this.tabPageIam.Controls.Add(this.labelIamInfo);
            this.tabPageIam.Controls.Add(this.Button2);
            this.tabPageIam.Controls.Add(this.Button1);
            this.tabPageIam.Location = new System.Drawing.Point(4, 25);
            this.tabPageIam.Name = "tabPageIam";
            this.tabPageIam.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageIam.Size = new System.Drawing.Size(1176, 792);
            this.tabPageIam.TabIndex = 1;
            this.tabPageIam.Text = ".iam";
            this.tabPageIam.UseVisualStyleBackColor = true;
            // 
            // labelIamInfo
            // 
            this.labelIamInfo.AutoSize = true;
            this.labelIamInfo.Location = new System.Drawing.Point(28, 23);
            this.labelIamInfo.Name = "labelIamInfo";
            this.labelIamInfo.Size = new System.Drawing.Size(305, 48);
            this.labelIamInfo.TabIndex = 2;
            this.labelIamInfo.Text = "Assembly mode (.iam)\r\nThis is the original Lesson 5 component code.\r\nSelect a component or subassembly.";
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(99, 164);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(186, 56);
            this.Button2.TabIndex = 1;
            this.Button2.Text = "Hide or Show\r\nComponents in Group";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(99, 91);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(186, 58);
            this.Button1.TabIndex = 0;
            this.Button1.Text = "Add selected\r\nComponents to Group";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 821);
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(1200, 860);
            this.Name = "Form1";
            this.Text = "Inventor external tool";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPageIpt.ResumeLayout(false);
            this.tabPageIpt.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewIptBrowserTree)).EndInit();
            this.tabPageIam.ResumeLayout(false);
            this.tabPageIam.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageIpt;
        private System.Windows.Forms.TabPage tabPageIam;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Button Button2;
        private System.Windows.Forms.Button ButtonIptAdd;
        private System.Windows.Forms.Button ButtonIptAddInsideBox;
        private System.Windows.Forms.Button ButtonIptToggle;
        private System.Windows.Forms.Label labelIptInfo;
        private System.Windows.Forms.Label labelIamInfo;
        private System.Windows.Forms.ListBox listBoxIptBodies;
        private System.Windows.Forms.Button ButtonIptClearList;
        private System.Windows.Forms.Button ButtonIptCopyList;
        private System.Windows.Forms.Button ButtonIptRemoveSelected;
        private System.Windows.Forms.Label labelIptGroupList;
        private System.Windows.Forms.ListBox listBoxIptFeatures;
        private System.Windows.Forms.Button ButtonIptClearFeatureList;
        private System.Windows.Forms.Button ButtonIptCopyFeatureList;
        private System.Windows.Forms.Button ButtonIptRemoveSelectedFeature;
        private System.Windows.Forms.Label labelIptFeatureList;
        private System.Windows.Forms.Button ButtonIptCreateBodyFolder;
        private System.Windows.Forms.Button ButtonIptCreateFeatureFolder;
        private System.Windows.Forms.Button ButtonIptCreateFeatureFolder2;
        private System.Windows.Forms.Label labelIptBrowserTree;
        private System.Windows.Forms.DataGridView dataGridViewIptBrowserTree;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIptTreeDepth;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIptTreeType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIptTreeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIptTreeX;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIptTreeY;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIptTreeZ;
        private System.Windows.Forms.Button ButtonIptRefreshBrowserTree;
        private System.Windows.Forms.Button ButtonIptRefreshBrowserTreeSpatialBase;
        private System.Windows.Forms.Button ButtonIptBuildVisibleListTree;
        private System.Windows.Forms.Button ButtonIptCopyBrowserTree;
        private System.Windows.Forms.Button ButtonIptSaveBrowserTree;
        private System.Windows.Forms.Button ButtonIptApplyBrowserTreeEdits;
    }
}
