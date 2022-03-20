using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DockablePanels {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        // Allow any panel to dock anywhere in this window
        private DockManager _dockManager = new DockManager();

        public MainWindow() {
            InitializeComponent();

            // Make all dock stations in this window use this manager.
            _dockManager.UseDockManagerForTree(this);
        }

    }
}
