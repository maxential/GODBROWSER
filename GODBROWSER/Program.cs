using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace DanTup.BrowserSelector
{
	class Program
	{
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 && (args[0] == "--register" || args[0] == "--unregister"))
            {
                EnsureAdmin("--register");
                return;
            }

            if (args.Length == 1 && args[0] == "--create")
            {
                CreateSampleSettings();
                return;
            }

            // Show GUI with registration button
            ShowRegistrationBox();
        }

        static void ShowRegistrationBox()
        {
            DialogResult result = MessageBox.Show("Click 'Register Browser' to set the browser as default.",
                                                  "Browser Registration", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.OK)
            {
                string selectedPath = ChooseExePath();

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    EnsureAdmin("--register");
                    RegistrySettings.RegisterBrowser(selectedPath);

                    // Show "Done!" message box
                    MessageBox.Show("Done!", "Registration Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        static string ChooseExePath()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable Files|*.exe";
                openFileDialog.Title = "Select the Browser Executable";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
                else
                {
                    return null;
                }
            }
        }


        static void EnsureAdmin(string arg)
		{
			WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = Assembly.GetExecutingAssembly().Location,
					Verb = "runas",
					Arguments = arg
				});
				Environment.Exit(0);
			}
		}



		static void CreateSampleSettings()
		{
			DialogResult r = DialogResult.Yes;

			if (File.Exists(ConfigReader.ConfigPath))
			{
				r = MessageBox.Show(@"The settings file already exists. Would you like to replace it with the sample file? (The existing file will be saved/renamed.)", "BrowserSelector", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
			}

			if (r == DialogResult.No)
				return;

			ConfigReader.CreateSampleIni();
		}
	}
}
