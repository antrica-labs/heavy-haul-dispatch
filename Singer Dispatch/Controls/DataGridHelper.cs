using Microsoft.Windows.Controls;
using Microsoft.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SingerDispatch.Controls
{
    public static class DataGridHelper
    {
        public static DataGridCell GetCell(DataGrid grid, int row, int column)
        {
            var rowContainer = GetRow(grid, row);

            if (rowContainer != null)
            {
                var presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    grid.ScrollIntoView(rowContainer, grid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public static DataGridRow GetRow(DataGrid grid, int index)
        {
            var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);

            if (row == null)
            {
                // may be virtualized, bring into view and try again
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < numVisuals; i++)
            {
                var v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T ?? GetVisualChild<T>(v);

                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
