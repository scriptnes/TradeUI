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
            _taskRunner.Post((IUpdateApi api) =>
            {
                // call Prosoft API function

                // perform a UI update
                // use InvokeAsync(), because all UI calls must happen on the UI thread
                Dispatcher.InvokeAsync(new Action(() =>
                {
                    buyStopLabel_value.Content = api.GetCountOrder(OP_BUYSTOP);
                    buyLimitLabel_value.Content = api.GetCountOrder(OP_BUYLIMIT);
                    sellStopLabel_value.Content = api.GetCountOrder(OP_SELLSTOP);
                    sellLimitLabel_value.Content = api.GetCountOrder(OP_SELLLIMIT);
                }));
            });
        }


        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _statusTimer.Stop();
        }

        private void DeletePendingButton_Click(object sender, RoutedEventArgs e)
        {

            _taskRunner.Post((IProsoftApi api) =>
            {
                // call Prosoft API function

                // perform a UI update
                // use InvokeAsync(), because all UI calls must happen on the UI thread
                Dispatcher.InvokeAsync(new Action(() =>
                {
                    if (buyStopRadioButton.IsChecked == true)
                    {
                        api.DeletePending(OP_BUYSTOP);
                    }
                    if (buyLimitRadioButton.IsChecked == true)
                    {
                        api.DeletePending(OP_BUYLIMIT);
                    }
                    if (sellStopRadioButton.IsChecked == true)
                    {
                        api.DeletePending(OP_SELLSTOP);
                    }
                    if (sellLimitRadioButton.IsChecked == true)
                    {
                        api.DeletePending(OP_SELLLIMIT);
                    }

                    
                }));
            });
           
        }
    }
}
