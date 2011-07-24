using System.IO;

namespace ZeroCommonClasses.Environment
{
    public class Directories
    {
        public static string ExtrasFolder
        {
            get { return Path.Combine(System.Environment.CurrentDirectory, "Extras"); }
        }
        public const string WorkingDirSubfix = ".WD";

        static Directories()
        {

            if (!Directory.Exists(ModulesFolder))
                Directory.CreateDirectory(ModulesFolder);
            if (!Directory.Exists(UpgradeFolder))
                Directory.CreateDirectory(UpgradeFolder);

        }

        public static string ModulesFolder
        {
            get { return Path.Combine(System.Environment.CurrentDirectory, "Modules"); }
        }

        public static string UpgradeFolder
        {
            get { return Path.Combine(System.Environment.CurrentDirectory, "Upgrade"); }
        }

        public static string ApplicationFolder
        {
            get { return System.Environment.CurrentDirectory; }
        }

        public static string ApplicationPath
        {
            get
            {
                if (System.Environment.GetCommandLineArgs().Length == 0)
                    return Path.Combine(System.Environment.CurrentDirectory, "");

                return System.Environment.GetCommandLineArgs()[0];
            }
        }

        public static string ApplicationUpdaterPath
        {
            get
            {
                return string.Format("{0}.Updater.exe", ApplicationPath);
            }
        }

    }
}