using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tips.UI_Resources;

namespace Tips.Model
{
    public class TaskPlan
    {
        List<ProcessTask> CurrentTask;
        //List<Task> CompletedTask;
        //List<Task> DelayTask;
        List<TaskItem> processtasklist;

        public List<TaskItem> ProcessTaskList
        {
            get { return processtasklist; }
        } 

        public TaskPlan()
        {
            CurrentTask = new List<ProcessTask>();
            processtasklist = new List<TaskItem>();
            RefreshProcessTask();
        }

        public void RefreshProcessTask()
        {
            string strName;
            DateTime start, end;
            ProcessTask newTask;
            TaskItem newTaskItem;
            short intCategory, intQlevel;
            double dubPriority;

            CurrentTask.Clear();
            processtasklist.Clear();
            TipsDBDataSetTableAdapters.ViewTaskSortTableAdapter adapter = new TipsDBDataSetTableAdapters.ViewTaskSortTableAdapter();
            TipsDBDataSet.ViewTaskSortDataTable table = new TipsDBDataSet.ViewTaskSortDataTable();
            adapter.Fill(table);
            foreach (DataRow currentRow in table.Rows)
            {
                strName = currentRow["TaskName"].ToString();
                start = (DateTime)currentRow["StartDate"];
                end = (DateTime)currentRow["DeadDate"];
                intCategory = (short)currentRow["CategoryPriority"];
                intQlevel = (short)currentRow["QPriority"];
                dubPriority = CalPriority(end - start, end - DateTime.Now, intCategory, intQlevel);
                newTask = new ProcessTask(strName, start, end, dubPriority);
                CurrentTask.Add(newTask);
            }
            CurrentTask.Sort();
            foreach (ProcessTask curTask in CurrentTask)
            {
                newTaskItem = new TaskItem();
                newTaskItem.Progress = curTask.Progress;
                newTaskItem.TaskName = curTask.TaskName;
                processtasklist.Add(newTaskItem);
            }

        }

        double CalPriority(TimeSpan total, TimeSpan remain, short iCategory, short iQlevel)
        {
            return (iCategory * 10 + iQlevel) *1.0* (total.TotalHours/remain.TotalHours);
        }

    }
}
