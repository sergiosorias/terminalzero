using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;

namespace ZeroStock.Entities
{
    internal class StockEntities : ZeroStock.Entities.Entities
    {
        public StockEntities()
            : base(ZeroCommonClasses.Context.ContextInfo.GetConnectionForCurrentEnvironment("Stock"))
        {

        }

        internal int GetNextStockHeaderCode()
        {
            return StockHeaders.Count() > 0 ? StockHeaders.Max(sh => sh.Code) + 1 : 1;
        }

        public bool IsDeliveryDocumentMandatory(int stockType)
        {
            return stockType == 0;
        }
        
        public StockHeader CreateStockHeader(int terminalCode, int stockType, int code, int terminalTo, Guid user)
        {
            StockHeader _header = null;
            _header = StockHeader.CreateStockHeader(
            terminalCode,
            code,
            true,
            0, DateTime.Now.Date, terminalTo);
            _header.UserCode = user;
            _header.StockType = StockTypes.FirstOrDefault(st => st.Code == stockType);

            AddToStockHeaders(_header);
            return _header;
        }

        public DeliveryDocumentHeader CreateDeliveryDocumentHeader(int terminalCode)
        {
            DeliveryDocumentHeader ret = DeliveryDocumentHeader.CreateDeliveryDocumentHeader(
                    terminalCode,
                    DeliveryDocumentHeaders.Count() + 1,
                    true,
                    (short)EntityStatus.New,
                    DateTime.Now, 
                    terminalCode);

            DeliveryDocumentHeaders.AddObject(ret);

            return ret;
        }
        
    }
}
