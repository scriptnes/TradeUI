using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeUI_WPF;

namespace DebugHost
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            NQuotes.DebugHost.Server.Start(args);

            // use this without debug host
            //TradeUI_Expert _tradeUI = new TradeUI_Expert();
            //_tradeUI.StartUI();

        }
    }
}
