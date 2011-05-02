using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroBusiness.Entities.Configuration
{
    public struct PropertyConst
    {
        public string Code;
        public string Description;
        public string DefaultValue;
    }

    public partial class SystemProperty
    {
        private static readonly PropertyConst _syncRecurrence = new PropertyConst() { Code = "SYNC_EVERY", Description = "Valor que determina cada cuanto (en minutos) la sucursal sincronizara automaticamente", DefaultValue = "10" };
        private static readonly PropertyConst _homeShortCut = new PropertyConst() { Code = "HOME_SHORTCUT", Description = "Accesos directos de la página de inicio", DefaultValue = "No se usa" };

        public static PropertyConst SyncRecurrence { get { return _syncRecurrence;} }
        public static PropertyConst HomeShortcut { get { return _homeShortCut; } } 
        
        
    }
}
