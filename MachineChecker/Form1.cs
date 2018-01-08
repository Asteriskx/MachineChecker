using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Diagnostics;

namespace MachineChecker
{
	public partial class Form1 : Form
	{
		#region Constants

		/// <summary>
		/// Brousing CPU Graph upper limit.
		/// </summary>
		private readonly int MAX_HISTORY = 40;

		#endregion

		#region Field 

		/// <summary>
		/// CPU Graph Counter.
		/// </summary>
		private Queue<int> _countHistory = new Queue<int>();

		/// <summary>
		/// Using update for CPU Graph.
		/// </summary>
		private Timer _timer { get; set; } = new Timer();

		/// <summary>
		/// Using PerformanceCounter (CPU resourse)
		/// </summary>
		private PerformanceCounter _pc { get; set; } 
			= new PerformanceCounter("Processor", "% Processor Time", "_Total", true);

		#endregion

		#region Constractor

		/// <summary>
		/// Constractor
		/// </summary>
		internal Form1()
		{
			InitializeComponent();
			_initialize(chart);

			_timer.Interval = 1000;
			_timer.Enabled = true;

			// EventHander for System.Windows.Forms.Timer.
			_timer.Tick += (s, v) =>
			{
				// Getting used CPU Resourse. Added history.
				_countHistory.Enqueue((int)_pc.NextValue());

				// If it exceeds the maximum number of history, delete the old one.
				while (_countHistory.Count > MAX_HISTORY)
					_countHistory.Dequeue();

				// Repaint CPU GraphChart.
				_BrowsingChart(chart);
			};

			_timer.Start();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initialize from CPU Line Graph.
		/// </summary>
		/// <param name="chart"></param>
		private void _initialize(Chart chart)
		{
			// Set the background color of the entire chart
			chart.BackColor = Color.DarkBlue;
			chart.ChartAreas[0].BackColor = Color.Transparent;

			// Cut the margin around the chart display area
			chart.ChartAreas[0].InnerPlotPosition.Auto = false;
			chart.ChartAreas[0].InnerPlotPosition.Width = 100;
			chart.ChartAreas[0].InnerPlotPosition.Height = 90; 
			chart.ChartAreas[0].InnerPlotPosition.X = 8;
			chart.ChartAreas[0].InnerPlotPosition.Y = 0;

			// Define X, Y axis information set function
			Action<Axis> setAxis = (axisInfo) => {

				// Limit font size upper limit of axis memory label
				axisInfo.LabelAutoFitMaxFontSize = 8;

				// Set character color of axis memory label
				axisInfo.LabelStyle.ForeColor = Color.White;

				// Set axis color
				axisInfo.MajorGrid.Enabled = true;
				axisInfo.MajorGrid.LineColor = ColorTranslator.FromHtml("#008242");
				axisInfo.MinorGrid.Enabled = false;
				axisInfo.MinorGrid.LineColor = ColorTranslator.FromHtml("#008242");
			};

			// Define display method of X and Y axes
			setAxis(chart.ChartAreas[0].AxisY);
			setAxis(chart.ChartAreas[0].AxisX);

			chart.ChartAreas[0].AxisX.MinorGrid.Enabled = true;
			chart.ChartAreas[0].AxisY.Maximum = 100;

			chart.AntiAliasing = AntiAliasingStyles.None;

			// Line graph
			chart.Series[0].ChartType = SeriesChartType.FastLine;

			// Designate line color
			chart.Series[0].Color = ColorTranslator.FromHtml("#00FF00");

			// Hide the legend, do not display numbers on each value
			chart.Series[0].IsVisibleInLegend = false;
			chart.Series[0].IsValueShownAsLabel = false;

			// Clear all the history of values displayed on the chart to 0
			while (_countHistory.Count <= MAX_HISTORY)
				_countHistory.Enqueue(0);
				
		}

		/// <summary>
		/// Browsing from CPU Line Graph.
		/// </summary>
		/// <param name="chart"></param>
		private void _BrowsingChart(Chart chart)
		{
			chart.Series[0].Points.Clear();
			foreach (int value in _countHistory)
				chart.Series[0].Points.Add(new DataPoint(0, value));
		}

		#endregion

	}
}
