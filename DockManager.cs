using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DockablePanels {
    /// <summary>
    /// Manages the list of dockable sections in a window (or across windows).
    /// The manager is given to each DockablePanel so it can handle docking into
    /// locations in any window that uses the same manager.
    /// </summary>
    public class DockManager {
        // How close you need to get to a dock station (squared) to dock to it
        private const double DockDistanceSquared = 100*100;

        // The stations that can be docked to in this manager
        private LinkedList<DockStation> _dockStations = new LinkedList<DockStation>();

        /// <summary>
        /// Adds a docking station to be tracked by this manager. This
        /// will enable it to be seen by DockablePanels that are also
        /// associated with this manager.
        /// </summary>
        /// <param name="station">The station to add.</param>
        public void AddDockStation(DockStation station) {
            station.DockManager = this;
        }

        /// <summary>
        /// Removes the given station from this manager.
        /// </summary>
        /// <param name="station">The station to remove.</param>
        public void RemoveDockStation(DockStation station) {
            station.DockManager = null;
        }

        /// <summary>
        /// Called by DockStation to link itself to this manager.
        /// </summary>
        /// <param name="station">The station that is linking itself.</param>
        internal void LinkDockStation(DockStation station) {
            _dockStations.AddLast(station);
        }

        /// <summary>
        /// Called by DockStation to unlink itself from this manager.
        /// </summary>
        /// <param name="station">The station that is unlinking itself.</param>
        internal void UnlinkDockStation(DockStation station) {
            _dockStations.Remove(station);
        }

        /// <summary>
        /// Gets the closest docking station within the docking range to the given location.
        /// </summary>
        /// <param name="locationScreen">The location in screen coordinates.</param>
        /// <returns>Returns the closest docking station or null if none are within the required distance.</returns>
        public DockStation GetClosestDockInRangeTo(Point locationScreen) {
            double closestDistanceSquared = Double.MaxValue;
            DockStation closestStation = null;

            foreach (DockStation station in _dockStations) {
                Point centerScreen = station.GetCenterScreen();
                Vector distance = centerScreen - locationScreen;
                double lengthSquared = distance.LengthSquared;
                if (lengthSquared < DockDistanceSquared) {
                    if (lengthSquared < closestDistanceSquared) {
                        closestDistanceSquared = lengthSquared;
                        closestStation = station;
                    }
                }
            }

            return closestStation;
        }

        /// <summary>
        /// Assigns this manager to every DockStation and DockablePanel in the given window.
        /// </summary>
        /// <param name="window">The window to assign.</param>
        public void UseDockManagerForWindow(Window window) {
            // ?? TODO
        }
    }
}
