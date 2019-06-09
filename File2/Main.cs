using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using File2.Common;

namespace File2
{
    public partial class Main : Form
    {
        private ILanguage _language;
        private bool _syncAggregateSourceWithTarget = true;
        private bool _aggregateTargetChangedByAutoSync = false;
        private FileTool _fileTool = new FileTool();
        private string _aggregateTaskKey;
        private TimeRange _aggregateTimes;

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
            this.timerProgress.Interval = 1000;
            this.timerProgress.Tick += TimerProgress_Tick;
            this.Text = Constants.AppName;
            //this.labelAggregateMessage.Text = "xxxxx xxx1xxxxxxxxxx2xxxxxxx3xxxxx34xxxxx5xxx6xxxx7xx9";
        }

        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            UpdateUIStopwatch();
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
                this.buttonAggregateAbort.Enabled = true;
                var source = this.textBoxAggregateSource.Text;
                var target = this.textBoxAggregateTarget.Text;

                var fileTask = _fileTool.AggregateFileAsync(source, target, (message) => UpdateUIAggregateMessage(message));
                _aggregateTaskKey = fileTask.Key;

                fileTask.Task.ContinueWith((_) =>
                {
                    _aggregateTimes.End = DateTime.UtcNow;
                    this.labelAggregateMessage.Text = fileTask.Task.Result;
                    this.groupBoxAggregate.Enabled = true;
                    this.buttonAggregateGo.Enabled = true;
                    this.buttonAggregateCancel.Enabled = false;
                    this.buttonAggregateAbort.Enabled = false;
                    _fileTool.Cleanup(_aggregateTaskKey);
                }, TaskScheduler.FromCurrentSynchronizationContext());

                fileTask.Task.Start();
                _aggregateTimes = new TimeRange();
                this.timerProgress.Start();
                UpdateUIStopwatch();
            }
            catch (Exception ex)
            {
                _aggregateTimes.End = DateTime.UtcNow;
                this.labelAggregateMessage.Text = "Oops! " + ex.Message;
                this.groupBoxAggregate.Enabled = true;
                this.buttonAggregateGo.Enabled = true;
                this.buttonAggregateCancel.Enabled = false;
                this.buttonAggregateAbort.Enabled = false;
            }
        }

        private void TextBoxAggregateSource_TextChanged(object sender, EventArgs e)
        {
            this.labelAggregateMessage.Text = null;
            this.SetDefaultAggregateTarget();
            this.buttonAggregateGo.Enabled = !string.IsNullOrWhiteSpace(this.textBoxAggregateSource.Text);
            this._aggregateTimes = null;
        }

        private void TextBoxAggregateTarget_TextChanged(object sender, EventArgs e)
        {
            this.buttonAggregateGo.Enabled = !string.IsNullOrWhiteSpace(this.textBoxAggregateTarget.Text);

            if (!_aggregateTargetChangedByAutoSync)
            {
                _syncAggregateSourceWithTarget = false;
            }

            this._aggregateTimes = null;
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

        private void UpdateUIAggregateMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateUIAggregateMessage), message);
            }
            else
            {
                this.labelAggregateMessage.Text = message;
            }
        }

        private void UpdateUIStopwatch()
        {
            string aggregatingSpent = null;
            if (_aggregateTimes != null)
            {
                aggregatingSpent = (_aggregateTimes.GetCancellationPeriod() ?? _aggregateTimes.GetPeriod()).ToFriendlyString();
            }

            this.Text = Constants.AppName + (aggregatingSpent != null ? (_aggregateTimes.Canceled ? " - aggr cxl " : " - aggregating ") + aggregatingSpent : "");
        }

        private void ButtonAggregateCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.buttonAggregateCancel.Enabled = false;
                this.buttonAggregateAbort.Enabled = false;
                if (_aggregateTaskKey != null)
                {
                    _aggregateTimes.CancellationTime = DateTime.UtcNow;
                    _fileTool.Cancel(_aggregateTaskKey);
                    _aggregateTaskKey = null;
                    this.labelAggregateMessage.Text = "Task will be canceled once current moving finished...";
                }
            }
            catch (Exception ex)
            {
                this.labelAggregateMessage.Text = "Oops! Got an error when canceling: " + ex.Message;
            }
        }

        private void ButtonAggregateAbort_Click(object sender, EventArgs e)
        {
            try
            {
                this.buttonAggregateAbort.Enabled = false;
                this.buttonAggregateCancel.Enabled = false;
                if (_aggregateTaskKey != null)
                {
                    _aggregateTimes.End = DateTime.UtcNow;
                    _fileTool.Abort(_aggregateTaskKey);
                    _aggregateTaskKey = null;
                    this.labelAggregateMessage.Text = "Aborting current moving...";
                }
            }
            catch (Exception ex)
            {
                this.labelAggregateMessage.Text = "Oops! Got an error when canceling: " + ex.Message;
            }
        }
    }
}
