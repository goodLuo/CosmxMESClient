using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;

namespace CosmxMESClient {
    public partial class FormPLCScanAddress:Form {
        public PLCScanAddress ScanAddress {
            get; private set;
            }

        public FormPLCScanAddress( ) : this(new PLCScanAddress( )) { }

        public FormPLCScanAddress( PLCScanAddress existingAddress ) {
            InitializeComponent( );
            ScanAddress=existingAddress;
            LoadFormData( );
            }

        private void LoadFormData( ) {
            txtAddress.Text=ScanAddress.Address;
            txtDescription.Text=ScanAddress.Description;
            numInterval.Value=ScanAddress.ReadInterval;
            chkEnabled.Checked=ScanAddress.IsEnabled;
            numPower.Value=ScanAddress.Power;
            numLength.Value=ScanAddress.Length;

            // 设置数据类型
            if (ScanAddress.DataType!=null) {
                foreach (var item in cmbDataType.Items) {
                    if (item is Type type&&type==ScanAddress.DataType) {
                        cmbDataType.SelectedItem=item;
                        break;
                        }
                    }
                }
            }

        private void FormPLCScanAddress_Load( object sender,EventArgs e ) {
            // 初始化数据类型下拉框
            cmbDataType.Items.Clear( );
            cmbDataType.Items.AddRange(new object[]
            {
                typeof(int), typeof(float), typeof(double),
                typeof(bool), typeof(string)
            });
            cmbDataType.DisplayMember="Name";

            // 设置默认选择
            if (cmbDataType.Items.Count>0&&cmbDataType.SelectedIndex==-1) {
                cmbDataType.SelectedIndex=0;
                }
            }

        private void btnOK_Click( object sender,EventArgs e ) {
            if (ValidateForm( )) {
                ScanAddress.Address=txtAddress.Text.Trim( );
                ScanAddress.DataType=(Type) cmbDataType.SelectedItem;
                ScanAddress.Description=txtDescription.Text.Trim( );
                ScanAddress.ReadInterval=(int) numInterval.Value;
                ScanAddress.IsEnabled=chkEnabled.Checked;
                ScanAddress.Power=(int) numPower.Value;
                ScanAddress.Length=(int) numLength.Value;

                DialogResult=DialogResult.OK;
                Close( );
                }
            }

        private bool ValidateForm( ) {
            if (string.IsNullOrEmpty(txtAddress.Text.Trim( ))) {
                MessageBox.Show("请输入PLC地址","验证错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                txtAddress.Focus( );
                return false;
                }

            if (cmbDataType.SelectedItem==null) {
                MessageBox.Show("请选择数据类型","验证错误",
                    MessageBoxButtons.OK,MessageBoxIcon.Warning);
                cmbDataType.Focus( );
                return false;
                }

            return true;
            }

        private void btnCancel_Click( object sender,EventArgs e ) {
            DialogResult=DialogResult.Cancel;
            Close( );
            }
        }
    }
