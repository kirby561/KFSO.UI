﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DockablePanels {
    /// <summary>
    /// Interaction logic for DockablePanel.xaml
    /// </summary>
    public partial class DockablePanel : UserControl {
        private UIElement _content;
        private DockablePanelWindow _floatingWindow = null;

        // Drag members
        private Point _mouseDownPointScreen;
        private Point _panelStartPositionScreen;
        private bool _isDragging = false;
        private bool _mouseDown = false;

        public DockablePanel() {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the content to display in this DockablePanel.
        /// It can be any UI element. The parent will behave like a grid
        /// that has the full space of the panel.
        /// </summary>
        public UIElement HostedContent {
            get {
                return _content;
            }
            set {
                _content = value;
                _contentArea.Children.Clear();

                if (_content != null)
                    _contentArea.Children.Add(_content);
            }
        }

        /// <summary>
        /// Gets or sets the title text of this panel.
        /// </summary>
        public String TitleText {
            get {
                return _titleTextBox.Text;
            }
            set {
                _titleTextBox.Text = value;
            }
        }

        private void _titleBarBackground_MouseDown(object sender, MouseButtonEventArgs e) {
            // Get the position relative to the screen because the panel is going to move
            _mouseDownPointScreen = PointToScreen(e.GetPosition(this));
            _mouseDown = true;
            _titleBarBackground.Fill = new SolidColorBrush(Colors.LightBlue);
            e.Handled = true;
        }

        private void _titleBarBackground_MouseUp(object sender, MouseButtonEventArgs e) {
            _mouseDown = false;
            _isDragging = false;
            _titleBarBackground.Fill = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));

            // Uncapture the mouse since we're done
            Mouse.Capture(null);
        }

        private void _titleBarBackground_MouseMove(object sender, MouseEventArgs e) {
            if (!_mouseDown) return;

            e.Handled = true;

            // Check if we're dragging or not
            if (!_isDragging) {
                // If the panel is docked, check if we've dragged far 
                // enough to pull it out into a window.
                if (_floatingWindow == null) {
                    // Get the current position in screen coordinates
                    Point position = PointToScreen(e.GetPosition(this));

                    // If we're not dragging yet, check if we should start.
                    // We wait a few pixels just to make sure it's not glitchy
                    double distanceSquared = (position - _mouseDownPointScreen).LengthSquared;
                    if (distanceSquared > 100) {
                        _isDragging = true;

                        // Panel screen position
                        _panelStartPositionScreen = PointToScreen(new Point(0, 0));
                        Point panelStartPositionWpf = ScreenCoordinateToWpfCoordinate(_panelStartPositionScreen);

                        // Undock from the parent
                        Undock();

                        // Break out the panel into its own window and continue dragging
                        _floatingWindow = new DockablePanelWindow();
                        _floatingWindow.HostedPanel = this;
                        _floatingWindow.Left = panelStartPositionWpf.X;
                        _floatingWindow.Top = panelStartPositionWpf.Y;
                        _floatingWindow.Show();

                        // Capture the mouse so that you can't lose the window while dragging and we will get
                        // the mouse up event when done.
                        Mouse.Capture(sender as IInputElement);
                    }
                } else {
                    // Capture the mouse so that you can't lose the window while dragging and we will get
                    // the mouse up event when done.
                    Mouse.Capture(sender as IInputElement);

                    // Just start dragging
                    _isDragging = true;

                    // Record the data needed by the dragging step
                    _panelStartPositionScreen = PointToScreen(new Point(0, 0));
                }
            } else {
                // We're dragging and we should have a window
                Point position = PointToScreen(e.GetPosition(this));
                Vector delta = position - _mouseDownPointScreen;
                Point newPanelPositionScreen = new Point(_panelStartPositionScreen.X + delta.X, _panelStartPositionScreen.Y + delta.Y);
                Point newPanelPositionWpf = ScreenCoordinateToWpfCoordinate(newPanelPositionScreen);

                // Move the window
                _floatingWindow.Left = newPanelPositionWpf.X;
                _floatingWindow.Top = newPanelPositionWpf.Y;
            }
        }

        /// <summary>
        /// Removes this panel from its parent.
        /// </summary>
        private void Undock() {
            // Remove this panel from its current parent
            DockStation station = Parent as DockStation;
            station.Undock(this);
            _pinButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// WPF has a different coordinate system than the Desktop's Screen coordinate system
        /// so this gets the given screen location in the WPF's coordinate system.
        /// </summary>
        /// <returns>The top/left coordinate of this panel in Window coordinates.</returns>
        private Point ScreenCoordinateToWpfCoordinate(Point screenCoordinate) {
            // Transform screen point to WPF device independent point
            PresentationSource source = PresentationSource.FromVisual(this);

            Point targetPoint = source.CompositionTarget.TransformFromDevice.Transform(screenCoordinate);
            return targetPoint;
        }

        private void _closeButton_MouseUp(object sender, MouseButtonEventArgs e) {
            if (_floatingWindow != null) {
                _floatingWindow.Close();
            } else {
                // Remove this panel from its current parent
                Panel parentPanel = Parent as Panel;
                parentPanel.Children.Remove(this);
            }
        }
    }
}
