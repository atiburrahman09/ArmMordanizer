namespace ARMMordanizerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ARMMordanizerProcess = new System.ServiceProcess.ServiceProcessInstaller();
            this.ARMMordanizer = new System.ServiceProcess.ServiceInstaller();
            // 
            // ARMMordanizerProcess
            // 
            this.ARMMordanizerProcess.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.ARMMordanizerProcess.Password = null;
            this.ARMMordanizerProcess.Username = null;
            // 
            // ARMMordanizer
            // 
            this.ARMMordanizer.ServiceName = "ARM File Upload";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ARMMordanizerProcess,
            this.ARMMordanizer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ARMMordanizerProcess;
        private System.ServiceProcess.ServiceInstaller ARMMordanizer;
    }
}