using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppFitbitUltraStats
{
    public class ConsoleMessages
    {
        Queue<string> queue;
        int capacity;
        int messageNumber = 0;

        public ConsoleMessages(int capacity = 10)
        {
            queue = new Queue<string>(capacity);
            this.capacity = capacity;
        }

        public string AddMessage(string text)
        {
            while (queue.Count >= capacity)
                queue.Dequeue();

            queue.Enqueue(++messageNumber + ": " + text + "\n");

            return this.ToString();
        }

        public override string ToString()
        {
            string ret = "";

            for (int index = 0; index < queue.Count; index++)
            {
                ret += queue.ElementAt(index);
            }

            return ret;
        }
    }

    /// <summary>
    /// Interaction logic for AppSwitchMultiLevel.xaml
    /// </summary>
    public partial class DummyWindow : Window
    {
        ConsoleMessages consoleMessages = new ConsoleMessages();

        public DummyWindow(AppFitbitUltraStats dummyApp, View.VLogger logger, string friendlyName, params string[] args)
        {
            InitializeComponent();
            Title = string.Format("Dummy HomeOS application: {0}\n", friendlyName);

            console.Text = consoleMessages.AddMessage("initialized application");
        }

        public void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        public void ConsoleLog(string message)
        {
            console.Text = consoleMessages.AddMessage(message);
        }

        public void OtherPortsLog(string message)
        {
            otherPortsList.Text = message;
        }
    }
}
