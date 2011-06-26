﻿using System;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroLogHandle
{
    public class ZeroLogHandleModule : ZeroModule, ILogBuilder
    {
        public ZeroLogHandleModule()
            : base(1, "Guarda un log detallado de las operaciones")
        {
            
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }
        
        public override void Initialize()
        {
            
        }
        
        #region ILogBuilder Members

        public void Add(string log)
        {

        }

        public void Add(Exception ex)
        {

        }

        #endregion

    }

    
}
