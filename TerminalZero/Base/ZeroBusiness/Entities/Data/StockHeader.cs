﻿using System;
using System.Data;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;
using Terminal = ZeroCommonClasses.Terminal;
using ZeroBusiness.Manager.Data;

namespace ZeroBusiness.Entities.Data
{
    public partial class StockHeader : IExportableEntity
    {
        internal StockHeader()
        {
            
        }

        public StockHeader(StockType type, int terminalDestination)
        {
            TerminalCode= Terminal.Instance.TerminalCode;
            TerminalToCode = terminalDestination;
            Date = DateTime.Now.Date;
            Code = GetNextStockHeaderCode();
            StockType = type;
            UserCode = User.GetCurrentUserCode();
            UpdateStatus(EntityStatus.New);
        }

        public object ViewTitle
        {
            get
            {
                return StockType != null && !string.IsNullOrWhiteSpace(StockType.Description)? StockType.Description: "Stock";
            }
        }

        public int TerminalDestination
        {
            get { return TerminalToCode; }
        }

        private static int GetNextStockHeaderCode()
        {
            return BusinessContext.Instance.ModelManager.StockHeaders.Count() > 0 ? BusinessContext.Instance.ModelManager.StockHeaders.Max(sh => sh.Code) + 1 : 1;
        }
        
        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        public StockItem AddNewStockItem(Product prod, double qty, string lot)
        {
            StockItem item = new StockItem(this, prod, qty, lot);
            StockItems.Add(item);
            return item;
        }

        public bool HasChanges
        {
            get
            {
                return EntityState != EntityState.Unchanged && StockItems != null && StockItems.Count > 0 &&
                       StockItems.All(si => si.EntityState != EntityState.Unchanged);
            }
        }
    }
}