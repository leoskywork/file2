namespace File2
{
    class Constants
    {
        public static readonly string[] ImportantFolders = new string[] {"C:\\Program Files",
            "C:\\Program Files (x86)",
            "C:\\Windows" };
    }

    interface ILanguage
    {
        string AggregageFile { get; }
        string AggregateFileSource { get; }
        string AggregateFileTarge { get; }
    }



    class LanguageEnglish : ILanguage
    {
        private readonly string _culture;

        public LanguageEnglish(string culture)
        {
            _culture = culture;
        }

        public string AggregageFile => "Aggregate file";

        public string AggregateFileSource => "Source folder: ";

        public string AggregateFileTarge => "Target folder: ";
    }



}
