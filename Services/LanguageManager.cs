using System;
using System.Windows;
using Microsoft.Win32;

namespace URUManager.Services
{
    public static class LanguageManager
    {
        private const string RegistryKey = @"Software\URUManager";
        private const string RegistryValue = "Language";

        public static string CurrentLanguage { get; private set; } = "en";

        public static void Initialize()
        {
            string lang = LoadSaved()
                ?? (System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "de" : "en");
            Apply(lang, save: false);
        }

        public static void Apply(string lang, bool save = true)
        {
            CurrentLanguage = lang;

            var dicts = Application.Current.Resources.MergedDictionaries;
            for (int i = dicts.Count - 1; i >= 0; i--)
            {
                string src = dicts[i].Source != null ? dicts[i].Source.ToString() : "";
                if (src.EndsWith("/en.xaml") || src.EndsWith("/de.xaml"))
                {
                    dicts.RemoveAt(i);
                    break;
                }
            }

            dicts.Add(new ResourceDictionary
            {
                Source = new Uri("Resources/" + lang + ".xaml", UriKind.Relative)
            });

            if (save)
                TrySave(lang);
        }

        private static string LoadSaved()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(RegistryKey))
                {
                    if (key != null)
                    {
                        string v = key.GetValue(RegistryValue) as string;
                        if (v == "en" || v == "de") return v;
                    }
                }
            }
            catch { }
            return null;
        }

        private static void TrySave(string lang)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RegistryKey))
                    key.SetValue(RegistryValue, lang);
            }
            catch { }
        }
    }
}
