using System;
using System.IO;
using System.Linq;
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
            this.labelAggregateError.Text = null;
            this.folderBrowserDialogMain.ShowNewFolderButton = true;
            this.folderBrowserDialogMain.RootFolder = Environment.SpecialFolder.MyComputer;
        }

        private void ButtonAggregateSource_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialogMain.ShowDialog() == DialogResult.OK)
            {
                this.buttonAggregateGo.Enabled = true;
                this.textBoxAggregateSource.Text = this.folderBrowserDialogMain.SelectedPath;
                SetDefaultAggregateTarget();
            }
        }

        private void ButtonAggregateTarget_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialogMain.ShowDialog() == DialogResult.OK)
            {
                this.buttonAggregateGo.Enabled = true;
                this.textBoxAggregateTarget.Text = this.folderBrowserDialogMain.SelectedPath;
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
                    this.labelAggregateError.Text = "Please select a source folder";
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.textBoxAggregateTarget.Text))
                {
                    this.labelAggregateError.Text = "Please select a targe folder";
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateSource.Text))
                {
                    this.labelAggregateError.Text = "Invalid source folder";
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateTarget.Text))
                {
                    this.labelAggregateError.Text = "Invalid target folder";
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

                this.labelAggregateError.Text = "Moving...";
                string message = AggregateFile(this.textBoxAggregateSource.Text, this.textBoxAggregateTarget.Text);
                this.labelAggregateError.Text = message;
                this.buttonAggregateGo.Enabled = true;
            }
            catch (Exception ex)
            {
                this.labelAggregateError.Text = "Ooops! " + ex.Message;
                this.buttonAggregateGo.Enabled = true;
            }
        }

        private void TextBoxAggregateSource_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.textBoxAggregateSource.Text))
            {
                this.SetDefaultAggregateTarget();
            }
            else
            {
                this.buttonAggregateGo.Enabled = false;
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

        private string AggregateFile(string source, string target)
        {
            int filesMoved = 0;
            int filesMoveFiled = 0;
            int filesSkipped = 0;

            var files = Directory.EnumerateFiles(source, "*.*", SearchOption.AllDirectories).ToList();
            files.Sort();

            foreach (var file in files)
            {
                string targetFile = Path.Combine(target, Path.GetFileName(file));
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

            return $"Files moved: {filesMoved}, failed: {filesMoveFiled}, skipped: {filesSkipped}";
        }

    }
}
