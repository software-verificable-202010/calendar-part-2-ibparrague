using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Proyecto_2_Software_Verificable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isButtonsAvailable = false;
        DateTime activeDate = DateTime.Now;
        const int MondayPosition = 0;
        const int TuesdayPosition = 1;
        const int WendesdayPosition = 2;
        const int ThursdayPosition = 3;
        const int FridayPosition = 4;
        const int SaturdayPosition = 5;
        const int SundayPosition = 6;
        public MainWindow()
        {
            InitializeComponent();
            UpdateCalendarGrid(activeDate);
            this.KeyDown += new KeyEventHandler(ReadArrowInput);
            UpdateCalendarWeekGrid();
        }

        private void UpdateCalendarWeekGrid()
        {
            ClearWeekGrids();
            UpdateWeekMonthLabel(DateTime.Now); //TODO: RECIBIR FECHA DINAMICAMENTE
            CreateGoBackToMonthButton();
            FillgridWeeklyDaysLabels(DateTime.Now); //TODO: RECIBIR FECHA DINAMICAMENTE
            FillGridWeekHours(gridDayHoursNumbers);
            ColourGridBorders(gridDayHoursNumbers, "grayLines");
        }
        private void CreateGoBackToMonthButton()
        {
            Button goBackToMonthButton = new Button
            {
                FontSize = 30,
                Content = "Back"
            };
            goBackToMonthButton.Click += GoBackToMonthButton_Click;
            gridWeeklyDays.Children.Add(goBackToMonthButton);
        }
        private void GoBackToMonthButton_Click(object sender, RoutedEventArgs e)
        {
            gridMonthlyView.Visibility = Visibility.Visible;
            gridWeeklyView.Visibility = Visibility.Hidden;
            isButtonsAvailable = true;
        }
        private void ClearWeekGrids()
        {
            gridDayHoursNumbers.Children.Clear();
            gridWeeklyDays.Children.Clear();
        }
        private void UpdateWeekMonthLabel(DateTime dateOfReference)
        {
            CultureInfo usEnglish = new CultureInfo("en-US");
            lblWeekMonth.Content = GetFirstDayOfWeek(dateOfReference).ToString("MMMM", usEnglish).ToUpper();
        }
        private void FillgridWeeklyDaysLabels(DateTime dateOfReference)
        {
            DateTime mondayDate = GetFirstDayOfWeek(dateOfReference);
            const int numberDaysOfWeek = 7;
            for (int xCoordinateOfWeek = 0; xCoordinateOfWeek < numberDaysOfWeek; xCoordinateOfWeek++)
            {
                string labelContent = "";
                switch (xCoordinateOfWeek)
                {
                    case MondayPosition:
                        labelContent = "MON ";
                        break;
                    case TuesdayPosition:
                        labelContent = "TUE ";
                        break;
                    case WendesdayPosition:
                        labelContent = "WED ";
                        break;
                    case ThursdayPosition:
                        labelContent = "THU ";
                        break;
                    case FridayPosition:
                        labelContent = "FRI ";
                        break;
                    case SaturdayPosition:
                        labelContent = "SAT ";
                        break;
                    case SundayPosition:
                        labelContent = "SUN ";
                        break;
                }
                labelContent += mondayDate.AddDays(xCoordinateOfWeek).Day.ToString();
                Label dayLabel = new Label
                {
                    Content = labelContent,
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#999999")),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 35,
                };
                int xCoordinateColumnOffset = 1;
                Grid.SetColumn(dayLabel, xCoordinateOfWeek + xCoordinateColumnOffset);
                gridWeeklyDays.Children.Add(dayLabel);
            }
        }
        private DateTime GetFirstDayOfWeek(DateTime dateOfReference)
        {
            string dayOfWeekOfReferenceDate = dateOfReference.DayOfWeek.ToString();
            DateTime firstDayOfWeek;
            int daysToSubtractToGetFirstDayOfWeek = 0;
            switch (dayOfWeekOfReferenceDate)
            {
                case "Monday":
                    daysToSubtractToGetFirstDayOfWeek = 0;
                    break;
                case "Tuesday":
                    daysToSubtractToGetFirstDayOfWeek = 1;
                    break;
                case "Wednesday":
                    daysToSubtractToGetFirstDayOfWeek = 2;
                    break;
                case "Thursday":
                    daysToSubtractToGetFirstDayOfWeek = 3;
                    break;
                case "Friday":
                    daysToSubtractToGetFirstDayOfWeek = 4;
                    break;
                case "Saturday":
                    daysToSubtractToGetFirstDayOfWeek = 5;
                    break;
                case "Sunday":
                    daysToSubtractToGetFirstDayOfWeek = 6;
                    break;
            }
            firstDayOfWeek = dateOfReference.AddDays(-daysToSubtractToGetFirstDayOfWeek);
            return firstDayOfWeek;
        }
        public void UpdateCalendarGrid(DateTime dateOfReference)
        {
            gridCalendar.Children.Clear();
            UpdateMonthYearLabel(dateOfReference);
            FillGridCalendar(gridCalendar, dateOfReference);
            ColourGridBorders(gridCalendar, "blueLines");
        }
        public void UpdateMonthYearLabel(DateTime date)
        {
            CultureInfo usEnglish = new CultureInfo("en-US");
            lblMonth.Content = date.ToString("MMMM", usEnglish).ToUpper();
            lblYear.Content = date.Year.ToString();
        }
        public void FillGridCalendar(Grid gridCalendar, DateTime dateWithinAMonth)
        {
            //x Coordate of each day on calendar grid

            int firstDayOfMonthNumber = 1;
            DateTime firstDayOfMonth = new DateTime(dateWithinAMonth.Year, dateWithinAMonth.Month, firstDayOfMonthNumber);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            int xCoordinateToInsertNumber = 0;
            int yCoordinateToInsertNumber = 0;
            int numberOfDay = 1;
            string dayOfWeekToStart = firstDayOfMonth.DayOfWeek.ToString();
            switch (dayOfWeekToStart)
            {
                case "Monday":
                    xCoordinateToInsertNumber = MondayPosition;
                    break;
                case "Tuesday":
                    xCoordinateToInsertNumber = TuesdayPosition;
                    break;
                case "Wednesday":
                    xCoordinateToInsertNumber = WendesdayPosition;
                    break;
                case "Thursday":
                    xCoordinateToInsertNumber = ThursdayPosition;
                    break;
                case "Friday":
                    xCoordinateToInsertNumber = FridayPosition;
                    break;
                case "Saturday":
                    xCoordinateToInsertNumber = SaturdayPosition;
                    break;
                case "Sunday":
                    xCoordinateToInsertNumber = SundayPosition;
                    break;
            }

            while (numberOfDay <= lastDayOfMonth.Day)
            {
                //Adding diffent color background to make stand out from days that aren't included in current month and are not weekends
                if (xCoordinateToInsertNumber != SaturdayPosition && xCoordinateToInsertNumber != SundayPosition)
                {
                    Rectangle rectangleDaysBackground = new Rectangle()
                    {
                        Fill = Brushes.White
                    };
                    Grid.SetColumn(rectangleDaysBackground, xCoordinateToInsertNumber);
                    Grid.SetRow(rectangleDaysBackground, yCoordinateToInsertNumber);
                    gridCalendar.Children.Add(rectangleDaysBackground);
                }
                else
                {
                    Rectangle rectangleWeekendBackground = new Rectangle()
                    {
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#c1f7da")),
                    };
                    Grid.SetColumn(rectangleWeekendBackground, xCoordinateToInsertNumber);
                    Grid.SetRow(rectangleWeekendBackground, yCoordinateToInsertNumber);
                    gridCalendar.Children.Add(rectangleWeekendBackground);
                }

                //Adding number Labels to each day
                Label label = new Label()
                {
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    FontSize = 25,
                    Content = numberOfDay
                };
                Grid.SetColumn(label, xCoordinateToInsertNumber);
                Grid.SetRow(label, yCoordinateToInsertNumber);
                gridCalendar.Children.Add(label);

                //Making the login of coordinates advance
                xCoordinateToInsertNumber++;
                if (xCoordinateToInsertNumber == gridCalendar.ColumnDefinitions.Count())
                {
                    yCoordinateToInsertNumber++;
                    xCoordinateToInsertNumber = 0;
                }

                numberOfDay++;
            }
        }
        public void FillGridWeekHours(Grid grid)
        {
            int gridXCoordinate = 0;
            for (int index = 0; index < 24; index++)
            {
                int gridYCoordinate = index;
                Label labelHour = new Label()
                {
                    FontSize = 20,
                    Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#999999")),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                labelHour.Content = index.ToString() + ":00";
                Grid.SetColumn(labelHour, gridXCoordinate);
                Grid.SetRow(labelHour, gridYCoordinate);
                grid.Children.Add(labelHour);
            }
        }
        public void ColourGridBorders(Grid grid, string customRectangleType)
        {
            int columnAmount = grid.ColumnDefinitions.Count();
            int rowAmount = grid.RowDefinitions.Count();
            int x_coordinate;
            int y_coordinate = 0;
            while (y_coordinate < rowAmount)
            {
                x_coordinate = 0;
                while (x_coordinate < columnAmount)
                {
                    Rectangle rectangleDayBorder = CustomRectangle(customRectangleType);
                    Grid.SetColumn(rectangleDayBorder, x_coordinate);
                    Grid.SetRow(rectangleDayBorder, y_coordinate);
                    grid.Children.Add(rectangleDayBorder);
                    x_coordinate++;
                }
                y_coordinate++;
            }
        }
        private void btnNextDay_Click(object sender, RoutedEventArgs e)
        {
            activeDate = activeDate.AddMonths(1);
            UpdateCalendarGrid(activeDate);
        }
        private void btnPrevDay_Click(object sender, RoutedEventArgs e)
        {
            activeDate = activeDate.AddMonths(-1);
            UpdateCalendarGrid(activeDate);
        }
        void ReadArrowInput(object sender, KeyEventArgs e)
        {
            if (isButtonsAvailable)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        activeDate = activeDate.AddMonths(1);
                        UpdateCalendarGrid(activeDate);
                        break;
                    case Key.Left:
                        activeDate = activeDate.AddMonths(-1);
                        UpdateCalendarGrid(activeDate);
                        break;
                    case Key.Up:
                        activeDate = activeDate.AddYears(1);
                        UpdateCalendarGrid(activeDate);
                        break;
                    case Key.Down:
                        activeDate = activeDate.AddYears(-1);
                        UpdateCalendarGrid(activeDate);
                        break;
                }
            }
        }
        public Rectangle CustomRectangle(string rectangleType)
        {
            Rectangle rectangle;
            if (rectangleType == "blueLines")
            {
                rectangle = new Rectangle()
                {
                    Fill = Brushes.Transparent,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1f3861")),
                    StrokeThickness = 2
                };
            }
            else if (rectangleType == "grayLines")
            {
                rectangle = new Rectangle()
                {
                    Fill = Brushes.Transparent,
                    Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#999999")),
                    StrokeThickness = 0.5
                };
            }
            else
            {
                rectangle = new Rectangle();
            }
            return rectangle;
        }

    }
}
