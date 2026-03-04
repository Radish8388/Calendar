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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for SelectYear.xaml
    /// </summary>
    public partial class SelectYear : Window
    {
        public int Year = 2026;
        public SelectYear(int year)
        {
            InitializeComponent();
            Year = year;
            YearText.Text = year.ToString();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            string input = YearText.Text;

            if (int.TryParse(input, out int year) && year > 1581)
            {
                // Valid integer
                Year = year;
                DialogResult = true;   // closes dialog if you're using ShowDialog()
            }
            else
            {
                MessageBox.Show("Please enter a valid year.",
                                "Invalid Input",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                YearText.Focus();
                YearText.SelectAll();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
