using System.IO;
using System.Text;
using Microsoft.Win32;

//Implementierung der Abstände in die CSV Datei
const string FORMAT = "{0,-100}|{1,-20}|{2,-30}|{3,-8}\n";
      
void LogInstalledSoftware()
{
    //Display in Tabbellen Form 
    var line = string.Format(FORMAT, "DisplayName", "Version", "Publisher", "InstallDate");
    var sb = new StringBuilder(line, 100000);
    //Registry der 32Bit- und 64Bit-Versionen wird nach installierter Software ausgelesen (Uninstallierte Software wird nicht aufgeführt)
    sb.Append($"\n[32 bit section]\n\n");
    ReadRegistryUninstall(ref sb, RegistryView.Registry32);
    sb.Append($"\n[64 bit section]\n\n");
    ReadRegistryUninstall(ref sb, RegistryView.Registry64);
    //CSV Ausgabe auf Desktop implentiert
    File.WriteAllText(@"C:\Users\felix.kupper\Desktop\SoftwareFinder.csv", sb.ToString());
}

//Die Suche wird zu den Registry Keys navigiert "regedit bei Windows ausführen" und dem entsprechenden Pfad zugewiesen.
//Dann wird die Local Maschine nach den existierenden Keys und subkeys durchsucht und angezeigt. Keys exisitieren nur, solange die Software o.a. installiert ist.
//Zu finden sind die Unterschlüssel unter "Programme". Da aber zum Beispiel Programme wie Dropbox unter anderen subnetzes Verknüpft wird, wird die Suche in der LocalMaschine Systemweit verwendet.
static void ReadRegistryUninstall(ref StringBuilder sb, RegistryView view)
{
    const string REGISTRY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
    using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
    using var subKey = baseKey.OpenSubKey(REGISTRY_KEY);
    foreach (string subkey_name in subKey.GetSubKeyNames())
    {
        using RegistryKey key = subKey.OpenSubKey(subkey_name);
        if (!string.IsNullOrEmpty(key.GetValue("DisplayName") as string))
        {
            var line = string.Format(FORMAT,
                key.GetValue("DisplayName"),
                key.GetValue("DisplayVersion"),
                key.GetValue("Publisher"),
                key.GetValue("InstallDate"));
            sb.Append(line);
        }
        key.Close();
    }
    subKey.Close();
    baseKey.Close();
}

LogInstalledSoftware();