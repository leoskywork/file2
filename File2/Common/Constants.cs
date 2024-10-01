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

        public const string AppName = "File2";
        public const string AppAsFilePrefix = "file2";
        public const int ProgressUpdateRateInMS = 200;// 10;//100;//1000;
        public const int UIMessageOffsetInMS = ProgressUpdateRateInMS + 10;
        public const int TopFolderCount = 100;
        public const int TopFileCount = 500;
        public const int TopErrorCount = 100;

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
