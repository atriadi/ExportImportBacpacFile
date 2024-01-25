using DevExpress.XtraEditors;
using Microsoft.SqlServer.Dac;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace ExportImportBacpacFile
{
    public partial class frmSetting : DevExpress.XtraEditors.XtraForm
    {
        public frmSetting()
        {
            InitializeComponent();

            ServerNameTextEdit.Properties.NullText = "BGALT-INFRA363\\SQLEXPRESS";
            UsernameTextEdit.Properties.NullText = "sa";
            PasswordTextEdit.Properties.NullText = "P@ssw0rd";
            ImportBacpacFilePathTextEdit.Properties.NullText = @"D:\RestoreSpartaQAS20240124.bacpac";
            ExportBacpacFilePathTextEdit.Properties.NullText = @"D:\spartaQAS20240124.bacpac";
            TargetDatabaseNameTextEdit.Properties.NullText = "SpartaRestoreQAS20240124";
            SourceDatabaseNameTextEdit.Properties.NullText = "SpartaQAS";

            btnTestConnection.Click += BtnTestConnection_Click;
            btnImport.Click += BtnImport_Click;
            btnExport.Click += BtnExport_Click;
            chbxIntegratedWindows.CheckedChanged += ChbxIntegratedWindows_CheckedChanged;
            SourceDatabaseNameTextEdit.TextChanged += SourceDatabaseNameTextEdit_TextChanged;

            PasswordTextEdit.ButtonClick += PasswordTextEdit_ButtonClick;
        }

        private void SourceDatabaseNameTextEdit_TextChanged(object sender, EventArgs e)
        {
            var value = SourceDatabaseNameTextEdit.Text;
            var date = DateTime.Now.ToString("yyyyMMdd");

            ExportBacpacFilePathTextEdit.Text = $@"D:\{value}{date}.bacpac";
        }

        private void PasswordTextEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (PasswordTextEdit.Properties.UseSystemPasswordChar == true)
            {
                PasswordTextEdit.Properties.UseSystemPasswordChar = false;
            }
            else
            {
                PasswordTextEdit.Properties.UseSystemPasswordChar = true;
            }
        }

        private void ChbxIntegratedWindows_CheckedChanged(object sender, EventArgs e)
        {
            if (!chbxIntegratedWindows.Checked)
            {
                this.UsernameTextEdit.Enabled = true;
                this.PasswordTextEdit.Enabled = true;
            }
            else
            {
                this.UsernameTextEdit.Enabled = false;
                this.PasswordTextEdit.Enabled = false;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (ExportBacpacFilePathTextEdit.EditValue != null)
            {
                if (!IsServerConnected())
                {
                    XtraMessageBox.Show("Failed Connected Server Database", MessageHelper.MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool success = false;
                MessageHelper.WaitFormShow(this);
                try
                {
                    SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
                    csb.DataSource = ServerNameTextEdit.EditValue.ToString();
                    csb.UserID = UsernameTextEdit.EditValue.ToString();
                    csb.Password = PasswordTextEdit.EditValue.ToString();

                    var local = new DacServices(csb.ConnectionString);
                    local.ProgressChanged += ServiceOnProgressChanged;
                    local.ExportBacpac(ExportBacpacFilePathTextEdit.EditValue.ToString(), SourceDatabaseNameTextEdit.EditValue.ToString());
                    success = true;
                }
                catch (Exception ex)
                {
                    MessageHelper.ShowMessageError(this, ex);
                }
                finally
                {
                    MessageHelper.WaitFormClose();

                    if (success)
                        XtraMessageBox.Show("Success Export", MessageHelper.MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (ImportBacpacFilePathTextEdit.EditValue != null)
            {
                if (!IsServerConnected())
                {
                    XtraMessageBox.Show("Failed Connected Server Database", MessageHelper.MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool success = false;
                MessageHelper.WaitFormShow(this);
                try
                {
                    using (BacPackage bacPackage = BacPackage.Load(Path.Combine(ImportBacpacFilePathTextEdit.EditValue.ToString())))
                    {
                        SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
                        csb.DataSource = ServerNameTextEdit.EditValue.ToString();
                        csb.UserID = UsernameTextEdit.EditValue.ToString();
                        csb.Password = PasswordTextEdit.EditValue.ToString();

                        var local = new DacServices(csb.ConnectionString);
                        local.ProgressChanged += ServiceOnProgressChanged;
                        local.ImportBacpac(bacPackage, TargetDatabaseNameTextEdit.EditValue.ToString());
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    MessageHelper.ShowMessageError(this, ex);
                }
                finally
                {
                    MessageHelper.WaitFormClose();
                    if (success)
                        XtraMessageBox.Show("Success Import", MessageHelper.MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public bool IsServerConnected()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = ServerNameTextEdit.EditValue.ToString();
            csb.UserID = UsernameTextEdit.EditValue.ToString();
            csb.Password = PasswordTextEdit.EditValue.ToString();
            using (var l_oConnection = new SqlConnection(csb.ConnectionString))
            {
                try
                {
                    l_oConnection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            if (IsServerConnected())
                XtraMessageBox.Show("Success Connected Server Database", MessageHelper.MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                XtraMessageBox.Show("Failed Connected Server Database", MessageHelper.MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void ServiceOnProgressChanged(object sender, DacProgressEventArgs dacProgressEventArgs)
        {
            MessageHelper.UpdateProgressWaitFormShow("", $"{dacProgressEventArgs.Status}{Environment.NewLine}{dacProgressEventArgs.Message}");
            Console.WriteLine("{0} {1}", dacProgressEventArgs.Message, dacProgressEventArgs.Status);
        }
    }
}
