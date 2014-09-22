using System.Windows.Controls;
using System.Windows.Documents;

namespace SingerDispatch.Controls
{
    /// <summary>
    /// A Hyperlink control that contains a TabItem to create links to tabs in a TabControl.
    /// </summary>
    class TabIndexHyperlink : Hyperlink
    {
        private TabItem tab;
        
        public TabItem Tab
        {
            get
            {
                return tab;
            }
        }

        public TabIndexHyperlink(TabItem tab) : base(new Run(tab.Header.ToString()))          
        {  
            this.tab = tab;
        }
    }
}
