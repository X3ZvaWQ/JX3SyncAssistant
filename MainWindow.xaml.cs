using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.IO;
using System.Net;

namespace JX3SyncAssistant
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
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
                    SourceFolder.Text = Helper.GetGameFolderFromReg(SourceSelect.SelectedIndex != 0) + (SourceSelect.SelectedIndex == 0 ? @"\Game\JX3\bin\zhcn_hd\userdata" : @"\Game\JX3_EXP\bin\zhcn_exp\userdata");
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
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceAccounts.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if(subDir.Name != "fight_stat")
                    {
                        Label label = new Label { Content = subDir };
                        SourceAccounts.Items.Add(label);
                    }
                }
                SourceAccounts.SelectedIndex = 0;
            }
            catch
            {
                Console.WriteLine(SourceFolder.Text);
                SourceAccounts.Items.Clear();
            }

        }
       
        private void SourceAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + "\\" + (SourceAccounts.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceAreas.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir };
                    SourceAreas.Items.Add(label);
                }
                SourceAreas.SelectedIndex = 0;
            }
            catch
            {
                SourceAreas.Items.Clear();
            }
        }

        private void SourceAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + "\\" + (SourceAccounts.SelectedItem as Label).Content + "\\" + (SourceAreas.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceServers.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir };
                    SourceServers.Items.Add(label);
                }
                SourceServers.SelectedIndex = 0;
            }
            catch
            {
                SourceServers.Items.Clear();
            }
        }

        private void SourceServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(SourceFolder.Text + "\\" + (SourceAccounts.SelectedItem as Label).Content + "\\" + (SourceAreas.SelectedItem as Label).Content + "\\" + (SourceServers.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                SourceRoles.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir };
                    SourceRoles.Items.Add(label);
                }
                SourceRoles.SelectedIndex = 0;
            }
            catch
            {
                SourceRoles.Items.Clear();
            }
        }

        private void TargetSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TargetSelect.SelectedIndex == 0 || TargetSelect.SelectedIndex == 1)
            {
                TargetRoleGrid.Visibility = Visibility.Visible;
                try
                {
                    TargetFolder.Text = Helper.GetGameFolderFromReg(TargetSelect.SelectedIndex != 0) + (TargetSelect.SelectedIndex == 0 ? @"\Game\JX3\bin\zhcn_hd\userdata" : @"\Game\JX3_EXP\bin\zhcn_exp\userdata");
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
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetAccounts.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    if (subDir.Name != "fight_stat")
                    {
                        Label label = new Label { Content = subDir };
                        TargetAccounts.Items.Add(label);
                    }
                }
                TargetAccounts.SelectedIndex = 0;
            }
            catch
            {
                Console.WriteLine(TargetFolder.Text);
                TargetAccounts.Items.Clear();
            }
        }

        private void TargetAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + "\\" + (TargetAccounts.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetAreas.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir };
                    TargetAreas.Items.Add(label);
                }
                TargetAreas.SelectedIndex = 0;
            }
            catch
            {
                TargetAreas.Items.Clear();
            }
        }

        private void TargetAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + "\\" + (TargetAccounts.SelectedItem as Label).Content + "\\" + (TargetAreas.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetServers.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir };
                    TargetServers.Items.Add(label);
                }
                TargetServers.SelectedIndex = 0;
            }
            catch
            {
                TargetServers.Items.Clear();
            }
        }

        private void TargetServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(TargetFolder.Text + "\\" + (TargetAccounts.SelectedItem as Label).Content + "\\" + (TargetAreas.SelectedItem as Label).Content + "\\" + (TargetServers.SelectedItem as Label).Content);
                DirectoryInfo[] subDirs = dir.GetDirectories();
                TargetRoles.Items.Clear();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    Label label = new Label { Content = subDir };
                    TargetRoles.Items.Add(label);
                }
                TargetRoles.SelectedIndex = 0;
            }
            catch
            {
                TargetRoles.Items.Clear();
            }
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            //get source zip file
            if(SourceSelect.SelectedIndex == 0 || SourceSelect.SelectedIndex == 1)
            {
                if(SourceAccounts.SelectedIndex != -1)
                {
                    try
                    {
                        string userdataFolder = SourceFolder.Text + "\\" + (SourceAccounts.SelectedItem as Label).Content + "\\" + (SourceAreas.SelectedItem as Label).Content + "\\" + (SourceServers.SelectedItem as Label).Content + "\\" + (SourceRoles.SelectedItem as Label).Content;
                        Helper.GetZipFromUserdata(userdataFolder, "userdata.zip");
                    }
                    catch
                    {
                        Console.WriteLine("getZipFromUserdata() Run Error!");
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
                if (SourceAccounts.SelectedIndex != -1)
                {
                    string userdataFolder = TargetFolder.Text + "\\" + (TargetAccounts.SelectedItem as Label).Content + "\\" + (TargetAreas.SelectedItem as Label).Content + "\\" + (TargetServers.SelectedItem as Label).Content + "\\" + (TargetRoles.SelectedItem as Label).Content;
                    try
                    {
                        Helper.UnpackToUserdata("userdata.zip", userdataFolder);
                    }
                    catch
                    {
                        Console.WriteLine("UnpackToUserdata() Run Error!");
                        return;
                    }
                    try
                    {
                        WebClient wc = new WebClient();
                        wc.DownloadFile("http://47.101.177.238/userpreferencesasync.jx3dat", userdataFolder + "\\userpreferencesasync.jx3dat");
                    }
                    catch
                    {
                        Console.WriteLine("Download async settings Error!");
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
                "随便怎么用，不要商业即可 \n" +
                "软件版本 0.1.0", "关于");
        }
    }
}
