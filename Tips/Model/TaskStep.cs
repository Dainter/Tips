using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tips.Model
{
    public class TaskStep
    {
        int intIndex;
        string strStepName;
        bool bolCompleted;

        public int Index
        {
            get { return intIndex; }
        }

        public string StepName
        {
            get{ return strStepName; }
        }

        public bool IsCompleted
        {
            get { return bolCompleted; }
            set { bolCompleted = value; }
        }

        public TaskStep(int Index, string sName, bool bCompleted = false)
        {
            intIndex = Index;
            strStepName = sName;
            bolCompleted = bCompleted;
        }
    }
}
