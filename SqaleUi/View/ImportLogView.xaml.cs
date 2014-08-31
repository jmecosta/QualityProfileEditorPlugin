using System;
using System.Collections.Generic;
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

namespace SqaleUi.View
{
    using SqaleUi.ViewModel;

    /// <summary>
    /// Interaction logic for ImportLogView.xaml
    /// </summary>
    public partial class ImportLogView : Window
    {
        public ImportLogView(ImportLogViewModel model)
        {
            InitializeComponent();

            this.DataContext = model;
        }
    }
}
