using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackTwo
{
    public partial class FormSet : Form
    {
        WebReference.SFBox Service = new WebReference.SFBox();
        private RFIDReader mRFIDReader;
        public FormSet(RFIDReader mRFIDReader)
        {
            InitializeComponent();
            txtServiceAddress.Text = Service.Url;
            txtRFID_IP.Text = XmlTool.Read("Root/IP");
            txtRFID_Port.Text = XmlTool.Read("Root/Port");
            this.mRFIDReader = mRFIDReader;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (txtServiceAddress.Text == "" || txtRFID_IP.Text == "" || txtRFID_Port.Text == "")
            {
                MessageBox.Show("请输入有效值");
                return;
            }
            Service.Url = txtServiceAddress.Text;
            XmlTool.Update("Root/IP", txtRFID_IP.Text);
            XmlTool.Update("Root/Port", txtRFID_Port.Text);

            MessageBox.Show("保存成功");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void txtAntenna_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == 13)
                {
                    ushort antenna = Convert.ToUInt16(txtAntenna.Text);
                    if (antenna < 1000 || antenna > 2900)
                        throw new Exception("不在功率范围内：" + "功率范围为:1000-2900");
                    ushort PowerIndex = (ushort)((antenna - 1000) / 10);
                    Antennas.Config antConfig = mRFIDReader.Config.Antennas[1].GetConfig();
                    antConfig.ReceiveSensitivityIndex = 0;
                    antConfig.TransmitPowerIndex = PowerIndex;
                    antConfig.TransmitFrequencyIndex = 1;
                    mRFIDReader.Config.Antennas[1].SetConfig(antConfig);
                    MessageBox.Show("设置成功");
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
