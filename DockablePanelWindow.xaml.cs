using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DockablePanels {
    /// <summary>
    /// Interaction logic for DockablePanelWindow.xaml
    /// </summary>
    public partial class DockablePanelWindow : Window {
        private DockablePanel _hostedPanel;

        public DockablePanelWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the dockable panel that is hosted
        /// by this window.
        /// </summary>
        public DockablePanel HostedPanel {
            get {
                return _hostedPanel;
            }
            set {
                _hostedPanel = value;
                _contentHost.Children.Add(_hostedPanel);
                Width = _hostedPanel.ActualWidth;
                Height = _hostedPanel.ActualHeight;
            }
        }
    }
}
