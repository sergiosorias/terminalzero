using System;
using System.Collections.Generic;
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
            set { this.newalue = value; }
        }

        public string Key
        {
            get { return key; }            
        }

        public SetVarScriptLine(string Line)
            : base("setvar")
        {
            string[] lineParams = Line.Split(' ');
            key = lineParams[1];

            int start = Line.IndexOf('"');
            int end = Line.LastIndexOf('"');
            this.Value = Line.Substring(start + 1, end - start - 1);
        }

        public override void Execute(DeployFile deployFile,ref StringBuilder outputFile)
        {
            outputFile.Replace("$(" + this.key + ")", this.NewValue);
        }

        public bool ReplaceAlways()
        {
            return (string.Equals(this.Key, "databasename",StringComparison.OrdinalIgnoreCase));
        }
    }
}
