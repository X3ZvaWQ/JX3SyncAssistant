using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JX3SyncAssistant
{
    static class Helper
    {
        public static void GetZipFromUserdata(string SourceData, string zip)
        {
            string SourceDataFolder = SourceData;
            string[] files =
            {
                "addon.jx3dat",
                "CoinShopOutfitData.jx3dat",
                "custom.dat",
                "custom.dat.addon",
                "userpreferences.jx3dat"
            };
            try
            {
                using (FileStream fs = new FileStream(zip, FileMode.Create))
                using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    foreach (string file in files)
                    {
                        try
                        {
                            zipArchive.CreateEntryFromFile(SourceDataFolder + "\\" + file, file);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(SourceDataFolder + "\\" + file);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void UnpackToUserdata(string zip, string dir)
        {
            using (FileStream fs = new FileStream(zip, FileMode.Open))
            {
                using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Read))
                {
                    try
                    {
                        string[] files =
                        {
                            "addon.jx3dat",
                            "CoinShopOutfitData.jx3dat",
                            "custom.dat",
                            "custom.dat.addon",
                            "userpreferences.jx3dat",
                        };
                        foreach (string file in files)
                        {
                            File.Delete(dir + "\\" + file);
                        }
                        File.Delete(dir + @"\\userpreferencesasync.jx3dat");
                        Directory.CreateDirectory(dir);
                        foreach (var it in zipArchive.Entries)
                        {
                            it.ExtractToFile(dir + "\\" + it.Name);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Unpack Error");
                    }
                }
            }
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
    }
}
