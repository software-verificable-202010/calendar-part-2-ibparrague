using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit;

namespace Proyecto_2_Software_Verificable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isArrowInputAvailable = true;
        DateTime activeMonthDate = DateTime.Now;
        DateTime activeWeekDate = DateTime.Now;
        List<Appointment> appointments = new List<Appointment>();
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
            appointments = DeserializeAppointments();
            UpdateCalendarGrid(activeMonthDate);
            this.KeyDown += new KeyEventHandler(ReadArrowInput);            
        }
        private Color GenerateRandomColor(Random randomizer)
        {
            Color randomColor = Color.FromRgb((byte)randomizer.Next(0, 255), (byte)randomizer.Next(0, 255), (byte)randomizer.Next(0, 255));
            return randomColor;
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
        private bool CheckifAppointmentIsValid()
        {
            bool isAppointmentTitleValid = false;
            bool isAppointmentDescriptionValid = false;
            bool isAppointmentDateValid = false;
            bool isAppointmentStartHourValid = false;
            bool isAppointmentEndHourValid = false;

            if (txtTitleNewAppointment.Text.Trim(' ') != "")
                isAppointmentTitleValid = true;

            if (txtDecriptionNewAppointment.Text.Trim(' ') != "")
                isAppointmentDescriptionValid = true;

            if (datePickerNewAppointment.SelectedDate.HasValue)
                isAppointmentDateValid = true;

            if (timePickerStartTimeNewAppointment.Value != null)
                isAppointmentStartHourValid = true;
            if (timePickerEndTimeNewAppointment.Value != null &&
            timePickerStartTimeNewAppointment.Value < timePickerEndTimeNewAppointment.Value)
                isAppointmentEndHourValid = true;

            if (isAppointmentTitleValid && isAppointmentDescriptionValid &&
                isAppointmentDateValid && isAppointmentStartHourValid && isAppointmentEndHourValid)
                return true;
            else
                return false;
        }
        private void ClearWeekGrids()
        {
            gridDayHoursNumbers.Children.Clear();
            gridWeeklyDays.Children.Clear();
        }
        public void CreateClickableGridBorders(Grid grid, string customRectangleType)
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
                    rectangleDayBorder.MouseDown += RectangleDayBorder_MouseDown;
                    Grid.SetColumn(rectangleDayBorder, x_coordinate);
                    Grid.SetRow(rectangleDayBorder, y_coordinate);
                    grid.Children.Add(rectangleDayBorder);
                    x_coordinate++;
                }
                y_coordinate++;
            }
        }
        private void DisplayMonthMenu()
        {
            gridMonthlyView.Visibility = Visibility.Visible;
            gridWeeklyView.Visibility = Visibility.Hidden;
            gridAppointmentCreation.Visibility = Visibility.Hidden;
            isArrowInputAvailable = true;
        }
        private void DisplayNewAppointmentMenu()
        {
            gridMonthlyView.Visibility = Visibility.Hidden;
            gridWeeklyView.Visibility = Visibility.Hidden;
            gridAppointmentCreation.Visibility = Visibility.Visible;
            isArrowInputAvailable = false;
        }
        private void DisplayWeeklyViewMenu()
        {
            gridMonthlyView.Visibility = Visibility.Hidden;
            gridWeeklyView.Visibility = Visibility.Visible;
            gridAppointmentCreation.Visibility = Visibility.Hidden;
            isArrowInputAvailable = false;
        }
        public void FillGridCalendar(Grid gridCalendar, DateTime dateWithinAMonth)
        {
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
                    Content = numberOfDay,              
                };
                Grid.SetColumn(label, xCoordinateToInsertNumber);
                Grid.SetRow(label, yCoordinateToInsertNumber);
                gridCalendar.Children.Add(label);

                //Adding Appointments to each day
                DateTime dayToCheckAppointments = firstDayOfMonth.Date.AddDays(numberOfDay-1); //The -1 is given so that we count from theorical day 0 insted of 1
                List<Appointment> dayAppointments = SelectDayAppointments(dayToCheckAppointments);
                if (dayAppointments.Count != 0)
                {
                    Label appointmentLabel = new Label()
                    {
                        Foreground = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 15,
                        Content = ""
                    };
                    foreach (Appointment appointment in dayAppointments)
                    {
                        if (appointmentLabel.Content.ToString() == "")
                        {
                            appointmentLabel.Content += appointment.title;
                        }
                        else
                        {
                            appointmentLabel.Content += "\n";
                            appointmentLabel.Content += appointment.title;
                        }
                    }
                    Grid.SetColumn(appointmentLabel, xCoordinateToInsertNumber);
                    Grid.SetRow(appointmentLabel, yCoordinateToInsertNumber);
                    gridCalendar.Children.Add(appointmentLabel);
                }

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
        private void FillGridWeekAppointments(DateTime dateOfReference)
        {
            DateTime mondayDate = GetFirstDayOfWeek(dateOfReference);
            const int numberDaysOfWeek = 7;
            DateTime sunday = mondayDate.AddDays(numberDaysOfWeek);
            List<Appointment> validAppointments = appointments.FindAll(a => a.isBetweenDates(mondayDate, sunday));
            Random randomizer = new Random();
            foreach (Appointment validAppointment in validAppointments)
            {
                int hoursInADay = 24;
                int minutesInADay = 1440;
                int weekGridOffset = 1;
                int xCoordinateToInsert = 0;
                string dayOfWeekOfAppointment = validAppointment.date.DayOfWeek.ToString();
                switch (dayOfWeekOfAppointment)
                {
                    case "Monday":
                        xCoordinateToInsert = MondayPosition;
                        break;
                    case "Tuesday":
                        xCoordinateToInsert = TuesdayPosition;
                        break;
                    case "Wednesday":
                        xCoordinateToInsert = WendesdayPosition;
                        break;
                    case "Thursday":
                        xCoordinateToInsert = ThursdayPosition;
                        break;
                    case "Friday":
                        xCoordinateToInsert = FridayPosition;
                        break;
                    case "Saturday":
                        xCoordinateToInsert = SaturdayPosition;
                        break;
                    case "Sunday":
                        xCoordinateToInsert = SundayPosition;
                        break;
                }

                double startHourInMinutes = (validAppointment.startTime - validAppointment.startTime.Date).TotalMinutes;
                double endHourInMinutes = (validAppointment.endTime - validAppointment.endTime.Date).TotalMinutes;
                double rectangleHeight = endHourInMinutes - startHourInMinutes;
                double topMargin = startHourInMinutes;
                double bottomMargin = minutesInADay - endHourInMinutes;
                Color randomColor = GenerateRandomColor(randomizer);
                SolidColorBrush randomColorBrush = new SolidColorBrush(randomColor);
                Rectangle rectangle = new Rectangle
                {
                    Height = rectangleHeight,
                    Margin = new Thickness(0, topMargin, 0, bottomMargin),
                    Fill = Brushes.Transparent,
                    StrokeThickness = 5,
                    Stroke = randomColorBrush,
                };
                Grid.SetColumn(rectangle, xCoordinateToInsert + weekGridOffset);
                Grid.SetRowSpan(rectangle, hoursInADay);
                gridDayHoursNumbers.Children.Add(rectangle);


                Label titleLabel = new Label
                {
                    Margin = new Thickness(0, topMargin, 0, bottomMargin),
                    FontSize = 20,
                    Content = "",
                };

                string titleForLabel = validAppointment.title;
                string labelContent = "";
                int maxHorizontalCharacterForLabelTitle = 14;
                int index = 0;
                int titleTextLines = 1;
                while (index < titleForLabel.Length)
                {
                    labelContent += validAppointment.title[index];
                    if (index % maxHorizontalCharacterForLabelTitle == 0 && index > 1)
                    {
                        labelContent += "\n";
                        titleTextLines += 1;
                    }
                    index++;
                }
                titleLabel.Content = labelContent;
                Grid.SetColumn(titleLabel, xCoordinateToInsert + weekGridOffset);
                Grid.SetRowSpan(titleLabel, hoursInADay);
                gridDayHoursNumbers.Children.Add(titleLabel);
                int titleFontLinePixels = 25;
                Label descriptionLabel = new Label
                {
                    Margin = new Thickness(0, topMargin+(titleFontLinePixels * titleTextLines), 0, bottomMargin),
                    FontSize = 10,
                    Content = "",
                };

                string desciptionForLabel = validAppointment.description;
                labelContent = "";
                index = 0;
                while (index < desciptionForLabel.Length)
                {
                    labelContent += validAppointment.description[index];
                    if (index % maxHorizontalCharacterForLabelTitle == 0 && index > 1)
                    {
                        labelContent += "\n";
                    }
                    index++;
                }
                descriptionLabel.Content = labelContent;
                Grid.SetColumn(descriptionLabel, xCoordinateToInsert + weekGridOffset);
                Grid.SetRowSpan(descriptionLabel, hoursInADay);
                gridDayHoursNumbers.Children.Add(descriptionLabel);
            }
        }
        private void FillGridWeeklyDaysLabels(DateTime dateOfReference)
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
        private void RectangleDayBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = e.Source as Rectangle;
            if (gridCalendar.Children.Contains(rectangle))
            {
                int index = Grid.GetRow(rectangle);
                int weekRowClicked = index;
                DateTime firstDayOfMonth = activeMonthDate.Date.AddDays(1-DateTime.Now.Day);
                int daysInAWeek = 7;
                activeWeekDate = firstDayOfMonth.AddDays(daysInAWeek * weekRowClicked);
                UpdateCalendarWeekGrid();
                DisplayWeeklyViewMenu();
            }    
        }
        void ReadArrowInput(object sender, KeyEventArgs e)
        {
            if (isArrowInputAvailable)
            {
                switch (e.Key)
                {
                    case Key.Right:
                        activeMonthDate = activeMonthDate.AddMonths(1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                    case Key.Left:
                        activeMonthDate = activeMonthDate.AddMonths(-1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                    case Key.Up:
                        activeMonthDate = activeMonthDate.AddYears(1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                    case Key.Down:
                        activeMonthDate = activeMonthDate.AddYears(-1);
                        UpdateCalendarGrid(activeMonthDate);
                        break;
                }
            }
        }
        private void SaveNewAppointment()
        {
            string newAppointmentTitle = txtTitleNewAppointment.Text;
            string newAppointmentDescription = txtDecriptionNewAppointment.Text;
            DateTime newAppointmentDate = datePickerNewAppointment.SelectedDate.Value.Date;
            DateTime newAppointmentStartTime = timePickerStartTimeNewAppointment.Value.Value;
            DateTime newAppointmentEndTime = timePickerEndTimeNewAppointment.Value.Value;
            Appointment appointment = new Appointment(newAppointmentTitle, newAppointmentDescription,
                newAppointmentDate, newAppointmentStartTime, newAppointmentEndTime);
            appointments.Add(appointment);
        }        
        private List<Appointment> SelectDayAppointments(DateTime day)
        {
            DateTime dayHourZero = day.Date;
            DateTime nextDay = dayHourZero.AddDays(1);
            List<Appointment> validAppointments = appointments.FindAll(a => a.isBetweenDates(dayHourZero, nextDay));
            return validAppointments;

        }
        public void UpdateCalendarGrid(DateTime dateOfReference)
        {
            gridCalendar.Children.Clear();
            UpdateMonthYearLabel(dateOfReference);
            FillGridCalendar(gridCalendar, dateOfReference);
            CreateClickableGridBorders(gridCalendar, "blueLines");
        }
        private void UpdateCalendarWeekGrid()
        {
            ClearWeekGrids();
            UpdateWeekMonthLabel(activeWeekDate);
            FillGridWeeklyDaysLabels(activeWeekDate);
            FillGridWeekHours(gridDayHoursNumbers);
            FillGridWeekAppointments(activeWeekDate);
            CreateClickableGridBorders(gridDayHoursNumbers, "grayLines");
        }
        public void UpdateMonthYearLabel(DateTime date)
        {
            CultureInfo usEnglish = new CultureInfo("en-US");
            lblMonth.Content = date.ToString("MMMM", usEnglish).ToUpper();
            lblYear.Content = date.Year.ToString();
        }
        private void UpdateWeekMonthLabel(DateTime dateOfReference)
        {
            CultureInfo usEnglish = new CultureInfo("en-US");
            string monthName = GetFirstDayOfWeek(dateOfReference).ToString("MMMM", usEnglish).ToUpper();
            lblWeekMonth.Content = monthName;
            int daysToLastDayOfWeek = 6;
            string weekendMonthName = GetFirstDayOfWeek(dateOfReference).AddDays(daysToLastDayOfWeek).ToString("MMMM", usEnglish).ToUpper();
            if (monthName != weekendMonthName)
            {
                lblWeekMonth.Content += " / " + weekendMonthName;
            }
            
        }
        private void btnGoBackToMonth_Click(object sender, RoutedEventArgs e)
        {
            DisplayMonthMenu();
            UpdateCalendarGrid(activeMonthDate);
        }
        private void btnNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            DisplayNewAppointmentMenu();
        }
        private void btnCancelNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            DisplayWeeklyViewMenu();
        }
        private void btnSaveNewAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (CheckifAppointmentIsValid())
            {
                SaveNewAppointment();
                UpdateCalendarWeekGrid();
                DisplayWeeklyViewMenu();
                SerializeAppointments(appointments);
            }
            else
            {
                lblErrorsNewAppointment.Content = "Error creating appointment, check fields";
            }
        }
        private void btnNextDay_Click(object sender, RoutedEventArgs e)
        {
            activeMonthDate = activeMonthDate.AddMonths(1);
            UpdateCalendarGrid(activeMonthDate);
        }
        private void btnPrevDay_Click(object sender, RoutedEventArgs e)
        {
            activeMonthDate = activeMonthDate.AddMonths(-1);
            UpdateCalendarGrid(activeMonthDate);
        }
        private void btnPreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            int daysInAWeek = 7;
            activeWeekDate = activeWeekDate.AddDays(-daysInAWeek);
            UpdateCalendarWeekGrid();
        }
        private void btnNextWeek_Click(object sender, RoutedEventArgs e)
        {
            int daysInAWeek = 7;
            activeWeekDate = activeWeekDate.AddDays(daysInAWeek);
            UpdateCalendarWeekGrid();
        }
        static public void SerializeAppointments(List<Appointment> appointments)
        {
            string path = Environment.CurrentDirectory + "\\SeriaizedAppointments.txt";
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            formatter.Serialize(stream, appointments);
            stream.Close();
        }
        static public List<Appointment> DeserializeAppointments()
        {
            IFormatter formatter = new BinaryFormatter();
            string path = Environment.CurrentDirectory + "\\SeriaizedAppointments.txt";
            if (File.Exists(path))
            {
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                List<Appointment> loadedAppointments = (List<Appointment>)formatter.Deserialize(stream);
                stream.Close();
                return loadedAppointments;
            }
            else
            {
                return new List<Appointment>();
            }
        }
    }
}
