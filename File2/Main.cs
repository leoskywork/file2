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
        private bool _AggregateTargetSetByUser = false;

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
                _AggregateTargetSetByUser = true;
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
                    this.labelAggregateMessage.Text = "Please select a targe folder";
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateSource.Text))
                {
                    this.labelAggregateMessage.Text = "Invalid source folder";
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateTarget.Text))
                {
                    this.labelAggregateMessage.Text = "Invalid target folder";
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
                    if (MessageBox.Show($"Are you sure to aggregate the entire {this.textBoxAggregateSource.Text} dirve?",
                        this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.OK)
                    {
                        this.buttonAggregateGo.Enabled = true;
                        return;
                    }
                }

                this.labelAggregateMessage.Text = "Moving...";
                this.groupBoxAggregate.Enabled = false;

                var task = new Task<string>(() =>
                {
                    string message = AggregateFile(this.textBoxAggregateSource.Text, this.textBoxAggregateTarget.Text,
                   (progressInfo) => UpdateProgressMessage(progressInfo));
                    return message;
                });

                task.ContinueWith((_) =>
                {
                    this.labelAggregateMessage.Text = task.Result;
                    this.groupBoxAggregate.Enabled = true;
                    this.buttonAggregateGo.Enabled = true;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                task.Start();

            }
            catch (Exception ex)
            {
                this.labelAggregateMessage.Text = "Ooops! " + ex.Message;
                this.groupBoxAggregate.Enabled = true;
                this.buttonAggregateGo.Enabled = true;
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

        private void TextBoxAggregateSource_TextChanged(object sender, EventArgs e)
        {
            this.labelAggregateMessage.Text = null;
            this.SetDefaultAggregateTarget();

            if (string.IsNullOrWhiteSpace(this.textBoxAggregateSource.Text))
            {
                this.buttonAggregateGo.Enabled = false;

            }
            else
            {
                this.buttonAggregateGo.Enabled = true;
            }
        }

        private void TextBoxAggregateTarget_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.textBoxAggregateTarget.Text))
            {
               this.buttonAggregateGo.Enabled = false;
            }
        }



        private void SetDefaultAggregateTarget()
        {
            if (!_AggregateTargetSetByUser)
            {
                this.textBoxAggregateTarget.Text = this.textBoxAggregateSource.Text;
            }
        }

        private string AggregateFile(string sourceFolder, string targetFolder, Action<string> progress)
        {
            int filesMoved = 0;
            int filesMoveFiled = 0;
            int filesSkipped = 0;
            int maxFileNameLength = 30;

            var files = Directory.EnumerateFiles(sourceFolder, "*.*", SearchOption.AllDirectories).ToList();
            files.Sort();

            foreach (var file in files)
            {
                var count = filesMoved + filesMoveFiled + filesSkipped;
                var name = Path.GetFileName(file);
                var nameInfo = name.Length > maxFileNameLength ? name.Substring(0, maxFileNameLength) : name;
                progress($"moving {count + 1}/{files.Count} ({nameInfo})...");
                string targetFile = Path.Combine(targetFolder, Path.GetFileName(file));

                if (File.Exists(targetFile))
                {
                    filesSkipped++;
                }
                else
                {
                    try
                    {
                        File.Move(file, targetFile);
                        filesMoved++;
                    }
                    catch (Exception)
                    {
                        filesMoveFiled++;
                    }
                }
            }

            return $"Files moved: {filesMoved}{(filesMoveFiled > 0 ? ", failed: " + filesMoveFiled : "")}{(filesSkipped > 0 ? ", files skipped: " + filesSkipped : "")}";
        }

    }
}
