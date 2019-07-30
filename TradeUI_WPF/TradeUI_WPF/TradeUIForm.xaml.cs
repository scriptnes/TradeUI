using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using NQuotes;

namespace TradeUI_WPF
{
    /// <summary>
    /// Interaction logic for TradeUIForm.xaml
    /// </summary>
    public partial class TradeUIForm : Window
    {
        private readonly ITaskRunner _taskRunner;
        private readonly Timer _statusTimer;
        private int OP_BUYLIMIT = 2;
        private int OP_SELLLIMIT = 3;
        private int OP_BUYSTOP = 4;
        private int OP_SELLSTOP = 5;

        public TradeUIForm(ITaskRunner taskRunner)
        {
            _taskRunner = taskRunner;
            InitializeComponent();

            _statusTimer = new Timer(1000);
            _statusTimer.Elapsed += statusTimer_Tick;
            _statusTimer.Start();
        }


        private void statusTimer_Tick(object sender, EventArgs e)
        {
            _taskRunner.Post((IProsoftApi api) =>
            {
                // call Prosoft API function
                string getBuyStop = api.GetCountOrder(OP_BUYSTOP);
                string getBuyLimit =  api.GetCountOrder(OP_BUYLIMIT);
                string getSellStop = api.GetCountOrder(OP_SELLSTOP);
                string getSellLimit = api.GetCountOrder(OP_SELLLIMIT);

                // perform a UI update
                // use InvokeAsync(), because all UI calls must happen on the UI thread
                Dispatcher.InvokeAsync(new Action(() =>
                {
                    buyStopLabel_value.Content = getBuyStop;
                    buyLimitLabel_value.Content = getBuyLimit;
                    sellStopLabel_value.Content = getSellStop;
                    sellLimitLabel_value.Content = getSellLimit;
                }));
            });
        }


        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _statusTimer.Stop();
        }

        private void DeletePendingButton_Click(object sender, RoutedEventArgs e)
        {

            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (buyStopRadioButton.IsChecked == true)
                {
                    _taskRunner.PostDeletePending(OP_BUYSTOP);
                }
                if (buyLimitRadioButton.IsChecked == true)
                {
                    _taskRunner.PostDeletePending(OP_BUYLIMIT);
                }
                if (sellStopRadioButton.IsChecked == true)
                {
                    _taskRunner.PostDeletePending(OP_SELLSTOP);
                }
                if (sellLimitRadioButton.IsChecked == true)
                {
                    _taskRunner.PostDeletePending(OP_SELLLIMIT);
                }

            }));
           
        }
    }
}
