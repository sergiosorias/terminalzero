using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace ZeroStock
{
    public class ZeroStockPackMaganer : ZeroCommonClasses.PackClasses.PackManager
    {
        ZeroCommonClasses.PackClasses.ExportEntitiesPackInfo MyInfo;

        public ZeroStockPackMaganer(string packPath)
            :base(packPath)
        {
            Importing += new EventHandler<ZeroCommonClasses.PackClasses.PackEventArgs>(ZeroStockPackMaganer_Importing);
        }

        void ZeroStockPackMaganer_Importing(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
            
        }
        
        public ZeroStockPackMaganer(ZeroCommonClasses.PackClasses.ExportEntitiesPackInfo info)
            :base(info)
        {
            MyInfo = info;
            base.Exporting += new EventHandler<ZeroCommonClasses.PackClasses.PackEventArgs>(ZeroStockPackMaganer_Exporting);
        }

        void ZeroStockPackMaganer_Exporting(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
            foreach (var item in MyInfo.Tables)
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(e.WorkingDirectory, item.RowType.ToString())))
                {
                    item.SerializeRows(xmlwriter);
                    xmlwriter.Close();
                }
            }
        }
        
    }
}
