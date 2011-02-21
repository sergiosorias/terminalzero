using System;
using System.Text;

namespace ZeroUpdateManager.Database.ScriptLines
{
    public class SetVarScriptLine: ScriptLine
    {
        private string key;
        private string value;
        private string newalue;
        
        public string Value
        {
            get { return value; }
            set { 
                this.value = value;
                newalue = value;
            }
        }

        public string NewValue
        {
            get { return newalue; }
            set { newalue = value; }
        }

        public string Key
        {
            get { return key; }            
        }

        public SetVarScriptLine(string Line)
            : base("setvar")
        {
            string[] lineParams = Line.Split(" ".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
            key = lineParams[1];

            Value = lineParams[2].Replace("\"","");
        }

        public override void Execute(DeployFile deployFile,ref StringBuilder outputFile)
        {
            outputFile.Replace("$(" + key + ")", NewValue);
        }

        public bool ReplaceAlways
        {
            get { return (string.Equals(Key, "databasename", StringComparison.OrdinalIgnoreCase)); }
        }
    }
}
