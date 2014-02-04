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
using Microsoft.Reporting.WinForms;

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for ReportsAutoGen.xaml
    /// </summary>
    public partial class ReportsAutoGen : Window
    {
        
        /// <summary>
        /// Constructor of the <see cref="ReportsAutoGen" /> class.
        /// </summary>
        public ReportsAutoGen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Loaded event of the reportsAuto control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void reportsAuto_Loaded(object sender, RoutedEventArgs e)
        {
            //Window load event
        }

        /// <summary>
        /// Handles the Click event of the btnGenerateReport control.
        /// Generates reports according to user selection
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
           //_reportViewer.Clear();
           _reportViewer.Reset();

            // If none selected show warning
            if (cmbSelectCategory.SelectedIndex == -1) //None
            {
                textBlockSelectWarning.Visibility = Visibility.Visible;
                this.Cursor = Cursors.Arrow;
                return;
                
 
            }
            else if (cmbSelectCategory.SelectedIndex == 0) //Assets
            {
                textBlockSelectWarning.Visibility = Visibility.Hidden;
                    
                    // Instantiate dataset, table adapter and datatable
                    ztATdbLocalDSReports1 ds = new ztATdbLocalDSReports1();
                    ztATdbLocalDSReports1TableAdapters.asset_mainDTTableAdapter assetMainTA1 = new ZTraka_App.ztATdbLocalDSReports1TableAdapters.asset_mainDTTableAdapter();
                    ztATdbLocalDSReports1.asset_mainDTDataTable assetMainDataTable = new ztATdbLocalDSReports1.asset_mainDTDataTable();
                    try
                    {
                        assetMainTA1.FillA(assetMainDataTable);

                        ReportDataSource reportDataSourceAsset = new ReportDataSource();
                        ds.BeginInit();
                        reportDataSourceAsset.Name = "ztATdbLocalDSReports1_asset_mainDT";
                        reportDataSourceAsset.Value = assetMainDataTable;
                        //_reportViewer.Reset();
                        this._reportViewer.LocalReport.DataSources.Clear();
                        this._reportViewer.LocalReport.DataSources.Add(reportDataSourceAsset);
                        this._reportViewer.LocalReport.ReportEmbeddedResource = "ZTraka_App.ReportAssets.rdlc";
                        //this._reportViewer.LocalReport.ReportPath = "../../Report1.rdlc";
                        ds.EndInit();
                        _reportViewer.LocalReport.Refresh();
                        //_reportViewer.Reset();
                        _reportViewer.RefreshReport();
                        //_isReportViewerLoaded = true;
                        //ds.Dispose();
                    }

                    catch
                    {
                        LogFile.Log("Error: Report for Assets could not be displayed. Possible issue with database connection.");
                        // Issues with db connection possibly
                    }

            }
            else if (cmbSelectCategory.SelectedIndex == 1) //Users
            {
                textBlockSelectWarning.Visibility = Visibility.Hidden;

                    ztATdbLocalDSReports1 ds = new ztATdbLocalDSReports1();
                    ztATdbLocalDSReports1TableAdapters.UserInfoDTTableAdapter userInfoTA1 = new ZTraka_App.ztATdbLocalDSReports1TableAdapters.UserInfoDTTableAdapter();
                    ztATdbLocalDSReports1.UserInfoDTDataTable userInfoDataTable = new ztATdbLocalDSReports1.UserInfoDTDataTable();

                    try
                    {
                        userInfoTA1.FillU(userInfoDataTable);
                        ReportDataSource reportDataSourceUser = new ReportDataSource();
                        ds.BeginInit();
                        reportDataSourceUser.Name = "ztATdbLocalDSReports1_UserInfoDT";
                        reportDataSourceUser.Value = userInfoDataTable;
                        this._reportViewer.LocalReport.DataSources.Clear();
                        this._reportViewer.LocalReport.DataSources.Add(reportDataSourceUser);
                        this._reportViewer.LocalReport.ReportEmbeddedResource = "ZTraka_App.ReportUsers.rdlc";
                        ds.EndInit();
                        _reportViewer.LocalReport.Refresh();
                        _reportViewer.RefreshReport();

                    }
                    catch
                    {
                        LogFile.Log("Error: Report for Users could not be displayed. Possible issue with database connection.");
                        // Issues with db connection possibly
                    }
                

            }
            else if (cmbSelectCategory.SelectedIndex == 2) //Maps Reader Info
            {
                textBlockSelectWarning.Visibility = Visibility.Hidden;

                ztATdbLocalDSReports1 ds = new ztATdbLocalDSReports1();
                ztATdbLocalDSReports1TableAdapters.ReaderInfoDTTableAdapter readerInfoTA1 = new ZTraka_App.ztATdbLocalDSReports1TableAdapters.ReaderInfoDTTableAdapter();
                ztATdbLocalDSReports1.ReaderInfoDTDataTable readerInfoDataTable = new ztATdbLocalDSReports1.ReaderInfoDTDataTable();

                try
                {
                    readerInfoTA1.FillR(readerInfoDataTable);
                    ReportDataSource reportDataSourceReader = new ReportDataSource();
                    ds.BeginInit();
                    reportDataSourceReader.Name = "ztATdbLocalDSReports1_ReaderInfoDT";
                    reportDataSourceReader.Value = readerInfoDataTable;
                    this._reportViewer.LocalReport.DataSources.Clear();
                    this._reportViewer.LocalReport.DataSources.Add(reportDataSourceReader);
                    this._reportViewer.LocalReport.ReportEmbeddedResource = "ZTraka_App.ReportMapReader.rdlc";                  
                    ds.EndInit();
                    _reportViewer.LocalReport.Refresh();                   
                    _reportViewer.RefreshReport();                
                }
                catch
                {
                    LogFile.Log("Error: Report for Maps Reader Info could not be displayed. Possible issue with database connection.");
                    // Issues with db connection possibly
                }


            }
            this.Cursor = Cursors.Arrow;
        }

   }
}
