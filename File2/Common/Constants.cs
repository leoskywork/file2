namespace File2.Common
{
    class Constants
    {
        public static readonly string[] ImportantFolders = new string[] {
            "C:\\",
            "C:\\$WinREAgent",
            "C:\\inetpub",
            "C:\\Program Files",
            "C:\\Program Files (x86)",
            "C:\\ProgramData",
            "C:\\Recovery",
            "C:\\Windows",
            "C:\\Users"
        };
        public static readonly string AppName = "File2";
        public static readonly int ProgressUpdateRateInMS = 200;// 10;//100;//1000;
        public static readonly int UIMessageOffsetInMS = ProgressUpdateRateInMS + 10;
        public static readonly int TopFolderCount = 100;
        public static readonly int TopFileCount = 500;
        public static readonly int TopErrorCount = 100;
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
