using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorkerRole
{
    public class ActiveTradersNewsElement
    {
        public string SourceId { get; set; }
        public int ElementId { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public string Symbol { get; set; }

        public override int GetHashCode()
        {
            return this.ElementId;
        }

        public override bool Equals(object obj)
        {
            ActiveTradersNewsElement other = obj as ActiveTradersNewsElement;

            if (other != null)
            {
                return this.ElementId == other.ElementId;
            }
            else
            {
                return base.Equals(obj);
            }
        }


        public class Comparer : IComparer<ActiveTradersNewsElement>
        {
            public int Compare(ActiveTradersNewsElement x, ActiveTradersNewsElement y)
            {
                // We want to keep the list sorted in descending order so that newer elements show at the top of the list
                return y.ElementId - x.ElementId;
            }
        }
    }
}
