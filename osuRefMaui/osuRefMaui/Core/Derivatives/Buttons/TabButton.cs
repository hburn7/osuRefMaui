using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using osuRefMaui.Core.Coloring;

namespace osuRefMaui.Core.Derivatives.Buttons
{
    public class TabButton : Button
    {
        /// <summary>
        /// Creates a new tab with the specified name
        /// </summary>
        /// <param name="channel">The channel this tab is mapped to</param>
        public TabButton(string channel)
        {
            Text = channel;
            TextColor = TabPalette.TabText;
            FontFamily = TabPalette.TabTextFontFamily;
            BackgroundColor = TabPalette.TabBackground;
            FontSize = TabPalette.FontSize;
            Padding = TabPalette.Padding;
            HorizontalOptions = TabPalette.HorizontalOptions;
            VerticalOptions = TabPalette.VerticalOptions;
        }
    }

    
}
