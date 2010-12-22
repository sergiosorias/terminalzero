using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ZeroUpdateManager.Database.ScriptLines;
using System.Text.RegularExpressions;

namespace ZeroUpdateManager.Database
{
    /// <summary>
    /// DeployFile levanta la definicion de un archivo generado por el VSTeam Suit for databases
    /// entiende las sintaxis :setvar - :on error
    /// </summary>
    public class DeployFile
    {
        private DeployFile()
        {
                
        }
        private string text;
        private ScriptLineCollection scriptLines;
        public ScriptLineCollection ScriptLines
        {
            get { return scriptLines; }
        }
        
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Carga un archivo .sql generado por el build de VS Team for Databases
        /// </summary>
        /// <param name="path">Path al archivo .sql</param>
        /// <returns></returns>
        public static DeployFile LoadFrom(string path)
        {
            DeployFile deployFile = new DeployFile();
            deployFile.Text = File.ReadAllText(path);
            deployFile.LoadScriptsLines();
            return deployFile;
        }

        private void LoadScriptsLines()
        {
            int lineNumber = 0;
            scriptLines = new ScriptLineCollection();
            using (StringReader strReader = new StringReader(this.Text))
            {
                string line = line = strReader.ReadLine();
                while (line != null)
                {
                    if (line.StartsWith(":"))
                    {
                        ScriptLines.Add(ScriptLine.FromString(line, lineNumber));
                    }
                    line = strReader.ReadLine();
                    lineNumber++;
                }
            }
        }

        /// <summary>
        /// Remueve las lineas que comienzan con :
        /// Como por ej. :setvar - :on error
        /// y Luego ejecuta los comandos :setvar
        /// </summary>
        /// <returns>retorna codigo sql que entiende el server</returns>
        public List<string> GetStatements()
        {
            List<string> ret = new List<string>();
            StringBuilder sb = new StringBuilder(text);
            foreach (var item in this.scriptLines)
            {
                item.Execute(this,ref sb);
            }
            GetTextWithoutScriptLines(ref sb);

            text = CleanComments(sb);
            string line, lineUpper, aux;
            sb.Length = 0;

            using (StringReader reader = new StringReader(text))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    lineUpper = SearchGOStatement(line.ToUpper().Trim());
                    if (!string.IsNullOrWhiteSpace(lineUpper))
                    {
                        if (lineUpper.IndexOf(kESC_Chars)>=0)
                        {
                            sb.AppendLine(line.Substring(0, lineUpper.IndexOf(kESC_Chars)));
                            aux = sb.ToString();
                            if(!string.IsNullOrWhiteSpace(aux))
                                ret.Add(aux);
                            sb.Length = 0;
                            aux = lineUpper.Substring(lineUpper.IndexOf(kESC_Chars) + kESC_Chars.Length);
                            if (!string.IsNullOrWhiteSpace(aux))
                                sb.AppendLine(aux);
                        }
                        else
                            sb.AppendLine(line);
                    }

                }
            }

            return ret;
        }

        /// <summary>
        /// Remueve las lineas que comienzan con :
        /// Como por ej. :setvar - :on error        
        /// </summary>
        /// <returns></returns>
        private void GetTextWithoutScriptLines(ref StringBuilder sb)
        {
            using (StringReader strReader = new StringReader(sb.ToString()))
            {
                sb.Length = 0;
                string line = strReader.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    if (!line.StartsWith(":") && !string.IsNullOrWhiteSpace(line))
                    {
                        sb.AppendLine(line);
                    }
                    line = strReader.ReadLine();
                }
            }
        }

        private const string kESC_Chars = "####ESC####";

        private static string SearchGOStatement(string sqlLineUpper)
        {
            sqlLineUpper = sqlLineUpper.Replace("\nGO\n", "\n" + kESC_Chars + "\n");
            sqlLineUpper = sqlLineUpper.Replace("\nGO\t", "\n+" + kESC_Chars + "\t");
            sqlLineUpper = sqlLineUpper.Replace("\tGO\n", "\t" + kESC_Chars + "\n");
            sqlLineUpper = sqlLineUpper.Replace("*/GO", "*/" + kESC_Chars + "");
            sqlLineUpper = sqlLineUpper.Replace("\tGO\t", "\t" + kESC_Chars + "\t");
            sqlLineUpper = sqlLineUpper.Replace("GO/*", "" + kESC_Chars + "/*");
            sqlLineUpper = sqlLineUpper.Replace("GO\t", "" + kESC_Chars + "\t");
            sqlLineUpper = sqlLineUpper.Replace("\tGO", "\t" + kESC_Chars + "");
            sqlLineUpper = sqlLineUpper.Replace(" GO ", "\t" + kESC_Chars + "");
            sqlLineUpper = sqlLineUpper.Replace("\nGO ", "\t" + kESC_Chars + "");
            sqlLineUpper = sqlLineUpper.Replace(" GO\n", "\t" + kESC_Chars + "");
            sqlLineUpper = sqlLineUpper.Replace("\tGO ", "\t" + kESC_Chars + "");
            sqlLineUpper = sqlLineUpper.Replace(" GO\t", "\t" + kESC_Chars + "");
            sqlLineUpper = (sqlLineUpper == "GO") ? "" + kESC_Chars + "" : sqlLineUpper;
            return sqlLineUpper;
        }

        private const string kRegexComments = "([\\r\\n]*/\\*([^*]|[\\r\\n]|(\\*+([^*/]|[\\r\\n])))*\\*+/)|([\\r\\n\\s]*--.*)";

        private string CleanComments(StringBuilder sb)
        {
            Regex reg = new Regex(kRegexComments);
            foreach (Match item in reg.Matches(this.Text))
            {
                sb.Replace(item.Value, "");
            }
            return sb.ToString();

        }
    }
}
