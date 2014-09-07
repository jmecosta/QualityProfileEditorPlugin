using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SqaleUi
{
    using SqaleUi.ViewModel;

    /// <summary>
	/// Interaction logic for CreateRuleWindow.xaml
	/// </summary>
	public partial class CreateRuleWindow : Window
	{
		public CreateRuleWindow(CreateRuleViewModel createRulesModel)
		{
			this.InitializeComponent();

		    this.DataContext = createRulesModel;
		}
	}
}