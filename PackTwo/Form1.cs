using PackTwo.WebReference;
using Symbol.RFID3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using winformVision;

namespace PackTwo
{
    public partial class Form1 : Form 
    {
        public delegate void AddTag(string data);

        public delegate void TwoTriggerUpdataUI();

        private SFBox Service = new SFBox();

        private uint Port = Convert.ToUInt32(XmlTool.Read("Root/Port"));

        private string IP = XmlTool.Read("Root/IP");

        private RFIDReader m_RFIDReader;

        private string BoxNO;

        private Form1.AddTag AddTagDelegate;

        private Form1.TwoTriggerUpdataUI twoTriggerUpdataUI;

        private Hashtable m_TagTable;

        private HashSet<string> m_BoundTags;

        private HashSet<string> EndBoundTags;

        private BoxInfo BoxInfo = new BoxInfo();

        private Printer printer = new Printer();

        private int BoundID = 1;

        private string BoxNofirst;

        private bool IsReciveRFIDdata = false;

        private bool IsSensorAlarms = false;

        private LiZuoSensor liZuoSensor = new LiZuoSensor();

        private bool IsRedLight = false;

        private Hashtable ErrorTags = new Hashtable();



        public Form1()
        {
            this.InitializeComponent();
            this.initListViewHand();
            this.InitData();
            this.AddTagDelegate = new Form1.AddTag(this.AddOneTag);
            this.twoTriggerUpdataUI = new Form1.TwoTriggerUpdataUI(this.TwoTriggerEventUpdatUI);
            this.liZuoSensor.OneTriggerEvent += new LiZuoSensor.delegateTriggerOne(this.OneTriggerEvent);
            this.liZuoSensor.TwoTriggerEvent += new LiZuoSensor.delegateTriggerTwo(this.TwoTriggerEvent);
            this.m_TagTable = new Hashtable();
            this.m_BoundTags = new HashSet<string>();
            this.EndBoundTags = new HashSet<string>();
            this.toolStripButton2.Enabled = false;
            this.toolStripButton3.Enabled = false;
            this.toolStripButton4.Enabled = false;
            this.toolStripButton5.Enabled = false;
        }

        private void initListViewHand()
        {
            this.listView_Boxs.Columns.Add("箱号", 100, HorizontalAlignment.Left);
            this.listView_Boxs.Columns.Add("装箱", 100, HorizontalAlignment.Left);
            this.listView_Boxs.Columns.Add("打印", 100, HorizontalAlignment.Left);
            this.listView_Bunchs.Columns.Add("捆号", 100, HorizontalAlignment.Left);
            this.listView_Bunchs.Columns.Add("数量", 100, HorizontalAlignment.Left);
            this.listView_Bunchs.Columns.Add("合格/不合格", 100, HorizontalAlignment.Left);
            this.listView_Tags.Columns.Add("标签", 200, HorizontalAlignment.Left);
            this.listView_Tags.Columns.Add("判断", 100, HorizontalAlignment.Left);
            this.listView_TagsInfo.Columns.Add("每捆标签", 200, HorizontalAlignment.Center);
        }

        private void InitData()
        {
            try
            {
                DataSet productInfo = this.Service.GetProductInfo();
                foreach (DataRow dataRow in productInfo.Tables[0].Rows)
                {
                    Console.Write(dataRow[0].ToString());
                }
                this.toolStripComboBox1.ComboBox.ValueMember = "FGUID";
                this.toolStripComboBox1.ComboBox.DisplayMember = "FName";
                this.toolStripComboBox1.ComboBox.DataSource = productInfo.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            string[] portNames = SerialPort.GetPortNames();
            string[] array = portNames;
            for (int i = 0; i < array.Length; i++)
            {
                string item = array[i];
                this.toolStripComboBox_ComPort.ComboBox.Items.Add(item);
            }
            bool flag = this.toolStripComboBox_ComPort.ComboBox.Items.Count > 0;
            if (flag)
            {
                this.toolStripComboBox_ComPort.ComboBox.SelectedIndex = 0;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.m_RFIDReader = new RFIDReader(this.IP, this.Port, 0u);
            try
            {
                this.m_RFIDReader.Connect();
                this.m_RFIDReader.Events.ReadNotify += new Events.ReadNotifyHandler(this.Events_ReadNotify);
                this.m_RFIDReader.Events.AttachTagDataWithReadEvent = false;
                this.m_RFIDReader.Events.StatusNotify += new Events.StatusNotifyHandler(this.Events_StatusNotify);
                this.m_RFIDReader.Events.NotifyGPIEvent = true;
                this.m_RFIDReader.Events.NotifyBufferFullEvent = true;
                this.m_RFIDReader.Events.NotifyBufferFullWarningEvent = true;
                this.m_RFIDReader.Events.NotifyReaderDisconnectEvent = true;
                this.m_RFIDReader.Events.NotifyReaderExceptionEvent = true;
                this.m_RFIDReader.Events.NotifyAccessStartEvent = true;
                this.m_RFIDReader.Events.NotifyAccessStopEvent = true;
                this.m_RFIDReader.Events.NotifyInventoryStartEvent = true;
                this.m_RFIDReader.Events.NotifyInventoryStopEvent = true;
                this.toolStripButton1.Enabled = false;
                this.toolStripButton2.Enabled = true;
                this.toolStripButton3.Enabled = false;
                this.toolStripButton4.Enabled = false;
                this.toolStripButton5.Enabled = false;

                SetAntennas();
            }
            catch (OperationFailureException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.ToString());
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            bool flag = this.liZuoSensor.OpenPort(this.toolStripComboBox_ComPort.Text) == this.toolStripComboBox_ComPort.Text + "开启成功" || this.liZuoSensor.OpenPort(this.toolStripComboBox_ComPort.Text) == this.toolStripComboBox_ComPort.Text + "串口已经开启";
            if (flag)
            {
                this.listView_Boxs.Items.Clear();
                this.listView_Bunchs.Items.Clear();
                this.listView_Tags.Items.Clear();
                this.m_TagTable.Clear();
                this.m_BoundTags.Clear();
                this.m_RFIDReader.Actions.Inventory.Perform(null, null, null);
                this.BoundID = 1;
                this.IsSensorAlarms = true;
                this.liZuoSensor.AlarmsNolight();
                this.IsRedLight = false;
                this.toolStripComboBox1.Enabled = false;
                this.toolStripTextBox_item.Enabled = false;
                this.toolStripTextBox_buonds.Enabled = false;
                this.toolStripButton2.Enabled = false;
                this.toolStripButton3.Enabled = true;
                this.toolStripButton3.Text = "暂停";
                this.toolStripButton4.Enabled = true;
                this.toolStripButton5.Enabled = false;
            }
            else
            {
                MessageBox.Show("请选择正确串口");
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.liZuoSensor.AlarmsNolight();
            Thread.Sleep(100);
            this.IsRedLight = false;
            Thread.Sleep(10);
            bool flag = this.m_RFIDReader.Actions.TagAccess.OperationSequence.Length > 0;
            if (flag)
            {
                this.m_RFIDReader.Actions.TagAccess.OperationSequence.StopSequence();
            }
            else
            {
                this.m_RFIDReader.Actions.Inventory.Stop();
            }
            this.IsSensorAlarms = false;
            this.toolStripButton2.Enabled = true;
            this.toolStripButton3.Enabled = false;
            this.toolStripButton4.Enabled = false;
            this.toolStripButton5.Enabled = true;
            this.toolStripComboBox1.Enabled = true;
            this.toolStripTextBox_item.Enabled = true;
            this.toolStripTextBox_buonds.Enabled = true;
        }

        private void TwoTriggerEventUpdatUI()
        {
            this.listView_Bunchs.BeginUpdate();
            try
            {
                this.AddBoundsItem();
            }
            catch (Exception ex)
            {
                bool flag = ex.GetType() == typeof(SoapException);
                if (flag)
                {
                    string text = ex.Message.Substring(16);
                    MessageBox.Show(text);
                    this.liZuoSensor.RedAlarms();
                    this.liZuoSensor.RejectTask();
                }
            }
            this.m_BoundTags.Clear();
            this.listView_Tags.Items.Clear();
            this.listView_Bunchs.EndUpdate();
        }

        private void DealException(Exception ex)
        {
            string text = ex.Message.ToString();
            text = text.Replace("：", ";");
            text = text.Replace("，", ",");
            text = text.Replace("\r", ",");
            string[] array = text.Split(new char[]
            {
                ';'
            });
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string text2 = array2[i];
                MessageBox.Show(text2);
            }
            string[] array3 = array[array.Length - 1].Split(new char[]
            {
                ','
            });
            string[] array4 = array3;
            for (int j = 0; j < array4.Length; j++)
            {
                string text3 = array4[j];
                MessageBox.Show(text3);
            }
            int num = 1;
            HashSet<string> hashSet = new HashSet<string>();
            List<int> list = new List<int>();
            foreach (DictionaryEntry dictionaryEntry in this.m_TagTable)
            {
                string[] tags = ((Bounds)dictionaryEntry.Value).getTags();
                string[] array5 = tags;
                for (int k = 0; k < array5.Length; k++)
                {
                    string text4 = array5[k];
                    string[] array6 = array3;
                    for (int l = 0; l < array6.Length; l++)
                    {
                        string b = array6[l];
                        bool flag = text4 == b;
                        if (flag)
                        {
                            MessageBox.Show(string.Concat(new object[]
                            {
                                "当前第",
                                num,
                                "捆的标签:",
                                text4,
                                "在数据库中已存在"
                            }));
                            hashSet.Add(dictionaryEntry.Key.ToString());
                            list.Add(num);
                        }
                    }
                }
                num++;
            }
            foreach (string current in hashSet)
            {
                this.m_TagTable.Remove(current);
                MessageBox.Show("第" + list[0] + "捆已经被剔除请替换此捆的重复标签,重新打捆");
                list.Remove(0);
            }
        }

        public void OneTriggerEvent()
        {
            this.IsReciveRFIDdata = true;
            bool isSensorAlarms = this.IsSensorAlarms;
            if (isSensorAlarms)
            {
                this.liZuoSensor.AlarmsNolight();
                this.IsRedLight = false;
            }
            //延迟1秒关闭读取
            delay(1000);
        }

        public void TwoTriggerEvent()
        {
            this.IsReciveRFIDdata = false;
            bool isSensorAlarms = this.IsSensorAlarms;
            if (isSensorAlarms)
            {
                base.Invoke(this.twoTriggerUpdataUI);
            }
        }

        private string GetBoxID()
        {
            DataSet newBoxNo = this.Service.GetNewBoxNo();
            this.BoxInfo.Guid = newBoxNo.Tables[0].Rows[0]["FGuid"].ToString();
            this.BoxInfo.No = newBoxNo.Tables[0].Rows[0]["FCode"].ToString();
            return this.BoxInfo.No;
        }

        private int GetBoundID()
        {
            int boundID = this.BoundID;
            this.BoundID = boundID + 1;
            return boundID;
        }

        private void AddOneTag(string tag)
        {
            this.listView_Tags.BeginUpdate();
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.SubItems[0].Text = tag;
            bool flag = tag.Substring(7, 1) == this.BoxNofirst;
            if (flag)
            {
                listViewItem.SubItems.Add("正确");
            }
            else
            {
                listViewItem.SubItems.Add("错误");
                bool flag2 = !this.IsRedLight;
                if (flag2)
                {
                    this.liZuoSensor.RedAlarms();
                    this.IsRedLight = true;
                }
            }
            this.listView_Tags.Items.Add(listViewItem);
            this.listView_Tags.EndUpdate();
        }

        private void AddBoundsItem()
        {
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.SubItems[0].Text = this.GetBoundID().ToString();
            listViewItem.SubItems.Add(this.m_BoundTags.Count.ToString());
            this.UpdataBoundsTags(this.m_BoundTags);
            bool flag = this.IsTagsCorrect() && this.CheckRepeat(this.m_BoundTags) && this.CheckBoxTagsInHistory(this.m_BoundTags);
            if (flag)
            {
                listViewItem.SubItems.Add("正确");
                listViewItem.SubItems[1].BackColor = Color.Green;
                this.liZuoSensor.GreenAlarms();
                this.listView_Bunchs.Items.Add(listViewItem);
                string[] array = new string[Convert.ToInt32(this.toolStripTextBox_item.Text)];
                int num = 0;
                foreach (string current in this.m_BoundTags)
                {
                    array[num] = current;
                    num++;
                }
                Bounds bounds = new Bounds(Convert.ToInt32(this.toolStripTextBox_item.Text));
                bounds.AddTags(array);
                bounds.CapTimer = DateTime.Now;
                bool flag2 = !this.m_TagTable.Contains(this.BoundID.ToString());
                if (flag2)
                {
                    this.m_TagTable.Add(this.BoundID.ToString(), bounds);
                }
                bool flag3 = this.m_TagTable.Count == Convert.ToInt32(this.toolStripTextBox_buonds.Text);
                if (flag3)
                {
                    bool flag4 = this.CheckBoxTags();
                    if (flag4)
                    {
                        this.AddBoxItem();
                        this.Upload();
                        DataRowView dataRowView = (DataRowView)this.toolStripComboBox1.ComboBox.SelectedItem;
                        Printer.AddPrintTask(dataRowView["FCode"].ToString(), dataRowView["FName"].ToString(), this.BoxNO, (this.m_TagTable.Count * Convert.ToInt32(this.toolStripTextBox_item.Text)).ToString());
                        this.UpdataBoxItem();
                        this.listView_Bunchs.Items.Clear();
                        this.m_TagTable.Clear();
                        this.BoundID = 1;
                    }
                    else
                    {
                        this.toolStripLabel6.Text = "错误信息:" + DateTime.Now.ToString() + "_一箱中标签重复";
                        this.toolStripLabel6.ForeColor = Color.Red;
                        this.liZuoSensor.RedAlarms();
                        this.listView_Bunchs.Items.Clear();
                        this.m_TagTable.Clear();
                        this.BoundID = 1;
                    }
                }
            }
            else
            {
                listViewItem.SubItems.Add("错误");
                listViewItem.SubItems[2].BackColor = Color.Red;
                this.liZuoSensor.RedAlarms();
                this.liZuoSensor.RejectTask();
                this.listView_Bunchs.Items.Add(listViewItem);
                this.HoldErrorTags();
            }
        }

        private bool CheckBoxTags()
        {
            HashSet<string> hashSet = new HashSet<string>();
            bool result;
            foreach (DictionaryEntry dictionaryEntry in this.m_TagTable)
            {
                string[] tags = ((Bounds)dictionaryEntry.Value).getTags();
                string[] array = tags;
                for (int i = 0; i < array.Length; i++)
                {
                    string item = array[i];
                    bool flag = !hashSet.Add(item);
                    if (flag)
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }

        private void AddBoxItem()
        {
            this.BoxNO = this.GetBoxID().ToString();
            this.listView_Boxs.BeginUpdate();
            ListViewItem listViewItem = new ListViewItem();
            bool flag = this.BoxNO != "";
            if (flag)
            {
                listViewItem.SubItems[0].Text = this.BoxNO;
                listViewItem.SubItems.Add("正在装箱");
                listViewItem.SubItems.Add("已打印");
                listViewItem.SubItems[1].BackColor = Color.Red;
                listViewItem.SubItems[2].BackColor = Color.Red;
                this.listView_Boxs.Items.Add(listViewItem);
            }
            this.listView_Boxs.EndUpdate();
        }

        private void UpdataBoxItem()
        {
            this.listView_Boxs.BeginUpdate();
            ListViewItem listViewItem = this.listView_Boxs.Items[this.listView_Boxs.Items.Count - 1];
            listViewItem.SubItems[1].Text = "装箱完成";
            listViewItem.SubItems[2].Text = "打印完成";
            this.listView_Boxs.EndUpdate();
        }

        private void Events_ReadNotify(object sender, Events.ReadEventArgs readEventArgs)
        {
            try
            {
                TagData[] readTags = this.m_RFIDReader.Actions.GetReadTags(1000);
                bool flag = readTags != null;
                if (flag)
                {
                    for (int i = 0; i < readTags.Length; i++)
                    {
                        bool flag2 = readTags[i].OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_NONE || (readTags[i].OpCode == ACCESS_OPERATION_CODE.ACCESS_OPERATION_READ && readTags[i].OpStatus == ACCESS_OPERATION_STATUS.ACCESS_SUCCESS);
                        if (flag2)
                        {
                            HashSet<string> boundTags = this.m_BoundTags;
                            lock (boundTags)
                            {
                                bool flag4 = readTags[i].TagID.ToString().Length == 16 && this.IsReciveRFIDdata;
                                if (flag4)
                                {
                                    bool flag5 = this.m_BoundTags.Add(readTags[i].TagID.ToString());
                                    if (flag5)
                                    {
                                        base.Invoke(this.AddTagDelegate, new object[]
                                        {
                                            readTags[i].TagID.ToString()
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Events_StatusNotify(object sender, Events.StatusEventArgs statusEventArgs)
        {
            try
            {
            }
            catch (Exception)
            {
            }
        }

        private void Upload()
        {
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable("T_Encasement");
            DataTable dataTable2 = new DataTable("T_EncasementBind");
            DataTable dataTable3 = new DataTable("T_EncasementBindProduct");
            dataTable.Columns.Add("FMaterial", Type.GetType("System.String"));
            dataTable.Columns.Add("FPackingBox", Type.GetType("System.String"));
            DataRow dataRow = dataTable.NewRow();
            dataRow["FPackingBox"] = this.BoxInfo.Guid;
            dataRow["FMaterial"] = ((DataRowView)this.toolStripComboBox1.ComboBox.SelectedItem)["FGUID"].ToString();
            dataTable.Rows.Add(dataRow);
            dataSet.Tables.Add(dataTable);
            dataTable2.Columns.Add("FRowIndex", Type.GetType("System.Int16"));
            dataTable2.Columns.Add("FMakeTime", Type.GetType("System.DateTime"));
            dataTable3.Columns.Add("FRowIndex", Type.GetType("System.Int16"));
            dataTable3.Columns.Add("FProductSN", Type.GetType("System.String"));
            foreach (DictionaryEntry dictionaryEntry in this.m_TagTable)
            {
                DataRow dataRow2 = dataTable2.NewRow();
                dataRow2["FRowIndex"] = Convert.ToInt32(dictionaryEntry.Key);
                dataRow2["FMakeTime"] = ((Bounds)dictionaryEntry.Value).CapTimer;
                dataTable2.Rows.Add(dataRow2);
                string[] tags = ((Bounds)dictionaryEntry.Value).getTags();
                string[] array = tags;
                for (int i = 0; i < array.Length; i++)
                {
                    string value = array[i];
                    DataRow dataRow3 = dataTable3.NewRow();
                    dataRow3["FRowIndex"] = Convert.ToInt32(dictionaryEntry.Key);
                    dataRow3["FProductSN"] = value;
                    dataTable3.Rows.Add(dataRow3);
                }
            }
            dataSet.Tables.Add(dataTable2);
            dataSet.Tables.Add(dataTable3);
            this.Service.CreateEncasementBill(dataSet);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView dataRowView = (DataRowView)this.toolStripComboBox1.ComboBox.SelectedItem;
            this.toolStripTextBox_item.Text = dataRowView["FPackageUnitRate"].ToString();
            this.toolStripTextBox_buonds.Text = dataRowView["FBoxUnitRate"].ToString();
            string text = dataRowView["FName"].ToString();
            this.BoxNofirst = text.Substring(0, 1);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            bool flag = this.toolStripButton3.Text == "暂停";
            if (flag)
            {
                this.IsSensorAlarms = false;
                bool flag2 = this.m_RFIDReader.Actions.TagAccess.OperationSequence.Length > 0;
                if (flag2)
                {
                    this.m_RFIDReader.Actions.TagAccess.OperationSequence.StopSequence();
                }
                else
                {
                    this.m_RFIDReader.Actions.Inventory.Stop();
                }
                this.toolStripButton3.Text = "继续";
            }
            else
            {
                bool flag3 = this.toolStripButton3.Text == "继续";
                if (flag3)
                {
                    this.IsSensorAlarms = true;
                    this.m_RFIDReader.Actions.Inventory.Perform(null, null, null);
                    this.toolStripButton3.Text = "暂停";
                }
            }
        }

        private bool IsTagsCorrect()
        {
            int num = 0;
            bool result;
            foreach (ListViewItem listViewItem in this.listView_Tags.Items)
            {
                bool flag = listViewItem.SubItems[1].Text == "错误";
                if (flag)
                {
                    result = false;
                    return result;
                }
                num++;
            }
            bool flag2 = num == Convert.ToInt32(this.toolStripTextBox_item.Text);
            result = flag2;
            return result;
        }

        private bool CheckRepeat(HashSet<string> msg)
        {
            int num = 1;
            bool result;
            foreach (DictionaryEntry dictionaryEntry in this.m_TagTable)
            {
                string[] tags = ((Bounds)dictionaryEntry.Value).getTags();
                string[] array = tags;
                for (int i = 0; i < array.Length; i++)
                {
                    string text = array[i];
                    foreach (string current in msg)
                    {
                        bool flag = text == current;
                        if (flag)
                        {
                            MessageBox.Show(string.Concat(new object[]
                            {
                                "第",
                                num,
                                "捆的标签:",
                                text,
                                "与当前捆中的数据重复"
                            }));
                            result = false;
                            return result;
                        }
                    }
                }
                num++;
            }
            result = true;
            return result;
        }

        private bool CheckBoxTagsInHistory(HashSet<string> Tasg)
        {
            string text = "";
            foreach (string current in Tasg)
            {
                text = text + current + ",";
            }
            this.Service.CheckBindData(text);
            return true;
        }

        private void HoldErrorTags()
        {
            this.ErrorTags.Clear();
            foreach (ListViewItem listViewItem in this.listView_Tags.Items)
            {
                this.ErrorTags.Add(listViewItem.SubItems[0].Text, listViewItem.SubItems[1].Text);
            }
        }

        private bool IsErrorTags()
        {
            bool flag = this.ErrorTags.Count == 0;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                foreach (DictionaryEntry dictionaryEntry in this.ErrorTags)
                {
                    bool flag2 = (string)dictionaryEntry.Value == "错误";
                    if (flag2)
                    {
                        result = false;
                        return result;
                    }
                }
                result = true;
            }
            return result;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                this.liZuoSensor.AlarmsNolight();
                this.IsRedLight = false;
                bool flag = !this.IsErrorTags();
                if (flag)
                {
                    MessageBox.Show("尾捆有错误标签不能上传,或尾捆无数据");
                }
                else
                {
                    bool flag2 = this.m_TagTable.Count != Convert.ToInt32(this.toolStripTextBox_buonds.Text);
                    if (flag2)
                    {
                        MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                        DialogResult dialogResult = MessageBox.Show(string.Concat(new object[]
                        {
                            "捆数量:",
                            this.m_TagTable.Count,
                            "\r\n标签数量:",
                            this.ErrorTags.Count,
                            "\r\n合计:",
                            this.m_TagTable.Count * Convert.ToInt32(this.toolStripTextBox_item.Text) + this.ErrorTags.Count
                        }), "警告", buttons);
                        bool flag3 = dialogResult == DialogResult.OK;
                        if (flag3)
                        {
                            this.AddBoxItem();
                            Bounds bounds = new Bounds(this.ErrorTags.Count);
                            string[] array = new string[this.ErrorTags.Count];
                            int num = 0;
                            foreach (string text in this.ErrorTags.Keys)
                            {
                                array[num++] = text;
                            }
                            bounds.AddTags(array);
                            bounds.CapTimer = DateTime.Now;
                            bool flag4 = !this.m_TagTable.Contains(this.BoundID.ToString());
                            if (flag4)
                            {
                                this.m_TagTable.Add(this.BoundID.ToString(), bounds);
                            }
                            this.Upload();
                            DataRowView dataRowView = (DataRowView)this.toolStripComboBox1.ComboBox.SelectedItem;
                            Printer.AddPrintTask(dataRowView["FCode"].ToString(), dataRowView["FName"].ToString(), this.BoxNO, ((this.m_TagTable.Count - 1) * Convert.ToInt32(this.toolStripTextBox_item.Text) + this.ErrorTags.Count).ToString());
                            this.ErrorTags.Clear();
                            this.UpdataBoxItem();
                            this.listView_Bunchs.Items.Clear();
                            this.m_TagTable.Clear();
                            this.BoundID = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.liZuoSensor.RedAlarms();
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.liZuoSensor.Dispose();
            this.printer.Dispose();
            Thread.Sleep(100);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            FormSet formSet = new FormSet(m_RFIDReader);
            formSet.ShowDialog();
        }

        private void ListView_Bunchs_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool flag = this.listView_Bunchs.SelectedItems.Count > 0;
            if (flag)
            {
                try
                {
                    string text = this.listView_Bunchs.SelectedItems[0].SubItems[0].Text.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpdataBoundsTags(HashSet<string> msg)
        {
            this.listView_TagsInfo.BeginUpdate();
            this.listView_TagsInfo.Items.Clear();
            foreach (string current in msg)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.SubItems[0].Text = current;
                this.listView_TagsInfo.Items.Add(listViewItem);
            }
            this.listView_TagsInfo.EndUpdate();
        }

        private void ShowErrorMsg()
        {
            this.toolStripLabel6.Text = "错误信息:" + DateTime.Now.ToString() + "_标签已上传过";
            this.toolStripLabel6.ForeColor = Color.Red;
        }

        private void delay(int msec) {

            Thread thr = new Thread(() => { 
                Thread.Sleep(msec);//休眠时间
                this.IsReciveRFIDdata = false;
            });
            thr.Start();
        }


        private void SetAntennas()
        {
            Antennas.Config antConfig = m_RFIDReader.Config.Antennas[1].GetConfig();
            antConfig.ReceiveSensitivityIndex = 0;
            antConfig.TransmitPowerIndex = 25;
            antConfig.TransmitFrequencyIndex = 1;
            m_RFIDReader.Config.Antennas[1].SetConfig(antConfig);
        }


    }
}
