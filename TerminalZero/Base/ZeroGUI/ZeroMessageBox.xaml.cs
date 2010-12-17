using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for ZeroMessageBox.xaml
    /// </summary>
    public partial class ZeroMessageBox : Window
    {
        bool isDialog = false;
        public ZeroMessageBox()
        {
            InitializeComponent();
            Owner = Application.Current.Windows[0];
        }

        public new object Content
        {
            get
            {
                return contentpress.Content;
            }

            set
            {
                contentpress.Content = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool exit = true;

            if (contentpress.Content is IZeroPage)
            {
                exit = ((IZeroPage)contentpress.Content).CanAccept();
            }

            if (isDialog && exit)
                DialogResult = true;

            if(exit)
                this.Close();


        }

        private void current_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool exit = true;

            if (contentpress.Content is IZeroPage)
            {
                exit = ((IZeroPage)contentpress.Content).CanCancel();
            }

            if (isDialog && exit)
                DialogResult = false;

            if (exit)
                this.Close();
        }

        public static bool? Show(object Content)
        {
            return Show(Content, "", SizeToContent.WidthAndHeight);
        }

        public static bool? Show(object Content, SizeToContent sizeToContent)
        {
            return Show(Content, "", sizeToContent);
        }

        private static bool? Show(object content, string title, SizeToContent sizeToContent)
        {
            ZeroMessageBox MB = new ZeroMessageBox();
            MB.Content = content;
            MB.Title = title;
            MB.SizeToContent = sizeToContent;
            MB.isDialog = true;
            return MB.ShowDialog();
        }
    }
}
