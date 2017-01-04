using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Model
{
    public class Category
    {
        int intIndex;
        string strName;
        int intPriority;

        public int Index
        {
            get { return intIndex; }
        }

        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }

        public int Priority
        {
            get { return intPriority; }
            set { intPriority = value; }
        }

        public Category(int iIndex, string sName, int iPriority)
        {
            intIndex = iIndex;
            strName = sName;
            intPriority = iPriority;
        }
    }
}
