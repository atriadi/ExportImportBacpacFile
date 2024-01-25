using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ExportImportBacpacFile
{
    public static class MessageHelper
    {
        public static string MessageAppTitle = "Export Import Bacpac";
        public static void ShowMessageError(Form owner, Exception fException, bool fLogError = false)
        {
            WaitFormClose();
            if (owner == null)
            {
                XtraMessageBox.Show(fException.Message, MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Form tf = Application.OpenForms.OfType<Form>().Where((t) => t.TopMost).FirstOrDefault();
                XtraMessageBox.Show(tf ?? (owner), fException.Message, MessageAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (fLogError == true)
            {
            }
        }
        public static void WaitFormShow(Form parentform, string caption = "")
        {
            // Open Wait Form
            SplashScreenManager.ShowForm(parentform, typeof(BaseWaitForm), true, true, false);
            if (caption != "")
                SplashScreenManager.Default.SendCommand(BaseWaitForm.WaitFormCommand.SetCaption, caption);
            else
                SplashScreenManager.Default.SendCommand(BaseWaitForm.WaitFormCommand.SetCaption, "Silahkan untuk menunggu saat memuat data dari server");
            //System.Threading.Thread.Sleep(100);
        }
        public static void WaitFormClose(Form parentform)
        {
            // Close Wait Form
            SplashScreenManager.CloseForm(false, 0, parentform);
        }
        public static void WaitFormClose()
        {
            // Close Wait Form
            SplashScreenManager.CloseForm(false);
        }

        public static void UpdateProgressWaitFormShow(string caption = "", string description = "")
        {
            if (!string.IsNullOrEmpty(caption))
                SplashScreenManager.Default?.SetWaitFormCaption(caption);

            if (!string.IsNullOrEmpty(description))
                SplashScreenManager.Default?.SetWaitFormDescription(description);
        }
    }
}
