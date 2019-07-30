using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using NQuotes;

namespace TradeUI_WPF
{
    public class TradeUI_Expert : MqlApi, ITaskRunner, IProsoftApi
    {
        private Thread _uiThread;
        private TradeUIForm _form;
        private bool _isFormClosed;
        private BlockingCollection<Action<IMqlApi>> _taskRunnerQueue;
        private BlockingCollection<Action<IProsoftApi>> _taskRunnerProsoftQueue;

        public void StartUI()
        {
            // show a form and wait until it's closed
            _form = new TradeUIForm(this);
            Application app = new Application();
            app.Run(_form);
            _isFormClosed = true;
        }

        public override int init()
        {
            _taskRunnerQueue = new BlockingCollection<Action<IMqlApi>>();
            _taskRunnerProsoftQueue = new BlockingCollection<Action<IProsoftApi>>();

            // create and start a thread for UI 
            // this is needed to avoid blocking the terminal interaction with the MQL API
            _uiThread = new Thread(this.StartUI);
            _uiThread.SetApartmentState(ApartmentState.STA);
            _uiThread.Start();
            return 0;
        }



        public override int start()
        {
            // waiting loop until the form is closed
            // the MQL API in the UI thread will work while we are waiting here
            // (without this loop the calls happening between ticks would be blocked waiting for the next tick)
            while (!IsStopped() && !_isFormClosed)
            {
                // execute a task posted from the UI
                if (_taskRunnerQueue.TryTake(out Action<IMqlApi> action1, TimeSpan.FromSeconds(1)))
                {
                    action1(this);
                }

                if (_taskRunnerProsoftQueue.TryTake(out Action<IProsoftApi> action2, TimeSpan.FromSeconds(1)))
                {
                    action2(this);
                }

            }

            // automatically remove EA from the chart when the form is closed
            if (!IsStopped())
            {
                ExpertRemove();
            }

            return 0;
        }

        public override int deinit()
        {
            // make sure that the form is closed
            // use Invoke(), because all UI calls must happen on the UI thread
            // after the form is closed the UI thread finishes
            if ((_form != null) && !_isFormClosed)
            {
                _form.Dispatcher.Invoke(new Action(_form.Close));
            }

            return 0;
        }

        void ITaskRunner.Post(Action<IMqlApi> action)
        {
            // add the task to the queue for later execution from the EA thread in start()
            _taskRunnerQueue.Add(action);
        }

        void ITaskRunner.Post(Action<IProsoftApi> action)
        {
            // add the task to the queue for later execution from the EA thread in start()
            _taskRunnerProsoftQueue.Add(action);
        }

        string IProsoftApi.GetCountOrder(int oType)
        {
            int countOrders = 0;
            int ordersTotal = OrdersTotal();
            if (ordersTotal > 0)
            {
                for (int i = 0; i < ordersTotal; i++)
                {
                    if (OrderSelect(i, SELECT_BY_POS, MODE_TRADES) == true)
                    {
                        if (OrderSymbol() == Symbol() && OrderType() == oType)
                        {
                            countOrders++;
                        }
                    }
                }
            }
            return countOrders.ToString();
        }

        void ITaskRunner.PostDeletePending(int oType)
        {
            for (int i = OrdersTotal() - 1; i >= 0; i--)
            {
                if (OrderSelect(i, SELECT_BY_POS) && OrderSymbol() == Symbol() && OrderType() == oType)
                {
                    bool ret = OrderDelete(OrderTicket());
                    if (!ret)
                    {
                        Print("Error: ", GetLastError());
                    }
                }
            }
        }
      
    }
}
