using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File2
{
    public partial class Main : Form
    {
        private ILanguage _language;
        private bool _syncAggregateSourceWithTarget = true;
        private bool _aggregateTargetChangedByAutoSync = false;
        private FileTool _fileTool = new FileTool();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //todo init language by system culture
            _language = new LanguageEnglish("en");
            this.labelAggregateMessage.Text = null;
            this.folderBrowserDialogMain.ShowNewFolderButton = true;
            this.folderBrowserDialogMain.RootFolder = Environment.SpecialFolder.MyComputer;
            this.buttonAggregateGo.Enabled = false;
        }

        private void ButtonAggregateSource_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialogMain.ShowDialog() == DialogResult.OK)
            {
                this.buttonAggregateGo.Enabled = true;
                this.textBoxAggregateSource.Text = this.folderBrowserDialogMain.SelectedPath;
                this.labelAggregateMessage.Text = null;
                SetDefaultAggregateTarget();
            }
        }

        private void ButtonAggregateTarget_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialogMain.ShowDialog() == DialogResult.OK)
            {
                this.buttonAggregateGo.Enabled = true;
                this.textBoxAggregateTarget.Text = this.folderBrowserDialogMain.SelectedPath;
                this.labelAggregateMessage.Text = null;
                _syncAggregateSourceWithTarget = false;
            }
        }

        private void ButtonAggregateGo_Click(object sender, EventArgs e)
        {
            try
            {
                this.buttonAggregateGo.Enabled = false;

                if (string.IsNullOrWhiteSpace(this.textBoxAggregateSource.Text))
                {
                    this.labelAggregateMessage.Text = "Please select a source folder";
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.textBoxAggregateTarget.Text))
                {
                    this.labelAggregateMessage.Text = "Please select a target folder";
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateSource.Text))
                {
                    this.labelAggregateMessage.Text = "Source folder not exist";
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateTarget.Text))
                {
                    this.labelAggregateMessage.Text = "Target folder not exist";
                    return;
                }

                //warn if access system folders
                if (Constants.ImportantFolders.Any(f => this.textBoxAggregateSource.Text.StartsWith(f)))
                {
                    if (MessageBox.Show("You are going to move files that used by Windows system, are you sure?",
                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.OK)
                    {
                        this.buttonAggregateGo.Enabled = true;
                        return;
                    }
                }

                //warn if aggregate entire drive
                if ((new DirectoryInfo(this.textBoxAggregateSource.Text)).Parent == null)
                {
                    if (MessageBox.Show($"Are you sure to aggregate the entire {this.textBoxAggregateSource.Text} drive?",
                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.OK)
                    {
                        this.buttonAggregateGo.Enabled = true;
                        return;
                    }
                }

                this.groupBoxAggregate.Enabled = false;
                this.buttonAggregateCancel.Enabled = true;
                var source = this.textBoxAggregateSource.Text;
                var target = this.textBoxAggregateTarget.Text;

                //var task = new Task<string>(() => _FileTool.AggregateFile(source, target, (message) => UpdateProgressMessage(message)));
                var taskInfo = _fileTool.AggregateFileAsync(source, target, (message) => UpdateProgressMessage(message));

                taskInfo.Value.ContinueWith((_) =>
                {
                    this.labelAggregateMessage.Text = taskInfo.Value.Result;
                    this.groupBoxAggregate.Enabled = true;
                    this.buttonAggregateGo.Enabled = true;
                    this.buttonAggregateCancel.Enabled = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                taskInfo.Value.Start();
            }
            catch (Exception ex)
            {
                this.labelAggregateMessage.Text = "Oops! " + ex.Message;
                this.groupBoxAggregate.Enabled = true;
                this.buttonAggregateGo.Enabled = true;
                this.buttonAggregateCancel.Enabled = false;
            }
        }

        private void TextBoxAggregateSource_TextChanged(object sender, EventArgs e)
        {
            this.labelAggregateMessage.Text = null;
            this.SetDefaultAggregateTarget();
            this.buttonAggregateGo.Enabled = !string.IsNullOrWhiteSpace(this.textBoxAggregateSource.Text);
        }

        private void TextBoxAggregateTarget_TextChanged(object sender, EventArgs e)
        {
            this.buttonAggregateGo.Enabled = !string.IsNullOrWhiteSpace(this.textBoxAggregateTarget.Text);

            if (!_aggregateTargetChangedByAutoSync)
            {
                _syncAggregateSourceWithTarget = false;
            }
        }



        private void SetDefaultAggregateTarget()
        {
            if (_syncAggregateSourceWithTarget)
            {
                this._aggregateTargetChangedByAutoSync = true;
                this.textBoxAggregateTarget.Text = this.textBoxAggregateSource.Text;
                this._aggregateTargetChangedByAutoSync = false;
            }
        }

        private void UpdateProgressMessage(string progressInfo)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateProgressMessage), progressInfo);
            }
            else
            {
                this.labelAggregateMessage.Text = progressInfo;
            }
        }



        private void ButtonAggregateCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
