using System.Windows.Controls;

namespace TradeUI_WPF
{
    public interface IProsoftApi
    {
        string GetCountOrder(int oType);
    }
}