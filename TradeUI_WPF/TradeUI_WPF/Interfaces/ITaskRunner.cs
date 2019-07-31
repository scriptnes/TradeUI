using System;
using NQuotes;

namespace TradeUI_WPF
{
    public interface ITaskRunner
    {
        void Post(Action<IMqlApi> action);
        void Post(Action<IProsoftApi> action);
        void Post(Action<IUpdateApi> action);
    }
}