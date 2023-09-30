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

namespace ThreadsandTokens
{
    public partial class MainWindow : Window
    {
        
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationTokenSource cts2 = new CancellationTokenSource();

        Thread SimpleNumSearch = null;
        Thread FibonacciNumSearch = null;

        CancellationToken token1;
        CancellationToken token2;

        int tmplast = 1, tmpold = 1;
        int tmplast2 = 1;

        public MainWindow()
        {
            InitializeComponent();

            token1 = cts.Token;
            token2 = cts2.Token;

            SimpleNumSearch = new Thread(AddSimpleNumbers);
            FibonacciNumSearch = new Thread(AddFibonacciNumbers);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            PauseSimple.IsEnabled = true;
            ResumeSimple.IsEnabled = true;

            cts.Cancel();
            cts2.Cancel();

            cts = new CancellationTokenSource();
            cts2 = new CancellationTokenSource();

            token1 = cts.Token;
            token2 = cts2.Token;

            SimpleNumSearch = new Thread(AddSimpleNumbers);
            FibonacciNumSearch = new Thread(AddFibonacciNumbers);

            
            SimpleNumSearch.IsBackground = false;
            FibonacciNumSearch.IsBackground = false;

            tmplast = 1; tmpold = 1;


            FibonacciNumbers.Items.Clear();
            SimpleNumbers.Items.Clear();

            SimpleNumSearch.Start(new KeyValuePair<KeyValuePair<int, int>, CancellationToken>(new KeyValuePair<int, int>(Convert.ToInt32(RightNum.Text), Convert.ToInt32(LeftNum.Text)), token1));
            FibonacciNumSearch.Start(new KeyValuePair<int, CancellationToken>(tmplast, token2));
        }

        private void AddSimpleNumbers(object a)
        {
            var tmp = (KeyValuePair<KeyValuePair<int, int>, CancellationToken>)a;
            var tmptoken = tmp.Value;
            int r = tmp.Key.Key, l = tmp.Key.Value;

            while (tmptoken.IsCancellationRequested == false)
            {
                for (int i = r; i <= l; i++)
                {
                    if (tmptoken.IsCancellationRequested == true) break;
                    if (isSimple(i))
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            SimpleNumbers.Items.Add(i);
                        }));

                    }
                    Thread.Sleep(1000);
                }
            }
        }

        private void AddFibonacciNumbers(object a)
        {
            var tmp = (KeyValuePair<int, CancellationToken>)a;
            tmplast = tmp.Key;
            while (tmp.Value.IsCancellationRequested == false)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    FibonacciNumbers.Items.Add(tmplast);
                    tmplast = GetFibonacci();
                }));
                Thread.Sleep(1000);
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            PauseSimple.IsEnabled = false;
            ResumeSimple.IsEnabled = false;

            cts.Cancel();
        }


        private int GetFibonacci()
        {
            int sum = tmpold + tmplast;
            tmpold = tmplast;
            return sum;
        }


        private void StopFibonacci_Click(object sender, RoutedEventArgs e)
        {
            PauseFibonacci.IsEnabled = false;
            ResumeFibonacci.IsEnabled = false;

            cts2.Cancel();
        }

        private void ResumeSimple_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            token1 = cts.Token;

            ++tmplast;

            SimpleNumSearch = new Thread(AddSimpleNumbers);
            SimpleNumSearch.Start(new KeyValuePair<KeyValuePair<int, int>, CancellationToken>(new KeyValuePair<int, int>(tmplast, Convert.ToInt32(LeftNum.Text)), token1));
        }

        private void ResumeFibonacci_Click(object sender, RoutedEventArgs e)
        {
            cts2 = new CancellationTokenSource();
            token2 = cts2.Token;

            FibonacciNumSearch = new Thread(AddFibonacciNumbers);
            FibonacciNumSearch.Start(new KeyValuePair<int, CancellationToken>(tmplast2, token2));
        }

        private void PauseSimple_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private void PauseFibonacci_Click(object sender, RoutedEventArgs e)
        {
            tmplast2 = (int)FibonacciNumbers.Items[FibonacciNumbers.Items.Count-1];
            cts2.Cancel();
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

    }
}
