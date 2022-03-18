using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DockablePanels {
    public class DockStation : Grid {
        private bool _previewActive = false;
        private Rectangle _previewRect = null;

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
            // Call base function
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            UpdatePanels();
        }

        public void Dock(DockablePanel panel) {
            Children.Add(panel);
        }

        public void Undock(DockablePanel panel) {
            Children.Remove(panel);
        }

        public void UpdatePanels() {
            RowDefinitions.Clear();

            foreach (DockablePanel panel in Children) {
                if (panel != null) {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                    RowDefinitions.Add(rowDefinition);
                }
            }

            if (_previewActive) {
                RowDefinition previewDefinition = new RowDefinition();
                previewDefinition.Height = new GridLength(1, GridUnitType.Star);
            }

            int row = 0;
            foreach (DockablePanel panel in Children) {
                if (panel != null) {
                    Grid.SetRow(panel, row);
                    row++;
                }
            }

            // Is there a preview right now?
            if (_previewActive) {
                if (_previewRect == null) {
                    _previewRect = new Rectangle();
                    _previewRect.Fill = new SolidColorBrush(Colors.LightBlue);
                    Children.Add(_previewRect);
                }
                Grid.SetRow(_previewRect, row);
            } else if (_previewRect != null) {
                Children.Remove(_previewRect);
                _previewRect = null;
            }
        }

        /// <returns>Gets the center of this dock station in screen coordinates.</returns>
        public Point GetCenterScreen() {
            double width = ActualWidth;
            double height = ActualHeight;
            return PointToScreen(new Point(width / 2, Height / 2));
        }

        public void PreviewDock(DockablePanel panel) {
            _previewActive = true;
            UpdatePanels();
        }

        public void CancelDockPreview(DockablePanel panel) {
            _previewActive = false;
            UpdatePanels();
        }
    }
}
