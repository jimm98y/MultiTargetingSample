using System.Windows;

namespace MultiTargetingSampleApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            textBlock1.Text = SampleLibrary.SampleClass.GetSampleString() +
                $" running inside {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture} host.";
        }
    }
}
