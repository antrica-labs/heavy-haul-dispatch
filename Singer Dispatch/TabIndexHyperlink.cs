using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Controls;

namespace Singer_Dispatch
{
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
