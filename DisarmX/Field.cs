using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DisarmX
{
    class Field : Position
    {
        public Position Location;
        public enum Mark { NULL, QUESTION, EXCLAMATION, DISCOVERED };
        public Mark Status = Mark.NULL;
        public string Value = "0";
        public Button button;

        public Field()
        {
            button = new Button();
            button.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            button.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }
        


    }
}
