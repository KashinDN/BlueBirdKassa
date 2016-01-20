using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueBirdKassa
{
    public class DepotList
    {
        protected List<depot> dataList;
        public List<depot> DepotDataList
        {
            get { return dataList; }
            set { dataList = value; }
        }
        public void PopulateList(int depotID)
        {
            if (depotID == 10)
            {
                DepotDataList = new List<depot>
                {
                
                    new depot(){ValueDepotId = "4", ValueDepotName="ТЦ Центральный"},
                    new depot(){ValueDepotId = "11", ValueDepotName="Зеленый остров"},
                    new depot(){ValueDepotId = "10", ValueDepotName="Склад 1"},
                    new depot(){ValueDepotId = "3", ValueDepotName="Бутик 4a, Универсам"}
                    //new depot(){ValueDepotId = "2", ValueDepotName="Бутик 30, Универбыт"}
                };
            }
            else if (depotID == 4)
            {
                DepotDataList = new List<depot>
                {
                
                    new depot(){ValueDepotId = "4", ValueDepotName="ТЦ Центральный"}
                };
            }
            else if (depotID == 11)
            {
                DepotDataList = new List<depot>
                {
                
                    new depot(){ValueDepotId = "11", ValueDepotName="Зеленый остров"}
                };
            }

        }
    }
}
