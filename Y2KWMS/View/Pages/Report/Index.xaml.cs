using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
using Y2KWMS.Controller;

namespace Y2KWMS.View.Pages.Report
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        ReportController reportController;
        public Index()
        {
            InitializeComponent();

            reportController = new ReportController();

            load();
        }
        private void load()
        {
            List<Expander> expanders =  reportController.load();

            for(int i =0; i<expanders.Count;i++)
            {
                RootGrid.Children.Add(expanders[i]);
            }
           
        }
    }
}
