using System.Linq;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using OrigindLauncher.Resources.Configs;

namespace OrigindLauncher.Resources.UI
{
    public static class ThemeManager
    {
        private static readonly SwatchesProvider SwatchesProvider = new SwatchesProvider();

        private static Swatch GetSwatchFromName(string name)
        {
            return SwatchesProvider.Swatches.FirstOrDefault(s => s.Name == name);
        }

        public static void ChangeLightDark()
        {
            Config.Instance.ThemeConfig.IsDark = !Config.Instance.ThemeConfig.IsDark;
            new PaletteHelper().SetLightDark(Config.Instance.ThemeConfig.IsDark);
            Config.Save();
        }

        public static void Init()
        {
            var paletteHelper = new PaletteHelper();
            paletteHelper.SetLightDark(Config.Instance.ThemeConfig.IsDark);
            paletteHelper.ReplaceAccentColor(GetSwatchFromName(Config.Instance.ThemeConfig.AccentName));
            paletteHelper.ReplacePrimaryColor(GetSwatchFromName(Config.Instance.ThemeConfig.PrimaryName));
        }

        public static void SavePalette()
        {
            /*
            var palette = new PaletteHelper().QueryPalette();
            Config.Instance.ThemeConfig.AccentName = palette.AccentSwatch.Name;
            Config.Instance.ThemeConfig.PrimaryName = palette.PrimarySwatch.Name;
            */
        }
    }
}