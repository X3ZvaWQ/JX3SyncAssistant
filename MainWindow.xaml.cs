using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;

namespace JX3SyncAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string VERSION = "0.4.0"; 
        public string NewVersionBody = "";
        public string NewVersionUrl = "";
        public string NewVersionName = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            SourceSelect.SelectedIndex = 0;
            TargetSelect.SelectedIndex = 0;
            SourceFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "userdata.zip";
            TargetFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "userdata.zip";
            try
            {
                SourceFolder.Text = Helper.GetGameFolderFromReg(SourceSelect.SelectedIndex != 0) + (SourceSelect.SelectedIndex == 0 ? @"\Game\JX3\bin\zhcn_hd" : @"\Game\JX3_EXP\bin\zhcn_exp");
                TargetFolder.Text = Helper.GetGameFolderFromReg(TargetSelect.SelectedIndex != 0) + (TargetSelect.SelectedIndex == 0 ? @"\Game\JX3\bin\zhcn_hd" : @"\Game\JX3_EXP\bin\zhcn_exp");
            }
            catch
            {
                TargetFolder.Text = "获取程序路径失败，请手动选择";
                SourceFolder.Text = "获取程序路径失败，请手动选择";
            }
            Release[] Versions = await Helper.GetVersionsFromGiteeRelease();
            NewVersionUrl = Versions[0].assets[0]["browser_download_url"];
            NewVersionName = Versions[0].assets[0]["name"];
            foreach (Release Version in Versions)
            {
                if( Version.name.Substring(1) == VERSION)
                {
                    break;
                }
                NewVersionBody += $"{Version.name}\n{Version.body}\n";
            }
            if (NewVersionBody != "")
            {
                UpdateButton.Visibility = Visibility.Visible;
            }
        }

        private void SourceSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SourceRoleGrid.Visibility = Visibility.Hidden;
            SourceFileGrid.Visibility = Visibility.Hidden;
            if (SourceSelect.SelectedIndex == 0 || SourceSelect.SelectedIndex == 1)
            {
                SourceRoleGrid.Visibility = Visibility.Visible;
            }
            else if(SourceSelect.SelectedIndex == 2)
            {
                SourceFileGrid.Visibility = Visibility.Visible;
            }

        }

        private void SourceFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(Directory.Exists(SourceFolder.Text + @"\userdata"))
                {
                    DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + @"\userdata");
                    DirectoryInfo[] subDirs = dir.GetDirectories();
                    SourceAccounts.Items.Clear();
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        if (subDir.Name != "fight_stat")
                        {
                            Label label = new Label { Content = subDir.Name };
                            SourceAccounts.Items.Add(label);
                        }
                    }
                    string[] roles = Helper.GetAllRoles(SourceFolder.Text + @"\userdata");
                    SourceRoleList.Items.Clear();
                    foreach (string role in roles)
                    {
                        Label label = new Label
                        {
                            Content = role,
                            Height = 13,
                            FontSize = 11,
                            Padding = new Thickness(5, 0, 0, 0)
                        };
                        SourceRoleList.Items.Add(label);
                    }
                    SourceAccounts.SelectedIndex = 0;
                }
                else
                {
                    SourceAccounts.Items.Clear();
                    SourceRoleList.Items.Clear();
                }
            }
            catch(Exception E)
            {
                Helper.Log(SourceFolder.Text, LogPanel);
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
                SourceAccounts.Items.Clear();
                SourceRoleList.Items.Clear();
            }

        }
       
        private void SourceAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceAccounts.SelectedIndex == -1) {
                SourceAreas.Items.Clear();
                SourceServers.Items.Clear();
                SourceRoles.Items.Clear();
                return;
            };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + @"\userdata\" + (SourceAccounts.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceAreas.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir.Name };
                    SourceAreas.Items.Add(label);
                }
                SourceAreas.SelectedIndex = 0;
            }
            catch ( Exception E)
            {
                SourceAreas.Items.Clear();
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
            }
        }

        private void SourceAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceAreas.SelectedIndex == -1)
            {
                SourceServers.Items.Clear();
                SourceRoles.Items.Clear();
                return;
            };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + @"\userdata\" + (SourceAccounts.SelectedItem as Label).Content + "\\" + (SourceAreas.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceServers.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir.Name };
                    SourceServers.Items.Add(label);
                }
                SourceServers.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                SourceServers.Items.Clear();
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
            }
        }

        private void SourceServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceServers.SelectedIndex == -1) {
                SourceRoles.Items.Clear();
                return;
            };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + @"\userdata\" + (SourceAccounts.SelectedItem as Label).Content + "\\" + (SourceAreas.SelectedItem as Label).Content + "\\" + (SourceServers.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceRoles.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir.Name };
                    SourceRoles.Items.Add(label);
                }
                SourceRoles.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                SourceRoles.Items.Clear(); 
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
            }
        }

        private void TargetSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TargetRoleGrid.Visibility = Visibility.Hidden;
            TargetFileGrid.Visibility = Visibility.Hidden;
            if (TargetSelect.SelectedIndex == 0 || TargetSelect.SelectedIndex == 1)
            {
                TargetRoleGrid.Visibility = Visibility.Visible;
            }
            else if (TargetSelect.SelectedIndex == 2)
            {
                TargetFileGrid.Visibility = Visibility.Visible;
            }
        }

        private void TargetFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Directory.Exists(TargetFolder.Text + @"\userdata"))
                {
                    DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + @"\userdata");
                    DirectoryInfo[] subDirs = dir.GetDirectories();
                    TargetAccounts.Items.Clear();
                    foreach (DirectoryInfo subDir in subDirs)
                    {
                        if (subDir.Name != "fight_stat")
                        {
                            Label label = new Label { Content = subDir.Name };
                            TargetAccounts.Items.Add(label);
                        }
                    }
                    TargetAccounts.SelectedIndex = 0;
                    string[] roles = Helper.GetAllRoles(TargetFolder.Text + @"\userdata");
                    TargetRoleList.Items.Clear();
                    foreach (string role in roles)
                    {
                        Label label = new Label
                        {
                            Content = role,
                            Height = 13,
                            FontSize = 11,
                            Padding = new Thickness(5, 0, 0, 0)
                        };
                        TargetRoleList.Items.Add(label);
                    }
                }
                else {
                    TargetAccounts.Items.Clear();
                    TargetRoleList.Items.Clear();
                }
            }
            catch(Exception E)
            {
                Helper.Log(TargetFolder.Text, LogPanel);
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
                TargetAccounts.Items.Clear();
                TargetRoleList.Items.Clear();
            }
        }

        private void TargetAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TargetAccounts.SelectedIndex == -1) {
                TargetAreas.Items.Clear();
                TargetServers.Items.Clear();
                TargetRoles.Items.Clear();
                return;
            };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + @"\userdata\" + (TargetAccounts.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetAreas.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir.Name };
                    TargetAreas.Items.Add(label);
                }
                TargetAreas.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                TargetAreas.Items.Clear();
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
            }
        }

        private void TargetAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TargetAreas.SelectedIndex == -1) {
                TargetServers.Items.Clear();
                TargetRoles.Items.Clear();
                return;

            };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + @"\userdata\" + (TargetAccounts.SelectedItem as Label).Content + "\\" + (TargetAreas.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetServers.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir.Name };
                    TargetServers.Items.Add(label);
                }
                TargetServers.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                TargetServers.Items.Clear();
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
            }
        }

        private void TargetServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TargetServers.SelectedIndex == -1) {
                TargetRoles.Items.Clear();
                return;
            };
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + @"\userdata\" + (TargetAccounts.SelectedItem as Label).Content + "\\" + (TargetAreas.SelectedItem as Label).Content + "\\" + (TargetServers.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetRoles.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir.Name };
                    TargetRoles.Items.Add(label);
                }
                TargetRoles.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                TargetRoles.Items.Clear();
                Helper.Log(E.Message, LogPanel);
                Helper.Log(E.StackTrace, LogPanel);
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            //get source zip file
            if(SourceSelect.SelectedIndex == 0 || SourceSelect.SelectedIndex == 1)
            {
                if(SourceAccounts.SelectedIndex != -1)
                {
                    //get sync options
                    Dictionary<string, bool> options = new Dictionary<string, bool>{
                        { "userdata", (bool)UISettings.IsChecked },
                        { "userdata_async", !(bool)ServerSyncSettings.IsChecked },
                        { "jx_role_config", (bool)JXNoticeSettings.IsChecked },
                        { "my_role_config", (bool)MYRoleSettings.IsChecked },
                        { "jx_config", (bool)JXGlobalSettings.IsChecked },
                        { "my_config", (bool)MYGlobalSettings.IsChecked } 
                    };
                    try
                    {
                        string SourceGameFolder = SourceFolder.Text;
                        Dictionary<string, string> roleInfo = new Dictionary<string, string> {
                            { "account", (string)(SourceAccounts.SelectedItem as Label).Content },
                            { "area", (string)(SourceAreas.SelectedItem as Label).Content },
                            { "server", (string)(SourceServers.SelectedItem as Label).Content },
                            { "role", (string)(SourceRoles.SelectedItem as Label).Content }
                        };
                        Helper.GetZipFromUserdata(SourceGameFolder, "userdata.zip", roleInfo, options, LogPanel);
                    }
                    catch(Exception E)
                    {
                        Helper.Log("getZipFromUserdata() Run Error!", LogPanel);
                        Helper.Log(E.Message, LogPanel);
                        Helper.Log(E.StackTrace, LogPanel);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请先选择作为数据来源的角色", "表示错误的对话框");
                }
            }
            else if(SourceSelect.SelectedIndex == 2)
            {
                if(SourceFilePath.Text != AppDomain.CurrentDomain.BaseDirectory + "\\userdata.zip")
                {
                    if(File.Exists(SourceFilePath.Text))
                    {
                        File.Copy(SourceFilePath.Text, AppDomain.CurrentDomain.BaseDirectory + "\\userdata.zip", true);
                    }
                    else
                    {
                        MessageBox.Show("源文件不存在哦，需要我为您变出来么（？ \n (可以在左边选择角色，右边选择本地数据文件进行文件的导出)", "表示疑惑地对话框");
                        return;
                    }
                }
            }

            //unpack zip file to target
            if(TargetSelect.SelectedIndex == 0 || TargetSelect.SelectedIndex == 1)
            {
                if (TargetAccounts.SelectedIndex != -1)
                {
                    try
                    {
                        string TargetGameFolder = TargetFolder.Text;
                        Dictionary<string, string> roleInfo = new Dictionary<string, string> {
                            { "account", (string)(TargetAccounts.SelectedItem as Label).Content },
                            { "area", (string)(TargetAreas.SelectedItem as Label).Content },
                            { "server", (string)(TargetServers.SelectedItem as Label).Content },
                            { "role", (string)(TargetRoles.SelectedItem as Label).Content }
                        };
                        Helper.UnpackToUserdata(TargetGameFolder, "userdata.zip", roleInfo, LogPanel);
                    }
                    catch (Exception E)
                    {
                        Helper.Log("UnpackToUserdata() Run Error!", LogPanel);
                        Helper.Log(E.Message, LogPanel);
                        Helper.Log(E.StackTrace, LogPanel);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请先选择作为数据目标的角色", "表示错误的对话框");
                }
            }
            else if (TargetSelect.SelectedIndex == 2)
            {
                
                if (TargetFilePath.Text != AppDomain.CurrentDomain.BaseDirectory + "\\userdata.zip")
                {
                    try
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(TargetFilePath.Text))){
                            Directory.CreateDirectory(Path.GetDirectoryName(TargetFilePath.Text));
                        }
                        if (File.Exists(TargetFilePath.Text))
                        {
                            File.Delete(TargetFilePath.Text);
                        }
                        File.Move("userdata.zip", TargetFilePath.Text);
                        Helper.Log($"INFO: file \"userdata.zip\" has been moved to {TargetFilePath.Text}", LogPanel);
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("输出数据文件的过程中遇到了问题，请检查目标目录是否存在以及目标目录是否有同名文件是否正在被占用", "文件错误");
                        Helper.Log(E.Message, LogPanel);
                        Helper.Log(E.StackTrace, LogPanel);
                    }
                }
            }
            if(SourceSelect.SelectedIndex == 2 && SourceSelect.SelectedIndex == 2)
            {
                MessageBox.Show("我已经听话地把这个文件已经从左边移到右边了\n\n但是您为什么不自己Ctrl+C Ctrl+V呢  w(ﾟДﾟ)w", "表示疑惑的提示框");
            }
            else
            {
                MessageBox.Show("没有意外的话应该成功了吧（？", "不太确定的表示操作成功的提示框");
            }
        }

        private void SourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XLauncher.exe|*.exe";
            ofd.FileName = "XLauncher.exe";
            if (ofd.ShowDialog() == true)
            {
                string folder = ofd.FileName;
                SourceFolder.Text = Path.GetDirectoryName(folder);
            }
        }

        private void TargetBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XLauncher.exe|*.exe";
            ofd.FileName = "XLauncher.exe";
            if (ofd.ShowDialog() == true)
            {
                string folder = ofd.FileName;
                TargetFolder.Text = System.IO.Path.GetDirectoryName(folder);
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("起因是玩体验服的时候玩的心法太多每次移文件移到吐\n" +
                "作品会开源在github，我的主页是 https://github.com/X3ZvaWQ \n" +
                "游戏id 秀秀不咕 坐标唯满侠 是一个秀萝 \n" +
                "如果有什么意见的话可以和我提，也可以自己改之后pr，第一次写C#也是第一次用WPF写窗口程序，可能代码很难看敬请谅解（x \n" +
                "根据MIT开源许可证，随便你怎么玩（x \n" +
                "0.2.0增加的内容主要有允许自定义是否关闭服务器同步、允许同步插件设置等。\n" +
                "0.2.1修复了0.2.0 迁移茗伊数据选项对于 没有导入目标监控数据 的用户会报错导致迁移失败\n" +
                "0.3.0增加的内容主要是美化了界面(随之程序体积也变大了x)、增加了一种用户建议的角色选择方式、增加了角色搜索功能以便更加快速找到目标角色\n" +
                "0.4.0增加的内容主要是支持了通过文件进行导入导出方便去网吧游玩的玩家、添加了每个按钮上的Tooltip\n" +
                "软件版本 0.4.0", "关于");
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.Height == 580)
            {
                this.Height = 350;
            }
            else
            {
                this.Height = 580;
            }
        }

        private void SourceRoleSelectModeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceRoleComboSelect.Visibility == Visibility.Visible)
            {
                SourceRoleComboSelect.Visibility = Visibility.Hidden;
                SourceRoleLayerSelect.Visibility = Visibility.Visible;
            }
            else
            {
                SourceRoleComboSelect.Visibility = Visibility.Visible;
                SourceRoleLayerSelect.Visibility = Visibility.Hidden;
            }
        }

        private void TargetRoleSelectModeSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (TargetRoleComboSelect.Visibility == Visibility.Visible)
            {
                Helper.FromComboToLayer(TargetRoleList, TargetAccounts, TargetAreas, TargetServers, TargetRoles);
                TargetRoleComboSelect.Visibility = Visibility.Hidden;
                TargetRoleLayerSelect.Visibility = Visibility.Visible;
            }
            else
            {
                Helper.FromLayerToCombo(TargetRoleList, TargetAccounts, TargetAreas, TargetServers, TargetRoles);
                TargetRoleComboSelect.Visibility = Visibility.Visible;
                TargetRoleLayerSelect.Visibility = Visibility.Hidden;
            }
        }

        private void SourceRoleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helper.FromComboToLayer(SourceRoleList, SourceAccounts, SourceAreas, SourceServers, SourceRoles);
        }

        private void SourceRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helper.FromLayerToCombo(SourceRoleList, SourceAccounts, SourceAreas, SourceServers, SourceRoles);
        }

        private void TargetRoleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helper.FromComboToLayer(TargetRoleList, TargetAccounts, TargetAreas, TargetServers, TargetRoles);
        }

        private void TargetRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Helper.FromLayerToCombo(TargetRoleList, TargetAccounts, TargetAreas, TargetServers, TargetRoles);
        }

        private void SourceSearchRoleSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string key = SourceSearchRoleSearch.Text;
            if (key.Length < 2)
            {
                return;
            };
            int i = 0;
            foreach(Label role in SourceRoleList.Items)
            {
                if(((string)role.Content).IndexOf(key) != -1)
                {
                    SourceRoleList.SelectedIndex = i;
                }
                else
                i++;
            }
        }

        private void TargetSearchRoleSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string key = TargetSearchRoleSearch.Text;
            if (key.Length < 2)
            {
                return; ;
            }
            int i = 0;
            foreach (Label role in TargetRoleList.Items)
            {
                if (((string)role.Content).IndexOf(key) != -1)
                {
                    TargetRoleList.SelectedIndex = i;
                }
                i++;
            }
        }

        private void SourceFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "ZipFile |*.zip";
            ofd.FileName = "XLauncher.exe";
            if (ofd.ShowDialog() == true)
            {
                string folder = ofd.FileName;
                SourceFilePath.Text = folder;
            }
        }

        private void TargetFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "ZipFile | *.zip";
            sfd.FileName = "userdata.zip";
            if (sfd.ShowDialog() == true)
            {
                string folder = sfd.FileName;
                TargetFilePath.Text = folder;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(NewVersionBody + "\n\n是否需要现在下载？按确定下载，取消当作没看到（ \n文件将会被下载到本程序的同目录，您也可以去本程序的发布页面自行下载。\n注意:由于我比较穷，所以使用的是gitee的release下载，所以下载的速度不会很快", "一个告诉你有更新的对话框", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                try
                {
                    DownloadProgressBar.Visibility = Visibility.Visible;
                    DownloadProgressLabel.Visibility = Visibility.Visible;
                    WebClient wc = new WebClient();
                    wc.DownloadProgressChanged += DownloadProgressChanged;
                    wc.DownloadFileCompleted += DownloadFileCompleted;
                    Helper.Log($"INFO: Start download new version app from \"{NewVersionUrl}\" to \"{NewVersionName}\" failed", LogPanel);

                    wc.DownloadFileAsync(new Uri(NewVersionUrl), NewVersionName);
                }
                catch(Exception E)
                {
                    Helper.Log($"ERROR: Downloading new version app from \"{NewVersionUrl}\" to \"{NewVersionName}\" failed", LogPanel);
                    Helper.Log(E.Message, LogPanel);
                    Helper.Log(E.StackTrace, LogPanel);
                }
                
            }
        }
        
        private void DownloadFileCompleted(object sender,AsyncCompletedEventArgs e)
        {
            DownloadProgressBar.Visibility = Visibility.Hidden;
            DownloadProgressLabel.Visibility = Visibility.Hidden;
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressBar.Maximum = 0;
            DownloadProgressBar.Maximum = (int)e.TotalBytesToReceive;
            DownloadProgressBar.Value = (int)e.BytesReceived;
            DownloadProgressLabel.Content = $"{string.Format("{0:F}", e.BytesReceived/1024/1024.0)}M/{string.Format("{0:F}", e.TotalBytesToReceive/1024/1024.0)}M";
        }
    }
}
