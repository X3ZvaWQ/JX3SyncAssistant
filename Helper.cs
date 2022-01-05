using JX3SyncAssistant.Properties;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Resources;

namespace JX3SyncAssistant
{
    static class Helper
    {
        public static void PrintVersion()
        {
            Console.WriteLine(Application.Current.MainWindow);
        }

        public static string[] GetAllRoles(string UserDataFolder)
        {
            string[] results;
            ArrayList result_list = new ArrayList();
            if(!File.Exists($@"{UserDataFolder}\config.dat"))
            {
                return new string[0];
            }
            string[] accounts = Directory.GetDirectories(UserDataFolder);
            foreach(string account in accounts)
            {
                string account_name = Path.GetFileName(account);
                if (account == "fight_stat") continue;
                string[] areas = Directory.GetDirectories($@"{UserDataFolder}\{account_name}");
                foreach(string area in areas)
                {
                    string area_name = Path.GetFileName(area);
                    string[] servers = Directory.GetDirectories($@"{UserDataFolder}\{account_name}\{area_name}");
                    foreach (string server in servers)
                    {
                        string server_name = Path.GetFileName(server);
                        string[] roles = Directory.GetDirectories($@"{UserDataFolder}\{account_name}\{area_name}\{server_name}");
                        foreach(string role in roles)
                        {
                            string role_name = Path.GetFileName(role);
                            result_list.Add($"{role_name}|{server_name}|{account_name}|{area_name}");
                        }
                    }
                }
            }
            results = (string[])result_list.ToArray(typeof(string));
            return results;
        }

        public static string GetGlobalIdByIdAndServer(string id, string server, string SourceData) {
            string interFaceFolder = SourceData + @"\interface\";
            string[] roleInterFaceFolderSearchResult = Directory.GetDirectories(interFaceFolder + @"\MY#DATA", id, SearchOption.AllDirectories);
            if (roleInterFaceFolderSearchResult.Length > 0)
            {
                foreach (string roleInterFaceFolder in roleInterFaceFolderSearchResult) {
                    DirectoryInfo ParentDirectory = Directory.GetParent(roleInterFaceFolder);
                    string userInfo = File.ReadAllText(ParentDirectory.FullName + @"\info.jx3dat", Encoding.Default);
                    if (userInfo.Contains($"relserver=\"{server}\"") || userInfo.Contains($"server=\"{server}\""))
                    {
                        string roleId = ParentDirectory.Name.Split('@')[0];
                        return roleId;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public static string[] GetAllPreset() {
            string presetDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\preset";
            List<string> presetFiles = new List<string>();
            if (Directory.Exists(presetDirectory))
            {
                string[] _presetFiles = Directory.GetFiles(presetDirectory);
                for (int i = 0; i < _presetFiles.Length; i++) {
                    try
                    {
                        string presetFile = _presetFiles[i];
                        presetFile = Path.GetFileName(presetFile).Replace(".zip", "");
                        presetFile = presetFile.Replace("_", "/");
                        presetFile = Encoding.UTF8.GetString(Convert.FromBase64String(presetFile));
                        presetFiles.Add(presetFile);
                        
                    }
                    catch (Exception E) {
                        Console.WriteLine($@"Error: illegal filename at {_presetFiles[i]}");
                        Console.WriteLine(E.StackTrace);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(presetDirectory);
            }
            return presetFiles.ToArray();
        }

        public static void GetZipFromUserdata(string SourceData, string zip, Dictionary<string, string> roleInfo, Dictionary<string, bool> contain_options, TextBox logPanel)
        {
            Profile profile = new Profile {
                version = MainWindow.VERSION
            };
            Dictionary<string, string[]> allFiles= new Dictionary<string, string[]>();
            try
            {
                using (FileStream fs = new FileStream(zip, FileMode.Create))
                using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    //userdata设置迁移，最基本的数据文件
                    if(contain_options["userdata"])
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            "addon.jx3dat",
                            "CoinShopOutfitData.jx3dat",
                            "custom.dat",
                            "custom.dat.addon",
                            "userpreferences.jx3dat",
                            "hotkey.data",
                            "hotkey_last.txt",
                            "hotkey_back0.txt",
                            "hotkey_back1.txt",
                            "hotkey_back2.txt",
                            "hotkey_back3.txt",
                            "hotkey_back4.txt",
                        };
                        string userdataFolder = SourceData + $@"\userdata\{roleInfo["account"]}\{roleInfo["area"]}\{roleInfo["server"]}\{roleInfo["role"]}\";
                        foreach (string file in file_list)
                        {
                            try
                            {
                                string file_name = @"\userdata\" + file;
                                zipArchive.CreateEntryFromFile(userdataFolder + file, file_name);
                                files.Add(file_name);

                            }
                            catch (Exception E)
                            {
                                Log($"Take \"{userdataFolder}{file}\" into Zip File Error！", logPanel);
                                Log(E.Message, logPanel);
                                Log(E.StackTrace, logPanel);
                            }
                        }
                        if (contain_options["userdata_async"])
                        {
                            try
                            {
                                string file_name = @"\userdata\userpreferencesasync.jx3dat";
                                Uri uri = new Uri(@"pack://application:,,,/JX3SyncAssistant;component/Resources/userpreferencesasync.jx3dat", UriKind.Absolute);
                                using (FileStream fs_sync = new FileStream("userpreferencesasync.jx3dat", FileMode.Create))
                                {
                                    StreamResourceInfo resource = Application.GetResourceStream(uri);
                                    resource.Stream.CopyTo(fs_sync);
                                }
                                files.Add(file_name);
                            }
                            catch (Exception E)
                            {
                                Log("Excract \"userpreferencesasync.jx3dat\" Error！", logPanel);
                                Log(E.Message, logPanel);
                                Log(E.StackTrace, logPanel);
                            }
                            try
                            {
                                string file_name = @"\userdata\userpreferencesasync.jx3dat";
                                zipArchive.CreateEntryFromFile("userpreferencesasync.jx3dat", file_name);
                                File.Delete("userpreferencesasync.jx3dat");
                            }
                            catch (Exception E)
                            {
                                Log("Take \"userpreferencesasync.jx3dat\" into Zip File Error！", logPanel);
                                Log(E.Message, logPanel);
                                Log(E.StackTrace, logPanel);
                            }
                        }
                        else
                        {
                            try
                            {
                                string file_name = @"\userdata\userpreferencesasync.jx3dat";
                                zipArchive.CreateEntryFromFile(userdataFolder + "userpreferencesasync.jx3dat", file_name);
                                files.Add(file_name);
                            }
                            catch (Exception E)
                            {
                                Log($"Take {userdataFolder}\"userpreferencesasync.jx3dat\" into Zip File Error！", logPanel);
                                Log(E.Message, logPanel);
                                Log(E.StackTrace, logPanel);
                            }
                        }
                        allFiles.Add("userdata", (string[])files.ToArray(typeof(string)));
                    }
                    //看能不能找到需要迁移角色的globalId，因为剑心 茗伊这些插件储存的时候都是用globalId存储的
                    //只通过文件的话只能找到茗伊插件数据里面对应id的文件夹来获取globalId
                    //如果找不到的话就没办法进性插件相关的设置2迁移了
                    string roleId = GetGlobalIdByIdAndServer(roleInfo["role"], roleInfo["server"], SourceData);
                    
                    if (roleId == null)
                    {
                        Console.WriteLine("Cannot find out role id in MY#DATA, All role plugin setting will be skip.");
                    }
                    //剑心插件集的一些角色设置，喊话，技能释放什么的
                    if (contain_options["jx_role_config"] && roleId!=null)
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            $@"\interface\JX#DATA\killnotice\{roleId}.jx3dat",   //技能喊话
                            $@"\interface\JX#DATA\othernotice\{roleId}.jx3dat",  //入组 入帮之类的喊话
                            $@"\interface\JX#DATA\skillnotice\{roleId}.jx3dat"  //击杀提示
                        };
                        foreach (string file in file_list)
                        {
                            try
                            {
                                string file_name = file.Replace($"{roleId}", "roleId");
                                zipArchive.CreateEntryFromFile(SourceData + file, file_name);
                                files.Add(file_name);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Take \"{SourceData}{file}\" into Zip File Error！");
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                        allFiles.Add("jx_role_config", (string[])files.ToArray(typeof(string)));
                    }
                    //茗伊插件集的角色设置
                    if (contain_options["my_role_config"] && roleId!=null)
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {

                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\anmerkungen.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\focus.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\infotip.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\memo.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\my_targetmon.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\settings.db",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\storageversion.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\config\tutorialed.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn_hd\manifest.jx3dat"
                        };
                        foreach (string file in file_list)
                        {
                            try
                            {
                                string file_name = file.Replace($"{roleId}@zhcn_hd", "roleId");
                                zipArchive.CreateEntryFromFile(SourceData + file, file_name);
                                files.Add(file_name);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Take \"{SourceData}{file}\" into Zip File Error！"); 
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                        allFiles.Add("my_role_config", (string[])files.ToArray(typeof(string)));

                    }
                    //剑心插件集的全局设置
                    if (contain_options["jx_config"])
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            $@"\interface\JX#DATA\hiderepresent\HideActionData.jx3dat",
                            $@"\interface\JX#DATA\skillcastmode\CastModeData.jx3dat",
                            $@"\interface\JX#DATA\targetlist\CustomMod.jx3dat",
                            $@"\interface\JX#DATA\targetlist\FocusTarget.jx3dat",
                            $@"\interface\JX#DATA\CustomData.jx3dat",
                        };
                        foreach (string file in file_list)
                        {
                            try
                            {
                                zipArchive.CreateEntryFromFile(SourceData + file, file);
                                files.Add(file);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Take \"{SourceData}{file}\" into Zip File Error！");
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                        allFiles.Add("jx_config", (string[])files.ToArray(typeof(string)));
                    } 
                    //茗伊插件集的全局设置
                    if (contain_options["my_config"])
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\settings.db",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\yy.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\show_notify.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\serendipity_autoshare.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\screenshot.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\my_targetmon.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\memo.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\fontconfig.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\chatmonitor.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\chatblockwords.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\cataclysm\common.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\focus\common.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\config\xlifebar\common.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn_hd\manifest.jx3dat"
                        };
                        if(Directory.Exists(SourceData + @"\interface\MY#DATA\!all-users@zhcn_hd\userdata\TargetMon"))
                        {
                            string[] TargetMonFiles = Directory.GetFiles(SourceData + @"\interface\MY#DATA\!all-users@zhcn_hd\userdata\TargetMon");
                            foreach (string TargetMonFile in TargetMonFiles)
                            {
                                string file_name = $@"\interface\MY#DATA\!all-users@zhcn_hd\userdata\TargetMon\{Path.GetFileName(TargetMonFile)}";
                                try
                                {
                                    zipArchive.CreateEntryFromFile(SourceData + file_name, file_name);
                                    files.Add(file_name);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine($"Take \"{SourceData}{file_name}\" into Zip File Error！");
                                    Console.WriteLine(e.StackTrace);
                                }
                            }
                        }

                        foreach (string file in file_list)
                        {
                            try
                            {
                                zipArchive.CreateEntryFromFile(SourceData + file, file);
                                files.Add(file);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Take \"{SourceData}{file}\" into Zip File Error！");
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                        allFiles.Add("my_config", (string[])files.ToArray(typeof(string)));
                    }
                    
                    string[] contains = allFiles.Keys.ToArray<string>();
                    profile.contains = contains;
                    profile.files = allFiles;
                    try
                    {
                        File.WriteAllText("profile.json", JsonSerializer.Serialize(profile));
                        zipArchive.CreateEntryFromFile("profile.json", "profile.json");
                        File.Delete("profile.json");
                    }
                    catch(Exception e)
                    {

                        Log($"Delete Temp File \"profile.json\" File Error！", logPanel);
                        Log(e.Message, logPanel);
                        Log(e.StackTrace, logPanel);
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void UnpackToUserdata(string TargetGameFolder, string zip, Dictionary<string, string> roleInfo, TextBox logPanel)
        {
            using (FileStream fs = new FileStream(zip, FileMode.Open))
            using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                try
                {
                    zipArchive.GetEntry("profile.json").ExtractToFile("profile.json");
                    Profile profile = JsonSerializer.Deserialize<Profile>(File.ReadAllText("profile.json"));
                    File.Delete("profile.json");
                    if(profile.version != MainWindow.VERSION) {
                        Log("WARN: The description document version of the data is inconsistent with the program version. This may cause data migration errors.", logPanel);
                    }
                    Dictionary<string, string[]> files = profile.files;
                    //释放userdata的文件
                    if(files.ContainsKey("userdata"))
                    {
                        string userdataFolder = TargetGameFolder + $@"\userdata\{roleInfo["account"]}\{roleInfo["area"]}\{roleInfo["server"]}\{roleInfo["role"]}\";
                        foreach (string f in Directory.GetFileSystemEntries(userdataFolder))
                        {
                            if (File.Exists(f))
                            {
                                File.Delete(f);
                            }
                        }
                        foreach (string file in files["userdata"])
                        {
                            try
                            {
                                ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                zipArchiveEntry.ExtractToFile(userdataFolder + Path.GetFileName(file), true);
                                Helper.Log($"INFO: File \"{file}\" has been extract to \"{userdataFolder + Path.GetFileName(file)}\"", logPanel);
                            }
                            catch (Exception E)
                            {
                                Helper.Log(E.Message, logPanel);
                                Helper.Log(E.StackTrace, logPanel);
                            }
                        }
                    }
                    //释放剑心插件集的全局文件
                    if(files.ContainsKey("jx_config"))
                    {
                        foreach (string file in files["jx_config"])
                        {
                            try
                            {
                                ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                string targetExtractFolder = Path.GetDirectoryName(TargetGameFolder + file);
                                Directory.CreateDirectory(targetExtractFolder);
                                zipArchiveEntry.ExtractToFile(TargetGameFolder + file, true);
                                Helper.Log($"INFO: File \"{file}\" has been extract to \"{TargetGameFolder + file}\"", logPanel);

                            }
                            catch (Exception E)
                            {
                                Helper.Log(E.Message, logPanel);
                                Helper.Log(E.StackTrace, logPanel);
                            }
                        }
                    }
                    //释放茗伊插件集的全局文件
                    if (files.ContainsKey("my_config"))
                    {
                        foreach (string file in files["my_config"])
                        {
                            try
                            {
                                ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                string targetExtractFolder = Path.GetDirectoryName(TargetGameFolder + file);
                                Directory.CreateDirectory(targetExtractFolder);
                                zipArchiveEntry.ExtractToFile(TargetGameFolder + file, true);
                                Helper.Log($"INFO: File \"{file}\" has been extract to \"{TargetGameFolder + file}\"", logPanel);
                            }
                            catch (Exception E)
                            {
                                Helper.Log(E.Message, logPanel);
                                Helper.Log(E.StackTrace, logPanel);
                            }
                        }
                    }

                    string roleId = GetGlobalIdByIdAndServer(roleInfo["role"], roleInfo["server"], TargetGameFolder);
                    if (roleId == null)
                    {
                        Console.WriteLine("Cannot find out role id in MY#DATA, All role plugin setting will be skip.");
                    }

                    if (files.ContainsKey("jx_role_config") && roleId!=null)
                    {
                        foreach (string file in files["jx_role_config"])
                        {
                            try
                            {
                                string file_name = file.Replace("roleId", $"{roleId}");
                                ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                string targetExtractFolder = Path.GetDirectoryName(TargetGameFolder + file_name);
                                Directory.CreateDirectory(targetExtractFolder);
                                zipArchiveEntry.ExtractToFile(TargetGameFolder + file_name, true);
                                Helper.Log($"INFO: File \"{file}\" has been extract to \"{TargetGameFolder + file_name}\"", logPanel);

                            }
                            catch (Exception E)
                            {
                                Helper.Log(E.Message, logPanel);
                                Helper.Log(E.StackTrace, logPanel);
                            }
                        }
                    }

                    if (files.ContainsKey("my_role_config") && roleId!=null)
                    {
                        foreach (string file in files["my_role_config"])
                        {
                            try
                            {
                                string file_name = file.Replace("roleId", $"{roleId}@zhcn_hd");
                                ZipArchiveEntry zipArchiveEntry = zipArchive.GetEntry(file);
                                string targetExtractFolder = Path.GetDirectoryName(TargetGameFolder + file_name);
                                Directory.CreateDirectory(targetExtractFolder);
                                zipArchiveEntry.ExtractToFile(TargetGameFolder + file_name, true);
                                Helper.Log($"INFO: File \"{file}\" has been extract to \"{TargetGameFolder + file_name}\"", logPanel);
                            }
                            catch (Exception E)
                            {
                                Helper.Log(E.Message, logPanel);
                                Helper.Log(E.StackTrace, logPanel);
                            }
                        }
                    }
                
                }
                catch(Exception e)
                {
                    throw e;
                }
            }

            File.Delete(zip);
        }

        public static string GetGameFolder(bool isExp, TextBox LogPanel)
        {
            string result;
            result = GetGameFolderFromSettings(isExp, LogPanel);
            if (result != null) return result;
            result = GetGameFolderFromInstallerReg(isExp, LogPanel);
            if (result != null) return result; 
            result = GetGameFolderFromKingsoftReg(isExp, LogPanel);
            if (result != null) return result;
            result = GetGameFolderFromLauncher(isExp, LogPanel);
            if (result != null) return result;
            return null;
        }

        public static string GetGameFolderFromLauncher(bool isExp, TextBox LogPanel)
        {
            string result = "";
            Log("INFO: Try to find the path through the running launcher", LogPanel);
            try
            {
                Process[] processes = Process.GetProcessesByName("XLauncher");
                if (processes.Length == 0)
                {
                    Log("WARN: The running launcher was not found. Try to find the path through the registry", LogPanel);
                    return null;
                }
                foreach (Process process in processes)
                {
                    string basePath = Path.GetDirectoryName(process.MainModule.FileName);
                    if (isExp)
                    {
                        if (Directory.Exists($@"{basePath}\Game\JX3_EXP\bin\zhcn_exp"))
                        {
                            Log($"INFO: The running launcher was found. The determined path is \"{basePath}\\Game\\JX3_EXP\\bin\\zhcn_exp\"", LogPanel);
                            result = $@"{basePath}\Game\JX3_EXP\bin\zhcn_exp";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (Directory.Exists($@"{basePath}\Game\JX3\bin\zhcn_hd"))
                        {
                            Log($"INFO: The running launcher was found. The determined path is \"{basePath}\\Game\\JX3\\bin\\zhcn_hd\"", LogPanel);
                            result = $@"{basePath}\Game\JX3\bin\zhcn_hd";
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                return result;
            }
            catch(Exception E)
            {
                Log("ERROR: Try to find the path through the registry running launcher error", LogPanel);
                Log(E.Message, LogPanel);
                Log(E.StackTrace, LogPanel);
                return null;
            }
            
        }

        public static string GetGameFolderFromInstallerReg(bool isExp, TextBox LogPanel)
        {
            Log("INFO: Try to find the path through the register \"HKEY_LOCAL_MACHINE\\SOFTWARE\\JX3Installer\"", LogPanel);
            RegistryKey localMachine = Environment.Is64BitOperatingSystem == true ?
                            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) :
                            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            string registry_key = !isExp ? @"SOFTWARE\JX3Installer" : @"SOFTWARE\JX3Installer_EXP";
            string basePath = "";
            try
            {
                basePath = localMachine.OpenSubKey(registry_key, false).GetValue("InstPath").ToString();
            }
            catch
            {
                Log($"WARN: the register \"HKEY_LOCAL_MACHINE\\SOFTWARE\\JX3Installer\" not found", LogPanel);
                Log("INFO: Try to find the path through the register \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Kingsoft\\JX3|JX3_EXP\"", LogPanel);
                return null;
            }
            if (isExp)
            {
                if (Directory.Exists($@"{basePath}\Game\JX3_EXP\bin\zhcn_exp"))
                {
                    Log($"INFO: The determined path is \"{basePath}\\Game\\JX3_EXP\\bin\\zhcn_exp\"", LogPanel);
                    return $@"{basePath}\Game\JX3_EXP\bin\zhcn_exp";
                }
            }
            else
            {
                if (Directory.Exists($@"{basePath}\Game\JX3\bin\zhcn_hd"))
                {
                    Log($"INFO: The determined path is \"{basePath}\\Game\\JX3\\bin\\zhcn_hd\"", LogPanel);
                    return $@"{basePath}\Game\JX3\bin\zhcn_hd";
                }
            }
            Log("INFO: Try to find the path through the register \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Kingsoft\\JX3|JX3_EXP\"", LogPanel);
            return null;
        }

        public static string GetGameFolderFromKingsoftReg(bool isExp, TextBox LogPanel)
        {
            Log("INFO: Try to find the path through the register \"HKEY_LOCAL_MACHINE\\SOFTWARE\\JX3Installer\"", LogPanel);
            RegistryKey localMachine = Environment.Is64BitOperatingSystem == true ?
                            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) :
                            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            string registry_key = !isExp ? @"SOFTWARE\Kingsoft\JX3\zhcn_hd" : @"SOFTWARE\Kingsoft\JX3\zhcn_exp";
            string basePath;
            try
            {
                basePath = localMachine.OpenSubKey(registry_key, false).GetValue("installPath").ToString();
            }
            catch
            {
                Log($"WARN: the register \"SOFTWARE\\Kingsoft\\JX3\" not found", LogPanel);
                Log("WARN: Sorry, Please select folder by yourself", LogPanel);
                return null;
            }
            if (isExp)
            {
                if (Directory.Exists($@"{basePath}\Game\JX3_EXP\bin\zhcn_exp"))
                {
                    Log($"INFO: The determined path is \"{basePath}\"", LogPanel);
                    return $@"{basePath}";
                }
            }
            else
            {
                if (Directory.Exists($@"{basePath}\Game\JX3\bin\zhcn_hd"))
                {
                    Log($"INFO: The determined path is \"{basePath}\"", LogPanel);
                    return $@"{basePath}";
                }
            }
            Log("WARN: Sorry, Please select folder by yourself", LogPanel);
            return null;
        }

        public static string GetGameFolderFromSettings(bool isExp, TextBox LogPanel)
        {
            string result;
            if (isExp)
            {
                result = Settings.Default.ExpGameFolder;
            }
            else
            {
                result = Settings.Default.GameFolder;
            }

            if (result != null && result != "") {
                Log($"INFO: The determined path is \"{result}\" from settings", LogPanel);
                return result;
            };
            return null;
        }

        public static void Log(string str, TextBox logPanel)
        {
            logPanel.Text = logPanel.Text + "\n" + str;
            Console.WriteLine(str);
        }
    
        public static void FromComboToLayer(ListBox lb, ComboBox account, ComboBox area, ComboBox server, ComboBox role)
        {
            if (lb.SelectedIndex == -1) return;
            string str = (string)(lb.SelectedItem as Label).Content;
            string[] roleInfo = str.Split('|');
            ComboBox[] cbs = { account, area, server, role };
            int[] ints = { 2, 3, 1, 0 };
            for (int i = 0; i < 4; i++)
            {
                int j = 0;
                foreach (Label cbi in cbs[i].Items)
                {
                    if ((string)cbi.Content == roleInfo[ints[i]])
                    {
                        cbs[i].SelectedIndex = j;
                        j = 0;
                        break;
                    }
                    j++;
                }
            }
        }

        public static void FromLayerToCombo(ListBox lb, ComboBox account, ComboBox area, ComboBox server, ComboBox role)
        {
            if (role.SelectedIndex == -1) return;
            Dictionary<string, string> roleInfo = new Dictionary<string, string> {
                            { "account", (string)(account.SelectedItem as Label).Content },
                            { "area", (string)(area.SelectedItem as Label).Content },
                            { "server", (string)(server.SelectedItem as Label).Content },
                            { "role", (string)(role.SelectedItem as Label).Content }
                        };
            string str = $"{roleInfo["role"]}|{roleInfo["server"]}|{roleInfo["account"]}|{roleInfo["area"]}";
            int j = 0;
            foreach (Label cbi in lb.Items)
            {
                if ((string)cbi.Content == str)
                {
                    lb.SelectedIndex = j;
                    lb.ScrollIntoView(cbi);
                    j = 0;
                    break;
                }
                j++;
            }
        }

        public async static Task<Release[]> GetVersionsFromGiteeRelease(TextBox LogPanel)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                string versionsString = await wc.DownloadStringTaskAsync("https://gitee.com/api/v5/repos/x3zvawq/JX3SyncAssistant/releases");
                Release[] versions = JsonSerializer.Deserialize<Release[]>(versionsString);
                return versions;
            }
            catch(Exception E)
            {
                Log(E.Message, LogPanel);
                Log(E.StackTrace, LogPanel);
                Log("ERROR: Get new version info error! please check your network", LogPanel);
                return new Release[0];
            }
        }
   
    }
}
