using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroUpdateManager.Database.ScriptLines
{
    public class OnScriptLine: ScriptLine        
    {
        private string errorType;

        public string ErrorType
        {
            get { return errorType; }            
        }

        public OnScriptLine(string Line)
            :base("on")
        {
            string[] lineParams = Line.Split(' ');
            errorType = lineParams[2];            
        }

        public override void Execute(DeployFile deployFile,ref StringBuilder outputFile)
        {
            
        }
    }
}
