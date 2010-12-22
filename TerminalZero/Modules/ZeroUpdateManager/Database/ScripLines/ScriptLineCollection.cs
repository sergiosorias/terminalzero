using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace ZeroUpdateManager.Database.ScriptLines
{
    public class ScriptLineCollection: Collection<ScriptLine>
    {

        public SetVarScriptLine FindSetVarByKey(string key)
        {
            foreach (ScriptLine aLine in this)
            {
                if (aLine is SetVarScriptLine)
                {
                    if (((SetVarScriptLine)aLine).Key == key)
                    {
                        return (SetVarScriptLine)aLine;
                    }
                }
            }
            return null;
        }
    }

    
}
