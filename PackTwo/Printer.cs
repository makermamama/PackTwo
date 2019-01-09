using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackTwo
{
    class Printer 
    {
        //打印队列
        private static Queue<PrintTask> PrintQueue = new Queue<PrintTask>();

        private static Thread PrintThread = new Thread(new ThreadStart(ThreadTask));

        private static Boolean OverThread  = false;

        //增加打印任务
        public Printer()
        {
            PrintThread.Start();
        }

        private static void ThreadTask()
        {
            while (true && !OverThread) {

                if (PrintQueue.Count > 0)
                {
                    PrintTask task =  PrintQueue.Dequeue();

                    Print(task);
                }
                Thread.Sleep(200);
            }
        }

        private static void Print(PrintTask task)
        {
            LabelManager2.ApplicationClass lbl = new LabelManager2.ApplicationClass();

            lbl.Documents.Open(XmlTool.Read("Root/Printaddress")+@"\"+XmlTool.Read("Root/PrintLabName"), true);//调用设计好的label文件

            try
            {
                LabelManager2.Document doc = lbl.ActiveDocument;

                doc.Variables.FormVariables.Item("code").Value = task.Code;

                doc.Variables.FormVariables.Item("name").Value = task.Name;

                doc.Variables.FormVariables.Item("boxId").Value = task.Boxid;

                doc.Variables.FormVariables.Item("sum").Value = task.Sum;

                doc.PrintDocument(2);                                              //打印
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                lbl.Documents.CloseAll(true);
                lbl.Quit();
            }
        }

        //增加打印任务
        public static void AddPrintTask(String code,string name,string boxId,string sum) {

            PrintTask task = new PrintTask();

            task.Code = code;
            task.Name = name;
            task.Boxid = boxId;
            task.Sum = sum;
            PrintQueue.Enqueue(task);

        }
        public void Dispose() {

            OverThread = true;
        }
    }
}
