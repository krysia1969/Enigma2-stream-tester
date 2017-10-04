﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Enigma2_stream_tester.UserView;
using Enigma2_stream_tester.Utils;
using Newtonsoft.Json;


namespace Enigma2_stream_tester
{
    public partial class Main : Form
    {
        public FileOperations Operation;
        public VideoTest Video;
        private List<string> _filePathList;
        private List<string[]> _filesContentList;
        public List<Item> ConfigurationItems; //cannot be readonly, settings forms needs to have posibility to chanege values 
        private readonly MainPage _mainView;
        private MainConfigView _mainConfigView;

        public Main()
        {
            InitializeComponent();
            ConfigurationItems = LoadJson(); //load config
            Video = new VideoTest(this);
            _mainView = new MainPage(this);

            //run tester page first
            pagePanel.Controls.Clear();
            pagePanel.Controls.Add(_mainView);
            _mainView.Dock = DockStyle.Fill;
            _mainView.BringToFront();
        }

        public void Start_Click(object sender, EventArgs e)
        {
            _mainView.StartButton.Enabled = false;
            var worker = new Thread(Work) {IsBackground = true};
            worker.Start();
        }
    
        public void Work()
        {
            ConfigurationItems = LoadJson(); // load new config before start tester
            if (_filePathList.Count == 0 || _filesContentList.Count == 0 || ConfigurationItems == null) return;

            //progressbar
            BeginInvoke((Action) delegate { ScanProgressBar.Minimum = 0; });
            BeginInvoke((Action) delegate { ScanProgressBar.Maximum = _filePathList.Count; });
            var progressBarCounter = 0;
            //

            //parallel restrict
            var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = ConfigurationItems[0].parallelOpt };
            //
            Parallel.ForEach(_filesContentList, parallelOptions, currentFile =>
            {
                var ip = Operation.FirstLineHttp(currentFile);
                if (ip == null) return;
                Video.Check(ip, ConfigurationItems[0].scanPort, ConfigurationItems[0].defaultChannel);
                var counter = progressBarCounter;
                BeginInvoke((Action) delegate
                {
                    ScanProgressBar.Value = counter;
                    progressValue_Label.Text = (int) ((float)ScanProgressBar.Value / (float)ScanProgressBar.Maximum * 100) + @"%";
                });
                progressBarCounter += 1;
            });

            GC.Collect(); //clean cache
            MessageBox.Show(@"Done!", @"Info!");
            BeginInvoke((Action) delegate
            {
                ScanProgressBar.Value = 0;
                progressValue_Label.Text = (int)((float)ScanProgressBar.Value / (float)ScanProgressBar.Maximum * 100) + "%";
                _mainView.StartButton.Enabled = true;
            });
            Operation.RemoveTemp(Operation.DirectoryPath); //remove files and dir with *.mpeg
            Operation.OpenOutputDirectory();
        }

        public class Item
        {
            public string defaultChannel;
            public string scanPort;
            public int timeout;
            public int parallelOpt;
        }

        public List<Item> LoadJson()
        {
            try
            {
                using (var r = new StreamReader(Directory.GetCurrentDirectory() + "\\configs\\default.cfg"))
                {
                    var json = r.ReadToEnd();
                    var items = JsonConvert.DeserializeObject<List<Item>>(json);
                    return items;
                }
            }
            catch (Exception e)
            {
                AddLogToFile(e.Message);
                return null;
            }
            
        }

        public void SaveJson(List<Item> config)
        {
            try
            {
                using (var sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\configs\\default.cfg"))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(config));
                }
                
            }
            catch (Exception e)
            {
                AddLogToFile(e.Message);
            }
        }

        public void AddToLog(IEnumerable<string> log)
        {
            BeginInvoke((Action) delegate
            {
                foreach (var item in log)
                {
                    _mainView.Log_listBox.Items.Add(DateTime.Now.ToString("HH:mm:ss tt") + " : " + item);
                    LogScroll();
                }
            });
        }

        public void AddToLog(string log)
        {
            BeginInvoke((Action) delegate
            {
                _mainView.Log_listBox.Items.Add(DateTime.Now.ToString("HH:mm:ss tt") + " : " + log);
                LogScroll();
            });
        }

        public void AddLogToFile(string exception)
        {
            BeginInvoke((Action)delegate
            {
                var di = Directory.GetCurrentDirectory();
                using (var stream = new StreamWriter(di + "\\log.txt", true))
                {
                    stream.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : " + exception);
                }
            });
        }

        public void LogScroll()
        {
            var visibleItems = _mainView.Log_listBox.ClientSize.Height / _mainView.Log_listBox.ItemHeight;
            _mainView.Log_listBox.TopIndex = Math.Max(_mainView.Log_listBox.Items.Count - visibleItems + 1, 0);
        }

        private void EnableMenuItem_Click(object sender, EventArgs e)
        {
            mysql_enable_MenuItem.Checked = !mysql_enable_MenuItem.Checked;
        }

        private void LoadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Operation = new FileOperations(this);
            var tuple = Operation.ScanDirectory();
            if (tuple == null) return;
            _mainView.StartButton.Enabled = true;
            _filePathList = tuple.Item1;
            _filesContentList = tuple.Item2;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Operation.RemoveTemp(Operation.DirectoryPath);
                GC.Collect();
                Application.Exit();
            }
            catch (Exception ec)
            {
                AddLogToFile(ec.Message);
                Application.Exit();
            }
        }

        private void HelpStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/xulek/Enigma2-stream-tester");
        }

        private void TesterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            //save to json config file
            ConfigurationItems[0].scanPort = _mainConfigView.Port8001_Checkbox.Checked ? "8001" : "8002";
            ConfigurationItems[0].parallelOpt = _mainConfigView.Speed_Trackbar.Value;
            ConfigurationItems[0].timeout = _mainConfigView.Timeout_Trackbar.Value;
            ///
            /// Channels added in _mainConfigView
            /// 
            SaveJson(ConfigurationItems);
            //
            pagePanel.Controls.Clear();
            pagePanel.Controls.Add(_mainView);
            _mainView.Dock = DockStyle.Fill;
            _mainView.BringToFront();
        }

        private void MainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _mainConfigView = new MainConfigView(this);
            pagePanel.Controls.Clear();
            pagePanel.Controls.Add(_mainConfigView);
            _mainConfigView.Dock = DockStyle.Fill;
            _mainConfigView.BringToFront();
        }
    }
}