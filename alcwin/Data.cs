using System;
using System.Collections.Generic;

namespace alcwin
{
    public class BatTypeItem
    {
        public int Type { get; set; }
        public string Caption { get; set; }
        public int Voltage { get; set; }
    }

    public static class Data
    {
        public static BatTypeItem[] BatTypes =
        {
            new BatTypeItem { Type=0, Caption="NiCd", Voltage=1200 },
            new BatTypeItem { Type=1, Caption="NiMH", Voltage=1200 },
            new BatTypeItem { Type=2, Caption="Li-4.1", Voltage=3600 },
            new BatTypeItem { Type=3, Caption="Li-4.2", Voltage=3700 },
            new BatTypeItem { Type=4, Caption="Pb", Voltage=2000 },
            new BatTypeItem { Type=5, Caption="LiFePo4", Voltage=3300 },
            new BatTypeItem { Type=6, Caption="Li4.35", Voltage=3780 },
            new BatTypeItem { Type=7, Caption="NiZn", Voltage=1650 },
            new BatTypeItem { Type=8, Caption="AGM/CA", Voltage=2000 }
        };

        public static string[] Functions =
        {
            "Keine",
            "Laden",
            "Entladen",
            "Entladen/Laden",
            "Test",
            "Wartung",
            "Formieren",
            "Zyklen",
            "Auffrischen"
        };

        public static string ChargeStateName(int State)
        {
            string result;
            if (State < 0x0b)
                result = "Leerlauf";
            else if (State < 0x2e)
                result = "Pause/Warten";
            else if (State < 0x38)
                result = "Entladen";
            else if (State < 0x6F)
                result = "Laden";
            else if (State < 0xA1)
                result = "Erhaltungsladung";
            else if (State < 0xC9)
                result = "Entladen beendet";
            else
                result = "Notabschaltung";
            return result;
        }
    }
}