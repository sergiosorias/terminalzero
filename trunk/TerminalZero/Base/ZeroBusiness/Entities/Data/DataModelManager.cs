using System;
using System.Collections.Generic;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public class DataModelManager : Entities
    {
        public DataModelManager()
            : base(ZeroCommonClasses.Context.ContextInfo.GetConnectionForCurrentEnvironment("Data.DataModel"))
        {

                
        }

        private ConfigurationModelManager _confModel;

        public int GetNextCustomerCode()
        {
            return Customers.Count()+1;
        }

        public int GetNextProductCode()
        {
            int ret = Products.Count() == 0 ? 1 : (int.Parse(Products.Select(p => p.MasterCode).Max()) + 1);
            return ret;
        }

        public int GetNextSaleHeaderCode(int terminal)
        {
            return SaleHeaders.Where(hh => hh.TerminalCode == terminal).Max(h => h.Code) + 1;
        }

        public int GetNextStockHeaderCode()
        {
            return StockHeaders.Count() > 0 ? StockHeaders.Max(sh => sh.Code) + 1 : 1;
        }

        public IEnumerable<Terminal> GetExportTerminal(int terminal)
        {
            if (_confModel == null)
                _confModel = new ConfigurationModelManager();

            return _confModel.Terminals;
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
            0, terminalTo, DateTime.Now.Date);
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
                    terminalCode,
                    DateTime.Now);

            DeliveryDocumentHeaders.AddObject(ret);

            return ret;
        }
        
    }
}
