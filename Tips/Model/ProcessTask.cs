using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace Tips.Model
{
    public enum TaskStatus
    {
        Created = 1,
        Delay =2,
        Abort = 3,
        Completed = 4,
    }

    public class ProcessTask: IComparable<ProcessTask>
    {
        string strName;
        DateTime datStart;
        DateTime datDeadline;
        double dubPriority;
        int intProgress;
        List<TaskStep> tasksteps;
        
        public string TaskName
        {
            get { return strName; }
        }

        public String StartTime
        {
            get { return datStart.ToString(); }
        }

        public String DeadLine
        {
            get { return datDeadline.ToString(); }
        }

        public double Priority
        {
            get { return dubPriority; }
        }

        public int Progress
        {
            get { return intProgress; }
        }

        public List<TaskStep> TaskSteps
        {
            get { return tasksteps; }
        }

        public ProcessTask(string sName, DateTime dStart, DateTime dDead, double dPriority)
        {
            strName = sName;
            datStart = dStart;
            datDeadline = dDead;
            dubPriority = dPriority;
            TaskStepInit(datStart.ToString());
        }

        private void TaskStepInit(string sDatetime)
        {
            DateTime start;
            int index;
            string sName;
            bool bCompleted;
            int intSteps = 0, intFinished = 0;
            TaskStep newStep;

            tasksteps = new List<TaskStep>();
            TipsDBDataSetTableAdapters.tabTaskStepTableAdapter adapter = new TipsDBDataSetTableAdapters.tabTaskStepTableAdapter();
            TipsDBDataSet.tabTaskStepDataTable table = new TipsDBDataSet.tabTaskStepDataTable();
            adapter.Fill(table);

            foreach (DataRow currentRow in table.Rows)
            {
                start = (DateTime)currentRow["StartDate"];
                if (start.ToString() != sDatetime)
                {
                    continue;
                }
                index = (int)currentRow["ID"];
                sName = (string)currentRow["TaskStep"];
                bCompleted = (bool)currentRow["StepCompleted"];
                newStep = new TaskStep(index, sName, bCompleted);
                tasksteps.Add(newStep);
                intSteps++;
                if (bCompleted == true)
                {
                    intFinished++;
                }
            }
            if (intSteps == 0)
            {
                intProgress = 0;
                return;
            }
            intProgress = (int)(intFinished * 100.0 / intSteps);
        }

        public int CompareTo(ProcessTask other)
        {
            // A null value means that this object is greater.
            if (other == null)
            {
                return 1;
            }
            return 0-this.Priority.CompareTo(other.Priority);
        }
    }
}
