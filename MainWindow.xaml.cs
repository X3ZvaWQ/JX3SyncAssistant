using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;
using JX3SyncAssistant.Properties;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Resources;

namespace JX3SyncAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string VERSION = "0.7.3"; 
        public string NewVersionBody = "";
        public string NewVersionUrl = "";
        public string NewVersionName = "";
        public string resultMD5 = "";
        public Dictionary<string, bool> options;

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
           
            Release[] Versions = await Helper.GetVersionsFromGiteeRelease(LogPanel);
            Versions = Versions.Reverse().ToArray();
            if(Versions.Length > 0)
            {
                NewVersionUrl = Versions[0].assets[0]["browser_download_url"];
                NewVersionName = Versions[0].assets[0]["name"];
                foreach (Release Version in Versions)
                {
                    if (Version.name.Substring(1) == VERSION)
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
        }

        private void SourceSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SourceRoleGrid.Visibility = Visibility.Hidden;
            SourceFileGrid.Visibility = Visibility.Hidden;
            SourceUrlGrid.Visibility = Visibility.Hidden;

            if (SourceSelect.SelectedIndex == 0 || SourceSelect.SelectedIndex == 1)
            {
                SourceRoleGrid.Visibility = Visibility.Visible;
                try
                {
                    SourceFolder.Text = Helper.GetGameFolder(SourceSelect.SelectedIndex != 0, LogPanel);
                }
                catch (Exception E)
                {
                    Helper.Log(E.Message, LogPanel);
                    Helper.Log(E.StackTrace, LogPanel);
                    SourceFolder.Text = "获取程序路径失败，请手动选择";
                }
                if(Settings.Default.SourceRoleSelect == "combo")
                {
                    if (Settings.Default.DataLoadFrom != null && Settings.Default.DataLoadFrom != "")
                    {
                        SourceFilePath.Text = Settings.Default.DataLoadFrom;
                    }
                    else
                    {
                        SourceFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "userdata.zip";
                    }

                    SourceRoleComboSelect.Visibility = Visibility.Visible;
                    SourceRoleLayerSelect.Visibility = Visibility.Hidden;
                }
                else
                {
                    SourceRoleComboSelect.Visibility = Visibility.Hidden;
                    SourceRoleLayerSelect.Visibility = Visibility.Visible;
                }
            }
            else if(SourceSelect.SelectedIndex == 2)
            {
                if (Settings.Default.DataLoadFrom != null && Settings.Default.DataLoadFrom != "")
                {
                    SourceFilePath.Text = Settings.Default.DataLoadFrom;
                }
                else
                {
                    SourceFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "userdata.zip";
                }
                SourceFileGrid.Visibility = Visibility.Visible;
            }
            else if (SourceSelect.SelectedIndex == 3)
            {
                SourceUrlGrid.Visibility = Visibility.Visible;
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
                            Height = 14,
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
                    Label label = new Label { Content = subDir.Name};
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
            TargetUrlGrid.Visibility = Visibility.Hidden;

            if (TargetSelect.SelectedIndex == 0 || TargetSelect.SelectedIndex == 1)
            {
                TargetRoleGrid.Visibility = Visibility.Visible;
                try
                {
                    TargetFolder.Text = Helper.GetGameFolder(TargetSelect.SelectedIndex != 0, LogPanel);
                }
                catch (Exception E)
                {
                    Helper.Log(E.Message, LogPanel);
                    Helper.Log(E.StackTrace, LogPanel);
                    TargetFolder.Text = "获取程序路径失败，请手动选择";
                }
                if (Settings.Default.TargetRoleSelect == "combo")
                {
                    TargetRoleComboSelect.Visibility = Visibility.Visible;
                    TargetRoleLayerSelect.Visibility = Visibility.Hidden;
                }
                else
                {
                    TargetRoleComboSelect.Visibility = Visibility.Hidden;
                    TargetRoleLayerSelect.Visibility = Visibility.Visible;
                }
            }
            else if (TargetSelect.SelectedIndex == 2)
            {
                if (Settings.Default.DataSaveTo != null && Settings.Default.DataSaveTo != "")
                {
                    TargetFilePath.Text = Settings.Default.DataSaveTo;
                }
                else
                {
                    TargetFilePath.Text = AppDomain.CurrentDomain.BaseDirectory + "userdata.zip";
                }
                TargetFileGrid.Visibility = Visibility.Visible;
            }
            else if (TargetSelect.SelectedIndex == 3)
            {
                TargetUrlGrid.Visibility = Visibility.Visible;
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
                            Height = 14,
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

           options = new Dictionary<string, bool>{
                { "userdata", (bool)UISettings.IsChecked },
                { "userdata_async", (bool)ServerSyncSettings.IsChecked },
                { "jx_role_config", (bool)JXNoticeSettings.IsChecked },
                { "my_role_config", (bool)MYRoleSettings.IsChecked },
                { "jx_config", (bool)JXGlobalSettings.IsChecked },
                { "my_config", (bool)MYGlobalSettings.IsChecked },
                { "backup", (bool)BackupSettings.IsChecked }
            };
            Go.IsEnabled = false;
            //get source zip file
            if (processLeft())
            {
                //unpack zip file to target
                if (processRight())
                {
                    processEnd();
                }
            }
        }

        private bool processLeft()
        {
            if (SourceSelect.SelectedIndex == 0 || SourceSelect.SelectedIndex == 1)
            {
                if (SourceAccounts.SelectedIndex != -1)
                {
                    //get sync options
                    
                    try
                    {
                        string SourceGameFolder = SourceFolder.Text;
                        if (SourceSelect.SelectedIndex == 0)
                        {
                            Settings.Default.GameFolder = SourceGameFolder;
                        }
                        else
                        {
                            Settings.Default.ExpGameFolder = SourceGameFolder;
                        }
                        if(SourceRoleComboSelect.Visibility == Visibility.Visible)
                        {
                            Settings.Default.SourceRoleSelect = "combo";
                        }
                        else
                        {
                            Settings.Default.SourceRoleSelect = "layer";
                        }
                        Settings.Default.Save();

                        Dictionary<string, string> roleInfo = new Dictionary<string, string> {
                            { "account", (string)(SourceAccounts.SelectedItem as Label).Content },
                            { "area", (string)(SourceAreas.SelectedItem as Label).Content },
                            { "server", (string)(SourceServers.SelectedItem as Label).Content },
                            { "role", (string)(SourceRoles.SelectedItem as Label).Content }
                        };
                        Helper.GetZipFromUserdata(SourceGameFolder, "__userdata.zip", roleInfo, options, LogPanel);
                    }
                    catch (Exception E)
                    {
                        Helper.Log("getZipFromUserdata() Run Error!", LogPanel);
                        Helper.Log(E.Message, LogPanel);
                        Helper.Log(E.StackTrace, LogPanel);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("请先选择作为数据来源的角色", "表示错误的对话框");
                }
            }
            else if (SourceSelect.SelectedIndex == 2)
            {
                if (SourceFilePath.Text != AppDomain.CurrentDomain.BaseDirectory + "\\userdata.zip")
                {
                    if (File.Exists(SourceFilePath.Text))
                    {
                        File.Copy(SourceFilePath.Text, AppDomain.CurrentDomain.BaseDirectory + "\\userdata.zip", true);
                        Settings.Default.DataLoadFrom = SourceFilePath.Text;
                        Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("源文件不存在哦，需要我为您变出来么（？ \n (可以在左边选择角色，右边选择本地数据文件进行文件的导出)", "表示疑惑地对话框");
                        Go.IsEnabled = true;
                        return false;
                    }
                }
            }
            else if (SourceSelect.SelectedIndex == 3)
            {
                try
                {
                    string md5 = SourceFileMD5.Text;
                    WebClient wc = new WebClient();
                    UploadProgressBar.Visibility = Visibility.Visible;
                    UploadProgressLabel.Visibility = Visibility.Visible;
                    wc.DownloadProgressChanged += UploadProgressChanged;
                    wc.DownloadFileCompleted += DownloadUserdataCompleted;
                    wc.DownloadFileAsync(new Uri($"http://47.101.177.238/userdata/{md5}.zip"), "userdata.zip");
                    return false;
                }
                catch (Exception E)
                {
                    MessageBox.Show("尝试从服务器获取文件的过程中出现错误，请检查网络连接以及数据代码是否正确", "网络错误");
                    Helper.Log(E.Message, LogPanel);
                    Helper.Log(E.StackTrace, LogPanel);
                }
            }
            return true;
        }

        private bool processRight()
        {
            if (TargetSelect.SelectedIndex == 0 || TargetSelect.SelectedIndex == 1)
            {
                if (TargetAccounts.SelectedIndex != -1)
                {
                    try
                    {
                        string TargetGameFolder = TargetFolder.Text;
                        if (TargetSelect.SelectedIndex == 0)
                        {
                            Settings.Default.GameFolder = TargetGameFolder;
                        }
                        else
                        {
                            Settings.Default.ExpGameFolder = TargetGameFolder;
                        }
                        if (TargetRoleComboSelect.Visibility == Visibility.Visible)
                        {
                            Settings.Default.TargetRoleSelect = "combo";
                        }
                        else
                        {
                            Settings.Default.TargetRoleSelect = "layer";
                        }
                        Settings.Default.Save();

                        Dictionary<string, string> roleInfo = new Dictionary<string, string> {
                            { "account", (string)(TargetAccounts.SelectedItem as Label).Content },
                            { "area", (string)(TargetAreas.SelectedItem as Label).Content },
                            { "server", (string)(TargetServers.SelectedItem as Label).Content },
                            { "role", (string)(TargetRoles.SelectedItem as Label).Content }
                        };
                        if (options["backup"])
                        {
                            Helper.GetZipFromUserdata(TargetGameFolder, "backup.zip", roleInfo, options, LogPanel);
                        }
                        Helper.UnpackToUserdata(TargetGameFolder, "__userdata.zip", roleInfo, LogPanel);
                    }
                    catch (Exception E)
                    {
                        Helper.Log("UnpackToUserdata() Run Error!", LogPanel);
                        Helper.Log(E.Message, LogPanel);
                        Helper.Log(E.StackTrace, LogPanel);
                        Go.IsEnabled = true;
                        return false;
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
                        if (!Directory.Exists(Path.GetDirectoryName(TargetFilePath.Text)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(TargetFilePath.Text));
                        }
                        if (File.Exists(TargetFilePath.Text))
                        {
                            File.Delete(TargetFilePath.Text);
                        }
                        File.Move("__userdata.zip", TargetFilePath.Text);
                        Helper.Log($"INFO: file \"userdata.zip\" has been moved to {TargetFilePath.Text}", LogPanel);
                        Settings.Default.DataSaveTo = TargetFilePath.Text;
                        Settings.Default.Save();
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("输出数据文件的过程中遇到了问题，请检查目标目录是否存在以及目标目录是否有同名文件是否正在被占用", "文件错误");
                        Helper.Log(E.Message, LogPanel);
                        Helper.Log(E.StackTrace, LogPanel);
                    }
                }
            }
            else if (TargetSelect.SelectedIndex == 3)
            {
                try
                {
                    MD5 md5 = MD5.Create();
                    byte[] file = File.ReadAllBytes("userdata.zip");
                    file = md5.ComputeHash(file);
                    resultMD5 = "";
                    foreach (byte b in file)
                        resultMD5 += b.ToString("x");
                    Helper.Log($"INFO: userdata's MD5 value is \"{resultMD5}\", start upload", LogPanel);

                    WebClient wc = new WebClient();
                    UploadProgressBar.Visibility = Visibility.Visible;
                    UploadProgressLabel.Visibility = Visibility.Visible;
                    wc.UploadProgressChanged += UploadProgressChanged;
                    wc.UploadFileCompleted += UploadUserdataCompleted;
                    wc.UploadFileAsync(new Uri($"http://47.101.177.238/userdata/{resultMD5}.zip"), "PUT", "userdata.zip");
                    return false;
                }
                catch (Exception E)
                {
                    MessageBox.Show("尝试上传文件到服务器错误，请打开日志区提交错误报告（x", "网络错误");
                    Helper.Log(E.Message, LogPanel);
                    Helper.Log(E.StackTrace, LogPanel);
                }
            }
            return true;
        }

        private void processEnd()
        {
            if (SourceSelect.SelectedIndex == 2 && TargetSelect.SelectedIndex == 2)
            {
                MessageBox.Show("我已经听话地把这个文件已经从左边移到右边了\n\n但是您为什么不自己Ctrl+C Ctrl+V呢  w(ﾟДﾟ)w", "表示疑惑的提示框");
            }
            else if (SourceSelect.SelectedIndex == 3 && TargetSelect.SelectedIndex == 3)
            {
                MessageBox.Show("别皮了，后台储存数据是根据文件的摘要储存的，就算你这么操作了也不会发生任何事情 w(ﾟДﾟ)w", "表示疑惑的提示框");
            }
            else
            {
                MessageBox.Show("没有意外的话应该成功了吧（？", "不太确定的表示操作成功的提示框");
            }
            Go.IsEnabled = true;
        }

        private void SourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择包含userdata文件夹与interface文件夹的目录即可。\n在你下一次使用时程序会自动选择旧的目录";
            if(fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folder = fbd.SelectedPath;
                if (Directory.Exists($@"{folder}\interface") && Directory.Exists($@"{folder}\userdata"))
                {
                    SourceFolder.Text = folder;
                }
                else
                {
                    MessageBox.Show("没有在你选择的目录下面找到userdata和interface\n是不是找错位置了啦？", "表示疑惑的对话框");
                }
            }
        }

        private void TargetBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.Description = "选择包含userdata文件夹与interface文件夹的目录即可。\n在你下一次使用时程序会自动选择旧的目录";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folder = fbd.SelectedPath;
                if (Directory.Exists($@"{folder}\interface") && Directory.Exists($@"{folder}\userdata"))
                {
                    TargetFolder.Text = folder;
                }
                else
                {
                    MessageBox.Show("没有在你选择的目录下面找到userdata和interface\n是不是找错位置了啦？", "表示疑惑的对话框");
                }
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
                "0.5.0增加的内容主要是新增了保存目录设置、更多的获取游戏目录方式、更新检查功能，同时修复了一些地方没法显示下划线的问题\n" +
                "0.5.1修复了0.5.0自动更新检测存在的问题\n" +
                "0.6.0修复了0.5.1在无网络环境下打开会崩溃的问题，添加了简单的云支持\n" +
                "0.7.0修复了0.6.0在存在某些旧文件的情况下无法同步键位的问题，添加了备份目标角色文件的功能。\n" +
                "0.7.1不再从网络获取userpreferencesasync.jx3dat文件，并且添加直观的文件夹浏览而不是选择XLauncher.exe，修复了自动关闭服务器同步选项的一个逻辑问题\n" +
                "0.7.2修复了茗伊插件最近更新 茗伊插件配置文件路径变化 导致无法同步茗伊配置的问题\n" +
                "0.7.3修复了茗伊插件最近更新 插件配置文件使用了新的存储方式 导致无法同步茗伊配置的问题\n" +
                "软件版本 0.7.3", "关于");
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
            try
            {
                ofd.InitialDirectory = Path.GetDirectoryName(SourceFilePath.Text);
                ofd.FileName = Path.GetFileName(SourceFilePath.Text);
            }
            catch
            {
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ofd.FileName = "userdata.zip";
            }
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
            try
            {
                sfd.InitialDirectory = Path.GetDirectoryName(TargetFilePath.Text);
            }
            catch
            {
                sfd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }
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
            DownloadProgressLabel.Content = string.Format("{0:F}M/{1:F}M", e.BytesReceived/1024/1024.0, e.TotalBytesToReceive/1024/1024.0);
        }

        private void UploadUserdataCompleted(object sender, AsyncCompletedEventArgs e)
        {
            processEnd();
            UploadProgressBar.Visibility = Visibility.Hidden;
            UploadProgressLabel.Visibility = Visibility.Hidden;
            TargetFileMD5.Text = resultMD5;
        }

        private void DownloadUserdataCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UploadProgressBar.Visibility = Visibility.Hidden;
            UploadProgressLabel.Visibility = Visibility.Hidden;
            processRight();
        }

        private void UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            UploadProgressBar.Maximum = (int)e.TotalBytesToSend;
            UploadProgressBar.Value = (int)e.BytesSent;
            UploadProgressLabel.Content = string.Format("{0}K / {1}K", e.BytesSent / 1024, e.TotalBytesToSend / 1024);
        }
        
        private void UploadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UploadProgressBar.Maximum = (int)e.TotalBytesToReceive;
            UploadProgressBar.Value = (int)e.BytesReceived;
            UploadProgressLabel.Content = string.Format("{0}K / {1}K", e.BytesReceived / 1024, e.TotalBytesToReceive / 1024);
        }
    }
}
