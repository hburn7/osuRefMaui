// ReSharper disable RedundantExtendsListEntry
// ReSharper disable SuggestBaseTypeForParameterInConstructor

namespace osuRefMaui;

public partial class App : Application
{
	public App(MainPage page)
	{
		InitializeComponent();

		MainPage = page;
	}
}