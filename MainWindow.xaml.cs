using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;

namespace JX3SyncAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string VERSION = "0.2.0"; 
        public MainWindow()
        {
            InitializeComponent();
            SourceSelect.SelectedIndex = 0;
            TargetSelect.SelectedIndex = 0;
        }

        private void SourceSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SourceSelect.SelectedIndex == 0 || SourceSelect.SelectedIndex == 1)
            {
                SourceRoleGrid.Visibility = Visibility.Visible;
                try
                {
                    SourceFolder.Text = Helper.GetGameFolderFromReg(SourceSelect.SelectedIndex != 0) + (SourceSelect.SelectedIndex == 0 ? @"\Game\JX3\bin\zhcn_hd" : @"\Game\JX3_EXP\bin\zhcn_exp");
                }
                catch
                {
                    SourceFolder.Text = "获取程序路径失败，请手动选择";
                }
            }
        }

        private void SourceFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + @"\userdata");
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceAccounts.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if(subDir.Name != "fight_stat")
                    {
                        Label label = new Label { Content = subDir.Name };
                        SourceAccounts.Items.Add(label);
                    }
                }
                SourceAccounts.SelectedIndex = 0;
            }
            catch
            {
                Helper.Log(SourceFolder.Text, LogPanel);
                SourceAccounts.Items.Clear();
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
            if (TargetSelect.SelectedIndex == 0 || TargetSelect.SelectedIndex == 1)
            {
                TargetRoleGrid.Visibility = Visibility.Visible;
                try
                {
                    TargetFolder.Text = Helper.GetGameFolderFromReg(TargetSelect.SelectedIndex != 0) + (TargetSelect.SelectedIndex == 0 ? @"\Game\JX3\bin\zhcn_hd" : @"\Game\JX3_EXP\bin\zhcn_exp");
                }
                catch
                {
                    TargetFolder.Text = "获取程序路径失败，请手动选择";
                }
            }
        }

        private void TargetFolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
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
            }
            catch
            {
                Helper.Log(TargetFolder.Text, LogPanel);
                TargetAccounts.Items.Clear();
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
                    MessageBox.Show("请先选择作为数据来源的角色", "操作错误");
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
                    MessageBox.Show("请先选择作为数据目标的角色", "操作错误");
                }
            }

            MessageBox.Show("没有意外的话应该成功了吧（？", "不太确定的提示框");
        }

        private void SourceBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XLauncher.exe|*.exe";
            ofd.FileName = "XLauncher.exe";
            if (ofd.ShowDialog() == true)
            {
                string folder = ofd.FileName;
                SourceFolder.Text = System.IO.Path.GetDirectoryName(folder);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("起因是玩体验服的时候玩的心法太多每次移文件移到吐\n" +
                "老习惯作品会开源在github，我的主页是 https://github.com/X3ZvaWQ \n" +
                "游戏id 秀秀不咕 坐标唯满侠 是一个秀萝 \n" +
                "如果有什么意见的话可以和我提，也可以自己改之后pr，第一次写C#也是第一次用WPF写窗口程序，可能代码很难看敬请谅解（x \n" +
                "根据GPLv0.3.0 随便怎么用，不要商业即可 \n" +
                "0.2.0相比0.1.0版本增加的内容主要有允许自定义是否关闭服务器同步、允许同步插件设置等。\n" +
                "软件版本 0.2.0", "关于");
        }
    }
}
