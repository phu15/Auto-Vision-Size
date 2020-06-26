using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Auto_Attach.Library;
using System.IO;
using System.IO.Ports;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Blob;
using Auto_Attach.RS232;
using Auto_Attach.GUI;



using PylonC.NET;


using HalconDotNet;


//using HalconVision;

namespace Auto_Attach
{
    public partial class MainForm : Form
    {
        public static MainForm instance;
        public Controller controller;
        public EziMOTIONPlusRLib.ITEM_NODE EziTable;
        private BaslerCamera camera1 = new BaslerCamera("Module1");
        private BaslerCamera camera2 = new BaslerCamera("Module2");
        private BaslerCamera camera3 = new BaslerCamera("Module3");
        private BaslerCamera camera4 = new BaslerCamera("Module4");
        public BackLight BL = new BackLight();
        private Inspection Inspect = new Inspection();

        public bool flgMainLoop = false;
        public string mchSetFileName = "Auto_ANI_Settings.xml";
        public string mchSetFileSpec = "Tilt_SettingSpec.xml";
        public string mchSetFileSpecTilt = "Tilt_SettingSpec.xml";
        public int iDelayTime = 70;
        private bool status = false;
        private string Position = "";
        private string TeachPos;
        private string type = "";
        private string page = "";
        private int isRetry = 0;
        uint velocity = 0, result = 0;
        byte m_nPortNo;
        bool m_bConnected;
        bool m_ServoOn;
        bool isDesitnation;

        // Loading information for Slave Servo
        public bool b_H_WLimitMin = false;
        public bool b_H_WLimitMax = false;
        public bool b_Original = false;
        public bool b_UserIN1 = false;
        public bool b_UserIN2 = false;
        public bool b_UserIN3 = false;
        public bool b_UserIN4 = false;
        public bool b_UserIN5 = false;

        public bool b_UserOUT1 = false;
        public bool b_UserOUT2 = false;
        public bool b_UserOUT3 = false;
        public bool b_UserOUT4 = false;


        public uint TablePosStep = 0;
        public uint intSpeedTeach = 10000;

        public int intStep = 0;
        public bool bServo0Run = false;
        public bool bServo1Run = false;


        public double dblLogo2CaliValue;
        public int mCountObject;
        protected Thread Mainthread;

        public string Version = "V0.3"; //Add Led control and delay Exposure time
        public string mchSettingsFilePath = "";
        public string mchSettingsFileSpecPath = "";
        public bool FlgStart;
        public int TotalLogo = 2;
        public double dblLogo1_WidthSpec;
        public double dblLogo2_WidthSpec;
        public double dblLogo1_LenghSpec;
        public double dblLogo2_LenghSpec;
        public double dblLogo1_XSpec;
        public double dblLogo2_XSpec;
        public double dblLogo1_YSpec;
        public double dblLogo2_YSpec;
        public string strModel;
        public string strBuyer;
        public int intTiltNotchValuePixel;
        public double dblTiltNotchValuePixel;
        public double dblTactTime;
        public int intLowThrehole_Cam1;
        public int intHiThrehole_Cam1;
        public int intLowThrehole_Cam2;
        public int intHiThrehole_Cam2;
        public bool blnCaliFlag = false;
        public bool blnResult;
        public double dblResult;
        public string CameraID_Cam1;
        public string strProgStatus;
        public int intCam1Exposure;
        public int intCam2Exposure;

        public string strVersion;
        public int intTiltResult_P1;
        public int intTiltResult_Cam1;
        public int intTiltResult_Cam2;
        public int intTiltResult_Cam3;
        public int intTiltResult_Cam4;
        public int intTiltResult_Cam5;
        public int intTiltResult_Cam6;
        public Double dblTiltResult_Cam1;
        public Double dblTiltResult_Cam2;
        public Double dblTiltResult_Cam3;
        public Double dblTiltResult_Cam4;
        public Double dblTiltResult_Cam5;
        public Double dblTiltResult_Cam6;

        public int intTiltResult_Notch;
        public Double dblTiltResult_Notch;


        public double dblTiltResult_P1;
        public int intTiltResult_P2;
        public double dblTiltResult_P2;

        public int intROI_X1;
        public int intROI_Y1;
        public int intROI_W1;
        public int intROI_L1;


        public int intROI_X2;
        public int intROI_Y2;
        public int intROI_W2;
        public int intROI_L2;

        public int RectROIWidth = 200;

        public double[] intTacTStep = new double[10];
        public bool[] blnFlagCheck = new bool[10];

        public int intMinResult = 0;
        public int intMaxResult = 0;

        public Boolean WorkStatus = false;
        public Boolean flgWaiting = false;
        public Boolean flgCheck = false;
        public Boolean flgOK = false;
        public Boolean flgNG = false;
        public Boolean flgSaveData = false;

        public int intProductTotalCount = 0;
        public int intProductOKCount = 0;
        public int intProductNGCount = 0;
        public double dblDataRessult = 0;
        public double[] dblArrDataRessult = new double[100];
        public int intArrDataRessult;

        public double dblCaliValueCam1;// Caliration value X1
        public double dblCaliValueCam2; //Caliration value X2
        public double dblCaliValueCam3; ///Calibration value Y2



        public double dblCaliValueC1; //Caliration value X2
        public double dblCaliValueC2; //Caliration value X2
        public double dblCaliValueC3; //Caliration value X2
        public double dblCaliValueC4; //Caliration value X2
        public double dblCaliValueC5; //Caliration value X2
        public double dblCaliValueC6; //Caliration value X2

        public string strResult;
        public Mat m_image = null;
        public Mat b_image = null;



        public int[] intResultStep = new int[10];
        public int TotalStepCheck = 0;
        public Boolean flgTotalCheck = true; // start program when power on
        public Boolean blnFinalJudge = false;

        public double[] dbResultCam = new double[60];
        public double[] dbResultCam1 = new double[60];
        public double[] dbResultCam2 = new double[60];
        public double[] dbResultCam3 = new double[60];
        public double dbResultNotch;
        public int[] intResultTiltCam = new int[100];
        public int ROI_NotchW, ROI_NotchL;

        public int RectROI_CamL = 200;
        public int RectROI_CamW = 800;
        // public int ROI_CamW, ROI_CamL,ROI_CamX,ROI_CamY;
        public int ROI_CamL = 400;
        public int ROI_CamW = 50;
        public int ROI_CamX = 140;
        public int ROI_CamY = 1160;
        // Rect ROI_Cam1 = new Rect()
        public int ROI_CamL_X = 400;
        public int ROI_CamW_X = 50;
        public int ROI_CamX_X = 200;
        public int ROI_CamY_X = 800;

        // Variable for Position Table.............
        public uint iStepPosServo0 = 0;
        public uint iStepPosServo1 = 0;

        public Byte PortNo = 3;
        public uint Baud = 115200;
        public bool bnStart1 = false;
        public bool bnStart2 = false;
        public bool bnEmer = false;
        public bool bnSafety = false;

        public ulong cmdPos;
        public ulong ActPos;
        public bool isFinishedCheck = false;
        //  public double dblTactTime;

        public DateTime st_time = DateTime.Now;
        public TimeSpan time_span;
        public bool bStartProg = false;


        public string Point;
        public int iProductCount = 0;
        public int iOKCount = 0;

        public Mat mMaskImage;
        public Mat mTestImage;
        private OpenCvSharp.Point[][] contours;
        public int intLogo1_Width, intLogo1_Length, intLogo1_X, intLogo1_Y;
        public int intLogo2_Width, intLogo2_Length, intLogo2_X, intLogo2_Y;
        public int GetValueFromCharacter = 4;
        public int TotalCharacterStream = 70;
        public int[] mValue_Width = new int[7];
        public int[] mValue_Length = new int[7];
        public int[] mValue_X = new int[7];
        public int[] mValue_Y = new int[7];

        public int iResult;
        public double[] arrResultFinal = new double[11];
        public int[] arrResult = new int[520];
        public int[] iThreHoleHi = new int[520];
        public int[] iThreHoleLow = new int[520];
        public int[] Exposure = new int[520];
        public double[] dLowSpec = new double[520];
        public double[] dHiSpec = new double[520];

        public int[] iPixel = new int[520];
        public double[] dCaliData = new double[520];

        public int iPointMea;
        public bool bFinalResult;
        public bool bFinalResult1;
        public bool bFinalResult2;
        public bool bFinalResult3;
        public int UsrLevel = 0;
        private UserLogin User = new UserLogin();
        public int iBreakLoop;
        public bool[] NGPosition = new bool[11];
        public int[] Cam1Result = new int[520];
        public int[] Cam2Result = new int[520];
        public int[] Cam3Result = new int[520];





        /// <summary>


        /// </summary>

        public MainForm()
        {
            instance = this;
            InitializeComponent();
        }
        private void MainProcess()
        {
            //******************* Checking start condition ********************************//
            AfterStartSenseProcess();
            //******************* Test Process ********************************//
            if (!bnEmer && !bnSafety) // make safety
            {
                TestProcess();
            }
            else
            {
                lblMainloop.BackColor = Color.Red;
                lblMainloop.Text = "Check Emergency or Safety ";
            }

            //******************* Finish Test ********************************//
            TestEndProcess();
            //******************* Save Data ********************************//
            TestDataSaving();

        }

        private void TestProcess()
        {
            // btnClose.Enabled = false;
            // btnClose.Visible = false;
            // if (bnStart1)


            if ((bStartProg) && (!isFinishedCheck))
            {
                st_time = DateTime.Now;
                bStartProg = false;
                dblTactTime = 0;
            }
            //TimeSpan time_span;



            // main prcoess start.........
            TestSelect();

            // Check tact time.............
            if (bnStart1)
                time_span = DateTime.Now - st_time;
            dblTactTime = time_span.TotalMilliseconds;
            txtTact.Text = (dblTactTime / 1000).ToString();

            if (isFinishedCheck)
            {


                txtTact.Text = (dblTactTime / 1000).ToString();
                int RowCount = dbViewData.Rows.Count;
                for (int j = 1; j < 11;j++)
                { 
                    if (j == 3)
                    {
                        dbResultCam1[j] = Math.Round(Cam1Result[j] * camera1.CaliValue.Y,2);
                    }
                    else
                        dbResultCam1[j] = Math.Round(Cam1Result[j] * camera1.CaliValue.X,2);
                }

                for (int j = 1; j < 11; j++)
                {
                    if (j == 3)
                    {
                        dbResultCam2[j] = Math.Round(Cam2Result[j] * camera2.CaliValue.Y,2);
                    }
                    else
                        dbResultCam2[j] = Math.Round(Cam2Result[j] * camera2.CaliValue.X,2);
                }
                for (int j = 1; j < 11; j++)
                {
                    if (j == 3)
                    {
                        dbResultCam3[j] = Math.Round(Cam3Result[j] * camera3.CaliValue.Y,2);
                    }
                    else
                        dbResultCam3[j] = Math.Round(Cam3Result[j] * camera3.CaliValue.X,2);
                }

                dbViewData.Rows.Add(dbResultCam1[1], dbResultCam1[2], dbResultCam1[3], dbResultCam1[4], dbResultCam1[5], dbResultCam1[6], dbResultCam1[7], dbResultCam1[8], dbResultCam1[9], dbResultCam1[10]);//, Cam1Result[11], Cam1Result[12], Cam1Result[13], Cam1Result[14], Cam1Result[15]);
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module1", "CurrentData_X", Cam1Result[1].ToString ());
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module1", "CurrentData_Y", Cam1Result[3].ToString());

             
                dbViewData.Rows.Add(dbResultCam2[1], dbResultCam2[2], dbResultCam2[3], dbResultCam2[4], dbResultCam2[5], dbResultCam2[6], dbResultCam2[7], dbResultCam2[8], dbResultCam2[9], dbResultCam2[10]);//, Cam2Result[11], Cam2Result[12], Cam2Result[13], Cam2Result[14], Cam2Result[15]);
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module2", "CurrentData_X", Cam2Result[1].ToString());
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module2", "CurrentData_Y", Cam2Result[3].ToString());

                dbViewData.Rows.Add(dbResultCam3[1], dbResultCam3[2], dbResultCam3[3], dbResultCam3[4], dbResultCam3[5], dbResultCam3[6], dbResultCam3[7], dbResultCam3[8], dbResultCam3[9], dbResultCam3[10]);//, Cam3Result[11], Cam3Result[12], Cam3Result[13], Cam3Result[14], Cam3Result[15]);
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module3", "CurrentData_X", Cam3Result[1].ToString());
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module3", "CurrentData_Y", Cam3Result[3].ToString());

                dbViewData.CurrentCell = dbViewData.Rows[RowCount - 1].Cells[0];// Đưa Control về vị trí của nó



                bFinalResult1 = true;
                bFinalResult2 = true;
                bFinalResult3 = true;
             
                BL.send(BL.Led1Off);
                BL.send(BL.Led2Off);
                BL.send(BL.Led3Off);
                NGPosition = new bool[11];
                for (int i = 1; i < 11; i++)
                {
                    if ((dbResultCam1[i] < dLowSpec[i]) || (dbResultCam1[i] > dHiSpec[i]))
                    {
                        bFinalResult1 = false;
                        for (int j = 0; j <5; j++)
                        {
                            BL.send(BL.LedValue1);
                            Delay(100);
                            BL.send(BL.Led1Off);
                            Delay(100);
                        }
                       
                        NGPosition[i] = true;
                    }
                }

                for (int i = 1; i < 11; i++)
                {
                    if ((dbResultCam2[i] < dLowSpec[i]) || (dbResultCam2[i] > dHiSpec[i]))
                    {
                        bFinalResult2 = false;
                        for (int j = 0; j < 5; j++)
                        {
                            BL.send(BL.LedValue2);
                            Delay(100);
                            BL.send(BL.Led2Off);
                            Delay(100);
                        }

                        NGPosition[i] = true;
                    }
                }
                for (int i = 1; i < 11; i++)
                {
                    if ((dbResultCam3[i] < dLowSpec[i]) || (dbResultCam3[i] > dHiSpec[i]))
                    {
                        bFinalResult3 = false;
                        for (int j = 0; j < 5; j++)
                        {
                            BL.send(BL.LedValue3);
                            Delay(100);
                            BL.send(BL.Led3Off);
                            Delay(100);
                        }
                        NGPosition[i] = true;
                    }
                }

                if (bFinalResult1 == false)
                {
                    //int RowCount = dbViewData.Rows.Count;
                    lblResult.ForeColor = Color.Red;
                    lblResult.Text = "Fail";
                }
                else
                {
                    lblResult.ForeColor = Color.Blue;
                    lblResult.Text = "Pass";
                }

                if (bFinalResult2 == false)
                {
                    //int RowCount = dbViewData.Rows.Count;
                    lblResult2.ForeColor = Color.Red;
                    lblResult2.Text = "Fail";
                }
                else
                {
                    lblResult2.ForeColor = Color.Blue;
                    lblResult2.Text = "Pass";
                }

                if (bFinalResult3 == false)
                {
                    //int RowCount = dbViewData.Rows.Count;
                    lblResult3.ForeColor = Color.Red;
                    lblResult3.Text = "Fail";
                }
                else
                {
                    lblResult3.ForeColor = Color.Blue;
                    lblResult3.Text = "Pass";
                }

                

            }
            txtOkCount.Text = Convert.ToString(iOKCount);
            if (iProductCount == 0)
                txtPassRate.Text = "";
            else
            {
                txtPassRate.Text = Convert.ToString(iOKCount * 100 / iProductCount);
            }

        }

        public void GetTickCount()
        {

        }

        private void TestEndProcess()
        {
            // indicate product couting.
            txtCount.Text = Convert.ToString(iProductCount);
            lbLog.Items.Clear();
            lbLog.Items.Add(intStep);
        }
        private void TestDataSaving()
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string Savepath = exePath + "Data";
            string RecordData = "";
            RecordData += iProductCount + ",";
            RecordData += "Roll1" + ",";
            for (int i = 1; i < 11; i++)
            {
                RecordData += dbResultCam1[i] + ",";
            }
            RecordData += bFinalResult + ",";
            RecordData += dLowSpec[1] + "," + dHiSpec[1] + ",";

            RecordData += DateTime.Now;
            if (isFinishedCheck)
            {

                SaveRawData(Savepath, txtModel.Text, strBuyer, RecordData);
                //isFinishedCheck = false;
            }

            /////////////Record Roll2/////////////////////
            RecordData = "";
            RecordData += iProductCount + ",";
            RecordData += "Roll2" + ",";
            for (int i = 1; i < 11; i++)
            {
                RecordData += dbResultCam2[i] + ",";
            }
            RecordData += bFinalResult + ",";
           // RecordData += dLowSpec[1] + "," + dHiSpec[1] + ",";

            RecordData += DateTime.Now;
            if (isFinishedCheck)
            {

                SaveRawData(Savepath, txtModel.Text, strBuyer, RecordData);
                //isFinishedCheck = false;
            }
            /////////////Record Roll3/////////////////////
            RecordData = "";
            RecordData += iProductCount + ",";
            RecordData += "Roll3" + ",";
            for (int i = 1; i < 11; i++)
            {
                RecordData += dbResultCam3[i] + ",";
            }
            RecordData += bFinalResult + ",";

            RecordData += DateTime.Now;
            if (isFinishedCheck)
            {

                SaveRawData(Savepath, txtModel.Text, strBuyer, RecordData);
                isFinishedCheck = false;
            }

        }
        public void CheckIsDestination(bool res, byte Motor)
        {
            int nRtn = 0;

            Delay(10);
            nRtn = IO.FAS_GetActualPos(m_nPortNo, Motor, out ActPos);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "CheckIsDestination() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
            Delay(10);
            nRtn = IO.FAS_GetCommandPos(m_nPortNo, Motor, out cmdPos);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "CheckIsDestination() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }

            isDesitnation = false;

            if (ActPos > 1000000)
                ActPos = 0;
            if (cmdPos > 1000000)
                cmdPos = 0;


            if (ActPos >= cmdPos)
            {
                if (ActPos - cmdPos < 25)
                    isDesitnation = true;
            }
            if (ActPos <= cmdPos)
            {
                if (cmdPos - ActPos < 25)
                    isDesitnation = true;
            }


        }



        private void TestSelect()
        {
            switch (intStep)
            {
                case 0:

                    TEST01();
                   
                    break;

                case 1:
                    TEST02();
                    
                    break;
                case 2:
                    TEST03();
                    break;
                case 3:
                    TEST04();
                    break;
                case 4:
                    TEST05();
                    break;
                case 5:
                    TEST06();
                    break;
                case 6:
                    TEST07();
                    break;
                case 7:
                    TEST08();
                    break;
                case 8:
                    TEST09();
                    break;
                case 9:
                    TEST10();
                    break;
                case 10:
                    TEST11();
                    break;
                case 11:
                    TEST12();
                    break;
                case 12:
                    TEST13();
                    break;
                case 13:
                    TEST14();
                    lblMainloop.ForeColor = Color.Green;
                    
                    lblMainloop.Text = "Finished...";
                    isFinishedCheck = true;
                    bStartProg = false;
                    break;

            }
        }



        public void TEST01()  // Moving Servo 1 to original - Check Point7 ~ 8
        {
            int nRtn;


         
            if (bnStart1 == true)
            {
                //BL.send(BL.Channel12_Cmd_On);
                //BL.send(BL.Channel34_Cmd_On);
                BL.send(BL.LedValue1);
                BL.send(BL.LedValue2);
                BL.send(BL.LedValue3);
                lblResult.Text = "";
                lblResult2.Text = "";
                lblResult3.Text = "";

                lblMainloop.ForeColor = Color.Yellow;
                lblResult.Text = "";

                lblMainloop.Text = "Checking...";

                bStartProg = true;
                 // nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 1, true, Convert.ToUInt32(0));
                if (bServo0Run == false)
                {
                    nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 1, false, 1);
                    if (nRtn != IO.FMM_OK)
                    {
                        string strMsg;
                        strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                        //   MessageBox.Show(strMsg, "Function Failed");
                        lbLog.Items.Clear();
                        lbLog.Items.Add(strMsg);
                        bServo1Run = false;
                        bStartProg = false;
                        goto Exit;
                    }
                    bServo0Run = true;
                }
               

                // intStep++;

                // get position of motor...........................


                CheckIsDestination(isDesitnation,1);
                //  Delay(iDelayTime);
                if (isDesitnation)
                {
                    intStep ++;
                }

                
                              

                
              }

        Exit:;
        }

        public void TEST02()// Moving Servo 1 to check point 1 - Check Point 9 ~10
        {
            int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;
            
            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }

            // get position..................


            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep ++;
                bServo0Run = false;

            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 100)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                int i;
                // save picture ................................
                camera1.ImageBuffer.Save("Point1_1.bmp", ImageFormat.Bmp);

                ///// Image processing........................... ...................................            
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point1_1.bmp", 1,1,0,2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point1_1.bmp");
                int ROI1 = 110; int ROI2 = 111;
                Cam1Result[1] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right",iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2],"Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
               // ROI1 = 112;ROI2 = 113;
               // Cam1Result[2] =Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1],"Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2],"Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
 
                //// Camera2 grapping............................
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point1_2.bmp", ImageFormat.Bmp);
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point1_2.bmp", 2, 1, 0, 2).ToBitmap();


                /// Image processing.............................................................................
                m_image = new Mat(strHeadFile + "\\Point1_2.bmp");
                ROI1 = 210; ROI2 = 211;
                Cam2Result[1] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                //ROI1 = 212; ROI2 = 213;
                //Cam2Result[2]  = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point1_3.bmp", ImageFormat.Bmp);
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point1_3.bmp", 3, 1, 0, 2).ToBitmap();
                //////////Image processing................................................................
                m_image = new Mat(strHeadFile + "\\Point1_3.bmp");
                ROI1 = 310; ROI2 = 311;
                Cam3Result[1] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
               // Cam3Result[2] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);

            }

            ///////////// Image analysis //////////////////////////


        Exit:;
        }

        public void TEST03() // Moving Servo 1 to check point 2
        {
            int nRtn;


            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;

            // Run..............
            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 1);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }

            // Check Position..................

            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep ++;
                Delay(1000);
                bServo0Run = false;
               // bnStart1 = false;
                //isFinishedCheck = true;
            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera1.ImageBuffer.Save("Point2_1.bmp", ImageFormat.Bmp);


                /// Image processing.....................................................
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point2_1.bmp", 1,2,0,5).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point2_1.bmp");
                int ROI1 = 120; int ROI2 = 121;
                Cam1Result[2] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 122; ROI2 = 123;
                Cam1Result[3] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 120; ROI2 = 124;
                Cam1Result[4] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point2_2.bmp", ImageFormat.Bmp);
                /// Image processing.....................................................
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point2_2.bmp", 2,2,0,5).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point2_2.bmp");
                ROI1 = 220; ROI2 = 221;
                Cam2Result[2] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 222; ROI2 = 223;
                Cam2Result[3] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 220; ROI2 = 224;
                Cam2Result[4] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point2_3.bmp", ImageFormat.Bmp);
                /// Image processing.....................................................
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point2_3.bmp", 3, 2, 0, 5).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point2_3.bmp");
                ROI1 = 320; ROI2 = 321;
                Cam3Result[2] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 322; ROI2 = 323;
                Cam3Result[3] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 320; ROI2 = 324;
                Cam3Result[4] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
            }
        Exit:;

        }

        public void TEST04() // Moving Servo 1 to check point 3
        {
            int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;


            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 2);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................
            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep++;
                Delay(1000);
                bServo0Run = false;
            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera1.ImageBuffer.Save("Point3_1.bmp", ImageFormat.Bmp);
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point3_1.bmp", 1, 3, 0, 5).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point3_1.bmp");
                int ROI1 = 130; int ROI2 = 131;
                Cam1Result[5] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 130; ROI2 = 132;
                Cam1Result[6] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ROI1 = 133; ROI2 = 134;
                Cam1Result[10] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point3_2.bmp", ImageFormat.Bmp);
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point3_2.bmp", 2, 3, 0, 5).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point3_2.bmp");
                ROI1 = 230; ROI2 = 231;
                Cam2Result[5] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 230; ROI2 = 232;
                Cam2Result[6] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ROI1 = 233; ROI2 = 234;
                Cam2Result[10] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point3_3.bmp", ImageFormat.Bmp);
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point3_3.bmp", 3, 3, 0, 5).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point3_3.bmp");
                ROI1 = 330; ROI2 = 331;
                Cam3Result[5] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 330; ROI2 = 332;
                Cam3Result[6] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ROI1 = 333; ROI2 = 334;
                Cam3Result[10] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
            }
        Exit:;


        }



        public void TEST05() //Moving Servo 0 to original - Point 6
        {
            int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;

            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 3);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................

            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep++;
                Delay(1000);
                bServo0Run = false;
            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera1.ImageBuffer.Save("Point4_1.bmp", ImageFormat.Bmp);
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point4_1.bmp", 1, 4, 0, 3).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point4_1.bmp");
                int ROI1 = 140; int ROI2 = 141;
                Cam1Result[7] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 140; ROI2 = 142;
                Cam1Result[8] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point4_2.bmp", ImageFormat.Bmp);
                Delay(100);
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point4_2.bmp", 2, 4, 0, 3).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point4_2.bmp");
                ROI1 = 240;ROI2 = 241;
                Cam2Result[7] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 240; ROI2 = 242;
                Cam2Result[8] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point4_3.bmp", ImageFormat.Bmp);
                Delay(100);
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point4_3.bmp", 3, 4, 0, 3).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point4_3.bmp");
                ROI1 = 340;ROI2 = 341;
                Cam3Result[7] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 340; ROI2 = 342;
                Cam3Result[8] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
            }
        Exit:;


        }


        public void TEST06() //Moving Servo 0 to  - check point 2
        {
            int nRtn;

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;

            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 4);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................
            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep = 10;
                Delay(1000);
                bServo0Run = false;
            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera1.ImageBuffer.Save("Point5_1.bmp", ImageFormat.Bmp);
                Delay(100);
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point5_1.bmp", 1, 5, 0, 2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point5_1.bmp");
                Int16 ROI1 = 150; Int16 ROI2 = 151;
               // Cam1Result[10] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 150; ROI2 = 151;
                Cam1Result[9] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point5_2.bmp", ImageFormat.Bmp);
                Delay(100);
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point5_2.bmp", 2, 5, 0, 2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point5_2.bmp");
                ROI1 = 250; ROI2 = 251;
              //  Cam2Result[10] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 250; ROI2 = 251;
                Cam2Result[9] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point5_3.bmp", ImageFormat.Bmp);
                Delay(100);
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point5_3.bmp", 3, 5, 0, 2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point5_3.bmp");
                ROI1 = 350;ROI2 = 351;
              //  Cam3Result[10] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 350; ROI2 = 351;
                Cam3Result[9] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
            }
        Exit:;

        }

        public void TEST07() //Moving Servo 0 to check point 5
        {
            int nRtn;

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;


            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 4);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................
            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep++;
                Delay(1000);
                bServo0Run = false;
            }
       

        }

        public void TEST08() // Moving Servo 1 
        {
            int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;



            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 6);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................


            // Move next step.............................
            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep++;
                Delay(1000);
                bServo0Run = false;
            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 100)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera1.ImageBuffer.Save("Point6_1.bmp", ImageFormat.Bmp);
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point6_1.bmp", 1, 6, 0, 3).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point6_1.bmp");
                Int16 ROI1 = 160; Int16 ROI2 = 161;
                Cam1Result[12] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 160; ROI2 = 162;
                Cam1Result[13] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 100)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point6_2.bmp", ImageFormat.Bmp);
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point6_2.bmp", 2, 6, 0, 3).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point6_2.bmp");
                ROI1 = 260;ROI2 = 261;
                Cam2Result[12] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 260; ROI2 = 262;
                Cam2Result[13] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);

                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 100)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point6_3.bmp", ImageFormat.Bmp);
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point6_3.bmp", 3, 6, 0, 3).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point6_3.bmp");
                ROI1 = 360; ROI2 = 361;
                Cam3Result[12] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Left", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 0, ROI1, ROI1, ROI2);
                ROI1 = 360; ROI2 = 362;
                Cam3Result[13] = Inspect.CurveDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Right", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Right", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
            }
        Exit:;

        }

        public void TEST09() //Move and check point 8
        {
            int nRtn;
            // int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;


            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 7);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................

            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep++;
                Delay(1000);
                bServo0Run = false;
            }
            if (isDesitnation)
            {

                //////// Grap camera No 1//////
                camera1.StartGrabbing();
                iBreakLoop = 0;
                while (!camera1.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera1.ImageBuffer != null)
                    picCam1.Image = camera1.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera1.ImageBuffer.Save("Point7_1.bmp", ImageFormat.Bmp);
                picCam1.Image = Inspect.DrawROI(strHeadFile + "\\Point7_1.bmp", 1, 7, 0, 2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point7_1.bmp");
                int ROI1 = 170; int ROI2 = 171;
                Cam1Result[14] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Up", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Down", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                ///////Grap camera No 2////////
                camera2.StartGrabbing();
                iBreakLoop = 0;
                while (!camera2.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera2.ImageBuffer != null)
                    picCam2.Image = camera2.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera2.ImageBuffer.Save("Point7_2.bmp", ImageFormat.Bmp);
                picCam2.Image = Inspect.DrawROI(strHeadFile + "\\Point7_2.bmp", 2, 7, 0, 2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point7_2.bmp");
                ROI1 = 270; ROI2 = 271;
                Cam2Result[14] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Up", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Down", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
                //////////Grap camera No3////////////////////////
                camera3.StartGrabbing();
                iBreakLoop = 0;
                while (!camera3.bGrabDone)
                {
                    Thread.Sleep(10);
                    iBreakLoop++;
                    if (iBreakLoop > 200)
                        //break;
                        goto Exit;
                }
                if (camera3.ImageBuffer != null)
                    picCam3.Image = camera3.ImageBuffer;
                else
                    goto Exit;

                // save picture ................................
                camera3.ImageBuffer.Save("Point7_3.bmp", ImageFormat.Bmp);
                picCam3.Image = Inspect.DrawROI(strHeadFile + "\\Point7_3.bmp", 3, 7, 0, 2).ToBitmap();
                m_image = new Mat(strHeadFile + "\\Point7_3.bmp");
                ROI1 = 370; ROI2 = 371;
                Cam3Result[14] = Inspect.LineDistanceDetector(m_image, Inspect.ArrROI[ROI1], "Up", iThreHoleLow[ROI1], iThreHoleHi[ROI1], Inspect.ArrROI[ROI2], "Down", iThreHoleLow[ROI2], iThreHoleHi[ROI2], 1, ROI1, ROI1, ROI2);
            }
        Exit:;


        }
        public void TEST10() //Do nothing
        {
            int nRtn;
            //  int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;


            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 7);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }
       



            // Check Position..................

            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {
                intStep ++;
                Delay(1000);
                bServo0Run = false;
                // bnStart1 = false;
            }
            

        }

        public void TEST11() //Moving to orrignal postion Upper Motor
        {
            int nRtn;

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;



            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;
            }


            // Check Position..................

            CheckIsDestination(isDesitnation,0);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {

                Delay(1000);
                intStep++;
                bServo0Run = false;
            }

        }

        public void TEST12() //Moving to orrignal postion Lower Motor
        {
            int nRtn;
            Mat m_image = new Mat();
            string strHeadFile;


            if (bServo0Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 1, false, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo0Run = true;

            }


            // Check Position..................

            CheckIsDestination(isDesitnation,1);
            //  Delay(iDelayTime);
            if (isDesitnation)
            {

                Delay(1000);
                intStep = 13;
                

            }

        }

        public void TEST13() // Moving Servo 1 to check point original
        {
            int nRtn;

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;



            if (bServo1Run == false)
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, 5);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_PosTableSingleRunItem_TEST() \nReturned: " + (intStep + 1).ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                bServo1Run = true;
            }


            // Check Position..................

            CheckIsDestination(isDesitnation,0);
          //  Delay(iDelayTime);
            if (isDesitnation)
            {
                bnStart1 = false;

                bServo1Run = false;
                intStep++;
                // iProductCount++;


            }
        }

        public void TEST14() // Image handle.............................................
        {

           bnStart1 = false;
                intStep = 0;
                bServo1Run = false;
                bnStart1 = false;
                bServo0Run = false;
                isFinishedCheck = true;
                


                iProductCount++;
                bnStart1 = false;
                intStep = 0;
                bServo0Run = false;

                //BL.send(BL.Channel1_Cmd_Off);
                //BL.send(BL.Channel2_Cmd_Off);
                //BL.send(BL.Channel3_Cmd_Off);
                //BL.send(BL.Channel4_Cmd_Off);
                

                // get path..............................
                string strHeadFile;
                strHeadFile = Application.StartupPath;
                String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string mchSettingsFilePath;
                string strread = "";
                mchSettingsFilePath = exePath + mchSetFileName;




               
               
                if (bFinalResult == true)
                {
                    iOKCount = iOKCount + 1;
                }
            
        }


        public Boolean f_GetTiltFromImage(Mat Image, int thresholdLow, int thresholdHigh, int NumberImage, int Direction, int iStep, int Result)
        {

            /// this function to calculate GAP and save Picture...................................
            Mat m_image = null;
            m_image = Image.Clone();

            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];
            int[,] linearray = new int[1, 4];
            int[,] LineMin = new int[1, 1];
            int[,] LineMax = new int[1, 1];

            int[] MinMax = new int[100000];

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThresholeLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThresholeHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            Mat iCanyLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCanyHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCany = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat ImageTest = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            // ROI clear bolder of image...........................................
            Rect ROITest = new Rect();
            ROITest.X = 10;
            ROITest.Y = 10;
            ROITest.Height = m_image.Height - 20;
            ROITest.Width = m_image.Width - 20;
            //......................................................................

            TestImage = m_image.Clone();


            iThresholeLow = m_image.Clone();
            iThresholeHigh = m_image.Clone();

            // Filting Image.............................................................................................
            Cv2.CvtColor(TestImage, TestImage, ColorConversionCodes.BGRA2GRAY);
            Cv2.Blur(TestImage, TestImage, new OpenCvSharp.Size(1, 1));
            Cv2.GaussianBlur(TestImage, TestImage, new OpenCvSharp.Size(3, 3), 0);
            Cv2.MedianBlur(TestImage, TestImage, 1);
            Cv2.Threshold(TestImage, iThresholeLow, thresh: thresholdLow, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.Threshold(TestImage, iThresholeHigh, thresh: thresholdHigh, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.BitwiseAnd(iThresholeHigh, ~iThresholeLow, TestImage);
            ImageTest = new Mat(TestImage, ROITest);
            //  Cv2.ImShow("ImageTest", ImageTest);
            //  Cv2.WaitKey(0);

            int intCountOperation = 0;
            contours = Cv2.FindContoursAsArray(ImageTest, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                return false;
            }
            int intTemp = 0;

            for (int i = 0; i < contours.Length; i++)
            {
                double area = Cv2.ContourArea(contours[i]);  //  Find the area of contour
                OpenCvSharp.Rect uRect = Cv2.BoundingRect(contours[i]);
                // Get data X Axis................................................
                if (Direction == 0)
                    for (int j = 0; j < contours[i].Length; j++)
                    {
                        MinMax[intCountOperation] = contours[i][j].X;
                        intCountOperation++;
                    }
                // Get data Y Axis................................................
                if (Direction == 1)
                    for (int j = 0; j < contours[i].Length; j++)
                    {
                        MinMax[intCountOperation] = contours[i][j].Y;
                        intCountOperation++;
                    }
                Cv2.DrawContours(ImageTest, contours, i, Scalar.White, 2);
            }

            //Cv2.ImShow("ImageTest", ImageTest);
            // Cv2.WaitKey(0);
            //...............MinMax finder........................................................................................
            for (int i = 0; i < intCountOperation; i++)
            {
                for (int j = i + 1; j < intCountOperation; j++)
                {
                    if (MinMax[j] < MinMax[i])

                    {
                        intTemp = MinMax[i];
                        MinMax[i] = MinMax[j];
                        MinMax[j] = intTemp;
                    }
                }
            }
            //................report result........................................................................................
            intMinResult = MinMax[0];
            intMaxResult = MinMax[intCountOperation - 1];
            arrResult[iStep] = intMaxResult - intMinResult;
            if (Direction == 1)
            {
                m_image.Line(0, intMinResult + 10, ImageTest.Width + 20, intMinResult + 10, Scalar.Orange, 4, LineTypes.AntiAlias, 0);
                m_image.Line(0, intMaxResult + 10, ImageTest.Width + 20, intMaxResult + 10, Scalar.Orange, 4, LineTypes.AntiAlias, 0);
            }
            else if (Direction == 0)
            {
                m_image.Line(intMinResult + 10, 0, intMinResult + 10, ImageTest.Height + 20, Scalar.Blue, 4, LineTypes.AntiAlias, 0);
                m_image.Line(intMaxResult + 10, 0, intMaxResult + 10, ImageTest.Height + 20, Scalar.Blue, 4, LineTypes.AntiAlias, 0);
            }

            //Cv2.ImShow("ImageTest", ImageTest);
            //Cv2.WaitKey(0);
            m_image.ToBitmap().Save("image" + iStep + "" + NumberImage + ".bmp", ImageFormat.Bmp);
            return true;

        }
        public Mat MakeROI(Mat ImageIn, Mat ImageOut, Rect ROI)
        {


            Cv2.Rectangle(ImageIn, ROI, Scalar.Red, 5);

            return ImageOut;

        }

        public Mat ImageAnalysis(string PathFileImage, int thresholdLow, int thresholdHi, Rect ROI, int iStep, int iResult)
        {





            m_image = new Mat(PathFileImage);
            MakeROI(m_image, m_image, ROI);
            Cv2.Rectangle(m_image, ROI, Scalar.Red, 5); // Draw Rectangles.......................
            Mat imgROI1 = new Mat(m_image, ROI);
            //Cv2.ImShow("ImageTest", imgROI1);
            // Cv2.WaitKey(0);

            if (f_GetTiltFromImage(imgROI1, thresholdLow, thresholdHi, 0, 0, iStep, iResult))
            {
                //arrResult[iStep] = iResult;
            }

            return m_image;
        }

        #region Moving Form
        private bool dragging = false;
        private System.Drawing.Point dragCursorPoint;
        private System.Drawing.Point dragFormPoint;
        string CvyrX, CvyrZ, DspnX, DspnZ, stdbX, stdbZ;
        string appPath = Path.GetDirectoryName(Application.ExecutablePath);
        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                System.Drawing.Point dif = System.Drawing.Point.Subtract(Cursor.Position, new System.Drawing.Size(dragCursorPoint));
                this.Location = System.Drawing.Point.Add(dragFormPoint, new System.Drawing.Size(dif));
            }
        }

        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void Main_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
        #endregion

        void loadConfig()
        {
            controller.CtrlIOPort.com = (Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "I/O COM") == "") ? "COM11" : Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "I/O COM"); //Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "I/O COM");
            controller.CtrlServoPort.com = (Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "SERVO COM") == "") ? "COM3" : Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "SERVO COM");
            controller.CtrlServoPort.baudrate = "115200";
            velocity = uint.Parse((Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "VELOCITY") == "") ? "50000" : Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "VELOCITY"));
            controller.DelayAttach = Convert.ToInt32((Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "DELAY ATTACH") == "") ? "1000" : Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "DELAY ATTACH"));
            controller.DelayPunch = Convert.ToInt32((Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "DELAY PUNCH") == "") ? "1000" : Inifiles.ReadValue(appPath + "/SETTING.INI", "SETTING", "DELAY PUNCH"));
            txtOrderSpeed.Text = Convert.ToString(intSpeedTeach);
            txtMoveSpeed.Text = Convert.ToString(intSpeedTeach);
        }

        void loadTeaching()
        {
            CvyrX = Inifiles.ReadValue(appPath + "/SERVO.INI", "SLAVE 0", "ITEM2");
            CvyrZ = Inifiles.ReadValue(appPath + "/SERVO.INI", "SLAVE 1", "ITEM2");
            DspnX = Inifiles.ReadValue(appPath + "/SERVO.INI", "SLAVE 0", "ITEM1");
            DspnZ = Inifiles.ReadValue(appPath + "/SERVO.INI", "SLAVE 1", "ITEM1");
            stdbX = Inifiles.ReadValue(appPath + "/SERVO.INI", "SLAVE 0", "ITEM0");
            stdbZ = Inifiles.ReadValue(appPath + "/SERVO.INI", "SLAVE 1", "ITEM0");
        }
        void ClearBtn()
        {
            btnAuto.Image = Auto_Attach.Properties.Resources.but_01auto_disable;
            btnManual.Image = Auto_Attach.Properties.Resources.but_02manual_disable;
            btnTeach.Image = Auto_Attach.Properties.Resources.but_04teach_disable;
            btnData.Image = Auto_Attach.Properties.Resources.but_03data_disable;
            btnExit.Image = Auto_Attach.Properties.Resources.but_07exit_disable;
        }
        void btnUnderBarIcon_Click(object sender, EventArgs e)
        {
            ClearBtn();
            PictureBox btn = sender as PictureBox;
            if (btn.Name.Contains("Auto"))
            {
                btnAuto.Image = Auto_Attach.Properties.Resources.but_01auto_select;
                Manual.SelectTab(0);
                loadTeaching();
                page = "Auto";
            }
            else if (btn.Name.Contains("Manual"))
            {
                btnManual.Image = Auto_Attach.Properties.Resources.but_02manual_select;
                Manual.SelectTab(1);
                page = "Manual";
                status = false;
                Position = "";
            }
            else if (btn.Name.Contains("Teach"))
            {
                btnTeach.Image = Auto_Attach.Properties.Resources.but_04teach_select;
                Manual.SelectTab(2);
                page = "Teach";
                status = false;
                Position = "";

            }
            else if (btn.Name.Contains("Data"))
            {
                btnData.Image = Auto_Attach.Properties.Resources.but_03data_select;
                Manual.SelectTab(3);
                page = "Data";
                status = false;
                Position = "";
            }

            else
            {

                Thread.Sleep(300);
                btnExit.Image = Auto_Attach.Properties.Resources.but_07exit_select;
                if (MessageBox.Show("Do you want to terminate this program?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    Log.AddLog("Program Ended...");
                    Log.AddPmLog("Program Ended...");
                    Log.SaveLog();
                    try
                    {
                        controller.CtrlServoPort.timelim.Abort();
                        timeDryRun.Abort();
                        // timelim.Abort();
                    }
                    catch { }
                    Application.Exit();
                }
                camera1.CloseCam();
                camera2.CloseCam();
                camera3.CloseCam();
                // camera4.CloseCam();
          

                int nRtn;
                nRtn = IO.FAS_ServoEnable(PortNo, 0, 0);
                Delay(100);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable Motor 0() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }


                /// Enable motor 1................................
                nRtn = IO.FAS_ServoEnable(PortNo, 1, 0);
                Delay(100);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable Motor 1() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                IO.FAS_Close(PortNo);
                Delay(100);
                BL.Close();

            }
        }

        public void DisplayLog(string log)
        {
            if (lbLog.InvokeRequired)
            {
                lbLog.Invoke(new MethodInvoker(() => { DisplayLog(log); }));
            }
            else
            {
                lbLog.Items.Add(log);
                lbLog.SelectedIndex = (lbLog.Items.Count - 1);
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            Form Mode = new frmMode();

            Mode.ShowDialog();
            string mchSettingsFilePath;
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            mchSettingsFilePath = exePath + mchSetFileName;


            controller = new Controller(this);
            controller.CtrlServoPort = new Servo();
            controller.CtrlIOPort = new IOboard();
            loadConfig();
            // controller.CtrlServoPort.ServoCon();


            //BL.LoadSettings(mchSettingsFilePath);
            BL.LoadSettings(mchSettingsFilePath);
            camera1.LoadSettings(mchSettingsFilePath);
            camera2.LoadSettings(mchSettingsFilePath);
            camera3.LoadSettings(mchSettingsFilePath);
          
            //camera4.LoadSettings(mchSettingsFilePath);
           
            UserIni();
            UpdateSerialPortList();
            DataGridViewInit();
            DataGridViewInitPos();
            Inspect.LoadSettings();




            //  controller.CtrlIOPort.Open(controller.CtrlIOPort.com, 115200);
            /*  if (controller.CtrlServoPort.statusServo)
              {
                  picServo.Image = Auto_Attach.Properties.Resources.bitmap51;
                  RunDryRun();
              }
              else { picServo.Image = Auto_Attach.Properties.Resources.bitmap31; }
              */
            //controller.CreateDevices();
            //controller.IOInitiate();

            string title = "Auto Attach Label Seal  ( Build : 2017-12-20 13:31:50)";
            Log log = new Log(this, Path.GetDirectoryName(Application.ExecutablePath) + @"\Log", "Auto Attach Label Seal");
            Log.AddLog(Environment.NewLine);
            Log.AddLog(Environment.NewLine);
            Log.AddLog(title);
            Log.AddPmLog(Environment.NewLine);
            Log.AddPmLog(Environment.NewLine);
            Log.AddPmLog(title);

            page = "Auto";
        }

        private void button9_Click(object sender, EventArgs e)
        {
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {

        }

        private void btnInfoSave_Click(object sender, EventArgs e)
        {
            // Inifiles.WriteValue(appPath + "/SETTING.INI", "SETTING", "I/O COM", txtIO.Text);
            // Inifiles.WriteValue(appPath + "/SETTING.INI", "SETTING", "SERVO COM", txtIO.Text);
            // Inifiles.WriteValue(appPath + "/SETTING.INI", "SETTING", "VELOCITY", txtVelocity.Text);
            // Inifiles.WriteValue(appPath + "/SETTING.INI", "SETTING", "DELAY ATTACH", txtDAttach.Text);
            // Inifiles.WriteValue(appPath + "/SETTING.INI", "SETTING", "DELAY PUNCH", txtDPunch.Text);
        }

        #region Bagian Theaching
        private void btnJogP_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (controller.CtrlServoPort.statusServo)
                        {
                            controller.CtrlServoPort.Jogmove(lblservo.Text, 1);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void btnJogP_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (controller.CtrlServoPort.statusServo)
                        {
                            controller.CtrlServoPort.JogStop(lblservo.Text);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void btnJogM_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (controller.CtrlServoPort.statusServo)
                        {
                            controller.CtrlServoPort.Jogmove(lblservo.Text, 0);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void btnJogM_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        if (controller.CtrlServoPort.statusServo)
                        {
                            controller.CtrlServoPort.JogStop(lblservo.Text);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void btnCvyX_Click(object sender, EventArgs e)
        {

        }

        private void btncvyZ_Click(object sender, EventArgs e)
        {

        }

        private void btndspnX_Click(object sender, EventArgs e)
        {

        }

        private void btndspnZ_Click(object sender, EventArgs e)
        {

        }

        private void btnStdbX_Click(object sender, EventArgs e)
        {

        }

        private void btnStdbZ_Click(object sender, EventArgs e)
        {

        }

        private void btnOrg_Click(object sender, EventArgs e)
        {

        }

        private void btnServoOn_Click(object sender, EventArgs e)
        {

        }

        private void btnResetAlarm_Click(object sender, EventArgs e)
        {

        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            //IO.FAS_PosTableRunItem(byte.Parse(com.Substring(3, com.Length - 3)), byte.Parse(SlaveNo), uint.Parse(PosItem));

            bool statusServo = true;
            byte iSlaveNo;
            iSlaveNo = byte.Parse(textSlaveNo.Text);
            TablePosStep = TablePosStep + 1;
            if (statusServo)
            {
                int nRtn;
                // nRtn = IO.FAS_SetIOInput(m_nPortNo, iSlaveNo, 0, 0);
                // nRtn = IO.FAS_PosTableRunItem(m_nPortNo, iSlaveNo,1);   //FAS_PosTableSingleRunItem
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, iSlaveNo, true, TablePosStep);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_MoveOriginSingleAxis() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }

        }

        private void btnGet_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void ClearTeaching()
        {

        }

        public void realtime()
        {

        }

        private void timerOn()
        {

        }

        Thread timeDryRun;
        private void RunDryRun()
        {

        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {

        }

        #region Manual ..
        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void btnManCvyrX_Click(object sender, EventArgs e)
        {

        }

        private void btnManStbyX_Click(object sender, EventArgs e)
        {

        }

        private void btnManCvyrZ_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnJogM_Click(object sender, EventArgs e)
        {

        }

        private void gbConfig_Enter(object sender, EventArgs e)
        {

        }

        private void btnXaxis_Click(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {

            if (comboBoxPortNo.Text.Length <= 0)
            {
                comboBoxPortNo.Focus();
                return;
            }

            if (m_bConnected == false)
            {
                uint dwBaud;

                m_nPortNo = byte.Parse(comboBoxPortNo.Text);
                dwBaud = uint.Parse(comboBaudrate.Text);

                if (IO.FAS_Connect(m_nPortNo, dwBaud) == 0)
                {
                    // Failed to connect
                    MessageBox.Show("Failed to connect");
                }
                else
                {
                    // connected.
                    m_bConnected = true;

                    buttonConnect.Text = "Disconnect";
                    comboBoxPortNo.Enabled = false;
                    comboBaudrate.Enabled = false;
                    textSlaveNo.Items.Clear();
                    cbSlaveTeach.Items.Clear();
                    for (byte i = 0; i < IO.MAX_SLAVE_NUMS; i++)
                    {
                        if (IO.FAS_IsSlaveExist(m_nPortNo, i) != 0)
                        {
                            textSlaveNo.Items.Add(i);// = i.ToString();
                            cbSlaveTeach.Items.Add(i);
                            //break;
                        }
                    }
                }
            }
            else
            {
                IO.FAS_Close(m_nPortNo);
                m_bConnected = false;

                buttonConnect.Text = "Connect";
                comboBoxPortNo.Enabled = true;
                comboBaudrate.Enabled = true;
            }
        }
        private void UpdateSerialPortList()
        {
            comboBoxPortNo.Items.Clear();

            // Port No.
            string[] portlist = SerialPort.GetPortNames();

            List<int> PortNoList = new List<int>();

            foreach (string port in portlist)
                PortNoList.Add(int.Parse(port.Substring(3)));

            PortNoList.Sort();

            foreach (int portno in PortNoList)
                comboBoxPortNo.Items.Add(string.Format("{0}", portno));
        }
        private void LoadingSlaveInfomation(byte nPortNo, byte iSlaveNo, ref uint Input, ref uint uLatch)
        {
            int nRtn;
            uint Result;
            nRtn = IO.FAS_GetIOInput(nPortNo, iSlaveNo, out Result);
            nRtn = IO.FAS_SetIOInput(nPortNo, iSlaveNo, 0, 0);
        }

        private void buttonMoveDEC_Click(object sender, EventArgs e)
        {

            byte iSlaveNo;
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

            nRtn = IO.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, lPosition * -1, lVelocity);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonMoveINC_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

            nRtn = IO.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, lPosition, lVelocity);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonMoveABS_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

            nRtn = IO.FAS_MoveSingleAxisAbsPos(m_nPortNo, iSlaveNo, lPosition, lVelocity);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisAbsPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonMoveAllINC_Click(object sender, EventArgs e)
        {
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

            nRtn = IO.FAS_AllMoveSingleAxisIncPos(m_nPortNo, lPosition, lVelocity);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_AllMoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonMoveAllABS_Click(object sender, EventArgs e)
        {
            int nRtn;
            int lPosition;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            lPosition = int.Parse(textPosition.Text);
            lVelocity = uint.Parse(textSpeed.Text);

            nRtn = IO.FAS_AllMoveSingleAxisAbsPos(m_nPortNo, lPosition, lVelocity);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_AllMoveSingleAxisAbsPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonJogPositive_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lVelocity = uint.Parse(textBoxJogSpd.Text);

            nRtn = IO.FAS_MoveVelocity(m_nPortNo, iSlaveNo, lVelocity, 1);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveVelocity() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonJogNegative_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lVelocity = uint.Parse(textBoxJogSpd.Text);

            nRtn = IO.FAS_MoveVelocity(m_nPortNo, iSlaveNo, lVelocity, 0);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveVelocity() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonSpdOverride_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;
            uint lVelocity;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            lVelocity = uint.Parse(textBoxJogSpd.Text);

            nRtn = IO.FAS_VelocityOverride(m_nPortNo, iSlaveNo, lVelocity);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_VelocityOverride() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonServoON_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            if (!m_ServoOn)
            {
                nRtn = IO.FAS_ServoEnable(m_nPortNo, iSlaveNo, 1);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                else
                {
                    buttonServoON.Text = "Servo Off";
                    buttonServoON.BackColor = Color.Red;
                    m_ServoOn = true;
                }

            }
            else
            {
                nRtn = IO.FAS_ServoEnable(m_nPortNo, iSlaveNo, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                else
                {
                    buttonServoON.Text = "Servo On";
                    buttonServoON.BackColor = Color.Blue;
                    m_ServoOn = false;
                }

            }

        }

        private void buttonSTOP_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            nRtn = IO.FAS_MoveStop(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveStop() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void buttonAlarmReset_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (textSlaveNo.Text.Length <= 0)
            {
                textSlaveNo.Focus();
                return;
            }

            iSlaveNo = byte.Parse(textSlaveNo.Text);

            nRtn = IO.FAS_ServoAlarmReset(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_ServoAlarmReset() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            bool statusServo = true;
            byte iSlaveNo;
            iSlaveNo = byte.Parse(textSlaveNo.Text);
            if (statusServo)
            {
                int nRtn;
                nRtn = IO.FAS_SetIOInput(m_nPortNo, iSlaveNo, 0, 0);
                nRtn = IO.FAS_MoveOriginSingleAxis(m_nPortNo, iSlaveNo);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_MoveOriginSingleAxis() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }
        }

        private void btnGetPos_Click_1(object sender, EventArgs e)
        {
            bool statusServo = true;
            byte iSlaveNo;
            ulong cmdPos;

            if ((statusServo) && (textSlaveNo.Text != ""))
            {
                int nRtn;
                iSlaveNo = byte.Parse(textSlaveNo.Text);
                //  command = IO.FAS_SetIOInput(m_nPortNo, iSlaveNo, 0, 0);
                nRtn = IO.FAS_GetActualPos(m_nPortNo, iSlaveNo, out cmdPos);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetActualPos() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                txtGetPos.Text = cmdPos.ToString();
            }
        }

        private void btnLimitMin_Click(object sender, EventArgs e)
        {
            bool statusServo = true;
            byte iSlaveNo;
            iSlaveNo = byte.Parse(textSlaveNo.Text);
            if (statusServo)
            {
                int nRtn;
                //  nRtn = IO.FAS_MoveOriginSingleAxis(m_nPortNo, iSlaveNo);
                nRtn = IO.FAS_MoveToLimit(m_nPortNo, iSlaveNo, 10000, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_MoveToLimit() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }
        }

        private void btnLimitMax_Click(object sender, EventArgs e)
        {
            bool statusServo = true;
            byte iSlaveNo;
            iSlaveNo = byte.Parse(textSlaveNo.Text);
            if (statusServo)
            {
                int nRtn;
                //  nRtn = IO.FAS_MoveOriginSingleAxis(m_nPortNo, iSlaveNo);
                nRtn = IO.FAS_MoveToLimit(m_nPortNo, iSlaveNo, 10000, 1);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_MoveToLimit() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }
        }

        private void btnmove_Click(object sender, EventArgs e)
        {

        }

        private void textSlaveNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* string strMsg;
             if (m_ServoOn == true)
             {
                 strMsg = "Servo On() \nReturned: ";
                 MessageBox.Show(strMsg, "Please turn off Servo ");
             }
             */
        }

        private void btnUserIn0_Click(object sender, EventArgs e)
        {
            int nRtn;
            uint Result;
            bool statusServo = true;
            string strResult;
            byte iSlaveNo;
            iSlaveNo = byte.Parse(textSlaveNo.Text);

            if (statusServo)
            {

                nRtn = IO.FAS_GetIOInput(m_nPortNo, iSlaveNo, out Result);
                var bin = Convert.ToString(Result, 2);
                if (bin != "")
                {
                    if (bin.Length > 0)
                    {
                        strResult = bin.Substring(0, 1);
                        if (strResult == "1")
                        {
                            btnUserIn0.BackColor = Color.Blue;
                        }
                        else
                        {
                            btnUserIn0.BackColor = Color.White;
                        }

                    }
                }
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetIOInput() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }
        }

        private void btnTeachOrigin_Click(object sender, EventArgs e)
        {
            bool statusServo = true;
            byte iSlaveNo;

            if ((statusServo) && (textSlaveNo.Text != ""))
            {
                int nRtn;
                iSlaveNo = byte.Parse(textSlaveNo.Text);
                nRtn = IO.FAS_SetIOInput(m_nPortNo, iSlaveNo, 0, 0);
                nRtn = IO.FAS_MoveOriginSingleAxis(m_nPortNo, iSlaveNo);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_MoveOriginSingleAxis() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }
        }

        private void btnServoOnTeach_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);

            if (!m_ServoOn)
            {
                nRtn = IO.FAS_ServoEnable(m_nPortNo, iSlaveNo, 1);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                else
                {
                    btnServoOnTeach.Text = "Servo Off";
                    btnServoOnTeach.BackColor = Color.Red;
                    m_ServoOn = true;
                }

            }
            else
            {
                nRtn = IO.FAS_ServoEnable(m_nPortNo, iSlaveNo, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_ServoEnable() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
                else
                {
                    btnServoOnTeach.Text = "Servo On";
                    btnServoOnTeach.BackColor = Color.Blue;
                    m_ServoOn = false;
                }

            }

        }

        private void btnStopTeach_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);

            nRtn = IO.FAS_MoveStop(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveStop() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void btnAlarmTeach_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);

            nRtn = IO.FAS_ServoAlarmReset(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_ServoAlarmReset() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void btnBeginTeach_Click(object sender, EventArgs e)
        {
            TablePosStep = 0;
            if (cbSlaveTeach.Text == "0")
                iStepPosServo0 = 0;
            else if (cbSlaveTeach.Text == "1")
                iStepPosServo1 = 0;
        }

        private void btnMinTeach_Click(object sender, EventArgs e)
        {

            if (cbSlaveTeach.Text == "0")
            {
                if (iStepPosServo0 > 0)
                    iStepPosServo0 = iStepPosServo0 - 1;
            }


            else if (cbSlaveTeach.Text == "1")
            {
                if (iStepPosServo1 > 0)
                    iStepPosServo1 = iStepPosServo1 - 1;
            }


        }

        private void btnPlusTeach_Click(object sender, EventArgs e)
        {
            if (cbSlaveTeach.Text == "0")
            {

                iStepPosServo0 = iStepPosServo0 + 1;
            }


            else if (cbSlaveTeach.Text == "1")
            {

                iStepPosServo1 = iStepPosServo1 + 1;
            }

        }

        private void btnJOGLow_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn = 1;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);



            nRtn = IO.FAS_MoveStop(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveStop() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
            Thread.Sleep(500);
            if (txtMoveSpeed.Text != "")
                nRtn = IO.FAS_MoveVelocity(m_nPortNo, iSlaveNo, Convert.ToUInt32(txtMoveSpeed.Text), 1);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveVelocity() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void btnJOGHigh_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn = 1;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);

            nRtn = IO.FAS_MoveStop(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveStop() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
            Thread.Sleep(500);

            if (txtMoveSpeed.Text != "")
                nRtn = IO.FAS_MoveVelocity(m_nPortNo, iSlaveNo, Convert.ToUInt32(txtMoveSpeed.Text), 0);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveVelocity() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void btnDecTeach_Click(object sender, EventArgs e)
        {

            byte iSlaveNo;
            int nRtn = 1;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);




            if (txtMoveSpeed.Text != "")
                nRtn = IO.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, Convert.ToInt32(txtOrderSpeed.Text) * -1, Convert.ToUInt32(txtMoveSpeed.Text));
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void btnIncTeach_Click(object sender, EventArgs e)
        {
            byte iSlaveNo;
            int nRtn = 1;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);




            if (txtMoveSpeed.Text != "")
                nRtn = IO.FAS_MoveSingleAxisIncPos(m_nPortNo, iSlaveNo, Convert.ToInt32(txtOrderSpeed.Text), Convert.ToUInt32(txtMoveSpeed.Text));
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {

            byte iSlaveNo;
            int nRtn = 1;

            if (m_bConnected == false)
                return;

            if (cbSlaveTeach.Text.Length <= 0)
            {
                cbSlaveTeach.Focus();
                return;
            }

            iSlaveNo = byte.Parse(cbSlaveTeach.Text);




            if (txtMoveSpeed.Text != "")
                nRtn = IO.FAS_ClearPosition(m_nPortNo, iSlaveNo);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }
        }
        private void LoadingDataGridView()
        {
            int Row;
            Row = dataGridView.Rows.Count;
            int intDataColumn = 8;   // 8 colums created
            int intDataRow = 10;     // 3 rowws created


            string[] strDataColumn = { "Notch", "Cam1", "Cam2", "Cam3", "Cam4", "Cam5", "Cam6" };

            dataGridView.ForeColor = Color.Gray;

            for (int i = 0; i < intDataColumn - 1; i++)
            {
                dataGridView.Columns.Add("Column", strDataColumn[i]);
            }


        }
        private void DataGridViewInit()
        {
            dbViewSpec.GridColor = Color.Red;

            for (int i = 1; i < 11; i++)
            {
                dbViewSpec.Columns.Add("", "Pt" + i);
            }
            for (int i = 1; i < 11; i++)
            {
                dbViewData.Columns.Add("", "Pt" + i);
            }


            dbViewSpec.Rows.Add(dHiSpec[1], dHiSpec[2], dHiSpec[3], dHiSpec[4], dHiSpec[5], dHiSpec[6], dHiSpec[7], dHiSpec[8], dHiSpec[9], dHiSpec[10]);//, dHiSpec[11], dHiSpec[12], dHiSpec[13], dHiSpec[15], dHiSpec[15]);
            dbViewSpec.Rows.Add(dLowSpec[1], dLowSpec[2], dLowSpec[3], dLowSpec[4], dLowSpec[5], dLowSpec[6], dLowSpec[7], dLowSpec[8], dLowSpec[9], dLowSpec[10]);//, dLowSpec[11], dLowSpec[12], dLowSpec[13], dLowSpec[14], dLowSpec[15]);
        }

        private void DataGridViewInitPos()
        {
            int intDataColumn = 7;   // 8 colums created
            int intDataRow = 10;     // 3 rowws created

            // Create HeaderFile for datagrid
            // string[] strDataColumn = { "LOGO1 Width", "LOGO1 Lenght", "LOGO1_X", "LOGO1_Y", "LOGO2 Lenght", "LOGO2 Width", "LOGO2_X", "LOGO2_Y" };
            string[] strDataColumn = { "Item", "Position", "HighSpeed", "Acc", "Dcc", "Wait" };

            dataGridView.ForeColor = Color.Gray;

            for (int i = 0; i < intDataColumn - 1; i++)
            {
                dataGridView.Columns.Add("Column", strDataColumn[i]);
            }

        }


        private void btnSavePos_Click(object sender, EventArgs e)
        {
            if (cbSlaveTeach.Text == "0")
            {
                try
                {
                    int command;
                    command = IO.FAS_PosTableWriteOneItem(m_nPortNo, 0, uint.Parse(lblStepPosTab.Text), 0, long.Parse(txtActPosTeach.Text));
                    command = IO.FAS_PosTableWriteROM(m_nPortNo, 0);
                }
                catch { }
            }
            else if (cbSlaveTeach.Text == "1")
            {
                int command;
                command = IO.FAS_PosTableWriteOneItem(m_nPortNo, 1, uint.Parse(lblStepPosTab.Text), 0, long.Parse(txtActPosTeach.Text));
                command = IO.FAS_PosTableWriteROM(m_nPortNo, 1);
            }
        }
        public void SaveRawData(string path, string Model, String Buyer, string Data)
        {
            //lock (sny_Obj)
            int intDataPrint;
            {
                string s_FileName = Model + "_" + Buyer + DateTime.Now.ToString("yyyyMMdd");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string FileName = path + "\\" + s_FileName + ".csv";

                FileStream objFileStream;
                bool bCreatedNew = false;

                if (!File.Exists(FileName))
                {
                    objFileStream = new FileStream(FileName, FileMode.CreateNew, FileAccess.Write);
                    bCreatedNew = true;
                }
                else
                {
                    objFileStream = new FileStream(FileName, FileMode.Append, FileAccess.Write);
                }
                StreamWriter sw = new StreamWriter(objFileStream, System.Text.Encoding.GetEncoding(-0));
                string columnTitle = "";
                int i;
                string columnValue = "";


                try
                {
                    //bCreatedNew = true;
                    if (bCreatedNew)
                    {


                        columnTitle = "Model:" + txtModel.Text + "," + "Buyer:" + "Open" + ",";
                        sw.WriteLine(columnTitle);
                        columnTitle = "";
                        columnTitle = "Number,HiSpec,"+ dHiSpec[1]+","+ dHiSpec[2] + "," + dHiSpec[3] + "," + dHiSpec[4] + "," + dHiSpec[5] + "," + dHiSpec[6] + "," + dHiSpec[7] + "," + dHiSpec[8] + "," + dHiSpec[9] + "," + dHiSpec[10];
                        sw.WriteLine(columnTitle);
                        columnTitle = "";
                        columnTitle = "Number,LowSpec," + dLowSpec[1] + "," + dLowSpec[2] + "," + dLowSpec[3] + "," + dLowSpec[4] + "," + dLowSpec[5] + "," + dLowSpec[6] + "," + dLowSpec[7] + "," + dLowSpec[8] + "," + dLowSpec[9] + "," + dLowSpec[10];
                        sw.WriteLine(columnTitle);
                        columnTitle = "";
                        columnTitle = "Number,Rolls,P1,P2, P3, P4, P5, P6, P7,P8,P9,P10,Judgment,LowSpec,HiSpec,Date";
                        sw.WriteLine(columnTitle);
                        columnTitle = "";
                        intDataPrint = 4;

                        sw.WriteLine(Data);
                    }




                    else
                    {

                        sw.WriteLine(Data);

                    }

                    sw.Close();
                    objFileStream.Close();
                }
                catch (Exception)
                {
                    //MessageBox.Show(e.ToString());
                }
                finally
                {
                    sw.Close();
                    objFileStream.Close();
                }
            }
        }


        private void btnGrapCam1_Click(object sender, EventArgs e)
        {
            btnGrapCam1.Enabled = false;
            camera1.StartGrabbing();
            while (!camera1.bGrabDone)
                Thread.Sleep(10);
            picCam1.Image = camera1.ImageBuffer;
            camera1.ImageBuffer = null;
            camera1.bGrabDone = false;
            btnGrapCam1.Enabled = true;
        }

        private void btnGrapCam2_Click(object sender, EventArgs e)
        {
            btnGrapCam2.Enabled = false;
            camera2.StartGrabbing();
            while (!camera2.bGrabDone)
                Thread.Sleep(10);
            picCam2.Image = camera2.ImageBuffer;
            camera2.ImageBuffer = null;
            camera2.bGrabDone = false;
            btnGrapCam2.Enabled = true;
        }
        public void Grapping(BaslerCam camera, int Delay)
        {

        }

        private void btnGrapCam3_Click(object sender, EventArgs e)
        {
            btnGrapCam3.Enabled = false;
            camera3.StartGrabbing();
            while (!camera3.bGrabDone)
                Thread.Sleep(10);
            picCam3.Image = camera3.ImageBuffer;
            camera3.ImageBuffer = null;
            camera3.bGrabDone = false;
            btnGrapCam3.Enabled = true;
        }

        private void btnGrapCam4_Click(object sender, EventArgs e)
        {
            btnGrapCam4.Enabled = false;
            camera4.StartGrabbing();
            while (!camera4.bGrabDone)
                Thread.Sleep(10);
            picCam4.Image = camera4.ImageBuffer;
            camera4.ImageBuffer = null;
            camera4.bGrabDone = false;
            btnGrapCam4.Enabled = true;
        }

        private void btnGetCamInfo_Click(object sender, EventArgs e)
        {
            txtSetThreLowCam1.Text = Convert.ToString(camera1.ThreCamLowSet);
            txtSetThreLowCam2.Text = Convert.ToString(camera2.ThreCamLowSet);
            txtSetThreLowCam3.Text = Convert.ToString(camera3.ThreCamLowSet);
            txtSetThreLowCam4.Text = Convert.ToString(camera4.ThreCamLowSet);

            txtSetThreHiCam1.Text = Convert.ToString(camera1.ThreCamHighSet);
            txtSetThreHiCam2.Text = Convert.ToString(camera2.ThreCamHighSet);
            txtSetThreHiCam3.Text = Convert.ToString(camera3.ThreCamHighSet);
            txtSetThreHiCam4.Text = Convert.ToString(camera4.ThreCamHighSet);

            txtSetExpCam1.Text = Convert.ToString(camera1.ExpCamTimeSet);
            txtSetExpCam2.Text = Convert.ToString(camera2.ExpCamTimeSet);
            txtSetExpCam3.Text = Convert.ToString(camera3.ExpCamTimeSet);
            txtSetExpCam4.Text = Convert.ToString(camera4.ExpCamTimeSet);
        }

        private void btnSetExpCam1_Click(object sender, EventArgs e)
        {


            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetExpCam1.Text != "")
            {
                camera1.SetExposureTime(Convert.ToInt32(txtSetExpCam1.Text));
                camera1.ExpCamTimeSet = Convert.ToInt32(txtSetExpCam1.Text);
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module1", "Exposure", txtSetExpCam1.Text);
            }
        }

        private void btnSetExpCam2_Click(object sender, EventArgs e)
        {


            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetExpCam2.Text != "")
            {
                camera2.ExpCamTimeSet = Convert.ToInt32(txtSetExpCam2.Text);
                camera2.SetExposureTime(Convert.ToInt32(txtSetExpCam2.Text));
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module2", "Exposure", txtSetExpCam2.Text);
            }
        }

        private void btnSetExpCam3_Click(object sender, EventArgs e)
        {


            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetExpCam3.Text != "")
            {
                camera3.ExpCamTimeSet = Convert.ToInt32(txtSetExpCam3.Text);
                camera3.SetExposureTime(Convert.ToInt32(txtSetExpCam3.Text));
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module3", "Exposure", txtSetExpCam3.Text);
            }

        }

        private void btnSetExpCam4_Click(object sender, EventArgs e)
        {

            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetExpCam4.Text != "")
            {
                camera4.ExpCamTimeSet = Convert.ToInt32(txtSetExpCam4.Text);
                camera4.SetExposureTime(Convert.ToInt32(txtSetExpCam4.Text));
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module4", "Exposure", txtSetExpCam4.Text);
            }
        }

        private void btnSetThreLowCam1_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreLowCam1.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module1", "LowThreshole", txtSetThreLowCam1.Text);
                camera1.ThreCamLowSet = Convert.ToInt16(txtSetThreLowCam1.Text);
            }

        }

        private void btnSetThreHiCam1_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreHiCam1.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module1", "HighThrehole", txtSetThreHiCam1.Text);
                camera1.ThreCamHighSet = Convert.ToInt16(txtSetThreHiCam1.Text);
            }

        }

        private void btnSetThreLowCam2_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreLowCam2.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module2", "LowThreshole", txtSetThreLowCam2.Text);
                camera2.ThreCamLowSet = Convert.ToInt16(txtSetThreLowCam2.Text);
            }

        }

        private void btnSetThreHiCam2_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreHiCam2.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module2", "HighThrehole", txtSetThreHiCam2.Text);
                camera2.ThreCamHighSet = Convert.ToInt16(txtSetThreHiCam2.Text);
            }
        }

        private void btnSetThreLowCam3_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreLowCam3.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module3", "LowThreshole", txtSetThreLowCam3.Text);
                camera3.ThreCamLowSet = Convert.ToInt16(txtSetThreLowCam3.Text);
            }
        }

        private void btnSetThreHiCam3_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreHiCam3.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module3", "HighThrehole", txtSetThreHiCam3.Text);
                camera3.ThreCamHighSet = Convert.ToInt16(txtSetThreHiCam3.Text);
            }
        }

        private void btnSetThreLowCam4_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreLowCam4.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module4", "LowThreshole", txtSetThreLowCam4.Text);
                camera4.ThreCamLowSet = Convert.ToInt16(txtSetThreLowCam4.Text);
            }
        }

        private void btnSetThreHiCam4_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;
            if (txtSetThreHiCam4.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module4", "HighThrehole", txtSetThreHiCam4.Text);
                camera4.ThreCamHighSet = Convert.ToInt16(txtSetThreHiCam4.Text);
            }
        }

        private void btnManDspnZ_Click(object sender, EventArgs e)
        {

        }

        private void btnLoadPosTab_Click_1(object sender, EventArgs e)
        {
            byte iSlaveNo;
            ulong cmdPos;
            // byte iSlaveNo = 0;
            int nRtn;
            // Update information for datalistview
            if ((cbSlaveTeach.Text != "") && (m_ServoOn == true))
            {
                iSlaveNo = byte.Parse(cbSlaveTeach.Text);


                dataGridView.Rows.Clear();

                for (ushort i = 0; i < 10; i++)
                {
                    byte[] buffer = new byte[64];
                    // if (txtMoveSpeed.Text != "")
                    nRtn = IO.FAS_PosTableReadItem(m_nPortNo, iSlaveNo, i, buffer);


                    if (nRtn != IO.FMM_OK)
                    {
                        string strMsg;
                        strMsg = "FAS_MoveSingleAxisIncPos() \nReturned: " + nRtn.ToString();
                        MessageBox.Show(strMsg, "Function Failed");
                    }
                    else
                    {
                        EziTable = new EziMOTIONPlusRLib.ITEM_NODE(buffer);
                        dataGridView.Rows.Add(i, EziTable.lPosition, EziTable.dwMoveSpd, EziTable.wAccelTime, EziTable.wDecelTime, EziTable.wWaitTime);
                    }
                }
            }

        }

        private void btnManStbyZ_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int nRtn;


            if (cbSlaveTeach.Text == "0")
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, iStepPosServo0);
                //  nRtn =  IO.FAS_PosTableRunItem(m_nPortNo, 0, 0);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "Move_FAS_MoveOriginSingleAxis() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }
            }


            else if (cbSlaveTeach.Text == "1")
            {
                nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 1, false, iStepPosServo1);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "Move_FAS_MoveOriginSingleAxis() \nReturned: " + nRtn.ToString();
                    MessageBox.Show(strMsg, "Function Failed");
                }

            }
        }

        private void btnRoiLoad_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;

            if (cbROI2.Text != "" && cbROI.Text != "" && cbROI3.Text != "")
            {

                        // Loading info for ROI..........................................
                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "Width", ref strread);
                        if (strread != "0")
                        {
                            txtRoiWid.Text = strread;
                            Inspect.ArrROI[Convert.ToInt16(Convert.ToString(cbROI.Text) + Convert.ToString(cbROI2.Text) + Convert.ToString(cbROI3.Text))].Width = Convert.ToInt32(strread);
                        }

                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "Len", ref strread);
                        if (strread != "0")
                        {
                           Inspect.ArrROI[Convert.ToInt16(Convert.ToString(cbROI.Text) + Convert.ToString(cbROI2.Text) + Convert.ToString(cbROI3.Text))].Height = Convert.ToInt32(strread);
                            txtRoiLen.Text = strread;
                        }


                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "X", ref strread);
                        if (strread != "0")
                        {
                            Inspect.ArrROI[Convert.ToInt16(Convert.ToString(cbROI.Text) + Convert.ToString(cbROI2.Text) + Convert.ToString(cbROI3.Text))].X = Convert.ToInt32(strread);
                            txtRoiX.Text = strread;
                        }


                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "Y", ref strread);
                        if (strread != "0")
                        {
                            Inspect.ArrROI[Convert.ToInt16(Convert.ToString(cbROI.Text) + Convert.ToString(cbROI2.Text) + Convert.ToString(cbROI3.Text))].Y = Convert.ToInt32(strread);
                            txtRoiY.Text = strread;
                        }
            }
        }
        

        private void btnRoiTest_Click(object sender, EventArgs e)
        {
            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;
            if (cbROI2.Text != "" && cbROI.Text != "" && cbROI3.Text != "")
            {

                // Loading info for ROI..........................................
                m_image = new Mat(strHeadFile + "\\Point" + cbROI2.Text + "_" + cbROI.Text + ".bmp");
                Cv2.Rectangle(m_image, Inspect.ArrROI[Convert.ToInt16(Convert.ToString(cbROI.Text) + Convert.ToString(cbROI2.Text) + Convert.ToString(cbROI3.Text))], Scalar.Red, 5); // Draw Rectangles.......................
                picCam4.Image = m_image.ToBitmap();                 
                
            }    
        }

        private void btnSpecUpdate_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;

            if (cbSpec.Text != "")
            {

                FileOperation.ReadData(mchSettingsFilePath, "HiSpec", "Point" + Convert.ToInt16(cbSpec.Text), ref strread);
                txtHiSpec.Text = strread;
                dHiSpec[Convert.ToInt16(cbSpec.Text)] = Convert.ToDouble(strread);

                FileOperation.ReadData(mchSettingsFilePath, "LowSpec", "Point" + Convert.ToInt16(cbSpec.Text), ref strread);
                txtLowSpec.Text = strread;
                dLowSpec[Convert.ToInt16(cbSpec.Text)] = Convert.ToDouble(strread);

            }
        }

        private void btnSpecSave_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;


            if (cbSpec.Text != "")
            {
                if (txtHiSpec.Text != "")
                    FileOperation.SaveData(mchSettingsFilePath, "HiSpec", "Point" + Convert.ToInt16(cbSpec.Text), txtHiSpec.Text);

                if (txtLowSpec.Text != "")
                    FileOperation.SaveData(mchSettingsFilePath, "LowSpec", "Point" + Convert.ToInt16(cbSpec.Text), txtLowSpec.Text);
            }

        }

        private void btnLoadCali_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            Double dbData;
            mchSettingsFilePath = exePath + mchSetFileName;

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;
            m_image = new Mat(strHeadFile + "\\Hubble.bmp");
            pMap.Image = m_image.ToBitmap();

            if (cbCalCam.Text != "" && cbCalPos.Text != "" && cbCalPt.Text != "")
            {
                m_image = new Mat(strHeadFile + "\\Image/image0" + cbCalCam.Text + cbCalPos.Text+ cbCalPt.Text + ".bmp");
                pPoint1.Image = m_image.ToBitmap();
            }

            if (cbCali.Text != "")
            {

                FileOperation.ReadData(mchSettingsFilePath, "Cam_Module"+ cbCalCam.Text, "CurrentData_"+ cbCali.Text, ref strread);
                txtPixel.Text = strread;

            }
        }

        private void btnSaveCali_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;

            if (cbCali.Text != "")
            {
                if (cbCali.Text != "")
                {
                    if (cbCali.Text == "X")
                    {
                        camera1.CaliValue.X = Convert.ToDouble(txtInput.Text) / Convert.ToInt32(txtPixel.Text);
                        FileOperation.SaveData(mchSettingsFilePath, "Cam_Module" + cbCalCam.Text, "Calibration" + cbCali.Text, Convert.ToString(camera1.CaliValue.X));
                    }
                    else
                    {
                        camera1.CaliValue.Y= Convert.ToDouble(txtInput.Text) / Convert.ToInt32(txtPixel.Text);
                        FileOperation.SaveData(mchSettingsFilePath, "Cam_Module" + cbCalCam.Text, "Calibration" + cbCali.Text, Convert.ToString(camera1.CaliValue.Y));
                    }
                      
                    
                }
               // btnSaveCali.Visible = false;
                btnSaveCali.Enabled = false;

            }
        }

        private void btnLoadLight_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;


            if (cbLightCam.Text != "" && cbLightPos.Text != "" && cbLightPt.Text != "")
            {


                FileOperation.ReadData(mchSettingsFilePath, "LowThreshole", "Point" + cbLightCam.Text +  cbLightPos.Text + cbLightPt.Text, ref strread);
                iThreHoleLow[Convert.ToInt16(Convert.ToString(cbLightCam.Text) + Convert.ToString(cbLightPos.Text) + Convert.ToString(cbLightPt.Text))] = Convert.ToInt32(strread);
                txtThreLowLight.Text = strread;


                FileOperation.ReadData(mchSettingsFilePath, "HiThreshole", "Point" + cbLightCam.Text + cbLightPos.Text + cbLightPt.Text, ref strread);
                iThreHoleHi[Convert.ToInt16(Convert.ToString(cbLightCam.Text) + Convert.ToString(cbLightPos.Text) + Convert.ToString(cbLightPt.Text))] = Convert.ToInt32(strread);
                txtThreHiLight.Text = strread;


            }
        }

        private void btnSaveLight_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;
            if (cbLightCam.Text != "" && cbLightPos.Text != "" && cbLightPt.Text != "")
            {
                if (txtThreLowLight.Text != "")
                {
                    FileOperation.SaveData(mchSettingsFilePath, "LowThreshole", "Point" + Convert.ToInt32(cbLightCam.Text)+ Convert.ToInt32(cbLightPos.Text)+Convert.ToInt32(cbLightPt.Text), txtThreLowLight.Text);
                }

                if (txtThreHiLight.Text != "")
                {
                    FileOperation.SaveData(mchSettingsFilePath, "HiThreshole", "Point" + Convert.ToInt32(cbLightCam.Text) + Convert.ToInt32(cbLightPos.Text) + Convert.ToInt32(cbLightPt.Text), txtThreHiLight.Text);
                }
            }
             


        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int access = User.GetAccess(userCB.SelectedItem.ToString(), textBoxPassword.Text);
            User_Set(access);
            btnLogIn.Enabled = false;
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            btnLogIn.Enabled = true;
            User_Set(0);
        }

        private void btnSaveCOM_Click_2(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;

            if (comboBoxPortNo.Text != "" && comboBaudrate.Text != "")
            {

                FileOperation.SaveData(mchSettingsFilePath, "ComPort", "PortNo", comboBoxPortNo.Text);
                FileOperation.SaveData(mchSettingsFilePath, "ComPort", "Baud", comboBaudrate.Text);

            }
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }

        private void btnSaveCom_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            string strread = "";
            mchSettingsFilePath = exePath + mchSetFileName;

            if (comboBoxPortNo.Text != "" && comboBaudrate.Text != "")
            {

                FileOperation.SaveData(mchSettingsFilePath, "ComPort", "PortNo", comboBoxPortNo.Text);
                FileOperation.SaveData(mchSettingsFilePath, "ComPort", "Baud", comboBaudrate.Text);

            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Reset_Click(object sender, EventArgs e)
        {
            bnStart1 = true;
        }

        private void btnLEDTest_Click(object sender, EventArgs e)
        {
            string sendtext = "";
            if (cbLED_No.Text != "" && txtLEDValue.Text != "")
            {
                sendtext = "02 3" + cbLED_No.Text + " 77 30 3" + txtLEDValue.Text.Substring(0, 1) + " 3" + txtLEDValue.Text.Substring(1, 1) + " 3" + txtLEDValue.Text.Substring(2, 1) + " 03";
            }
           BL.send(sendtext);
        }

        private void btnLEDSave_Click(object sender, EventArgs e)
        {
            string sendtext;
            string data;
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;

            if (cbLED_No.Text != "" && txtLEDValue.Text != "")
            {
                data = cbLED_No.Text;
                sendtext = "02 3" + cbLED_No.Text + " 77 30 3" + txtLEDValue.Text.Substring(0, 1) + " 3" + txtLEDValue.Text.Substring(1, 1) + " 3" + txtLEDValue.Text.Substring(2, 1) + " 03";

                if(data == "1")
                    FileOperation.SaveData(mchSettingsFilePath, "BackLight", "LED1", sendtext);
                else if (data == "2")
                    FileOperation.SaveData(mchSettingsFilePath, "BackLight", "LED2", sendtext);
                else if (data == "3")
                    FileOperation.SaveData(mchSettingsFilePath, "BackLight", "LED3", sendtext);
            }
        }

        private void btnCali_Click(object sender, EventArgs e)
        {
            btnSaveCali.Enabled = true;
        }

        private void label58_Click(object sender, EventArgs e)
        {

        }

        private void groupBox20_Enter(object sender, EventArgs e)
        {

        }

        private void btnSaveCamName_Click(object sender, EventArgs e)
        {

            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;

            if (cbROI2.Text != "" && cbROI.Text != "" && cbROI3.Text != "")
            {
                if (txtRoiWid.Text != "")
                    FileOperation.SaveData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "Width", txtRoiWid.Text);
                if (txtRoiLen.Text != "")
                    FileOperation.SaveData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "Len", txtRoiLen.Text);
                if (txtRoiX.Text != "")
                    FileOperation.SaveData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "X", txtRoiX.Text);
                if (txtRoiY.Text != "")
                    FileOperation.SaveData(mchSettingsFilePath, "ROI" + cbROI.Text + cbROI2.Text + cbROI3.Text, "Y", txtRoiY.Text);
            }

        }

        private void BtnSetDevice1_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;

            if (txtSetCam1.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module1", "CameraID", txtSetCam1.Text);
            }

        }

        private void BtnSetDevice2_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;

            if (txtSetCam2.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module2", "CameraID", txtSetCam2.Text);
            }
        }

        private void BtnSetDevice3_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;

            if (txtSetCam3.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module3", "CameraID", txtSetCam3.Text);
            }
        }

        private void BtnSetDevice4_Click(object sender, EventArgs e)
        {
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string mchSettingsFilePath;
            mchSettingsFilePath = exePath + mchSetFileName;

            if (txtSetCam4.Text != "")
            {
                FileOperation.SaveData(mchSettingsFilePath, "Cam_Module4", "CameraID", txtSetCam4.Text);
            }
        }

        private void btnManVacum_Click(object sender, EventArgs e)
        {

        }

        private void btnManSelP_Click(object sender, EventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// make beat in program......................................
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {

            tmrMainloop.Enabled = false;

            flgMainLoop = !flgMainLoop;
            WorkStatus = true;
            //if (flgMainLoop == true && blnCaliFlag == false )
            if (flgMainLoop == true)
            {
                lblMainloop.BackColor = Color.Yellow;
                // lblMainloop.Text = " Checking...";
            }
            else
            {
                lblMainloop.BackColor = Color.Blue;
                // lblMainloop.Text = " Finished ";
            }

            MainProcess();


            // if (!blnCaliFlag)
            WorkStatus = false;
            tmrMainloop.Enabled = true;

        }

        private void btnReset_Click(object sender, EventArgs e)
        {

        }
        public void Delay(int iDelay)
        {
            Thread.Sleep(iDelay);
        }
        // user access...............................................
        public void User_Set(int UsrLevel)
        {
            switch (UsrLevel)
            {
                case 0:
                    buttonServoON.Enabled = false;
                    buttonSTOP.Enabled = false;
                    buttonAlarmReset.Enabled = false;

                    buttonJogPositive.Enabled = false;
                    buttonJogNegative.Enabled = false;
                    //buttonConnect.Enabled = false;
                    textSlaveNo.Enabled = false;
                    cbSlaveTeach.Enabled = false;

                    // disable spec table
                    btnBeginTeach.Enabled = false;
                    btnSavePos.Enabled = false;

                    btnMoveStepTeach.Enabled = false;


                    btnSetExpCam1.Enabled = false;



                    btnSetExpCam1.Enabled = false;
                    btnSetExpCam2.Enabled = false;
                    btnSetExpCam3.Enabled = false;
                    btnSetExpCam4.Enabled = false;

                    btnRoiSave.Enabled = false;
                    btnSpecSave.Enabled = false;
                    btnSaveCali.Enabled = false;
                    btnSaveLight.Enabled = false;







                    break;
                case 1:
                    buttonServoON.Enabled = false;
                    buttonSTOP.Enabled = false;
                    buttonAlarmReset.Enabled = false;

                    buttonJogPositive.Enabled = false;
                    buttonJogNegative.Enabled = false;
                    //buttonConnect.Enabled = false;
                    textSlaveNo.Enabled = false;
                    cbSlaveTeach.Enabled = false;

                    // disable spec table
                    btnBeginTeach.Enabled = false;
                    btnSavePos.Enabled = false;

                    btnMoveStepTeach.Enabled = false;


                    btnSetExpCam1.Enabled = false;



                    btnSetExpCam1.Enabled = false;
                    btnSetExpCam2.Enabled = false;
                    btnSetExpCam3.Enabled = false;
                    btnSetExpCam4.Enabled = false;

                    btnRoiSave.Enabled = false;
                    btnSpecSave.Enabled = true;
                    btnSaveCali.Enabled = true;
                    btnSaveLight.Enabled = false;

                    break;
                case 2:
                    buttonServoON.Enabled = true;
                    buttonSTOP.Enabled = true;
                    buttonAlarmReset.Enabled = true;

                    buttonJogPositive.Enabled = true;
                    buttonJogNegative.Enabled = true;
                    buttonConnect.Enabled = true;
                    textSlaveNo.Enabled = true;
                    cbSlaveTeach.Enabled = true;

                    // disable spec table
                    btnBeginTeach.Enabled = true;
                    btnSavePos.Enabled = true;

                    btnMoveStepTeach.Enabled = true;


                    btnSetExpCam1.Enabled = true;



                    btnSetExpCam1.Enabled = true;
                    btnSetExpCam2.Enabled = true;
                    btnSetExpCam3.Enabled = true;
                    btnSetExpCam4.Enabled = true;

                    btnRoiSave.Enabled = true;
                    btnSpecSave.Enabled = true;
                    btnSaveCali.Enabled = true;
                    btnSaveLight.Enabled = true;

                    break;
            }
        }
        // scanning 1 time sequency...................................
        private void UserIni()
        {
            bool statusServo = true;
            byte iSlaveNo;
            ulong cmdPos;
            int nRtn;

            cbSlaveTeach.Items.Clear();
            textSlaveNo.Items.Clear();
            textSlaveNo.Items.Add(0);
            textSlaveNo.Items.Add(1);
            cbSlaveTeach.Items.Add(0);
            cbSlaveTeach.Items.Add(1);


            cbSpec.Items.Clear();
            cbCali.Items.Clear();
            cbROI2.Items.Clear();
            cbROI.Items.Clear();
            cbROI3.Items.Clear();
            cbLightSet.Items.Clear();
            cbLightCam.Items.Clear();
            cbLightPos.Items.Clear();
            cbLightPt.Items.Clear();
            cbCalCam.Items.Clear();
            cbCalPos.Items.Clear();
            cbCalPt.Items.Clear();

            Mat m_image = new Mat();
            string strHeadFile;
            strHeadFile = Application.StartupPath;
            m_image = new Mat(strHeadFile + "\\Hubble.bmp");
            pMap.Image = m_image.ToBitmap();

            for (int i = 1; i < 11; i++)
            {
                cbSpec.Items.Add(i);
               
               // cbROI.Items.Add(i);
               // cbROI2.Items.Add(i);
                cbLightSet.Items.Add(i);
            }
            cbCali.Items.Add("X");
            cbCali.Items.Add("Y");
            for (int i = 1; i < 8; i++)
            {
                cbROI2.Items.Add(i);
                cbCalPos.Items.Add(i);
                cbLightPos.Items.Add(i);
            }
            for (int i = 1; i < 4; i++)
            {
                cbROI.Items.Add(i);
                cbCalCam.Items.Add(i);
                cbLightCam.Items.Add(i);
            }
            
            for (int i = 0; i < 5; i++)
            {
                cbROI3.Items.Add(i);
                cbCalPt.Items.Add(i);
                cbLightPt.Items.Add(i);
                cbLED_No.Items.Add(i);
            }

            // Disable all button.............
            User_Set(0);

            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            mchSettingsFilePath = exePath + mchSetFileName;


            string strread = "";

            for ( int i = 0; i <520; i ++)
            {
                // Loading  Threshole information........................................
                FileOperation.ReadData(mchSettingsFilePath, "LowThreshole", "Point" + i, ref strread);
                iThreHoleLow[i] = Convert.ToInt32(strread);

                FileOperation.ReadData(mchSettingsFilePath, "HiThreshole", "Point" + i, ref strread);
                iThreHoleHi[i] = Convert.ToInt32(strread);
            }
            for (int i = 1; i < 11; i++)
            {

                // loading  specification....................................................................
                FileOperation.ReadData(mchSettingsFilePath, "HiSpec", "Point" + i, ref strread);
                dHiSpec[i] = Convert.ToDouble(strread);

                FileOperation.ReadData(mchSettingsFilePath, "LowSpec", "Point" + i, ref strread);
                dLowSpec[i] = Convert.ToDouble(strread);

                // loading Pixel & CaliData.........................................

                // loading  specification....................................................................
                FileOperation.ReadData(mchSettingsFilePath, "Pixel", "Point" + i, ref strread);
                iPixel[i - 1] = Convert.ToInt16(strread);

                FileOperation.ReadData(mchSettingsFilePath, "CaliData", "Point" + i, ref strread);
                dCaliData[i - 1] = Convert.ToDouble(strread);

                FileOperation.ReadData(mchSettingsFilePath, "Exposure", "Point" + i, ref strread);
                Exposure[i - 1] = Convert.ToInt32(strread);


            }




            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
            // Get comPort to connect Servo...........................................



            FileOperation.ReadData(mchSettingsFilePath, "MachineName", "Model", ref strread);
            if (strread != "0")
            {
                // Baud = Convert.ToUInt32(strread);
                txtModel.Text = strread;
            }

            FileOperation.ReadData(mchSettingsFilePath, "MachineName", "Version", ref strread);
            if (strread != "0")
            {
                // Version = Convert.ToUInt32(strread);
                txtVersion.Text = strVersion;
            }

            // Get comPort to connect Servo...........................................
            FileOperation.ReadData(mchSettingsFilePath, "ComPort", "PortNo", ref strread);
            if (strread != "0")
            {
                PortNo = Convert.ToByte(strread);
                m_nPortNo = PortNo;
                comboBoxPortNo.Items.Add(strread);
            }

            FileOperation.ReadData(mchSettingsFilePath, "ComPort", "Baud", ref strread);
            if (strread != "0")
            {
                Baud = Convert.ToUInt32(strread);
                comboBaudrate.Text = strread;
            }

            if (IO.FAS_Connect(PortNo, Baud) == 0)
            {
                // Failed to connect
                MessageBox.Show("Failed to connect");
                goto Exit;

            }
            m_bConnected = true;
            m_ServoOn = true;
            /// Enable motor 0................................
            nRtn = IO.FAS_ServoEnable(PortNo, 0, 1);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "UserIni_FAS_ServoEnable Motor 0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                m_ServoOn = false;
            }

            Delay(100);
            /// Enable motor 1................................
            nRtn = IO.FAS_ServoEnable(PortNo, 1, 1);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "UserIni_FAS_ServoEnable Motor 1() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
                m_ServoOn = false;
            }


            // set Dirrection of original.motor 0................................................
            nRtn = IO.FAS_SetParameter(m_nPortNo, 0, 21, 1);

            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_SetParameter Original_Servo0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }

            // set Dirrection of Move .motor 0................................................
            nRtn = IO.FAS_SetParameter(m_nPortNo, 0, 28, 1);

            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_SetParameter Original_Servo0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }


            // set Dirrection of original.motor 1................................................
            nRtn = IO.FAS_SetParameter(m_nPortNo, 1, 21, 1);

            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_SetParameter Original_Servo0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }

            // set Dirrection of Move .motor 0................................................
            nRtn = IO.FAS_SetParameter(m_nPortNo, 1, 28, 0);

            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_SetParameter Original_Servo0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }

            // set Position of original.motor 1................................................
            //nRtn = IO.FAS_SetParameter(m_nPortNo, 1, 20, 1);

            //if (nRtn != IO.FMM_OK)
            //{
            //    string strMsg;
            //    strMsg = "FAS_SetParameter Original_Servo0() \nReturned: " + nRtn.ToString();
            //    MessageBox.Show(strMsg, "Function Failed");
            //}

            Delay(5000);

            /// Servo move to orginal....................................................
            nRtn = IO.FAS_MoveOriginSingleAxis(PortNo, 0); // Set original for Servo 0
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "UserIni_AfterStartSenseProcess_ Original_Servo0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }

            Delay(5000);
            nRtn = IO.FAS_MoveOriginSingleAxis(PortNo, 1); // Set original for Servo 1
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "UserIni_AfterStartSenseProcess_ Original_Servo2() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }

            Delay(3000);
            // Check point
            nRtn = IO.FAS_PosTableSingleRunItem(m_nPortNo, 0, false, iStepPosServo0 = 0);
            if (nRtn != IO.FMM_OK)
            {
                string strMsg;
                strMsg = "FAS_PosTableSingleRunItem Original_Servo0() \nReturned: " + nRtn.ToString();
                MessageBox.Show(strMsg, "Function Failed");
            }




            

            FileOperation.ReadData(mchSettingsFilePath, "Delay", "Time", ref strread);
            if (strread != "0")
                iDelayTime = Convert.ToInt32(strread);


            // set Exposure time for camera..........................................
            camera1.SetExposureTime(camera1.ExpCamTimeSet);
            camera2.SetExposureTime(camera2.ExpCamTimeSet);
            camera3.SetExposureTime(camera3.ExpCamTimeSet);
          //  camera4.SetExposureTime(camera4.ExpCamTimeSet);

            userCB.Items.Clear();
            for (int i = 0; i < 3; i++)
                userCB.Items.Add(User.user[i]);

            userCB.SelectedIndex = 0;



        Exit:; Delay(100);
        }




        private void AfterStartSenseProcess()
        {
            bool statusServo = true;
            byte iSlaveNo;

            int nRtn;
            if (page != "Auto")
            {
                //BL.send(BL.Channel12_Cmd_On);
                //BL.send(BL.Channel34_Cmd_On);
            }
            else
            {
               //// BL.send(BL.Channel1_Cmd_Off);
               // BL.send(BL.Channel2_Cmd_Off);
               // BL.send(BL.Channel3_Cmd_Off);
               // BL.send(BL.Channel4_Cmd_Off);
            }


            // Update data Motor position................................................
            if ((m_bConnected == true) && (m_ServoOn == true))
            {
                //iSlaveNo = byte.Parse(cbSlaveTeach.Text);
                Thread.Sleep(10);
                if (statusServo)
                {

                }

                String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
                string mchSettingsFilePath;
                string strread = "";
                mchSettingsFilePath = exePath + mchSetFileName;

                if (cbSlaveTeach.Text != "")
                {

                    //.........................................................................................................
                    nRtn = IO.FAS_GetCommandPos(m_nPortNo, Convert.ToByte(cbSlaveTeach.Text), out cmdPos);
                    if (nRtn != IO.FMM_OK)
                    {
                        string strMsg;
                        strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                       // MessageBox.Show(strMsg, "Function Failed");
                        lbLog.Items.Clear();
                        lbLog.Items.Add(strMsg);
                        goto Exit;
                    }

                    txtCmdPosTeach.Text = cmdPos.ToString();

                    nRtn = IO.FAS_GetCommandPos(m_nPortNo, Convert.ToByte(cbSlaveTeach.Text), out ActPos);
                    if (nRtn != IO.FMM_OK)
                    {
                        string strMsg;
                        strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                    //    MessageBox.Show(strMsg, "Function Failed");
                        lbLog.Items.Clear();
                        lbLog.Items.Add(strMsg);
                        goto Exit;
                    }

                    txtActPosTeach.Text = ActPos.ToString();

                }



                //.........................................................................................................
                nRtn = IO.FAS_GetCommandPos(m_nPortNo, 0, out cmdPos);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                   // MessageBox.Show(strMsg, "Function Failed");
                    lbLog.Items.Clear();
                    lbLog.Items.Add(strMsg);
                    goto Exit;

                }

                txtCmdPos.Text = cmdPos.ToString();

                nRtn = IO.FAS_GetCommandPos(m_nPortNo, 0, out ActPos);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                   // MessageBox.Show(strMsg, "Function Failed");
                    lbLog.Items.Clear();
                    lbLog.Items.Add(strMsg);
                    goto Exit;
                }

                txtGetPos.Text = ActPos.ToString();
                ///////////////////////////////////////////////////////////////////////
                nRtn = IO.FAS_GetCommandPos(m_nPortNo, 1, out cmdPos);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                    //MessageBox.Show(strMsg, "Function Failed");
                    lbLog.Items.Clear();
                    lbLog.Items.Add(strMsg);
                    goto Exit;
                }

                txtCmdPosSer1.Text = cmdPos.ToString();

                nRtn = IO.FAS_GetCommandPos(m_nPortNo, 1, out ActPos);
                if (nRtn != IO.FMM_OK)
                {
                    string strMsg;
                    strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                   // MessageBox.Show(strMsg, "Function Failed");
                    lbLog.Items.Clear();
                    lbLog.Items.Add(strMsg);
                    goto Exit;
                }

                txtGetPosSer1.Text = ActPos.ToString();





                // Check status of button...............................................
                uint Result;
                uint SafeResult;
                string strResult;
                uint Pin = 1;
                byte level = 0;
                byte iPinNo;
                uint dLogiMask = 27;
                byte bLevel;

                if ((m_bConnected == true) && (m_ServoOn == true))
                {

                    nRtn = IO.FAS_GetIOInput(m_nPortNo, 0, out Result);
                    if (nRtn != IO.FMM_OK)
                    {
                        string strMsg;
                        strMsg = "FAS_GetCommandPos() \nReturned: " + nRtn.ToString();
                        // MessageBox.Show(strMsg, "Function Failed");
                        lbLog.Items.Clear();
                        lbLog.Items.Add(strMsg);
                        goto Exit;
                    }
                    // nRtn = IO.FAS_GetIOInput(m_nPortNo, 0, ref 0000800000);
                    //var bin = Convert.ToString(Result, 2);
                    // string myHex = Result.ToString("X");

                    camera1.ImageBuffer = null;
                    camera2.ImageBuffer = null;
                    camera3.ImageBuffer = null;

                    var bin = Convert.ToString(Result, 8);
                    if (bin != "0")
                    {
                        if (bin.Length > 6)
                        {
                            strResult = bin.Substring(0, 4);        // Right Button press
                            if (strResult == "1000")
                            {
                                btnUserIn0.BackColor = Color.Blue;
                                btnUserIn1.BackColor = Color.White;
                                btnUserIn2.BackColor = Color.White;

                            }
                            else if (strResult == "2000")  // Emer button press...............................
                            {
                                btnUserIn1.BackColor = Color.Blue;
                                btnUserIn0.BackColor = Color.White;
                                btnUserIn2.BackColor = Color.White;
                                bnStart1 = false;
                                bnEmer = true;
                                int i;
                                i = IO.FAS_EmergencyStop(6, 0); // Emer Stop
                                i = IO.FAS_EmergencyStop(6, 1);

                            }

                            else if (strResult == "3000") // 2 Start Button press....
                            {
                                btnUserIn0.BackColor = Color.Blue;
                                btnUserIn1.BackColor = Color.Blue;
                                btnUserIn2.BackColor = Color.White;
                                bnStart1 = true;
                                
                            }
                            else if (strResult == "4000")  // Safety sensor..........................
                            {
                                btnUserIn0.BackColor = Color.White;
                                btnUserIn1.BackColor = Color.White;
                                btnUserIn2.BackColor = Color.Red;
                                bnSafety = true;
                            }

                            strResult = bin.Substring(0, 4);

                            if (strResult == "4004") // 2 Start Button press....
                            {
                                btnUserIn3.BackColor = Color.Red;
                                
                                bnStart1 = true;
                            }
                            else
                            {
                                btnUserIn3.BackColor = Color.White;
                               // bnEmer = false;
                            }


                        }
                        else
                        {
                            btnUserIn0.BackColor = Color.White;
                            btnUserIn1.BackColor = Color.White;
                            btnUserIn2.BackColor = Color.White;
                            btnUserIn3.BackColor = Color.White;
                            
                            //bnEmer = false;
                        }
                    }
                    nRtn = IO.FAS_GetIOInput(m_nPortNo, 1, out SafeResult);


                    if (SafeResult != 0)  // Safety sensor..........................
                    {
                        btnUserIn0.BackColor = Color.White;
                        btnUserIn1.BackColor = Color.White;
                        btnUserIn2.BackColor = Color.Red;
                        bnSafety = true;
                    }
                    else
                        bnSafety = false;
                    if (nRtn != IO.FMM_OK)
                    {
                        string strMsg;
                        strMsg = "FAS_GetIOInput() \nReturned: " + nRtn.ToString();
                        MessageBox.Show(strMsg, "Function Failed");
                    }



                }
                if (cbSlaveTeach.Text == "0")
                    lblStepPosTab.Text = Convert.ToString(iStepPosServo0);
                else if (cbSlaveTeach.Text == "1")
                {
                    lblStepPosTab.Text = Convert.ToString(iStepPosServo1);
                }


            Exit:;          }
        }
    }
}
