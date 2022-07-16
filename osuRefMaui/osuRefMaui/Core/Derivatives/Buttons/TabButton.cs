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
        /// <param name="name"></param>
        public TabButton(string name)
        {
            Text = name;
            TextColor = TabPalette.TabText;
            FontFamily = TabPalette.TabTextFontFamily;
            BackgroundColor = TabPalette.TabBackground;
            FontSize = TabPalette.FontSize;
            Padding = TabPalette.Padding;
            HorizontalOptions = TabPalette.HorizontalOptions;
            VerticalOptions = TabPalette.VerticalOptions;

            // todo: possibly set Clicked event handler here
        }
    }
}
