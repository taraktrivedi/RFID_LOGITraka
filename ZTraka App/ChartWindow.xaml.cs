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
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        // Instantiates table adapter and data table
        //ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter chartingTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter();
        //ztATdbLocalDataSet1.ChartingQueryDataTable chartingDataTable = new ztATdbLocalDataSet1.ChartingQueryDataTable();

        private int valSum;
        /// <summary>
        /// Constructor of the <see cref="ChartWindow" /> class.
        /// </summary>
        public ChartWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the chartToolWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void chartToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Chart loading...
        }

        /// <summary>
        /// Handles the Selected event of the tviByAssetCategory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviByAssetCategory_Selected(object sender, RoutedEventArgs e)
        {
            displayCharts("AssetCategory");
        }

        /// <summary>
        /// Handles the Selected event of the tviByAssetStatus control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviByAssetStatus_Selected(object sender, RoutedEventArgs e)
        {
            displayCharts("AssetStatus");

        }

        /// <summary>
        /// Handles the Selected event of the tviAssetsperfloor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviAssetsperfloor_Selected(object sender, RoutedEventArgs e)
        {
            displayCharts("Assetsperfloor");
        }

        /// <summary>
        /// Handles the Selected event of the tviroomsperfloor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviroomsperfloor_Selected(object sender, RoutedEventArgs e)
        {
            displayCharts("roomsperfloor");
        }

        /// <summary>
        /// Handles the Selected event of the tviByAssetValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void tviByAssetValue_Selected(object sender, RoutedEventArgs e)
        {
            displayCharts("AssetValue");

        }

        /// <summary>
        /// Displays the charts.
        /// Function used to determine type of chart to be displayed and fill chart data.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        private void displayCharts(string chartType)
        {
            ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter chartingTA = new ZTraka_App.ztATdbLocalDataSet1TableAdapters.ChartingQueryTableAdapter();
            ztATdbLocalDataSet1.ChartingQueryDataTable chartingDataTable = new ztATdbLocalDataSet1.ChartingQueryDataTable();

            int rowCount;
            chartingDataTable.Clear();
            pChart.Visibility = Visibility.Visible;
            //pChart.Refresh();
            makeLabelsInvisible();

            //nEw keyvalue pair
            Dictionary<string, int> dataPieChart = new Dictionary<string, int>();
            this.Cursor = Cursors.Wait;

            try
            {
                //Use case switch statements.

                if (chartType.CompareTo("AssetCategory") == 0)
                {
                    rowCount = chartingTA.ChartingACategory(chartingDataTable);
                    reportChart.Title = "Asset Category Chart";
                }

                else if (chartType.CompareTo("AssetStatus") == 0)
                {

                    rowCount = chartingTA.ChartingAStatus(chartingDataTable);
                    reportChart.Title = "Asset Status Chart";
                }

                else if (chartType.CompareTo("Assetsperfloor") == 0)
                {

                    rowCount = chartingTA.ChartingfAPF(chartingDataTable);
                    reportChart.Title = "Assets per floor Chart";
                }

                else if (chartType.CompareTo("roomsperfloor") == 0)
                {
                    rowCount = chartingTA.ChartingfRPF(chartingDataTable);
                    reportChart.Title = "Rooms per floor Chart";
                }

                else if (chartType.CompareTo("AssetValue") == 0)
                {
                    rowCount = chartingTA.ChartingATotalValue(chartingDataTable);
                    reportChart.Title = "Asset Value Chart";
                }


                for (int i = 0; i < chartingDataTable.Rows.Count; i++)
                {

                    dataPieChart.Add(chartingDataTable.Rows[i]["aType"].ToString().TrimEnd(),
                    int.Parse(chartingDataTable.Rows[i]["aCount"].ToString().TrimEnd()));

                }

                valSum = dataPieChart.Values.Sum();

                textBlockChartDisplayError.Visibility = Visibility.Hidden;
                reportChart.DataContext = dataPieChart;

            }

            catch
            {
                textBlockChartDisplayError.Visibility = Visibility.Visible;
                LogFile.Log("Error: Chart display problem. Connection issues with database or server");
                //Issues with datatable
            }

            finally
            {
                this.Cursor = Cursors.Arrow;
            }
 
        }


        /// <summary>
        /// Handles the SelectionChanged event of the pChart control.
        /// Selection changes on the slice brings up details in readable format
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void pChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string sliceKey = ((System.Collections.Generic.KeyValuePair<string, int>)(((object[])((e.AddedItems)))[0])).Key;
                int sliceCount = ((System.Collections.Generic.KeyValuePair<string, int>)(((object[])((e.AddedItems)))[0])).Value;
                decimal slicePercent = Math.Round( (decimal)((float)sliceCount / valSum) * 100);

                lblCount.Visibility = Visibility.Visible;
                lblItem.Visibility = Visibility.Visible;
                lblpercent.Visibility = Visibility.Visible;
                lblPercentage.Visibility = Visibility.Visible;
                lblSliceKey.Visibility = Visibility.Visible;
                lblSliceCount.Visibility = Visibility.Visible;

                borderSliceResults.Visibility = Visibility.Visible;


                lblSliceKey.Content = sliceKey;
                lblSliceCount.Content = sliceCount;
                lblPercentage.Content = slicePercent + " %";
            }

        }

        /// <summary>
        /// Makes the labels invisible.
        /// </summary>
        private void makeLabelsInvisible()
        {
            lblCount.Visibility = Visibility.Hidden;
            lblItem.Visibility = Visibility.Hidden;
            lblpercent.Visibility = Visibility.Hidden;
            lblPercentage.Visibility = Visibility.Hidden;
            lblSliceKey.Visibility = Visibility.Hidden;
            lblSliceCount.Visibility = Visibility.Hidden;

            borderSliceResults.Visibility = Visibility.Hidden;
 
        }


    }
}
