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
    /// Interaction logic for SelectMonth.xaml
    /// </summary>
    public partial class SelectMonth : Window
    {
        public int Year = 2026;
        public int Month = 0;

        public SelectMonth(int year)
        {
            InitializeComponent();
            Year = year;
            YearText.Text = year.ToString();

            chooseMonth.Items.Add("January");
            chooseMonth.Items.Add("February");
            chooseMonth.Items.Add("March");
            chooseMonth.Items.Add("April");
            chooseMonth.Items.Add("May");
            chooseMonth.Items.Add("June");
            chooseMonth.Items.Add("July");
            chooseMonth.Items.Add("August");
            chooseMonth.Items.Add("September");
            chooseMonth.Items.Add("October");
            chooseMonth.Items.Add("November");
            chooseMonth.Items.Add("December");
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            string input = YearText.Text;

            if (int.TryParse(input, out int year) && year > 1581 && chooseMonth.SelectedIndex >= 0)
            {
                // Valid integer
                Year = year;
                Month = chooseMonth.SelectedIndex;
                DialogResult = true;   // closes dialog if you're using ShowDialog()
            }
            else
            {
                MessageBox.Show("Please enter a valid month and year.",
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
