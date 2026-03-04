using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int selectedMonth = 0;
        private int selectedYear = 0;
        private bool isDrawYear = true;
        private int fontSize = 10;

        public MainWindow()
        {
            InitializeComponent();
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            selectedMonth = today.Month - 1; // 0 - 11
            selectedYear = today.Year; // for example, 2026

            //Debug.WriteLine($"Month={selectedMonth} Year={selectedYear}");

            canvas.SizeChanged += canvas_SizeChanged;
        }

        private void DrawYear_Click(object sender, RoutedEventArgs e)
        {
            SelectYear dialog = new SelectYear(selectedYear);
            dialog.Owner = this;
            bool? result = dialog.ShowDialog(); // Modal dialog
            if (result == true)
            {
                selectedYear = dialog.Year;
                isDrawYear = true;
                DrawCalendar();
            }
        }

        private void DrawMonth_Click(object sender, RoutedEventArgs e)
        {
            SelectMonth dialog = new SelectMonth(selectedYear);
            dialog.Owner = this;
            bool? result = dialog.ShowDialog(); // Modal dialog
            if (result == true)
            {
                selectedYear = dialog.Year;
                selectedMonth = dialog.Month;
                isDrawYear = false;
                DrawCalendar();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            bool? result = aboutWindow.ShowDialog(); // Modal dialog
        }

        private void DrawCalendar()
        {
            canvas.Children.Clear();
            if (isDrawYear) DrawYear();
            else DrawBigMonth();
        }

        private void DrawYear()
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            GetFontSize(width, height);

            // draw year at top center
            TextBlock label = new TextBlock();
            label.Text = selectedYear.ToString();
            label.FontSize = fontSize * 2;
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = label.DesiredSize.Width;
            Canvas.SetLeft(label, width/2 - textWidth/2 - 5); // centered horizontally
            Canvas.SetTop(label, 5); // vertically at the top
            canvas.Children.Add(label);

            double monthWidth = (width - 50) / 4.0;
            double monthHeight = (height - 40 - fontSize*2) / 3.0;
            for (int month = 0; month < 12; month++)
            {
                int row = month / 4;
                int col = month % 4;
                double monthTop = fontSize * 2 + 10 + row * (monthHeight + 10);
                double monthLeft = 15 + col * (monthWidth + 10);
                DrawLittleMonth(month, monthLeft, monthTop, monthWidth, monthHeight);
            }

        }

        private void DrawBigMonth()
        {
            double width = canvas.ActualWidth;
            double height = canvas.ActualHeight;
            GetFontSize(width, height);

            // draw month and year at top center
            TextBlock label = new TextBlock();
            switch (selectedMonth)
            {
                case 0: label.Text = "January "; break;
                case 1: label.Text = "February "; break;
                case 2: label.Text = "March "; break;
                case 3: label.Text = "April "; break;
                case 4: label.Text = "May "; break;
                case 5: label.Text = "June "; break;
                case 6: label.Text = "July "; break;
                case 7: label.Text = "August "; break;
                case 8: label.Text = "September "; break;
                case 9: label.Text = "October "; break;
                case 10: label.Text = "November "; break;
                case 11: label.Text = "December "; break;
            }
            label.Text += selectedYear.ToString();
            label.FontSize = fontSize * 3 / 2;
            label.Foreground = GetBrush(selectedMonth);
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = label.DesiredSize.Width;
            Canvas.SetLeft(label, width / 2 - textWidth / 2 - 5); // centered horizontally
            Canvas.SetTop(label, 5); // vertically at the top
            canvas.Children.Add(label);

            // draw days of the week (Su Mo Tu We Th Fr Sa)
            double dayWidth = (width - 50) / 7.0;
            double dayHeight = (height - fontSize * 3 / 2 - 20) / 6.5;
            int row = 0;
            int col = 0;
            double left = 25;
            double top = 10 + fontSize * 3 / 2 + 3;
            for (int day = 0; day < 7; day++)
            {
                label = new TextBlock();
                switch (day)
                {
                    case 0: label.Text = "Su"; break;
                    case 1: label.Text = "Mo"; break;
                    case 2: label.Text = "Tu"; break;
                    case 3: label.Text = "We"; break;
                    case 4: label.Text = "Th"; break;
                    case 5: label.Text = "Fr"; break;
                    case 6: label.Text = "Sa"; break;
                }
                col = day;
                label.FontSize = fontSize;
                label.Foreground = GetBrush(selectedMonth);
                Canvas.SetLeft(label, left + col * dayWidth + dayWidth/2 - fontSize/2); // centered horizontally
                Canvas.SetTop(label, top); // vertically at the top
                canvas.Children.Add(label);
            }

            // draw all the dates in the month
            // get first day of the month
            int fdotm = GetFirstDayOfMonth(selectedMonth, selectedYear);
            // get number of day in the month
            int ditm = GetDaysInMonth(selectedMonth, selectedYear);
            top += fontSize + 10;
            for (int day = 1; day <= ditm; day++)
            {
                row = (fdotm + day - 1) / 7;
                col = (fdotm + day - 1) % 7;
                label = new TextBlock();
                label.Text = day.ToString();
                if (day < 10) label.Text = " " + label.Text;
                label.FontSize = fontSize;
                label.Foreground = GetBrush(selectedMonth);
                Canvas.SetLeft(label, left + col * dayWidth + 5); // horizontally at the left
                Canvas.SetTop(label, top + row * dayHeight); // vertically at the top
                canvas.Children.Add(label);

                // draw a rectangle around the date
                Rectangle rect = new Rectangle();
                rect.Width = dayWidth + 1;
                rect.Height = dayHeight + 1;
                rect.Stroke = GetBrush(selectedMonth);
                rect.StrokeThickness = 1;
                rect.Fill = Brushes.Transparent;
                Canvas.SetLeft(rect, left + col * dayWidth);  // x position
                Canvas.SetTop(rect, top + row * dayHeight);   // y position
                canvas.Children.Add(rect);
            }

        }

        private void DrawLittleMonth(int month, double left, double top, double width, double height)
        {
            // draw month at top center
            TextBlock label = new TextBlock();
            switch (month)
            {
                case  0: label.Text = "January"; break;
                case  1: label.Text = "Febrary"; break;
                case  2: label.Text = "March"; break;
                case  3: label.Text = "April"; break;
                case  4: label.Text = "May"; break;
                case  5: label.Text = "June"; break;
                case  6: label.Text = "July"; break;
                case  7: label.Text = "August"; break;
                case  8: label.Text = "September"; break;
                case  9: label.Text = "October"; break;
                case 10: label.Text = "November"; break;
                case 11: label.Text = "December"; break;
            }
            label.FontSize = fontSize * 3 / 2;
            label.Foreground = GetBrush(month);
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = label.DesiredSize.Width;
            Canvas.SetLeft(label, left + width / 2 - textWidth / 2 - 5); // centered horizontally
            Canvas.SetTop(label, top); // vertically at the top
            canvas.Children.Add(label);

            // draw days of the week (Su Mo Tu We Th Fr Sa)
            double dayWidth = width / 7;
            double dayHeight = (height - fontSize * 3 / 2) / 7;
            int row = 0;
            int col = 0;
            for (int day = 0; day < 7; day++)
            {
                label = new TextBlock();
                switch (day)
                {
                    case 0: label.Text = "Su"; break;
                    case 1: label.Text = "Mo"; break;
                    case 2: label.Text = "Tu"; break;
                    case 3: label.Text = "We"; break;
                    case 4: label.Text = "Th"; break;
                    case 5: label.Text = "Fr"; break;
                    case 6: label.Text = "Sa"; break;
                }
                col = day;
                label.FontSize = fontSize;
                label.Foreground = GetBrush(month);
                Canvas.SetLeft(label, left + col * dayWidth); // centered horizontally
                Canvas.SetTop(label, top + fontSize * 3 / 2 + 3 + row * dayHeight); // vertically at the top
                canvas.Children.Add(label);
            }

            // draw all the dates in the month
            // get first day of the month
            int fdotm = GetFirstDayOfMonth(month, selectedYear);
            // get number of day in the month
            int ditm = GetDaysInMonth(month, selectedYear);
            for (int day = 1; day <= ditm; day++)
            {
                row = 1 + (fdotm + day - 1) / 7;
                col = (fdotm + day - 1) % 7;
                label = new TextBlock();
                label.Text = day.ToString();
                if (day < 10) label.Text = " " + label.Text;
                label.FontSize = fontSize;
                label.Foreground = GetBrush(month);
                Canvas.SetLeft(label, left + col * dayWidth); // centered horizontally
                Canvas.SetTop(label, top + fontSize * 3 / 2 + 3 + row * dayHeight); // vertically at the top
                canvas.Children.Add(label);
            }
        }

        private bool IsLeapYear(int year)
        {
            bool ly = false;
            if (year % 4 == 0) ly = true;
            if (year % 100 == 0) ly = false;
            if (year % 400 == 0) ly = true;
            return ly;
        }

        private int GetDaysInMonth(int month, int year)
        {
            int[] daysInMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int days = 0;
            days = daysInMonth[month];
            if (IsLeapYear(year) && month == 1)
                days += 1;
            return days;
        }

        private int GetFirstDayOfMonth(int month, int year)
        {
            // months are numbered 0 through 11
            //int[] months = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int day = 0;

            // get first day of year
            // years since year 1
            int dy = year - 1;
            // set day to Monday (day 1)
            // add one day for each year since year 1
            // add one day for each leap year since year 1
            day = 1 + dy + dy / 4 - dy / 100 + dy / 400;

            // get first day of month
            // add number of days in each previous month
            for (int i = 0; i < month; i++)
                day += GetDaysInMonth(i, year);

            // return day of the week
            // Sunday to Saturday == 0 to 6
            return day % 7;
        }

        private void GetFontSize(double width, double height)
        {
            if (isDrawYear)
                fontSize = 10;
            else
                fontSize = 20;
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawCalendar();
        }

        private Brush GetBrush(int i)
        {
            Brush[] brushes = {
                Brushes.Blue,
                Brushes.Red,
                Brushes.Green,
                Brushes.Orange,
                Brushes.Purple,
                Brushes.Brown,
                Brushes.Magenta,
                Brushes.DarkBlue,
                Brushes.DarkRed,
                Brushes.DarkGreen,
                Brushes.Teal,
                Brushes.Navy,
                Brushes.Maroon,
                Brushes.Olive,
                Brushes.Lime,
                Brushes.Indigo,
                Brushes.Gold,
                Brushes.Violet,
                Brushes.Pink,
                Brushes.Cyan
            };
            if (IsColor.IsChecked)
                return brushes[i % 3];
            else
                return Brushes.Black;
        }

        private void IsColor_Click(object sender, RoutedEventArgs e)
        {
            DrawCalendar();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+P - Print
            if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Print_Click(sender, e);
                e.Handled = true;
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            // if we are already at the minimum month and year, do nothing
            if (isDrawYear && selectedYear == 1582)
                return;
            else if (selectedYear == 1582 && selectedMonth == 0)
                return;

            if (isDrawYear)
            {
                selectedYear--;
                DrawCalendar();
            }
            else
            {
                selectedMonth--;
                if (selectedMonth < 0)
                {
                    selectedMonth = 11;
                    selectedYear--;
                }
                DrawCalendar();
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (isDrawYear)
            {
                selectedYear++;
                DrawCalendar();
            }
            else
            {
                selectedMonth++;
                if (selectedMonth > 11)
                {
                    selectedMonth = 0;
                    selectedYear++;
                }
                DrawCalendar();
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    double pageWidth = printDialog.PrintableAreaWidth;
                    double pageHeight = printDialog.PrintableAreaHeight;

                    RenderTargetBitmap renderBitmap = RenderElementToBitmap(mainGrid);

                    Image printImage = new Image();
                    printImage.Source = renderBitmap;
                    printImage.Stretch = Stretch.Uniform;

                    // Calculate exact scale
                    double scaleX = pageWidth / renderBitmap.PixelWidth;
                    double scaleY = pageHeight / renderBitmap.PixelHeight;
                    double scale = Math.Min(scaleX, scaleY);

                    // Larger safety margin for larger bitmaps
                    if (renderBitmap.PixelWidth > 2000)
                        scale *= 0.96;  // 4% margin for very large
                    else if (renderBitmap.PixelWidth > 1500)
                        scale *= 0.97;  // 3% margin for large
                    else
                        scale *= 0.98;  // 2% margin for normal

                    printImage.Width = renderBitmap.PixelWidth * scale;
                    printImage.Height = renderBitmap.PixelHeight * scale;

                    printImage.Measure(new Size(pageWidth, pageHeight));
                    printImage.Arrange(new Rect(0, 0, printImage.Width, printImage.Height));

                    printDialog.PrintVisual(printImage, "Graph Application - Graph Print");
                    InvalidateWindowLayout();
                    MessageBox.Show("Calendar sent to printer!", "Success",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Extract your rendering logic into a reusable method
        private RenderTargetBitmap RenderElementToBitmap(FrameworkElement element)
        {
            element.SnapsToDevicePixels = true;

            element.Measure(new Size(element.ActualWidth, element.ActualHeight));
            element.Arrange(new Rect(new Size(element.ActualWidth, element.ActualHeight)));

            double width = element.ActualWidth + 1;
            double height = element.ActualHeight + 1;

            double padding = 30;
            double totalWidth = width + (padding * 2);
            double totalHeight = height + (padding * 2);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
                (int)Math.Ceiling(totalWidth),
                (int)Math.Ceiling(totalHeight),
                96, 96, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                // White background
                context.DrawRectangle(Brushes.White, null, new Rect(0, 0, totalWidth + 1, totalHeight + 1));

                // Draw element
                VisualBrush brush = new VisualBrush(element);
                context.DrawRectangle(brush, null, new Rect(padding, padding, width, height));
            }

            renderBitmap.Render(visual);

            return renderBitmap;
        }

        // Force window to re-layout after printing
        private void InvalidateWindowLayout()
        {
            mainGrid.InvalidateVisual();
            mainGrid.UpdateLayout();
            this.InvalidateVisual();

            // Or use your fake resize trick
            double originalWidth = this.Width;
            double originalHeight = this.Height;
            this.Width = originalWidth + 1;
            this.Height = originalHeight + 1;
            this.UpdateLayout();
            this.Width = originalWidth;
            this.Height = originalHeight;

            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                mainGrid.InvalidateMeasure();
                mainGrid.InvalidateArrange();
                mainGrid.UpdateLayout();
            }));

            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.UpdateLayout();
                this.WindowState = WindowState.Maximized;
            }

            DrawCalendar();
        }

    }
}