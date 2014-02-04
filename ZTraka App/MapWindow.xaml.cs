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

namespace ZTraka_App
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        

        /// <summary>
        /// Constructor of the <see cref="MapWindow" /> class.
        /// </summary>
        public MapWindow()
        {
            InitializeComponent();
        }

        private void mapToolWindow_Loaded(object sender, RoutedEventArgs e)
        {
            maxRange = (int) sliderMapZoom.Maximum;
            mOriginal = imageMap.RenderTransform.Value;
        }

        // Maps -panning and zooming features START
        // Intialize variables
        private Point origin;
        private Point start;
        private Point p;

        private int maxRange;
        private Matrix mOriginal;
        private bool flagSliderZoom = true;
        private bool flagSliderUNZoom = true;

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the imageMap control.
        /// Release the mouse from map panning
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor cursorOpenHand = new Cursor(new System.IO.MemoryStream(ZTraka_App.Properties.Resources.openhand));
            this.Cursor = cursorOpenHand;
            imageMap.ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles the MouseEnter event of the imageMap control.
        /// Gets the open hand mouse shape
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor cursorOpenHand = new Cursor(new System.IO.MemoryStream(ZTraka_App.Properties.Resources.openhand));
            this.Cursor = cursorOpenHand;

        }

        /// <summary>
        /// Handles the MouseLeave event of the imageMap control.
        /// Restore default mouse arrow
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the imageMap control.
        /// MAP panning feature activated on left button down
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Cursor cursorClosedHand = new Cursor(new System.IO.MemoryStream(ZTraka_App.Properties.Resources.closedhand));
            //Changes the cursor figure to indicate drag on map avaiable
            this.Cursor = cursorClosedHand;

            if (imageMap.IsMouseCaptured) return;
            imageMap.CaptureMouse();

            start = e.GetPosition(borderMap);
            //origin.X = imageMap.RenderTransform.Value.OffsetX;
            //origin.Y = imageMap.RenderTransform.Value.OffsetY;
        }

        /// <summary>
        /// Handles the MouseMove event of the imageMap control.
        /// Map panning feature
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (!imageMap.IsMouseCaptured) return;
            p = e.MouseDevice.GetPosition(borderMap);

            Matrix m = imageMap.RenderTransform.Value;
            m.OffsetX = origin.X + (p.X - start.X);
            m.OffsetY = origin.Y + (p.Y - start.Y);

            imageMap.RenderTransform = new MatrixTransform(m);
        }

        /// <summary>
        /// Handles the MouseWheel event of the imageMap control.
        /// Zoom feature also available through mouse wheel scroll
        /// Subject to the slider max and min value
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs" /> instance containing the event data.</param>
        private void imageMap_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            p = e.MouseDevice.GetPosition(imageMap);

            Matrix m = imageMap.RenderTransform.Value;
            if (e.Delta > 0)
            {
                sliderMapZoom.Value++;
                if ( (sliderMapZoom.Value <= maxRange) && (flagSliderZoom) )
                {
                    if (sliderMapZoom.Value == maxRange)
                    {
                        flagSliderZoom = false;
                    }

                    m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
                    flagSliderUNZoom = true;
                }
            }
            else
            {
                sliderMapZoom.Value--;
                if ((sliderMapZoom.Value >= 0) && (flagSliderUNZoom))
                {
                    if (sliderMapZoom.Value == 0)
                    {
                        flagSliderUNZoom = false;
                    }
                    m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);
                }
                flagSliderZoom = true;
            }
            imageMap.RenderTransform = new MatrixTransform(m);

          
        }

        /// <summary>
        /// map zoom_ value changed.
        /// Zooming feature on slider
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void sliderMapZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double xcenter = (this.Width) / 2; //333;
            double ycenter = (this.Height) / 2; //107
            p.X = xcenter;
            p.Y = ycenter;

            Matrix m = imageMap.RenderTransform.Value;
            if (e.NewValue > e.OldValue) //then zoom
                m.ScaleAtPrepend(1.1, 1.1, p.X, p.Y);
            else // UnZoom
                m.ScaleAtPrepend(1 / 1.1, 1 / 1.1, p.X, p.Y);

            imageMap.RenderTransform = new MatrixTransform(m);
        }

        /// <summary>
        /// Handles the Click event of the btnExit control.
        /// Closes the window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        /// <summary>
        /// Handles the Click event of the btnDefaultView control.
        /// Restore default view by creating a new instance
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnDefaultView_Click(object sender, RoutedEventArgs e)
        {
            sliderMapZoom.Value = 0;
            imageMap.RenderTransform = new MatrixTransform(mOriginal);
            
            // Or a creude method
            //this.Close();
            //MapWindow mp = new MapWindow();
            //mp.Show();

        }

        // Maps -panning and zooming features END
    }
}
