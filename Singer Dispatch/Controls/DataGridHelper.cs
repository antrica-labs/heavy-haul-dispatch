using System.Windows.Controls;

namespace SingerDispatch.Controls
{
    public static class DataGridHelper
    {
        public static void EditFirstColumn(DataGrid grid, object item)
        {
            EditColumn(grid, item, 0);
        }

        public static void EditSecondColumn(DataGrid grid, object item)
        {
            EditColumn(grid, item, 1);
        }

        public static void EditColumn(DataGrid grid, object item, int column)
        {
            grid.UpdateLayout();
            grid.Focus();
            grid.CurrentCell = new DataGridCellInfo(item, grid.Columns[column]);
            grid.ScrollIntoView(item);
            grid.SelectedItem = item;
            grid.BeginEdit();
        }


    }
}
