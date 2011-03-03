using System.Linq;

namespace ZeroMasterData.Entities
{
    public class MasterDataEntities : Entities
    {
        public MasterDataEntities()
            : base(ZeroCommonClasses.Context.ContextBuilder.GetConnectionForCurrentEnvironment("MasterData"))
        {

                
        }

        internal int GetNextCustomerCode()
        {
            return Customers.Count()+1;
        }

        internal int GetNextProductCode()
        {
            int ret = Products.Count() == 0 ? 1 : (Products.Select(p => p.MasterCode).Max() + 1);
            return ret;
        }
    }
}
