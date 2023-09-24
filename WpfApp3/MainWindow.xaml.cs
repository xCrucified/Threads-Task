using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class Alpha
    {
        public KeyValuePair<int, int> Nums { get; set; }
        public CancellationToken Token { get; init; }

        public Alpha(KeyValuePair<int, int> tmp, CancellationToken token)
        {
            Nums = tmp;
            Token = token;
        }
    }


    public partial class MainWindow : Window
    {
        public static int fibonacci(int number)
        {
            if (number == 0)
                return 0;
            if (number == 1)
                return 1; 
            return fibonacci(number - 1) + fibonacci(number - 2);
        }
        public static bool isSimple(int x)
        {
            if (x > 1)
            {
                for (int i = 2; i < x; i++)
                {
                    if (x % i == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            else return true;
        }

        CancellationTokenSource cts = new CancellationTokenSource();

        bool stopped = false;

        Thread SimpleNumSearch = null;
        
        List<string> listofSimple = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            SimpleNumSearch = new Thread(SProcess);
            SimpleNumSearch.Priority = ThreadPriority.Highest;
            SimpleNumSearch.IsBackground = false;
            SimpleNumSearch.Start(new Alpha(new KeyValuePair<int, int>(Convert.ToInt32(RightNum.Text), Convert.ToInt32(LeftNum.Text)), cts.Token));
        }

        private void SProcess(object a)
        {
            var tmp = (Alpha)a;
            var tmptoken = tmp.Token;

            int r = tmp.Nums.Key, l = tmp.Nums.Value;
            
            for (int i = r; i <= l; i++)                            
            {
                if (isSimple(i))
                {
                    tmptoken.ThrowIfCancellationRequested();
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        SimpleNumbers.Items.Add(i.ToString());
                    }));
                }
                Thread.Sleep(1000);
            }
            
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            SimpleNumSearch = new Thread(SProcess);
            unsafe
            {
                try
                {
                    cts.Cancel();
                }
                catch (OperationCanceledException ex)
                {
                    MessageBox.Show("Thread was cancelled");
                }
                catch (ThreadInterruptedException ee)
                {
                    MessageBox.Show("Thread was cancelled");
                }
                finally
                {
                    MessageBox.Show("Thread was cancelled");
                }
            }
            //if (stopped == false)
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => {
            //        SimpleNumSearch.Suspend();
            //    }));
            //    stopped = true;
            //}
            //else
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => {
            //    }));
            //    stopped = false;
            //}
        }
        

    }
}
