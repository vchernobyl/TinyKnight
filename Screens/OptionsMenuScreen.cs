namespace TinyKnight
{
    class OptionsMenuScreen : MenuScreen
    {
        MenuEntry ungulateMenuEntry;
        MenuEntry languageMenuEntry;
        MenuEntry frobnicateMenuEntry;
        MenuEntry elfMenuEntry;

        enum Ungulate
        {
            BactrianCamel,
            Dromedary,
            LLama,
        }

        static Ungulate currentUngulate = Ungulate.Dromedary;
        static string[] languages = { "C#", "French", "Deoxybrinacucleic acid" };
        static int currentLanguage = 0;
        static bool frobnicate = true;
        static int elf = 23;

        public OptionsMenuScreen() : base(menuTitle: "Options")
        {
            ungulateMenuEntry = new MenuEntry(string.Empty);
            languageMenuEntry = new MenuEntry(string.Empty);
            frobnicateMenuEntry = new MenuEntry(string.Empty);
            elfMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            ungulateMenuEntry.Selected += UngulateMenuEntrySelected;
            languageMenuEntry.Selected += LanguageMenuEntrySelected;
            frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
            elfMenuEntry.Selected += ElfMenuEntrySelected;
            back.Selected += OnCancel;

            MenuEntries.Add(ungulateMenuEntry);
            MenuEntries.Add(languageMenuEntry);
            MenuEntries.Add(frobnicateMenuEntry);
            MenuEntries.Add(elfMenuEntry);
            MenuEntries.Add(back);
        }

        public void SetMenuEntryText()
        {
            ungulateMenuEntry.Text = $"Preferred ungulate: {currentUngulate}";
            languageMenuEntry.Text = $"Language: {languages[currentLanguage]}";
            frobnicateMenuEntry.Text = "Frobnicate: " + (frobnicate ? "on" : "off");
            elfMenuEntry.Text = $"Elf: {elf}";
        }

        void UngulateMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
        {
            currentUngulate++;
            if (currentUngulate > Ungulate.LLama)
                currentUngulate = 0;
            SetMenuEntryText();
        }

        void LanguageMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;
            SetMenuEntryText();
        }

        void FrobnicateMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
        {
            frobnicate = !frobnicate;
            SetMenuEntryText();
        }

        void ElfMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
        {
            elf++;
            SetMenuEntryText();
        }
    }
}
