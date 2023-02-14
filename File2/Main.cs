﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private System.Collections.Concurrent.ConcurrentQueue<Tuple<string, DateTime>> _sizingMessages = new System.Collections.Concurrent.ConcurrentQueue<Tuple<string, DateTime>>();
        private ManualResetEvent _manualReset;


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
            this.timerProgress.Interval = Constants.ProgressUpdateRateInMS;
            this.timerProgress.Tick += TimerProgress_Tick;
            this.timerProgress.Start();
            this.Text = Constants.AppName;
            //this.labelAggregateMessage.Text = "xxxxx xxx1xxxxxxxxxx2xxxxxxx3xxxxx34xxxxx5xxx6xxxx7xx9";
            this.richTextBoxReadme.Text = "useful dir\nC:\\Users\n";
        }

        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            UpdateUIStopwatch();

            if (!_sizingMessages.IsEmpty)
            {
                Tuple<string, DateTime> first;
                if (_sizingMessages.TryDequeue(out first))
                {
                    UpdateUIWarningMessage(first.Item1);
                }

                var utcWithOffset = DateTime.UtcNow.AddMilliseconds(-Constants.UIMessageOffsetInMS); //only show most recent messages

                while (!_sizingMessages.IsEmpty)
                {
                    Tuple<string, DateTime> head;
                    if (_sizingMessages.TryPeek(out head))
                    {
                        if (head.Item2 < utcWithOffset)
                        {
                            _sizingMessages.TryDequeue(out _);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (_manualReset != null && _sizingMessages.IsEmpty)
            {
                UpdateUIWarningMessage("done");
                _manualReset.Set();
                _manualReset = null;
            }
        }

        private void ButtonAggregateSource_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.folderBrowserDialogMain.SelectedPath))
            {
                //todo: scroll to it, ref https://blog.csdn.net/weixin_43145361/article/details/91350783
            }

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

                if(this.textBoxAggregateSource.Text == this.textBoxAggregateTarget.Text)
                {
                    this.labelAggregateMessage.Text = "Target folder is the same as source";
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

                var fileTask = _fileTool.AggregateFileAsync(source, target, (message) => UpdateUIWarningMessage(message));
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

        private void UpdateUIWarningMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action<string>)UpdateUIWarningMessage, message);
            }
            else
            {
                this.labelAggregateMessage.Text = message;
                DebugLine("updating msg: " + message);
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

        private void buttonFolderInfo_Click(object sender, EventArgs e)
        {
            try
            {
                while(!_sizingMessages.IsEmpty)
                {
                    this._sizingMessages.TryDequeue(out _);
                }

                this.buttonFolderInfo.Enabled = false;
                this.checkBoxAutoOpen.Enabled = false;
                // UpdateUIWarningMessage("Please run with admin permission accordingly.");

                if (string.IsNullOrWhiteSpace(this.textBoxAggregateSource.Text))
                {
                    this.labelAggregateMessage.Text = "Please select a source folder";
                    this.buttonFolderInfo.Enabled = true;
                    this.checkBoxAutoOpen.Enabled = true;
                    return;
                }

                if (!Directory.Exists(this.textBoxAggregateSource.Text))
                {
                    this.labelAggregateMessage.Text = "Source folder not exist";
                    this.buttonFolderInfo.Enabled = true;
                    this.checkBoxAutoOpen.Enabled = true;
                    return;
                }

                var folder = this.textBoxAggregateSource.Text;
                var fileTask = new FileTool().GetSubFolderInfoAsync(folder, 0 * 1024 * 1024 , SizingProgress);

                fileTask.Task.ContinueWith(_ =>
                {
                    if (fileTask.Task.Exception != null)
                    {
                        //fixme: seems can not be catch by outside
                        //throw fileTask.Task.Exception;
                        MessageBox.Show("Error while sizing folder：" + string.Join(",", fileTask.Task.Exception.InnerExceptions.Select(ex => ex.Message)));
                    }
                    else
                    {
                        var timestamp = DateTime.Now.ToString("yyMMdd-HHmmss");
                        var lines = fileTask.Task.Result.FilesAndSizes.Select(r => r.Item1 + ", " + r.Item2.ToFriendlyFileSize()).ToList();
                        lines.Add(Environment.NewLine);
                        lines.Add("--------- source: " + folder);
                        lines.Add("--- file scanned: " + fileTask.Task.Result.FilesAndSizes.Count.ToString("#,###"));
                        lines.Add("----- total size: " + fileTask.Task.Result.FilesAndSizes.Sum(item => item.Item2).ToFriendlyFileSize());
                        lines.Add("-- error occured: " + fileTask.Task.Result.Errors.Count.ToString());

                        string resultPrefix = $"{Environment.CurrentDirectory}\\sizing-{timestamp}";
                        string resultFilePath = resultPrefix + ".txt";
                        string errorFilePath = resultPrefix + "-error.txt";

                        File.WriteAllLines(resultFilePath, lines);
                        bool hasError = fileTask.Task.Result.Errors.Count > 0;

                        if (hasError)
                        {
                            File.WriteAllLines(errorFilePath, fileTask.Task.Result.Errors.Select(ex => ex.Message));
                        }

                        DebugLine("going to show msg box, file count: " + fileTask.Task.Result.FilesAndSizes.Count);
                        if (_sizingMessages.Count > 0)
                        {
                            DebugLine("msg remaining: " + _sizingMessages.Count);
                        }


                        _manualReset = new ManualResetEvent(false);
                        System.Threading.ThreadPool.QueueUserWorkItem(__ =>
                        {
                            // not working, use ManualResetEvent instead
                            // while(_sizingMessages.Count > 0)
                            // {
                            // DebugLine("waiting, msg remaining: " + _sizingMessages.Count);
                            // System.Threading.Thread.Sleep(100);
                            // DebugLine("waiting end, msg remaining: " + _sizingMessages.Count);
                            //}

                            WaitHandle.WaitAll(new WaitHandle[] { _manualReset });
                            DebugLine("waiting end, msg remaining: " + _sizingMessages.Count);

                            AfterSizingDelay(hasError, resultFilePath, errorFilePath);
                        });
                    }

                    this.buttonFolderInfo.Enabled = true;
                    this.checkBoxAutoOpen.Enabled = true;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                fileTask.Task.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get folder info due to：" + ex.ToString());
                this.buttonFolderInfo.Enabled = true;
                this.checkBoxAutoOpen.Enabled = true;
            }
        }

        private static void DebugLine(string line)
        {
            System.Diagnostics.Debug.WriteLine($"----------> {System.Threading.Thread.CurrentThread.ManagedThreadId:##}: " + line);
        }

        private void AfterSizingDelay(bool hasError, string resultFilePath, string errorFilePath)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action<bool,string,string>)AfterSizingDelay, hasError, resultFilePath, errorFilePath);
            }
            else
            {
                if (this.checkBoxAutoOpen.Checked)
                {
                    System.Diagnostics.Process.Start(resultFilePath);
                    if (hasError)
                    {
                        System.Diagnostics.Process.Start(errorFilePath);
                    }
                }
                else
                {
                    MessageBox.Show($"Result saved to {resultFilePath}{(hasError ? ", error saved to " + errorFilePath : "")}");
                }
            }
        }

        private void SizingProgress(string message)
        {
            //  UpdateUIWarningMessage(message)
            _sizingMessages.Enqueue(Tuple.Create(message, DateTime.UtcNow));

        }

    }
}
