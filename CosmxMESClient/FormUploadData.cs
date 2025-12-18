using HslCommunication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CosmxMESClient
{
    public partial class FormUploadData : Form
    {
        #region 属性 / 字段
        private UploadDataConfig _config = new UploadDataConfig();

        private Dictionary<string, string> _cachePLCScanAddress = new Dictionary<string, string>();
        #endregion

        #region 构造 / 初始化

        public FormUploadData()
        {
            InitializeComponent();
        }

        private void FormUploadData_Load(object sender, EventArgs e)
        {
            // 枚举绑定
            cboType.DataSource = Enum.GetValues(typeof(ParameterType));

            // 事件
            dgvUploadItems.SelectionChanged += DgvUploadItems_SelectionChanged;
            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnImport.Click += BtnImport_Click;
            btnExport.Click += BtnExport_Click;
            btnSave.Click += BtnSave_Click;
            cboPLCScanAddress.SelectedIndexChanged += new System.EventHandler(this.cboPLCScanAddress_SelectedIndexChanged);

            // 加载配置
            _config = UploadDataConfigManager.GetConfig();
            ConfigToUI();

            // 异步加载已经配置好的PLC扫描地址
            cboPLCScanAddress.Items.Clear();
            _cachePLCScanAddress.Clear();
            this.BeginInvoke(new Action(() =>
            {
                foreach (var item in GlobalVariables.AllAvailableTriggerAddresses)
                {
                    cboPLCScanAddress.Items.Add(item.Value.Key + "|" + item.Value.Description);
                    _cachePLCScanAddress[item.Value.Key] = item.Value.Key + "|" + item.Value.Description;
                } 
            }));



        }

        #endregion


        #region 配置操作 UI <-> Config

        private bool UIToConfig()
        {
            var dict = new Dictionary<string, UploadItem>();

            foreach (DataGridViewRow row in dgvUploadItems.Rows)
            {
                string name = Convert.ToString(row.Cells[colParameterName.Name].Value)?.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("参数名不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (dict.ContainsKey(name))
                {
                    MessageBox.Show($"参数名重复：{name}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                ParameterType type = (ParameterType)row.Cells[colType.Name].Value;
                int? length = null;
                if (type == ParameterType.字符串String)
                {
                    if (!int.TryParse(Convert.ToString(row.Cells[colLength.Name].Value), out int len))
                    {
                        MessageBox.Show($"参数 {name} 为字符串时长度必填", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    length = len;
                }
               
                dict[name] = new UploadItem
                {
                    ParameterName = name,
                    IsUpload = Convert.ToBoolean(row.Cells[colIsUpload.Name].Value ?? false),
                    Description = Convert.ToString(row.Cells[colDescription.Name].Value),
                    Type = type,
                    IsNullable = Convert.ToBoolean(row.Cells[colIsNullable.Name].Value ?? false),
                    Length = length,
                    ParameterValue = Convert.ToString(row.Cells[colFormatExample.Name].Value),
                    PLCScanAddressKey = Convert.ToString(row.Cells[PLCScanAddress.Name].Value),
                    BindPLCScanAddress = (GlobalVariables.AllAvailableTriggerAddresses.FirstOrDefault(p => p.Key == row.Cells[PLCScanAddress.Name].Value.ToString().Split('|')[0]).Value) == null ? 
                    null : 
                    (GlobalVariables.AllAvailableTriggerAddresses.FirstOrDefault
                    (p => p.Key == row.Cells[PLCScanAddress.Name].Value.ToString().Split('|')[0]).Value)
                };
            }

            _config.UploadItems = dict;

            _config.EnableMESUploadData = chkEnableMES.Checked;
            return true;
        }

        private void ConfigToUI()
        {
            dgvUploadItems.Rows.Clear();
            chkEnableMES.Checked = _config.EnableMESUploadData;

            foreach (var item in _config.UploadItems.Values)
            {
                dgvUploadItems.Rows.Add(
                    item.ParameterName,
                    item.IsUpload,
                    item.Description,
                    item.Type,
                    item.IsNullable,
                    item.Length,
                    item.ParameterValue,
                    item.PLCScanAddressKey);
            }
        }

        #endregion

        #region DataGridView <-> 右侧同步

        private void DgvUploadItems_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUploadItems.CurrentRow == null)
            {
                ClearDetailPanel(); // 可选：无选中时清空右侧
                return;
            }

            var row = dgvUploadItems.CurrentRow;

            txtParameterName.Text = Convert.ToString(row.Cells[colParameterName.Name].Value);
            chkIsUpload.Checked = Convert.ToBoolean(row.Cells[colIsUpload.Name].Value ?? false);
            txtDescription.Text = Convert.ToString(row.Cells[colDescription.Name].Value);
            cboType.SelectedItem = row.Cells[colType.Name].Value;
            chkIsNullable.Checked = Convert.ToBoolean(row.Cells[colIsNullable.Name].Value ?? false);
            txtLength.Text = Convert.ToString(row.Cells[colLength.Name].Value);
            txtParameterValue.Text = Convert.ToString(row.Cells[colFormatExample.Name].Value);
           // var cc= _cachePLCScanAddress.FirstOrDefault(p => p.Value.Contains(row.Cells[PLCScanAddress.Name].Value.ToString())).Value;
            cboPLCScanAddress.Text = _cachePLCScanAddress.FirstOrDefault(p => p.Value.Contains(row.Cells[PLCScanAddress.Name].Value.ToString())).Value;

            //cboPLCScanAddress.SelectedValue = (cboPLCScanAddress.Items.GetEnumerator()).;//. FirstOrDefault(p=>p.Key.Contains(row.Cells[PLCScanAddress.Name].Value.ToString()));

        }

        private void ClearDetailPanel()
        {
            txtParameterName.Text = "";
            chkIsUpload.Checked = true;
            txtDescription.Text = "";
            cboType.SelectedIndex = 0;
            chkIsNullable.Checked = false;
            txtLength.Text = "";
            txtParameterValue.Text = "";
        }

        #endregion

        #region 按钮逻辑

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string key = txtParameterName.Text.Trim();
            if (string.IsNullOrEmpty(key))
            {
                MessageBox.Show("请先填写参数名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in dgvUploadItems.Rows)
            {
                if (Convert.ToString(row.Cells[colParameterName.Name].Value) == key)
                {
                    MessageBox.Show($"参数名已存在：{key}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            int rowIndex = dgvUploadItems.Rows.Add(
                key,
                chkIsUpload.Checked,
                txtDescription.Text.Trim(),
                cboType.SelectedItem,
                chkIsNullable.Checked,
                txtLength.Text.Trim(),
                txtParameterValue.Text.Trim());

            dgvUploadItems.CurrentCell = dgvUploadItems.Rows[rowIndex].Cells[0];
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUploadItems.CurrentRow == null) return;

            if (MessageBox.Show("确定删除当前选中项？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            dgvUploadItems.Rows.Remove(dgvUploadItems.CurrentRow);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (dgvUploadItems.CurrentRow != null)
            {
                ApplyDetailToCurrentRow();
            }

            if (!UIToConfig()) return;

            // 改为通过管理器保存
            UploadDataConfigManager.SaveConfig(_config);
            MessageBox.Show("配置已保存", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv",
                FileName = "UploadDataConfig.csv"
            })
            {
                if (sfd.ShowDialog() != DialogResult.OK) return;
                ExportCsv(sfd.FileName);
                MessageBox.Show("CSV 导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void BtnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "CSV 文件 (*.csv)|*.csv"
            })
            {
                if (ofd.ShowDialog() != DialogResult.OK) return;
                ImportCsv(ofd.FileName);
            }
        }

        #endregion

        #region 成员
        private void ApplyDetailToCurrentRow()
        {
            if (dgvUploadItems.CurrentRow == null) return;

            var row = dgvUploadItems.CurrentRow;

            string newName = txtParameterName.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("参数名不能为空", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow r in dgvUploadItems.Rows)
            {
                if (r == row) continue;
                if (Convert.ToString(r.Cells[colParameterName.Name].Value)?.Trim() == newName)
                {
                    MessageBox.Show($"参数名已存在：{newName}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            row.Cells[colParameterName.Name].Value = newName;
            row.Cells[colIsUpload.Name].Value = chkIsUpload.Checked;
            row.Cells[colDescription.Name].Value = txtDescription.Text.Trim();
            row.Cells[colType.Name].Value = cboType.SelectedItem;
            row.Cells[colIsNullable.Name].Value = chkIsNullable.Checked;
            row.Cells[colLength.Name].Value = txtLength.Text.Trim();
            row.Cells[colFormatExample.Name].Value = txtParameterValue.Text.Trim();
            row.Cells[PLCScanAddress.Name].Value = cboPLCScanAddress.Text.Trim();
        }
        #endregion


        #region 数据导入导出
        private void ExportCsv(string filePath)
        {
            if (!UIToConfig()) return;


            var sb = new StringBuilder();
            sb.AppendLine("参数名称,是否上传,参数描述,参数类型,是否可为空,长度,参数值,PLC扫描地址");


            foreach (var item in _config.UploadItems.Values)
            {
                sb.AppendLine(string.Join(",",
                Escape(item.ParameterName),
                item.IsUpload,
                Escape(item.Description),
                item.Type,
                item.IsNullable,
                item.Length?.ToString() ?? string.Empty,
                Escape(item.ParameterValue),
                Escape(item.PLCScanAddressKey)
                ));
            }
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private void ImportCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            if (lines.Length <= 1)
            {
                MessageBox.Show("CSV 文件无数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            dgvUploadItems.Rows.Clear();
            _config.UploadItems.Clear();


            for (int i = 1; i < lines.Length; i++)
            {
                var cols = ParseCsvLine(lines[i]);
                if (cols.Length < 8) continue;


                dgvUploadItems.Rows.Add(
                cols[0],
                bool.Parse(cols[1]),
                cols[2],
                Enum.Parse(typeof(ParameterType), cols[3]),
                bool.Parse(cols[4]),
                cols[5],
                cols[6],
                cols[7]
                );
            }
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(",") || value.Contains("\"") || value.Contains(""))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;


            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }

        #endregion

        private void cboPLCScanAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            var row = dgvUploadItems.CurrentRow;
            row.Cells[PLCScanAddress.Name].Value = cboPLCScanAddress.Text.Trim();
            
        }
    }
}
