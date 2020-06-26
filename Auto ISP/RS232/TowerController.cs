using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Auto_Attach.RS232
{
    public class TowerController
    {

            SerialPort sp1 = new SerialPort();
            public string strPortNumber = "";
            public string strBaudrate = "";
            public string strDataBit = "";
            public string strStopBit = "";
            public string LedValue1 = "";
            public string LedValue2 = "";
            public string LedValue3 = "";


            public string strParity = "";
            public string strSend = "";
            public string Channel1_Cmd_On = "02 32 77 30 30 30 30 03";
            public string Channel2_Cmd_On = "95 02 00 01 01 99 95 02 02 01 32 cc";
            public string Channel3_Cmd_On = "95 02 00 01 64 fc 95 02 03 01 32 cd";
            public string Channel4_Cmd_On = "95 02 00 01 ff 97 95 02 04 01 32 ce";
            public string Channel1_Cmd_Off = "02 31 77 30 30 30 30 03";
            public string Channel2_Cmd_Off = "95 02 02 01 01 9b";
            public string Channel3_Cmd_Off = "95 02 03 01 01 9c";
            public string Channel4_Cmd_Off = "95 02 04 01 01 9d";
            public string Led1Off = "02 31 77 30 30 30 30 03";
            public string Led2Off = "02 32 77 30 30 30 30 03";
            public string Led3Off = "02 33 77 30 30 30 30 03";


            // 2 channel on & off
            public string Channel12_Cmd_On = "95 02 01 01 64 fd 95 02 02 01 9b 35";
            public string Channel34_Cmd_On = "95 02 03 01 96 31 95 02 04 01 ff 9b";
            public TowerController()
            {
            Initial_TowerController();
            }

            public void Initial_TowerController()
            {
                sp1.DataReceived += new SerialDataReceivedEventHandler(sp1_DataReceived);
            }
            public void LoadSettings(string fileName)
            {
                Profile.LoadProfile();//加载所有////./. 
                string strread = "";
                string headerStr = "Tower";
                FileOperation.ReadData(fileName, headerStr, "PortNo", ref strread);
                if (strread != "0")
                    strPortNumber = strread;

                FileOperation.ReadData(fileName, headerStr, "Baud", ref strread);
                if (strread != "0")
                    strBaudrate = strread;

                FileOperation.ReadData(fileName, headerStr, "DataBit", ref strread);
                if (strread != "0")
                    strDataBit = strread;

                FileOperation.ReadData(fileName, headerStr, "StopBit", ref strread);
                if (strread != "0")
                    strStopBit = strread;

                FileOperation.ReadData(fileName, headerStr, "Parity", ref strread);
                if (strread != "0")
                    strParity = strread;

                FileOperation.ReadData(fileName, headerStr, "LED1", ref strread);
                if (strread != "0")
                    LedValue1 = strread;

                FileOperation.ReadData(fileName, headerStr, "LED2", ref strread);
                if (strread != "0")
                    LedValue2 = strread;

                FileOperation.ReadData(fileName, headerStr, "LED3", ref strread);
                if (strread != "0")
                    LedValue3 = strread;

                Open();
                send(Led1Off); // Yellow LED
          
                send(Led2Off); // Red LED
        
                send(Led3Off); // Green LED

            }
            public void send(string strData)
            {
                //处理数字转换
                string sendBuf = strData;
                string sendnoNull = sendBuf.Trim();
                string sendNOComma = sendnoNull.Replace(',', ' ');    //去掉英文逗号
                string sendNOComma1 = sendNOComma.Replace('，', ' '); //去掉中文逗号
                string strSendNoComma2 = sendNOComma1.Replace("0x", "");   //去掉0x
                strSendNoComma2.Replace("0X", "");   //去掉0X
                string[] strArray = strSendNoComma2.Split(' ');

                int byteBufferLength = strArray.Length;
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (strArray[i] == "")
                    {
                        byteBufferLength--;
                    }
                }
                // int temp = 0;
                byte[] byteBuffer = new byte[byteBufferLength];
                int ii = 0;
                for (int i = 0; i < strArray.Length; i++)        //对获取的字符做相加运算
                {

                    Byte[] bytesOfStr = Encoding.Default.GetBytes(strArray[i]);

                    int decNum = 0;
                    if (strArray[i] == "")
                    {
                        //ii--;     //加上此句是错误的，下面的continue以延缓了一个ii，不与i同步
                        continue;
                    }
                    else
                    {
                        decNum = Convert.ToInt32(strArray[i], 16); //atrArray[i] == 12时，temp == 18 
                    }

                    try    //防止输错，使其只能输入一个字节的字符
                    {
                        byteBuffer[ii] = Convert.ToByte(decNum);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("字节越界，请逐个字节输入！", "Error");
                        // tmSend.Enabled = false;
                        return;
                    }

                    ii++;
                }
                try    //防止输错，使其只能输入一个字节的字符
                {
                    sp1.Write(byteBuffer, 0, byteBuffer.Length);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Port Backlight！", "Error");
                    // tmSend.Enabled = false;
                    return;
                }

            }
            void sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
            {
                if (sp1.IsOpen)     //此处可能没有必要判断是否打开串口，但为了严谨性，我还是加上了
                {
                    //输出当前时间
                    DateTime dt = DateTime.Now;
                    //txtReceive.Text += dt.GetDateTimeFormats('f')[0].ToString() + "\r\n";
                    //txtReceive.SelectAll();
                    //txtReceive.SelectionColor = Color.Blue;         //改变字体的颜色

                    //byte[] byteRead = new byte[sp1.BytesToRead];    //BytesToRead:sp1接收的字符个数
                    //if (rdSendStr.Checked)                          //'发送字符串'单选按钮
                    //{
                    //    txtReceive.Text += sp1.ReadLine() + "\r\n"; //注意：回车换行必须这样写，单独使用"\r"和"\n"都不会有效果
                    //    sp1.DiscardInBuffer();                      //清空SerialPort控件的Buffer 
                    //}
                    //else                                            //'发送16进制按钮'
                    {
                        try
                        {
                            Byte[] receivedData = new Byte[sp1.BytesToRead];        //创建接收字节数组
                            sp1.Read(receivedData, 0, receivedData.Length);         //读取数据
                                                                                    //string text = sp1.Read();   //Encoding.ASCII.GetString(receivedData);
                            sp1.DiscardInBuffer();                                  //清空SerialPort控件的Buffer

                            string strRcv = null;

                            for (int i = 0; i < receivedData.Length; i++) //窗体显示
                            {

                                strRcv += receivedData[i].ToString("X2");  //16进制显示
                            }
                            // txtReceive.Text += strRcv + "\r\n";
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message, "出错提示");
                            //txtSend.Text = "";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请打开某个串口", "错误提示");
                }
            }
            private void Open()
            {
                //serialPort1.IsOpen
                if (!sp1.IsOpen)
                {
                    try
                    {
                        //设置串口号
                        //  string serialName = cbSerial.SelectedItem.ToString();
                        sp1.PortName = strPortNumber;

                        //设置各“串口设置”
                        //string strBaudRate = cbBaudRate.Text;
                        //string strDateBits = cbDataBits.Text;
                        //string strStopBits = cbStop.Text;
                        Int32 iBaudRate = Convert.ToInt32(strBaudrate);
                        Int32 iDateBits = Convert.ToInt32(strDataBit);

                        sp1.BaudRate = Convert.ToInt32(strBaudrate);
                        sp1.DataBits = Convert.ToInt32(strDataBit);       //数据位
                        switch (strStopBit)            //停止位
                        {
                            case "1":
                                sp1.StopBits = StopBits.One;
                                break;
                            case "1.5":
                                sp1.StopBits = StopBits.OnePointFive;
                                break;
                            case "2":
                                sp1.StopBits = StopBits.Two;
                                break;
                            default:
                                MessageBox.Show("Error：参数不正确!", "Error");
                                break;
                        }
                        switch (strParity)             //校验位
                        {
                            case "None":
                                sp1.Parity = Parity.None;
                                break;
                            case "Odd":
                                sp1.Parity = Parity.Odd;
                                break;
                            case "Even":
                                sp1.Parity = Parity.Even;
                                break;
                            default:
                                MessageBox.Show("Error：参数不正确!", "Error");
                                break;
                        }

                        if (sp1.IsOpen == true)//如果打开状态，则先关闭一下
                        {
                            sp1.Close();
                        }
                        //状态栏设置
                        //tsSpNum.Text = "串口号：" + sp1.PortName + "|";
                        //tsBaudRate.Text = "波特率：" + sp1.BaudRate + "|";
                        //tsDataBits.Text = "数据位：" + sp1.DataBits + "|";
                        //tsStopBits.Text = "停止位：" + sp1.StopBits + "|";
                        //tsParity.Text = "校验位：" + sp1.Parity + "|";

                        //设置必要控件不可用
                        //cbSerial.Enabled = false;
                        //cbBaudRate.Enabled = false;
                        //cbDataBits.Enabled = false;
                        //cbStop.Enabled = false;
                        //cbParity.Enabled = false;

                        sp1.Open();     //打开串口
                                        //btnSwitch.Text = "关闭串口";
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Error:" + ex.Message, "Error");
                        //tmSend.Enabled = false;
                        sp1.Close();

                        return;
                    }
                }
                else
                {
                    sp1.Close();                    //关闭串口

                }

            }
            public void Close()
            {
                if (sp1.IsOpen == true)//如果打开状态，则先关闭一下
                {
                    sp1.Close();
                }
            }
        }
    }




