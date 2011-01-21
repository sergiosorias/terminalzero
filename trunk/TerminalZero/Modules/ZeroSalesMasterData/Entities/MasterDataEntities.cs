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
            return Products.Select(p => p.Code).Max() + 1;
        }
    }
}
