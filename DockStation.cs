using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DockablePanels {
    public class DockStation : Grid {
        private DockManager _dockManager;
        private bool _previewActive = false;
        private Rectangle _previewRect = null;

        public DockManager DockManager {
            get {
                return _dockManager;
            }
            set {
                DockManager oldManager = _dockManager;
                _dockManager = value;

                if (oldManager != null)
                    oldManager.UnlinkDockStation(this);
                _dockManager.LinkDockStation(this);
            }
        }

        public DockStation() : base() {
            DockManager = new DockManager();
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
            // Call base function
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            UpdatePanels();
        }

        /// <summary>
        /// Docks the given panel with this station.
        /// It will be added to the end.
        /// </summary>
        /// <param name="panel">The panel to dock.</param>
        public void Dock(DockablePanel panel) {
            Children.Add(panel);
        }

        /// <summary>
        /// Undocks the given panel with this sation.
        /// </summary>
        /// <param name="panel">The panel to undock.</param>
        public void Undock(DockablePanel panel) {
            Children.Remove(panel);
        }

        private void UpdatePanels() {
            RowDefinitions.Clear();

            foreach (UIElement uiElement in Children) {
                DockablePanel panel = uiElement as DockablePanel;

                // The panel can be null because UpdatePanels is called mid-update
                // and it seems like WPF sets children to null first before removing them
                // entirely so we just treat that as being removed already
                if (panel != null) {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                    RowDefinitions.Add(rowDefinition);
                }
            }

            int row = 0;
            foreach (UIElement uiElement in Children) {
                DockablePanel panel = uiElement as DockablePanel;

                // The panel can be null because UpdatePanels is called mid-update
                // and it seems like WPF sets children to null first before removing them
                // entirely so we just treat that as being removed already
                if (panel != null) {
                    Grid.SetRow(panel, row);
                    row++;
                }
            }

            // Is there a preview right now?
            if (_previewActive) {
                RowDefinition previewDefinition = new RowDefinition();
                previewDefinition.Height = new GridLength(1, GridUnitType.Star);
                RowDefinitions.Add(previewDefinition);
                Grid.SetRow(_previewRect, row);
            }
        }

        /// <returns>Gets the center of this dock station in screen coordinates.</returns>
        public Point GetCenterScreen() {
            double width = ActualWidth;
            double height = ActualHeight;
            return PointToScreen(new Point(width / 2, height / 2));
        }

        public void PreviewDock(DockablePanel panel) {
            if (_previewRect == null) {
                _previewActive = true;
                _previewRect = new Rectangle();
                _previewRect.Fill = new SolidColorBrush(Colors.LightBlue);
                Children.Add(_previewRect);
            }
        }

        public void CancelDockPreview(DockablePanel panel) {
            _previewActive = false;
            Children.Remove(_previewRect);
            _previewRect = null;
        }
    }
}
