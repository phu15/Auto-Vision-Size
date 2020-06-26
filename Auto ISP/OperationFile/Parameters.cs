using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace Auto_Attach
{
    public class CalibrationData
    {
        public string Serial = "";
        public List<float> TransRatio = new List<float>(); //Per point 32.
        public List<float> MeasTransRatio = new List<float>(); //Per point 32.
        public bool MeasureResult = true;    //NG---false     OK----true
        public List<float> MeasTiltPercent = new List<float>(); //Per point 32.
        public List<float> MeasTPercent = new List<float>(); //Per point 32.
    }

      public class BlackPara
        {
            public int BlackCounts = 0;
            public List<WhiteAndBlackPara> blackPara = new List<WhiteAndBlackPara>();
        }

        public class WhitePara
        {
            public int WhiteCounts = 0;
            public List<WhiteAndBlackPara> whitePara = new List<WhiteAndBlackPara>();
        }
        public class WhiteAndBlackPara
        {
            public double X = 0;
            public double y = 0;
            public double Area = 0;
        }
    public class CalibrationSpec
    {
        public double UpperLimit;
        public double LowerLimit;
    }
    public class FPoint
    {
        public float X = 0.0f;
        public float Y = 0.0f;        
    }
    public class NDFileterInfo
    {
        public string Name = "";
        public List<CalibrationInfo> data = new List<CalibrationInfo>();
    }

    public class CalibrationInfo
    {
        public int waveLength = 400;
        public double min = 1.0;
        public double max = 5.0;
        public double Nominal = 0.0;
        public double Measured = 0.0;
        public double Diff = 0.0;
        public string result = "";
    }

    public class DPoint
    {
        public double X = 0.0;
        public double Y = 0.0;
        public DPoint()
        { }

        public DPoint(double myX, double myY)
        {
            X = myX;
            Y = myY;
        }
    }
    public class PosPoint
    {
        public double X = 0.0;
        public double Y = 0.0;
        public double Z = 0.0;
        public PosPoint()
        { }

        public PosPoint(double myX, double myY, double myZ)
        {
            X = myX;
            Y = myY;
            Z = myZ;
        }
    }
    public class ModuleSettings
    {
        public List<DPoint> TestPt = new List<DPoint>();
        public List<bool> TestPtEnb = new List<bool>();
        public List<PosPoint> TeachPos = new List<PosPoint>();
        public DPoint CamToOriginOffset = new DPoint();
        public float AngleOffset = 0.0F;
    }
    public class WhiteParaList
    {
        public int whiteCounts = 0;
        public float[] whiteX = new float[5];
        public float[] whiteY = new float[5];
        public float[] whiteArea = new float[5];
    }
    public class BlackParaList
    {
        public int blackCounts = 0;
        public float[] blackX = new float[5];
        public float[] blackY = new float[5];
        public float[] blackArea = new float[5];
    }
    public class Para
    {
        public static int AvgTimes = 10;
        public static int DarkDotCount = 0;
        public static int WhiteDotCount = 0;
        public static int NDTime = 8;
        public static object logobj = new object();
        public static string MchName = "CGMachine01";
        public static string SWComment = "P1 For D3X(QE)";
        public static string SWVersion = "Version 18.01.12.10";
        public static string HWVersion = "Version 16.8.4.20";
        public static string CurLoadConfigFileName = "";
        public static string MchConfigFileName = "";
        public static string LightSourceType = "EQ_XXXXXXXXX";//20170303@Brando
        public static string Spectrometer1SN = "162114216";
        public static string Spectrometer2SN = "155914216";
        public static bool MachineOnline = true;
        public static bool EngineerMode = false;
        public static bool DebugMode = false;
        public static bool DryRunMode = false;
        public static bool DisableSpecTest = false;
        public static bool DisableBarcode = false;
        public static bool DisableUpTriggerMode = false;
        public static bool DisableSafeDoor = false; 
        public static bool EndLot = false;
        public static bool ContTestRunData = false;
        public static bool EnableSingleModule = false;
        public static bool EnableCGHost = true;
        public static bool EnableP2Test = true;
        public static bool EnableZaxis = false;
        public static bool isRotaryAt45 = false;
        public static bool isOutShutter = false;
        public static bool isWidth818 = false;
        public static bool isRotaryingGrab = true;
        public static int SampleShape = 0;
  
        public static bool isRotaryError = false;
        public static bool Enb45DegTest = false;
        public static bool Enb3TestPtOnly = true;
        //20171014
        public static bool IsWhiteDark = false;

        public static bool EnbStation4Unloading = false; //setting li huang, 这里换没作用, pao ba
        public static string startTime = "";
        public static bool EnBin_Code = true;
        public static DateTime SystemRunTime;
        public static DateTime NDSystemRunTime;
        public static DateTime LSSystemRunTime;
        public static DateTime GetDarkTimeModule1;
        public static DateTime GetDarkTimeModule2;
        public static bool BothBtnAreLow = false;

        public static bool EnableBuzzer = false;
        public static int CurrentRotaryIndex = 0;
        public static int _Max_Test_Point = 5;

        public static int TotalTestUnit = 0;
        public static int Mod1FailUnitCnt = 0;
        public static int Mod2FailUnitCnt = 0;

        public static int Cam1Exposure = 19200;
        public static int Cam2Exposure= 19200;


        public static int Cam1ExposureTime1 = 19200;
        public static int Cam1ExposureTime3 = 19200;
        public static int Cam1ExposureTimeB = 19200;
        public static int Cam1ExposureTimeW = 19200;
        public static int Cam2ExposureTimeB = 19200;
        public static int Cam2ExposureTimeW = 19200;
        public static int Cam2ExposureTime1 = 19200;
        public static int Cam2ExposureTime3 = 19200;
        public static int selected1BorW = 0;
        public static int selected2BorW = 0;
        public static bool disableAutoExpTime1 = false;
        public static bool disableAutoExpTime2 = false;
        public static int Threshold = 50;
        public static bool bRepaintCross1 = false;
        public static bool bRepaintCross2 = false;
        public static int CrossX1 = 0;
        public static int CrossY1 = 0;
        public static int CrossStep1 = 10;
        public static int CrossX2 = 0;
        public static int CrossY2 = 0;
        public static int CrossStep2 = 10;
        //public static bool res1FromMini = true;
        //public static bool res2FromMini = true;
        //public static string returnCode= "";
        //public static string returnCode2 = "";
        public static bool firstFinish = false;
        public static string PicPath = "G";
        public static float CenWL1 = 0;
        public static float PixDen1 = 0;
        public static float BeamSize1 = 0;
        public static float CenWL2 = 0;
        public static float PixDen2 = 0;
        public static float BeamSize2 = 0;
        public static string timeStip1 = "0";
        public static string timeStip2 = "0";

        public static double Slope1B = 0.072;
        public static double Slope2B = 0.072;
        public static double Intercept1B = 4.5;
        public static double Intercept2B = 4.5;
        public static double MeansB = 145;

        public static double Slope1W = 0.072;
        public static double Slope2W = 0.072;
        public static double Intercept1W = 4.5;
        public static double Intercept2W = 4.5;
        public static double MeansW = 145;


        public static double CaliX1 = 0.005488;
        public static double CaliY1 = 0.005488;
        public static double CaliX2 = 0.005488;
        public static double CaliY2 = 0.005488;
        public static double CamOffsetX = 0;
        public static double CamOffsetY = 0;

        //20171016
        public static int WhiteAndBlackCounts = 100;
        
        //Module
        public static int ModCount = 2;
        public static int TeachPosCount = 3;
        public static int TestPtCnt = 5;
        public static string[] posName = new string[] { "Origin", "White Reference" ,"Find Center"};//{"Origin","Pos 1"};
        public static string[] errorCode1Array = new string[] { "0", "0", "0", "0" };
        public static string[] errorCode2Array = new string[] { "0", "0", "0", "0" };
        public static string[] errorStr1Array = new string[] { "0", "0", "0", "0" };
        public static string[] errorStr2Array = new string[] { "0", "0", "0", "0" };
        public static string[] bin_Code1Array = new string[] { "0", "0", "0", "0" };
        public static string[] bin_Code2Array = new string[] { "0", "0", "0", "0" };
        
        public static List<ModuleSettings> Module = new List<ModuleSettings>();


        public static bool Robotablebool = true;//huang20171027  
        
    }
}
