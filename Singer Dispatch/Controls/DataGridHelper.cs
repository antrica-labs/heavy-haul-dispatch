using System.Windows.Controls;

namespace SingerDispatch.Controls
{
    public static class DataGridHelper
    {
        public static void EditFirstColumn(DataGrid grid, object item)
        {
            grid.Focus();
            grid.CurrentCell = new DataGridCellInfo(item, grid.Columns[0]);
            grid.ScrollIntoView(item);
            grid.SelectedItem = item;
            grid.BeginEdit();
        }
    }
}
