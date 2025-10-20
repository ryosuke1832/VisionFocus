using Microsoft.Maui.Controls;
using ScottPlot.Maui;
using VisionFocus.Services;
using System.Globalization;
using System.IO;

namespace VisionFocus;

public partial class StatisticsPage : ContentPage
{
    private string _selectedSubject = string.Empty;
    private DateTime _selectedDate = DateTime.Today;

    public StatisticsPage()
    {
        InitializeComponent();
        LoadSubjects();   // Reuse function logic
        DatePickerFilter.Date = _selectedDate;
        DatePickerFilter.MaximumDate = DateTime.Today;
    }

    // Handles navigation back to the previous page
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    // Reuse logic from SessionPage for loading subjects
    private void LoadSubjects()
    {
        try
        {
            var settings = SettingsService.LoadSettings();
            SubjectPicker.ItemsSource = settings.Subjects;

            if (settings.Subjects.Count > 0)
            {
                SubjectPicker.SelectedIndex = 0;
                _selectedSubject = settings.Subjects[0];
                LoadChartsForSubjectAndDate();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Subject loading error: {ex.Message}");
        }
    }

    // Triggered when user changes the selected subject
    private void OnSubjectChanged(object sender, EventArgs e)
    {
        if (SubjectPicker.SelectedItem is string subject)
        {
            _selectedSubject = subject;
            LoadChartsForSubjectAndDate(); // Refresh charts based on new selection
        }
    }

    // Triggered when user selects a new date from the DatePicker
    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        _selectedDate = e.NewDate;
        LoadChartsForSubjectAndDate(); // Refresh charts based on new selection
    }

    // Loads and plots alert data for the selected subject and date
    private void LoadChartsForSubjectAndDate()
    {
        ChartsContainer.Children.Clear();

        if (string.IsNullOrEmpty(_selectedSubject))
            return;

        try
        {
            // Find all CSV session files that match the selected subject and date
            var files = SessionDataService.GetAllSessionDetailFiles()
                .Where(f =>
                {
                    // Extract session date & subject from filename
                    // Format: Session_20251014_130432_Math.csv
                    var parts = Path.GetFileNameWithoutExtension(f).Split('_');
                    if (parts.Length < 4) return false;

                    string datePart = parts[1];
                    string subject = parts[3];

                    if (!DateTime.TryParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime fileDate))
                        return false;

                    return subject.Equals(_selectedSubject, StringComparison.OrdinalIgnoreCase)
&& fileDate.Date == _selectedDate.Date;
                })
                .ToList();

            // Show message if no session data is available
            if (files.Count == 0)
            {
                ChartsContainer.Children.Add(new Label
                {
                    Text = "No session data available for selected filters.",
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Microsoft.Maui.Graphics.Colors.Gray
                });
                return;
            }

            // Process and plot data for each matching file
            foreach (var file in files)
            {
                var filePath = Path.Combine(SessionDataService.EachDataFolderPath, file);
                var lines = File.ReadAllLines(filePath).Skip(2).ToList(); // skip first 2 header lines

                double[] time = new double[lines.Count];
                double[] alerts = new double[lines.Count];

                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double t) &&
                        double.TryParse(parts[1], out double a))
                    {
                        time[i] = t;
                        alerts[i] = a;
                    }
                }

                // Create a new plot for the session
                var plot = new MauiPlot
                {
                    HeightRequest = 350,
                    WidthRequest = 650,
                    Margin = new Thickness(0, 10)
                };

                // Plot alert data over time
                plot.Plot.Add.Scatter(time, alerts, color: ScottPlot.Colors.Blue);

                // Add titles and axis labels
                plot.Plot.Title(Path.GetFileNameWithoutExtension(file));
                plot.Plot.XLabel("Minutes");
                plot.Plot.YLabel("Alert Count");

                // Set Y-axis range dynamically based on alert values
                plot.Plot.Axes.SetLimitsY(0, alerts.Max() + 1);

                // Render and display the chart
                plot.Refresh();

                ChartsContainer.Children.Add(plot);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading charts: {ex.Message}");
        }
    }
}