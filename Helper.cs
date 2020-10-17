using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Windows.Controls;

namespace JX3SyncAssistant
{
    static class Helper
    {
        public static Dictionary<string, string> getAllRoles()
        {

        }

        public static void GetZipFromUserdata(string SourceData, string zip, Dictionary<string, string> roleInfo, Dictionary<string, bool> contain_options, TextBox logPanel)
        {
            Profile profile = new Profile {
                version = MainWindow.VERSION,
                created_at = DateTime.Now
            };
            Dictionary<string, string[]> allFiles= new Dictionary<string, string[]>();
            try
            {
                using (FileStream fs = new FileStream(zip, FileMode.Create))
                using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    if(contain_options["userdata"])
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            "addon.jx3dat",
                            "CoinShopOutfitData.jx3dat",
                            "custom.dat",
                            "custom.dat.addon",
                            "userpreferences.jx3dat"
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
                            catch (Exception)
                            {
                                Console.WriteLine($"Take \"{userdataFolder}{file}\" into Zip File Error！");
                            }
                        }
                        if (contain_options["userdata_async"])
                        {
                            try
                            {
                                WebClient wc = new WebClient();
                                wc.DownloadFile("http://47.101.177.238/userpreferencesasync.jx3dat", "userpreferencesasync.jx3dat");
                                wc.Dispose();
                            }
                            catch
                            {
                                Console.WriteLine("Download \"userpreferencesasync.jx3dat\" Error！");
                            }
                            try
                            {
                                string file_name = @"\userdata\userpreferencesasync.jx3dat";
                                zipArchive.CreateEntryFromFile("userpreferencesasync.jx3dat", file_name);
                                File.Delete("userpreferencesasync.jx3dat");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Take \"userpreferencesasync.jx3dat\" into Zip File Error！");
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
                            catch (Exception)
                            {
                                Console.WriteLine($"Take {userdataFolder}\"userpreferencesasync.jx3dat\" into Zip File Error！");
                            }
                        }
                        allFiles.Add("userdata", (string[])files.ToArray(typeof(string)));
                    }
                   
                    bool RoleIdEnable = false;
                    string roleId = "";
                    string interFaceFolder = SourceData + @"\interface\";
                    string[] roleInterFaceFolderSearchResult = Directory.GetDirectories(interFaceFolder + @"\MY#DATA", roleInfo["role"], SearchOption.AllDirectories);
                    if (roleInterFaceFolderSearchResult.Length > 0)
                    {
                        RoleIdEnable = true;
                        roleId = Directory.GetParent(roleInterFaceFolderSearchResult[0]).Name.Split('@')[0];
                    }
                    else
                    {
                        Console.WriteLine("Cannot find out role id in MY#DATA, All role plugin setting will be skip.");
                    }
                    
                    if (contain_options["jx_role_config"] && RoleIdEnable)
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            $@"\interface\JX#DATA\killnotice\{roleId}.jx3dat",
                            $@"\interface\JX#DATA\othernotice\{roleId}.jx3dat",
                            $@"\interface\JX#DATA\skillnotice\{roleId}.jx3dat"
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

                    if (contain_options["my_role_config"] && RoleIdEnable)
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {

                            $@"\interface\MY#DATA\{roleId}@zhcn\config\anmerkungen.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\config\focus.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\config\infotip.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\config\memo.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\config\my_targetmon.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\config\storageversion.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\config\tutorialed.jx3dat",
                            $@"\interface\MY#DATA\{roleId}@zhcn\manifest.jx3dat"
                        };
                        foreach (string file in file_list)
                        {
                            try
                            {
                                string file_name = file.Replace($"{roleId}@zhcn", "role");
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

                    if (contain_options["my_config"])
                    {
                        ArrayList files = new ArrayList();
                        string[] file_list =
                        {
                            @"\interface\MY#DATA\!all-users@zhcn\config\yy.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\show_notify.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\serendipity_autoshare.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\screenshot.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\my_targetmon.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\memo.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\fontconfig.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\chatmonitor.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\chatblockwords.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\cataclysm\common.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\focus\common.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\config\xlifebar\common.jx3dat",
                            @"\interface\MY#DATA\!all-users@zhcn\manifest.jx3dat"
                        };
                        if(Directory.Exists(SourceData + @"\interface\MY#DATA\!all-users@zhcn\userdata\TargetMon"))
                        {
                            string[] TargetMonFiles = Directory.GetFiles(SourceData + @"\interface\MY#DATA\!all-users@zhcn\userdata\TargetMon");
                            foreach (string TargetMonFile in TargetMonFiles)
                            {
                                string file_name = $@"\interface\MY#DATA\!all-users@zhcn\userdata\TargetMon\{Path.GetFileName(TargetMonFile)}";
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
                        Console.WriteLine($"Delete Temp File \"profile.json\" File Error！");
                        Console.WriteLine(e.StackTrace);
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
                        Helper.Log("WARN: The description document version of the data is inconsistent with the program version. This may cause data migration errors.", logPanel);
                    }
                    Dictionary<string, string[]> files = profile.files;
                    if(files.ContainsKey("userdata"))
                    {
                        string userdataFolder = TargetGameFolder + $@"\userdata\{roleInfo["account"]}\{roleInfo["area"]}\{roleInfo["server"]}\{roleInfo["role"]}\";
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

                    bool RoleIdEnable = false;
                    string roleId = "";
                    string interFaceFolder = TargetGameFolder + @"\interface\";
                    string[] roleInterFaceFolderSearchResult = Directory.GetDirectories(interFaceFolder + @"\MY#DATA", roleInfo["role"], SearchOption.AllDirectories);
                    if (roleInterFaceFolderSearchResult.Length > 0)
                    {
                        RoleIdEnable = true;
                        roleId = Directory.GetParent(roleInterFaceFolderSearchResult[0]).Name.Split('@')[0];
                    }
                    else
                    {
                        Helper.Log("WARN: Cannot find out role id in MY#DATA, All role plugin setting will be skip.", logPanel);
                    }
                    if (files.ContainsKey("jx_role_config") && RoleIdEnable)
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

                    if (files.ContainsKey("my_role_config") && RoleIdEnable)
                    {
                        foreach (string file in files["my_role_config"])
                        {
                            try
                            {
                                string file_name = file.Replace("role", $"{roleId}@zhcn");
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

        public static string GetGameFolderFromReg(bool isExp)
        {
            RegistryKey localMachine = Environment.Is64BitOperatingSystem == true ?
                            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) :
                            RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            string registry_key = !isExp ? @"SOFTWARE\JX3Installer" : @"SOFTWARE\JX3Installer_EXP";
            string result = localMachine.OpenSubKey(registry_key, false).GetValue("InstPath").ToString();
            return result;
        }

        public static void Log(string str, TextBox logPanel)
        {
            logPanel.Text = logPanel.Text + "\n" + str;
            Console.WriteLine(str);
        }
    }
}
