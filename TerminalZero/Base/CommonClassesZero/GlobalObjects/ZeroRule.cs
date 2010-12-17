using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects
{
    public class ZeroRule
    {
        public string ID { get; private set; }
        public bool? Satisfied { get; set; }
        public ZeroAction CheckRuleAction { get; set; }
        public string CheckRuleActionName { get; set; }

        public string Result 
        {
            get
            {
                if (!Satisfied.HasValue)
                    return "";
                else
                    return Satisfied.Value?_OKResult:_NotOKResult;
            }
        }

        private string _OKResult;
        private string _NotOKResult;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">
        /// Nombre único en toda la app para identificar a esta regla
        /// </param>
        /// <param name="messageOK">
        /// Texto con un mensaje de regla validada
        /// </param>
        /// <param name="messageNotOk">
        /// Texto con un mensaje de regla NO validada
        /// </param>
        /// <param name="checkRuleActionName">
        /// ID de la acción que valida a esta regla, en otras palabras, el método (acción) a ejecutar para validar esta regla
        /// </param>
        public ZeroRule(string id, string messageOK, string messageNotOk, string checkRuleActionName)
        {
            ID = id;
            _OKResult = messageOK;
            _NotOKResult = messageNotOk;
            CheckRuleActionName = checkRuleActionName;
        }

        public void Check()
        {
            if (!Satisfied.HasValue)
                if (CheckRuleAction != null)
                {
                    Satisfied = CheckRuleAction.CanExecute(null);
                    if (Satisfied.Value)
                        CheckRuleAction.Execute(this);
                }
                else
                    Satisfied = true;
        }
        
    }
}
