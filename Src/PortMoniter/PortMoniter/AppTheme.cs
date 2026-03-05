using System;
using System.Collections.Generic;
using System.Windows;

namespace PortMoniter
{
    public class AppTheme
    {
        public static void ChangeTheme(List<Uri> themeuri)
        {
            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = themeuri[0] });
            Application.Current.Resources.MergedDictionaries.RemoveAt(1);
            Application.Current.Resources.MergedDictionaries.Insert(1, new ResourceDictionary { Source = themeuri[1] });
            Application.Current.Resources.MergedDictionaries.RemoveAt(7);
            Application.Current.Resources.MergedDictionaries.Insert(7, new ResourceDictionary { Source = themeuri[2] });
            
        }
    }
}
