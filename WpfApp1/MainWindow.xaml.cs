using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {

        Dictionary<UIElement, UIElement> uncached = new Dictionary<UIElement, UIElement>();

        //Stack<(UIElement, double)> leftstack = new Stack<(UIElement, double)>();
        //Stack<(UIElement, double)> rightstack = new Stack<(UIElement, double)>();
        //
        // W, H, C, X
        Stack<(double, double, Color, double)> leftstack = new Stack<(double, double, Color, double)>();
        Stack<(double, double, Color, double)> rightstack = new Stack<(double, double, Color , double)>();

        List<UIElement> temp = new List<UIElement>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void generator_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            rframe.Children.Clear();
            uncached.Clear();
            int generation_count = 1;

            sbx.Value = 0;


            // Использовать Avl дерево для кэширования элементов чтобы потом упорядочено их пихать в стек
            //Dictionary<double, (double, double, Color, double)> tmpR = new Dictionary<double, (double, double, Color, double)>();
            //Dictionary<double, (double, double, Color, double)> tmpL = new Dictionary<double, (double, double, Color, double)>();
            Bitlush.AvlTree<double, (double, double, Color, double)> tmpR = new Bitlush.AvlTree<double, (double, double, Color, double)>();
            Bitlush.AvlTree<double, (double, double, Color, double)> tmpL = new Bitlush.AvlTree<double, (double, double, Color, double)>();

            int.TryParse(textbox1.Text, out generation_count);


            for (int i = 0; i < generation_count; i++)
            {
                Rectangle rect = new Rectangle();

                rect.Width = rand.Next(20, 100);
                rect.Height = 15;
                rect.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));

                //double x = rand.Next(-4100, 4100);
                double x = 500;
                double y = rand.Next(0, 100);
                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                //rect.Visibility = Visibility.Collapsed;
                //rect.CacheMode = null;



                //Trace.WriteLine(x);
                // Вот в этом месте сразу сделать кэширование в стеки
                if (x < 0)
                {
                    // Если оно ушло влево, надо в левую очередь его спрятать и удалить из дочерних эл-тов
                    tmpL.Insert(x, (rect.Width, y, Color.FromArgb(255, 110, 110, 110), -1 * x));
                    
                }
                else
                if (x > 588)
                {
                    // Если оно ушло влево, надо в левую очередь его спрятать и удалить из дочерних эл-тов
                    tmpR.Insert(-1 * x, (rect.Width, y, Color.FromArgb(255, 110, 110, 110), -1 * x + 588));

                }
                else
                {
                    rframe.Children.Add(rect);
                }


            }

            foreach(var res in tmpL)
            {
                leftstack.Push(res.Value);
            }

            foreach (var res in tmpR)
            {
                rightstack.Push(res.Value);
            }

            rframe.CacheMode = new BitmapCache();
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            

            int newVal = (int)double.Round(sbx.Value);
            int oldVal = (int)double.Round(e.OldValue);

            //sbx.Value = newVal;
            rframe.CacheMode = null;
            double delta = (newVal - oldVal);



            //Canvas.SetLeft(rframe, e.NewValue);

            //foreach (UIElement uncachedChild in uncached.Values)
            //{
            //    double x = Canvas.GetLeft(uncachedChild) + delta;
            //    Canvas.SetLeft(uncachedChild, x );

            //    if (x < (rframe.ActualWidth - rframe.Margin.Left) || x > rframe.Margin.Left)
            //    {

            //        rframe.Children.Add(uncachedChild);
            //        //if (child.Visibility != Visibility.Collapsed)
            //        //{
            //        //    child.Visibility = Visibility.Collapsed;
            //        //}

            //    }
            //    else
            //    {
            //    }
            //}

            

            foreach (UIElement child in rframe.Children)
            {

                //this.Title = "123";

                double x = Canvas.GetLeft(child);

                Canvas.SetLeft((UIElement)child, x + delta);
                x = Canvas.GetLeft(child);

               // this.Title = " X = " + x + " New val  = " + newVal; 
                

                //if (x+ rframe.Margin.Left < rframe.Margin.Left)
                if (x < -1)
                {
                    // Если оно ушло влево, надо в левую очередь его спрятать и удалить из дочерних эл-тов
                    temp.Add(child);
                    leftstack.Push((child.RenderSize.Width, Canvas.GetTop(child), Color.FromArgb(255,0,0,0), newVal - x));
                    child.Visibility = Visibility.Collapsed;
                    child.CacheMode = null;

                
                    //Trace.WriteLine("EXIT : X " + x + " | Old value " + oldVal + " + New Val " + newVal + " | Return at " + leftstack.Peek().Item4);

                    // this.Title = oldVal.ToString();

                }
                else
                if (x > 600)
                {
                    // Если оно ушло влево, надо в левую очередь его спрятать и удалить из дочерних эл-тов


                    temp.Add(child);

                    // надо понять что тут применить чтобы правую границу правильно выставить как на видосе
                    rightstack.Push((child.RenderSize.Width, Canvas.GetTop(child), Color.FromArgb(255, 110, 110, 110), newVal - (x - 600)));
                    child.Visibility = Visibility.Collapsed;
                    child.CacheMode = null;
                    //Trace.WriteLine("EXIT : X " + x + " | Old value " + oldVal + " + New Val " + newVal + " | Return at " + rightstack.Peek().Item4);
                    //this.Title = oldVal.ToString();
                }



                //if (child.CacheMode == null)
                //{
                //    //child.CacheMode = new BitmapCache();
                //}
                //if (x > (rframe.ActualWidth - rframe.Margin.Left) || x < rframe.Margin.Left)
                //{

                //    //if (!uncached.ContainsKey(child))
                //    //{
                //    //    uncached.Add(child, child);
                //    //}
                //    //if (child.Visibility != Visibility.Collapsed)
                //    //{
                //    //    child.Visibility = Visibility.Collapsed;
                //    //}

                //}
                //else
                //{
                //    //if (uncached.ContainsKey(child))
                //    //{
                //    //    uncached.Remove(child);
                //    //}

                //    //if (child.Visibility != Visibility.Visible)
                //    //{
                //    //    child.Visibility = Visibility.Visible;
                //    //}
                //}


            }

            foreach (UIElement child in temp)
            {
                rframe.Children.Remove(child);
            }

            // w h x
            (double, double, Color, double) resulta = (0, 0, Color.FromArgb(0, 0, 0, 0), 0);

            if (delta > 0)
            // двигаем ползунок влево
            {
                while (leftstack.TryPeek(out resulta))
                {

                    if (resulta.Item4 <= newVal)
                    {
                        (double, double, Color, double) result = leftstack.Pop();
                        Rectangle UIetemp = new Rectangle();
                        UIetemp.Width = result.Item1;
                        UIetemp.Height = 15;
                        UIetemp.Fill = new SolidColorBrush(result.Item3);
                        //this.Title = result.ToString();


                        Canvas.SetLeft(UIetemp, newVal - resulta.Item4);
                        Canvas.SetTop(UIetemp, result.Item2);

                        rframe.Children.Add(UIetemp);

                        UIetemp.Visibility = Visibility.Visible;
                        UIetemp.CacheMode = new BitmapCache();
                        //double x = Canvas.GetLeft(UIetemp);
                       // Trace.WriteLine("ENTER: X " + x + "item4 = " + resulta.Item4 + " | newval = " + newVal);
                        //
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            // двигаем ползунок право
            {


                while (rightstack.TryPeek(out resulta))
                {
                    if (resulta.Item4 >= newVal)
                    {
                        (double, double, Color, double) result = rightstack.Pop();

                        Rectangle UIetemp = new Rectangle();
                        UIetemp.Width = result.Item1;
                        UIetemp.Height = 15;
                        UIetemp.Fill = new SolidColorBrush(result.Item3);

                        Canvas.SetLeft(UIetemp, 600 + newVal - resulta.Item4);
                        Canvas.SetTop(UIetemp, result.Item2);
                        UIetemp.Visibility = Visibility.Visible;
                        UIetemp.CacheMode = new BitmapCache();

                        rframe.Children.Add(UIetemp);
                        //UIElement UIetemp = rightstack.Pop().Item1;
                        //rframe.Children.Add(UIetemp);
                        //Canvas.SetLeft((UIElement)UIetemp, Canvas.GetLeft(UIetemp) + delta);
                        //UIetemp.Visibility = Visibility.Visible;
                        //UIetemp.CacheMode = new BitmapCache();
                        //double x = Canvas.GetLeft(UIetemp);
                        //Trace.WriteLine("ENTER: X " + x + "item4 = " + resulta.Item4 + " | newval = " + newVal);

                    }
                    else
                    {
                        break;
                    }
                }
            }



            rframe.CacheMode = new BitmapCache();
            temp.Clear();

            //foreach (UIElement uncachedChild in uncached.Values)
            //{
            //    rframe.Children.Remove(uncachedChild);
            //}

            //this.Title = oldVal + " | " + newVal + " rframe left border " + ( rframe.Margin.Left) + " | rb " + rframe.Margin.Left;

            //rframe.CacheMode = new BitmapCache();
        }
    }
}
