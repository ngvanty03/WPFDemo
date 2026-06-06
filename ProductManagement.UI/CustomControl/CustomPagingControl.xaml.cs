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

namespace ProductManagement.UI.CustomControl
{
    /// <summary>
    /// Interaction logic for CustomPagingControl.xaml
    /// </summary>
    public partial class CustomPagingControl : UserControl
    {
        public CustomPagingControl()
        {
            InitializeComponent();           
        }
         
        public static readonly DependencyProperty PrevPageCommandProperty =
            DependencyProperty.Register(nameof(PrevPageCommand), typeof(ICommand), typeof(CustomPagingControl));

        public ICommand PrevPageCommand
        {
            get => (ICommand)GetValue(PrevPageCommandProperty);
            set => SetValue(PrevPageCommandProperty, value);
        }

        public static readonly DependencyProperty NextPageCommandProperty =
            DependencyProperty.Register(nameof(NextPageCommand), typeof(ICommand), typeof(CustomPagingControl));

        public ICommand NextPageCommand
        {
            get => (ICommand)GetValue(NextPageCommandProperty);
            set => SetValue(NextPageCommandProperty, value);
        }

        public static readonly DependencyProperty HasPrevPageProperty =
            DependencyProperty.Register(nameof(HasPrevPage), typeof(bool), typeof(CustomPagingControl), new PropertyMetadata(false));

        public bool HasPrevPage
        {
            get => (bool)GetValue(HasPrevPageProperty);
            set => SetValue(HasPrevPageProperty, value);
        }

        public static readonly DependencyProperty HasNextPageProperty =
            DependencyProperty.Register(nameof(HasNextPage), typeof(bool), typeof(CustomPagingControl), new PropertyMetadata(false));

        public bool HasNextPage
        {
            get => (bool)GetValue(HasNextPageProperty);
            set => SetValue(HasNextPageProperty, value);
        }

        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(CustomPagingControl), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(CustomPagingControl), new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            set => SetValue(TotalPagesProperty, value);
        }        
    }
}
