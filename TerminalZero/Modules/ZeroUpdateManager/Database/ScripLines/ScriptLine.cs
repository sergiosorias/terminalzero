using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroUpdateManager.Database.ScriptLines
{
    public abstract class ScriptLine
    {
        private string commandText;
        private int lineNumber;
        
        public int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }
	

        public string CommandText
        {
            get { return commandText; }            
        }

        public ScriptLine(string CommandText)
        {
            this.commandText = CommandText;
            
        }

        internal static ScriptLine FromString(string line, int lineNumber)
        {
            ScriptLine result = null;
            if (line.StartsWith(":setvar "))            
                result = new SetVarScriptLine(line);
            else if (line.StartsWith(":on "))
                result = new OnScriptLine(line);
            else
            {
                throw new Exception("Error line Number: " + lineNumber.ToString() + ". script line unknow: " + line);
            }
            if (result!=null)
                result.LineNumber = lineNumber;
            return result;
        }

        public abstract void Execute(DeployFile deployFile, ref StringBuilder outputFile);
        
    }
}
