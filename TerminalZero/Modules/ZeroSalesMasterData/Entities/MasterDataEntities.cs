using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroMasterData.Entities
{
    public class MasterDataEntities : Entities
    {
        public MasterDataEntities()
            : base(ZeroCommonClasses.Context.ContextBuilder.GetConnectionForCurrentEnvironment("MasterData"))
        {

                
        }
    }
}
