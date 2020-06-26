using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

using System.Drawing.Imaging;


namespace Auto_Attach
{
    class Inspection
    {

        public LineIntersectionFinder LineFinder = new LineIntersectionFinder();
        public Mat mMaskImage;
        public Mat mTestImage;
        public OpenCvSharp.Point[][] contours;
        public int intLogo1_Width, intLogo1_Length, intLogo1_X, intLogo1_Y;
        public int intLogo2_Width, intLogo2_Length, intLogo2_X, intLogo2_Y;
        public int GetValueFromCharacter = 4;
        public int TotalCharacterStream = 70;
        public int[] mValue_Width = new int[7];
        public int[] mValue_Length = new int[7];
        public int[] mValue_X = new int[7];
        public int[] mValue_Y = new int[7];
        public ROI[] ROINum = new ROI[4500];
        public int intMinResult = 0;
        public int intMaxResult = 0;
        public int[] MinMax = new int[1000];
        public int[] MinMax_X = new int[1000];
        public int[] MinMax_Y = new int[1000];
        public int intCountOperation = 0;

        public Rect ROI = new Rect();
        public Mat m_image = null;
        public Mat b_image = null;
        public Mat ViewSet = null;
        public int mCountObject;

        // Rect for Cam1////////
        public Rect ROI11 = new Rect();
        public Rect ROI12 = new Rect();
        public Rect ROI13 = new Rect();
        public Rect ROI14 = new Rect();
        public Rect ROI15 = new Rect();
        public Rect ROI16 = new Rect();
        public Rect ROI17 = new Rect();
        public Rect ROI18 = new Rect();
        public Rect ROI19 = new Rect();
        public Rect ROI110 = new Rect();


        /// <summary>
        /// /Rect for Cam2
        /// </summary>
        public Rect ROI21 = new Rect();
        public Rect ROI22 = new Rect();
        public Rect ROI23 = new Rect();
        public Rect ROI24 = new Rect();
        public Rect ROI25 = new Rect();
        public Rect ROI26 = new Rect();
        public Rect ROI27 = new Rect();
        public Rect ROI28 = new Rect();
        public Rect ROI29 = new Rect();
        public Rect ROI210 = new Rect();


        /// <summary>
        /// Rect for Cam 3////////////
        /// </summary>
        public Rect ROI31 = new Rect();
        public Rect ROI32 = new Rect();
        public Rect ROI33 = new Rect();
        public Rect ROI34 = new Rect();
        public Rect ROI35 = new Rect();
        public Rect ROI36 = new Rect();
        public Rect ROI37 = new Rect();
        public Rect ROI38 = new Rect();
        public Rect ROI39 = new Rect();
        public Rect ROI310 = new Rect();

        public Rect[] ArrROI = new Rect[520];
        public int[] DistanceResult = new int[100];
        public int[] LineDetectorResult = new int[520];
        public int[] LineDistanceResult = new int[520];

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
        public double[] dbResultCam = new double[60];
        public double dbResultNotch;
        public int[] intResultTiltCam = new int[100];
        public int ROI_NotchW, ROI_NotchL;
        public int intStep;
        public Mat[] imgROI = new Mat[100];
        public int NumofROI = 11;

        public int X, y, W, H;
        public System.Drawing.Point LStart { get; set; }
        public System.Drawing.Point LEnd { get; set; }
        public System.Drawing.Point L1Start { get; set; }
        public System.Drawing.Point L1End { get; set; }
        public System.Drawing.Point L2Start { get; set; }
        public System.Drawing.Point L2End { get; set; }
        public int MedianFilter = 0;
        public string ErrorView;
        public int XGap = 0;
        public int YGap = 0;
        public Mat picViewSet;





        public void LoadSettings()
        {
            string mchSettingsFilePath;
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string strread = "";
            mchSettingsFilePath = exePath + MainForm.instance.mchSetFileName;

            imgROI = new Mat[520];
            ArrROI = new Rect[520];


          

            for (int i = 1; i < 4; i++)
            {
                for (int j =1;j< NumofROI; j++)
                {
                    for (int x = 0; x <5; x ++)
                    {
                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + i + j + x, "Width", ref strread);
                        if (strread != "0")
                            ArrROI[Convert.ToInt16(Convert.ToString(i) + Convert.ToString(j) +Convert.ToString(x))].Width = Convert.ToInt32(strread);
                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + i + j + x, "Len", ref strread);
                        if (strread != "0")
                            ArrROI[Convert.ToInt16(Convert.ToString(i) + Convert.ToString(j) + Convert.ToString(x))].Height = Convert.ToInt32(strread);

                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + i + j + x, "X", ref strread);
                        if (strread != "0")
                            ArrROI[Convert.ToInt16(Convert.ToString(i) + Convert.ToString(j) + Convert.ToString(x))].X = Convert.ToInt32(strread);

                        FileOperation.ReadData(mchSettingsFilePath, "ROI" + i + j + x, "Y", ref strread);
                        if (strread != "0")
                            ArrROI[Convert.ToInt16(Convert.ToString(i) + Convert.ToString(j) + Convert.ToString(x))].Y = Convert.ToInt32(strread);
                    }
                    // Loading info for ROI..........................................
                   
                }
            }


        }
        public Mat InspectGetData(Mat image, int thresholdLow, int thresholdHi, int NumOfROI, ROI ArryROI,int ROISave)
        {
            // Mat rectROI = new Mat();
           // m_image = new Mat(PathFileImage);

            //Mat rectROI = new Mat(m_image, ArryROI);


            int data = GetGap(image, thresholdLow, thresholdHi, ArryROI, ROISave);
            if (data == -1)
            {
                strProgStatus = "Waiting";
               // MainForm.instance.bResetObject = true;
                return m_image;

            }
            else
            {
                MainForm.instance.Cam1Result[NumOfROI] = data;

                strProgStatus = "";
            }

            return m_image;
        }

        public Mat DrawROI(String PathFileImage, int FirstROI, int LastROI)
        {

            m_image = new Mat(PathFileImage);

            if (LastROI - FirstROI >= 1)
            {
                for (int i = 0; i < LastROI - FirstROI; i++)
                {
                    Cv2.Rectangle(m_image, ArrROI[FirstROI + i], Scalar.Red, 1);
                }
            }


            return m_image;

        }
        public Mat DrawROI(String PathFileImage, int CamNo,int Pos, int FromROI, int ToROI)
        {

            m_image = new Mat(PathFileImage);

            if (ToROI - FromROI >= 1)
            {
                for (int i = FromROI; i < ToROI; i++)
                {
                    Cv2.Rectangle(m_image,ArrROI[Convert.ToInt16(Convert.ToString(CamNo) + Convert.ToString(Pos) + Convert.ToString(i))], Scalar.Red, 5);
                }
            }


            return m_image;

        }
        public Mat DrawROI(Mat m_image, ROI ROI)
        {
            Rect ROIDraw = new Rect();
            //Mat m_image = new Mat(PathFileImage);
            ROIDraw.X = ROI.X;
            ROIDraw.Y = ROI.Y;
            ROIDraw.Width = ROI.Width;
            ROIDraw.Height = ROI.Height;

            Cv2.Rectangle(m_image, ROIDraw, Scalar.Red, 2);
            return m_image;

        }

        public Mat DrawROI(Mat Image, ROI ROI1, ROI RI2)
        {
            Rect ROIDraw = new Rect();
            ROIDraw.X = ROI.X;
            ROIDraw.Y = ROI.Y;
            ROIDraw.Width = ROI.Width;
            ROIDraw.Height = ROI.Height;

            Cv2.Rectangle(m_image, ROIDraw, Scalar.Red, 5);
            return m_image;

        }

  


        private Boolean GetTiltFromImageLine(Mat Image, int thresholdLow, int thresholdHigh, int NumberImage, int Direction)
        {
            Mat m_image = null;
            m_image = Image.Clone();
            mCountObject = 0;
            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];
            int[,] linearray = new int[1, 4];
            int[,] LineMin = new int[1, 1];
            int[,] LineMax = new int[1, 1];

            int[] MinMax = new int[100000];

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            // Cv2.ImShow("ImageTest", m_image);
            //  Cv2.WaitKey(0);



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
            // Cv2.ImShow("ImageTest", ImageTest);
            // Cv2.WaitKey(0);

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
            if (Direction == 1)
            {
                m_image.Line(0, intMinResult + 10, ImageTest.Width + 20, intMinResult + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
               // m_image.Line(0, intMaxResult + 10, ImageTest.Width + 20, intMaxResult + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
            }
            else if (Direction == 0)
            {
                m_image.Line(intMinResult + 10, 0, intMinResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
              //  m_image.Line(intMaxResult + 10, 0, intMaxResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
            }

            //Cv2.ImShow("ImageTest", ImageTest);
            //Cv2.WaitKey(0);
            m_image.ToBitmap().Save("image" + intStep + "" + NumberImage + ".bmp", ImageFormat.Bmp);
            return true;

        }


        private Boolean GetTiltFromImageLine(Mat Image, int thresholdLow, int thresholdHigh, int NumberImage, int Direction, string Side)
        {
            Mat m_image = null;
            m_image = Image.Clone();
            mCountObject = 0;
            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];
            int[,] linearray = new int[1, 4];
            int[,] LineMin = new int[1, 1];
            int[,] LineMax = new int[1, 1];

            int[] MinMax = new int[100000];

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            // Cv2.ImShow("ImageTest", m_image);
            //  Cv2.WaitKey(0);



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
            // Cv2.ImShow("ImageTest", ImageTest);
            // Cv2.WaitKey(0);

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
            if (Direction == 1)
            {
                if (Side == "Up")
                  m_image.Line(0, intMinResult + 10, ImageTest.Width + 20, intMinResult + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
                else if (Side == "Down")
                 m_image.Line(0, intMaxResult + 10, ImageTest.Width + 20, intMaxResult + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
            }
            else if (Direction == 0)
            {
                if (Side == "Left")
                    m_image.Line(intMinResult + 10, 0, intMinResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
                else if (Side == "Right")
                 m_image.Line(intMaxResult + 10, 0, intMaxResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
            }

            //Cv2.ImShow("ImageTest", ImageTest);
            //Cv2.WaitKey(0);
            m_image.ToBitmap().Save("Image/image" + intStep + "" + NumberImage + ".bmp", ImageFormat.Bmp);
            return true;

        }
        private Boolean GetTiltFromImageCurve(Mat Image, int thresholdLow, int thresholdHigh, int NumberImage, int Direction, string Side)
        {
            Mat m_image = null;
            m_image = Image.Clone();
            mCountObject = 0;
            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];
            int[,] linearray = new int[1, 4];
            int[,] LineMin = new int[1, 1];
            int[,] LineMax = new int[1, 1];

            int[] MinMax = new int[100000];
            int[] MinMax1 = new int[100000];

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            // Cv2.ImShow("ImageTest", m_image);
            //  Cv2.WaitKey(0);



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
            // Cv2.ImShow("ImageTest", ImageTest);
            // Cv2.WaitKey(0);

            int intCountOperation = 0;
            contours = Cv2.FindContoursAsArray(ImageTest, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                
                MinMax1[0] = 9999; MinMax1[1] = 9999; intCountOperation = 1;
                goto Exit;
            }
            int intTemp = 0;
            int intTemp1 = 0;


            for (int i = 0; i < contours.Length; i++)
            {
                double area = Cv2.ContourArea(contours[i]);  //  Find the area of contour
                OpenCvSharp.Rect uRect = Cv2.BoundingRect(contours[i]);
                // Get data X Axis................................................
                if (Side == "Right" || Side == "Left")
                    for (int j = 0; j < contours[i].Length; j++)
                    {
                        MinMax[intCountOperation] = contours[i][j].X;
                        MinMax1[intCountOperation] = contours[i][j].Y;
                        intCountOperation++;
                    }
                // Get data Y Axis................................................
                else if (Side == "Up" || Side == "Down")
                    for (int j = 0; j < contours[i].Length; j++)
                    {
                        MinMax[intCountOperation] = contours[i][j].Y;
                        MinMax1[intCountOperation] = contours[i][j].X;
                        intCountOperation++;
                    }
                Cv2.DrawContours(ImageTest, contours, i, Scalar.White, 2);
                //Cv2.ImShow("ImageTest", ImageTest);
                //Cv2.WaitKey(0);
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
                        intTemp1 = MinMax1[i];

                        MinMax[i] = MinMax[j];
                        MinMax1[i] = MinMax1[j];


                        MinMax[j] = intTemp;
                        MinMax1[j] = intTemp1;
                    }
                }
            }
        //................report result........................................................................................
        Exit: intMinResult = MinMax1[0];
    
             intMaxResult = MinMax1[intCountOperation - 1];
            
            if (Direction == 1)
            {
                if (Side == "Up" || Side == "Left")
                    m_image.Line(0, MinMax1[0]+10, ImageTest.Width + 20 , MinMax1[0] + 10 , Scalar.Orange, 2, LineTypes.AntiAlias, 0);
                else if (Side == "Down" || Side == "Right")
                    m_image.Line(0, MinMax1[intCountOperation - 1] + 10, ImageTest.Width+20 , MinMax1[intCountOperation - 1] + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
            }
            else if (Direction == 0)
            {
                if (Side == "Up" || Side == "Left")
                    m_image.Line(intMinResult + 10, 0, intMinResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
                else if (Side == "Down" || Side == "Right")
                    m_image.Line(intMaxResult + 10, 0, intMaxResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
            }

            //Cv2.ImShow("ImageTest", ImageTest);
            //Cv2.WaitKey(0);
            m_image.ToBitmap().Save("Image/image" + intStep + "" + NumberImage + ".bmp", ImageFormat.Bmp);
        return true;
         

        }
        public int MedialFilter (int[] Value, int Pos)
        {
            int Data = 0;
            for (int i = Pos; i < Pos +50; i++)
            {
                Data = Data + Value[i];
            }
            Data = Data / 50;
            return Data;
        }
        public Boolean f_GetTiltFromImage(Mat Image, int thresholdLow, int thresholdHigh, int NumberImage, int Direction, int iStep)
        {
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
            // Cv2.ImShow("ImageTest", ImageTest);
            // Cv2.WaitKey(0);

            int intCountOperation = 0;
            contours = Cv2.FindContoursAsArray(ImageTest, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                return false;
                goto Exit;
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
            if (Direction == 1)
            {
                m_image.Line(0, intMinResult + 10, ImageTest.Width + 20, intMinResult + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
                m_image.Line(0, intMaxResult + 10, ImageTest.Width + 20, intMaxResult + 10, Scalar.Orange, 2, LineTypes.AntiAlias, 0);
            }
            else if (Direction == 0)
            {
                m_image.Line(intMinResult + 10, 0, intMinResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
                m_image.Line(intMaxResult + 10, 0, intMaxResult + 10, ImageTest.Height + 20, Scalar.Blue, 2, LineTypes.AntiAlias, 0);
            }

            //Cv2.ImShow("ImageTest", ImageTest);
            //Cv2.WaitKey(0);
            m_image.ToBitmap().Save("image" + NumberImage + ".bmp", ImageFormat.Bmp);
            return true;
        Exit:;

        }
        public Mat MakeROI(Mat ImageIn, Mat ImageOut, Rect ROI)
        {
            ImageOut = new Mat(ImageIn, ROI);

            return ImageOut;

        }
        public int LineFounder(Mat PathFileImage, int thresholdLow, int thresholdHigh, Rect ROI, int Direction, int Pos, string Side)
        {
            m_image = PathFileImage.Clone();

            Mat rectROI = new Mat(m_image, ROI);



            if (!GetTiltFromImageLine(rectROI, thresholdLow, thresholdHigh, Pos, Direction,Side))
            {
                strProgStatus = "Waiting";
                // MainForm.instance.bResetObject = true;
                return 9999;

            }
            else
            {
                if (Side == "Up" ||  Side == "Left")
                  LineDetectorResult[Pos] = intMinResult+20;
                else if (Side == "Down" || Side == "Right")
                    LineDetectorResult[Pos] = intMaxResult + 20;

                strProgStatus = "";
            }

            return 0;

        }
        public int CurveFounder(Mat PathFileImage, int thresholdLow, int thresholdHigh, Rect ROI, int Direction, int Pos, string Side)
        {
            m_image = PathFileImage.Clone();

            Mat rectROI = new Mat(m_image, ROI);



            if (!GetTiltFromImageCurve(rectROI, thresholdLow, thresholdHigh, Pos, Direction, Side))
            {
                strProgStatus = "Waiting";
                // MainForm.instance.bResetObject = true;
                return 9999;

            }
            else
            {
                if (Side == "Up" || Side == "Left")
                    LineDetectorResult[Pos] = intMinResult + 20;
                else if (Side == "Down" || Side == "Right")
                    LineDetectorResult[Pos] = intMaxResult + 20;

                strProgStatus = "";
            }

            return 0;

        }
        public int LineDistanceDetector(Mat Image, Rect ROI1,String Side1,int thresholdLowROI1, int thresholdHighROI1, Rect ROI2, String Side2, int thresholdLowROI2, int thresholdHighROI2, int Direction,int Pos, int Pos1, int Pos2)
        {
            m_image = Image.Clone();
            if (Direction == 0)
            {
                if (ROI1.X + LineFounder(m_image, thresholdLowROI1, thresholdHighROI1, ROI1, Direction,  Pos1, Side1) >= ROI2.X + LineFounder(m_image, thresholdLowROI2, thresholdHighROI2, ROI2, Direction,  Pos2, Side2))
                    LineDistanceResult[Pos] = ROI1.X + LineDetectorResult[Pos1] - ROI2.X - LineDetectorResult[Pos2];
                else
                    LineDistanceResult[Pos] = ROI2.X + LineDetectorResult[Pos2] - ROI1.X - LineDetectorResult[Pos1];
            }
           else if (Direction == 1)
            {
                if (ROI1.Y + LineFounder(m_image, thresholdLowROI1, thresholdHighROI1, ROI1, Direction,  Pos1, Side1) >= ROI2.Y + LineFounder(m_image, thresholdLowROI2, thresholdHighROI2, ROI2, Direction,  Pos2, Side2))
                {
                    LineDistanceResult[Pos] = ROI1.Y + LineDetectorResult[Pos1] - ROI2.Y - LineDetectorResult[Pos2];
                }
                  
                else
                {
                    LineDistanceResult[Pos] = ROI2.Y + LineDetectorResult[Pos2] - ROI1.Y - LineDetectorResult[Pos1];
                }
                   
            }
                return LineDistanceResult[Pos];

        }

        public int GetGap(Mat Image,int thresholdLow, int thresholdHigh, ROI ROI, int ROISave)
        {
            /// m_image = Image.Clone();
            int bDir = 0;
           
            //Mat m_image = Image.Clone();
            Rect iRect = new Rect();
            iRect.X = ROI.X;
            iRect.Y = ROI.Y;
            iRect.Width = ROI.Width;
            iRect.Height = ROI.Height;

            Mat m_image = new Mat(Image, iRect);

            if (m_image.Width > m_image.Height)
                bDir = 1;
            else
                bDir = 2;

            int rtnValue = 0;
            if (Image.Width == 0 || Image.Height == 0) goto Exit;
           // m_image = Image.Clone();
            mCountObject = 0;
            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];


            int[] MinMax = new int[1000];
            int[] MinMax_X = new int[10000];
            int[] MinMax_Y = new int[10000];
            int[] MinMax_X2 = new int[1000];
            int[] MinMax_Y2 = new int[1000];

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);




            Mat iThresholeLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThresholeHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThreshole = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            Mat iCanyLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCanyHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCany = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat ImageTest = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            // ROI clear bolder of image...........................................
            Rect ROITest = new Rect();
            ROITest.X = 1;
            ROITest.Y = 1;
            ROITest.Height = m_image.Height - 2;
            ROITest.Width = m_image.Width - 2;
            //......................................................................

            TestImage = m_image.Clone();
            iThresholeLow = m_image.Clone();
            iThresholeHigh = m_image.Clone();

            // Filting Image.............................................................................................
            Cv2.CvtColor(TestImage, TestImage, ColorConversionCodes.BGRA2GRAY);
            //Cv2.ImShow("Test", TestImage);
            //Cv2.WaitKey(1);

            Cv2.Blur(TestImage, TestImage, new OpenCvSharp.Size(1, 1));
            Cv2.GaussianBlur(TestImage, TestImage, new OpenCvSharp.Size(3, 3), 0);
            Cv2.MedianBlur(TestImage, TestImage, 1);
            Cv2.Threshold(TestImage, iThresholeLow, thresh: thresholdLow, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.Threshold(TestImage, iThresholeHigh, thresh: thresholdHigh, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.BitwiseAnd(iThresholeHigh, ~iThresholeLow, TestImage);
            ViewSet = m_image.Clone();
            //Cv2.ImShow("Test", TestImage);
            //Cv2.WaitKey(0);
            int intCountOperation = 0;
            int intTemp = 0;
         
            contours = Cv2.FindContoursAsArray(TestImage, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                return -1;
            }


            for (int i = 0; i < contours.Length; i++)
            {
                double area = Cv2.ContourArea(contours[i]);  //  Find the area of contour
                OpenCvSharp.Rect uRect = Cv2.BoundingRect(contours[i]);


                //  Cv2.DrawContours(MainForm.instance.m_image, contours, Scalar red);
                if (area > 100)
                {
                    for (int j = 0; j < contours[i].Length; j++)
                    {
                        MinMax_X[intCountOperation] = contours[i][j].X;
                        MinMax_Y[intCountOperation] = contours[i][j].Y;
                        intCountOperation++;
                    }

                    Cv2.DrawContours(ViewSet, contours, i, Scalar.Red, 1);
                    Cv2.Rectangle(ViewSet, uRect, Scalar.White, 2);

                }

                //    LineSegmentPolar[] segStd = Cv2.HoughLines(ViewSet, 1, Math.PI / 180, 50, 0, 0);
            }

            if (intCountOperation == 0)
            {
                return -2;
            }

            int intTemp_X;
            //////////////////////////////////////////////// get Y..................////////////////////////
            if (bDir == 2)
            {
                for (int i = 0; i < intCountOperation; i++)
                {
                    for (int j = i + 1; j < intCountOperation; j++)
                    {
                        if (MinMax_Y[j] < MinMax_Y[i])

                        {
                            intTemp = MinMax_Y[i];
                            intTemp_X = MinMax_X[i];

                            MinMax_Y[i] = MinMax_Y[j];
                            MinMax_X[i] = MinMax_X[j];



                            MinMax_Y[j] = intTemp;
                            MinMax_X[j] = intTemp_X;
                        }
                    }
                }
            }

            ////////////////////////////////////////////// get X..................////////////////////////
            if (bDir == 1)
            {
                for (int i = 0; i < intCountOperation; i++)
                {
                    for (int j = i + 1; j < intCountOperation; j++)
                    {
                        if (MinMax_X[j] < MinMax_X[i])

                        {
                            intTemp = MinMax_Y[i];
                            intTemp_X = MinMax_X[i];

                            MinMax_Y[i] = MinMax_Y[j];
                            MinMax_X[i] = MinMax_X[j];



                            MinMax_Y[j] = intTemp;
                            MinMax_X[j] = intTemp_X;
                        }
                    }
                }
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            int sampling = 5;
            if (bDir == 1)
            {
                ViewSet.Line(MinMax_X[sampling], 0, MinMax_X[sampling], ViewSet.Height, Scalar.Green, 3, LineTypes.AntiAlias, 0);
                ViewSet.Line(MinMax_X[intCountOperation - sampling], 0, MinMax_X[intCountOperation - sampling], ViewSet.Height, Scalar.Green, 3, LineTypes.AntiAlias, 0);

                XGap = MinMax_X[intCountOperation - sampling] - MinMax_X[sampling];
                rtnValue =   XGap;
                ViewSet.SaveImage("ROI" + ROISave + ".bmp");
                picViewSet = ViewSet.Clone();

                // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMin.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMax.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
            }
            else if (bDir == 2)
            {
                ViewSet.Line(0, MinMax_Y[sampling], ViewSet.Width, MinMax_Y[sampling], Scalar.Yellow, 3, LineTypes.AntiAlias, 0);
                ViewSet.Line(0, MinMax_Y[intCountOperation - sampling], ViewSet.Width, MinMax_Y[intCountOperation - sampling], Scalar.Yellow, 3, LineTypes.AntiAlias, 0);

                YGap = MinMax_Y[intCountOperation - sampling] - MinMax_Y[sampling]; ;
                rtnValue =  YGap;
                ViewSet.SaveImage("ROI" + ROISave + ".bmp");
                picViewSet = ViewSet.Clone();

                //MainForm.instance.m_MouserImage.Line(MainForm.instance.m_MouserImage.Width + PXMin.X - ViewSet.Width, y, PXMin.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMax.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
            }
        Exit:;
            return rtnValue;

        }

        public int LineDistanceDetector1(Mat Image, ROI ROI1, String Side1, int thresholdLowROI1, int thresholdHighROI1, ROI ROI2, String Side2, int thresholdLowROI2, int thresholdHighROI2, int Direction, int Pos, int Pos1, int Pos2)
        {
            m_image = Image.Clone();

            Rect nRect1 = new Rect();
            nRect1.X = ROI1.X;
            nRect1.Y = ROI1.Y;
            nRect1.Width = ROI1.Width;
            nRect1.Height = ROI1.Height;

            Rect nRect2 = new Rect();
            nRect2.X = ROI2.X;
            nRect2.Y = ROI2.Y;
            nRect2.Width = ROI2.Width;
            nRect2.Height = ROI2.Height;

            if (Direction == 0)
            {
                if (ROI1.X + LineFounder(m_image, thresholdLowROI1, thresholdHighROI1, nRect1, Direction, Pos1, Side1) >= ROI2.X + LineFounder(m_image, thresholdLowROI2, thresholdHighROI2, nRect2, Direction, Pos2, Side2))
                    LineDistanceResult[Pos] = ROI1.X + LineDetectorResult[Pos1] - ROI2.X - LineDetectorResult[Pos2];
                else
                    LineDistanceResult[Pos] = ROI2.X + LineDetectorResult[Pos2] - ROI1.X - LineDetectorResult[Pos1];
            }
            else if (Direction == 1)
            {
                if (ROI1.Y + LineFounder(m_image, thresholdLowROI1, thresholdHighROI1, nRect1, Direction, Pos1, Side1) >= ROI2.Y + LineFounder(m_image, thresholdLowROI2, thresholdHighROI2, nRect2, Direction, Pos2, Side2))
                {
                    LineDistanceResult[Pos] = ROI1.Y + LineDetectorResult[Pos1] - ROI2.Y - LineDetectorResult[Pos2];
                }

                else
                {
                    LineDistanceResult[Pos] = ROI2.Y + LineDetectorResult[Pos2] - ROI1.Y - LineDetectorResult[Pos1];
                }

            }
            return LineDistanceResult[Pos];

        }
        public int CurveDistanceDetector(Mat Image, Rect ROI1, String Side1, int thresholdLowROI1, int thresholdHighROI1, Rect ROI2, String Side2, int thresholdLowROI2, int thresholdHighROI2, int Direction, int Pos, int Pos1, int Pos2)
        {
            m_image = Image.Clone();
          
            if (Direction == 0)
            {
                if (ROI1.X + CurveFounder(m_image, thresholdLowROI1, thresholdHighROI1, ROI1, Direction, Pos1, Side1) >= ROI2.X + CurveFounder(m_image, thresholdLowROI2, thresholdHighROI2, ROI2, Direction, Pos2, Side2))
                    LineDistanceResult[Pos] = ROI1.X + LineDetectorResult[Pos1] - ROI2.X - LineDetectorResult[Pos2];
                else
                    LineDistanceResult[Pos] = ROI2.X + LineDetectorResult[Pos2] - ROI1.X - LineDetectorResult[Pos1];
            }
            else if (Direction == 1)
            {
                if (ROI1.Y + CurveFounder(m_image, thresholdLowROI1, thresholdHighROI1, ROI1, Direction, Pos1, Side1) >= ROI2.Y + CurveFounder(m_image, thresholdLowROI2, thresholdHighROI2, ROI2, Direction, Pos2, Side2))
                {
                    if (LineDetectorResult[Pos1] != 10019 && LineDetectorResult[Pos2] != 10019)
                        LineDistanceResult[Pos] = ROI1.Y + LineDetectorResult[Pos1] - ROI2.Y - LineDetectorResult[Pos2];
                    else
                        LineDistanceResult[Pos] = 9999;
                }

                else
                {
                    if (LineDetectorResult[Pos1] != 10019 && LineDetectorResult[Pos2] != 10019)
                        LineDistanceResult[Pos] = ROI2.Y + LineDetectorResult[Pos2] - ROI1.Y - LineDetectorResult[Pos1];
                    else
                        LineDistanceResult[Pos] = 9999;
                }

            }
            return LineDistanceResult[Pos];

        }

        public int CurveDistanceDetector1(Mat Image, ROI ROI1, String Side1, int thresholdLowROI1, int thresholdHighROI1, ROI ROI2, String Side2, int thresholdLowROI2, int thresholdHighROI2, int Direction, int Pos, int Pos1, int Pos2)
        {
            m_image = Image.Clone();
            Rect nRect1 = new Rect();
            nRect1.X =ROI1.X;
            nRect1.Y = ROI1.Y;
            nRect1.Width = ROI1.Width;
            nRect1.Height = ROI1.Height;

            Rect nRect2 = new Rect();
            nRect2.X = ROI2.X;
            nRect2.Y = ROI2.Y;
            nRect2.Width = ROI2.Width;
            nRect2.Height = ROI2.Height;

            if (Direction == 0)
            {
                if (ROI1.X + CurveFounder(m_image, thresholdLowROI1, thresholdHighROI1, nRect1, Direction, Pos1, Side1) >= ROI2.X + CurveFounder(m_image, thresholdLowROI2, thresholdHighROI2, nRect2, Direction, Pos2, Side2))
                    LineDistanceResult[Pos] = ROI1.X + LineDetectorResult[Pos1] - ROI2.X - LineDetectorResult[Pos2];
                else
                    LineDistanceResult[Pos] = ROI2.X + LineDetectorResult[Pos2] - ROI1.X - LineDetectorResult[Pos1];
            }
            else if (Direction == 1)
            {
                if (ROI1.Y + CurveFounder(m_image, thresholdLowROI1, thresholdHighROI1, nRect1, Direction, Pos1, Side1) >= ROI2.Y + CurveFounder(m_image, thresholdLowROI2, thresholdHighROI2, nRect2, Direction, Pos2, Side2))
                {
                    if (LineDetectorResult[Pos1] != 10019 && LineDetectorResult[Pos2] != 10019)
                        LineDistanceResult[Pos] = ROI1.Y + LineDetectorResult[Pos1] - ROI2.Y - LineDetectorResult[Pos2];
                    else
                        LineDistanceResult[Pos] = 9999;
                }

                else
                {
                    if (LineDetectorResult[Pos1] != 10019 && LineDetectorResult[Pos2] != 10019)
                        LineDistanceResult[Pos] = ROI2.Y + LineDetectorResult[Pos2] - ROI1.Y - LineDetectorResult[Pos1];
                    else
                        LineDistanceResult[Pos] = 9999;
                }

            }
            return LineDistanceResult[Pos];

        }

        public int CurveDistanceDetector(Mat Image, ROI ROI1, String Side1, int thresholdLowROI1, int thresholdHighROI1, ROI ROI2, String Side2, int thresholdLowROI2, int thresholdHighROI2, int Direction, int Pos, int Pos1, int Pos2)
        {
            m_image = Image.Clone();

            Rect nRect1 = new Rect();
            nRect1.X = ROI1.X;
            nRect1.Y = ROI1.Width;
            nRect1.Width = ROI1.X;
            nRect1.Height = ROI1.Height;

            Rect nRect2 = new Rect();
            nRect2.X = ROI2.X;
            nRect2.Y = ROI2.Width;
            nRect2.Width = ROI2.X;
            nRect2.Height = ROI2.Height;

            if (Direction == 0)
            {
                if (ROI1.X + CurveFounder(m_image, thresholdLowROI1, thresholdHighROI1, nRect1, Direction, Pos1, Side1) >= ROI2.X + CurveFounder(m_image, thresholdLowROI2, thresholdHighROI2, nRect2, Direction, Pos2, Side2))
                    LineDistanceResult[Pos] = ROI1.X + LineDetectorResult[Pos1] - ROI2.X - LineDetectorResult[Pos2];
                else
                    LineDistanceResult[Pos] = ROI2.X + LineDetectorResult[Pos2] - ROI1.X - LineDetectorResult[Pos1];
            }
            else if (Direction == 1)
            {
                if (ROI1.Y + CurveFounder(m_image, thresholdLowROI1, thresholdHighROI1, nRect1, Direction, Pos1, Side1) >= ROI2.Y + CurveFounder(m_image, thresholdLowROI2, thresholdHighROI2, nRect2, Direction, Pos2, Side2))
                {
                    if (LineDetectorResult[Pos1] != 10019 && LineDetectorResult[Pos2] != 10019)
                        LineDistanceResult[Pos] = ROI1.Y + LineDetectorResult[Pos1] - ROI2.Y - LineDetectorResult[Pos2];
                    else
                        LineDistanceResult[Pos] = 9999;
                }

                else
                {
                    if (LineDetectorResult[Pos1] != 10019 && LineDetectorResult[Pos2] != 10019)
                        LineDistanceResult[Pos] = ROI2.Y + LineDetectorResult[Pos2] - ROI1.Y - LineDetectorResult[Pos1];
                    else
                        LineDistanceResult[Pos] = 9999;
                }

            }
            return LineDistanceResult[Pos];

        }
        public Mat ImageAnalysis(string PathFileImage, int thresholdLow, int thresholdHi, Rect ROI, int iStep)
        {


            m_image = new Mat(PathFileImage);
            MakeROI(m_image, b_image, ROI);
            if (f_GetTiltFromImage(b_image, thresholdLow, thresholdHi, 0, 0, iStep))
            {

            }

            return m_image;
        }
        /// <summary>
        /// ///////////////////
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="thresholdLow"></param>
        /// <param name="thresholdHi"></param>
        /// <param name="NumOfROI"></param>
        /// <param name="ArryROI"></param>
        /// <param name="Direction"></param>
        /// <returns></returns>
        public Mat ImageAnalysisMouseDrop(Mat Image, int thresholdLow, int thresholdHi, int NumOfROI, Rect ArryROI, int Direction)
        {

                if (Image != null)
                {
                    m_image = Image.Clone();

                    if (MainForm.instance.inMouseMove1)
                    {
                        ArryROI.X = Convert.ToInt32(Convert.ToDouble(ArryROI.X) * MainForm.instance.ConvertImageToPictureBox1_W);
                        ArryROI.Y = Convert.ToInt32(Convert.ToDouble(ArryROI.Y) * MainForm.instance.ConvertImageToPictureBox1_H);
                        ArryROI.Width = Convert.ToInt32(Convert.ToDouble(ArryROI.Width) * MainForm.instance.ConvertImageToPictureBox1_W);
                        ArryROI.Height = Convert.ToInt32(Convert.ToDouble(ArryROI.Height) * MainForm.instance.ConvertImageToPictureBox1_H);
                    }


                    X = ArryROI.X;
                    y = ArryROI.Y;
                    W = ArryROI.Width;
                    H = ArryROI.Height;
                    Mat rectROI = new Mat(m_image, ArryROI);

                    //Cv2.ImShow("Test", rectROI);
                    //Cv2.WaitKey(0);

                    m_image = MouseLineFinder(rectROI, thresholdLow, thresholdHi, NumOfROI);
                    if (m_image == null)
                    {
                        strProgStatus = "Waiting";
                        // MainForm.instance.bResetObject = true;
                    }
                    else
                    {
                        strProgStatus = "";
                    }

                }
            m_image = Image.Clone();
            // return m_image;
            return m_image;


        }

        /// <summary>
        /// Calculate Distance from 2 Point............................................................
        public Mat Distance2PointCalculation(Mat Image, ROI ROI1, ROI ROI2, ROI ROI3, ROI ROI4, int Direction, int CameraID, int ResultData,string ImageID)
        {

            int PointNo;
            if (Image != null)
            {
                m_image = Image.Clone();


                /// Finding Line in ROI1................................
                
                PointNo = 1;
                m_image = DistanceLineFinder(m_image, ROI1, PointNo);
                if (m_image == null)
                {
                    strProgStatus = "Waiting";
                    ErrorView = ImageID + "_ROI1" + PointNo + " ERROR";
                    //goto Exit;
                }
                else
                {
                    strProgStatus = "";
                    Image.Line(MinMax_X[MedianFilter] + ROI1.X, MinMax_Y[MedianFilter] + ROI1.Y, MinMax_X[intCountOperation - MedianFilter - 1] + ROI1.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI1.Y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                }
                //Rect RECT = new Rect();
                //RECT.X = Convert.ToInt32(Convert.ToDouble(RECT.X) * MainForm.instance.ConvertImageToPictureBox1_W);
                //RECT.Y = Convert.ToInt32(Convert.ToDouble(RECT.Y) * MainForm.instance.ConvertImageToPictureBox1_H);
                //RECT.Width = Convert.ToInt32(Convert.ToDouble(RECT.Width) * MainForm.instance.ConvertImageToPictureBox1_W);
                //RECT.Height = Convert.ToInt32(Convert.ToDouble(RECT.Height) * MainForm.instance.ConvertImageToPictureBox1_H);

                
                // End finding...........................................


                /// Finding Line in ROI2................................
              //  Rect ROIRect = new Rect();

                PointNo = 2;
                m_image = DistanceLineFinder(m_image, ROI2, PointNo);
                if (m_image == null)
                {
                    strProgStatus = "Waiting";
                    ErrorView = ErrorView + ImageID + "_ROI2" + PointNo + " ERROR";
                    //goto Exit;
                    // MainForm.instance.bResetObject = true;
                }
                else
                {
                    strProgStatus = "";
                    Image.Line(MinMax_X[MedianFilter] + ROI2.X, MinMax_Y[MedianFilter] + ROI2.Y, MinMax_X[intCountOperation - MedianFilter - 1] + ROI2.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI2.Y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                }
                
                // End finding...........................................



                // find the Line in ROI areea............................
                Line line1 = LineFinder.lines[0][0];
                Line line2 = LineFinder.lines[0][1];

                line1.Start.X = L1Start.X;
                line1.Start.Y = L1Start.Y;

                line1.End.X = L1End.X;
                line1.End.Y = L1End.Y;

                line2.Start.X = L2Start.X;
                line2.Start.Y = L2Start.Y;

                line2.End.X = L2End.X;
                line2.End.Y = L2End.Y;
                Point intersection1 = line1.IntersectsWith(line2);
                if (intersection1 == null) goto Exit;
                // End finding................................................'

                /// Finding Line in ROI3................................


                PointNo = 1;
                m_image = DistanceLineFinder(m_image, ROI3, PointNo);
                if (m_image == null)
                {
                    strProgStatus = "Waiting";
                    ErrorView = ErrorView + ImageID + "_ROI2" + PointNo + " ERROR";
                    // goto Exit;
                    // MainForm.instance.bResetObject = true;
                }
                else
                {
                    strProgStatus = "";
                    Image.Line(MinMax_X[MedianFilter] + ROI3.X, MinMax_Y[MedianFilter] + ROI3.Y, MinMax_X[intCountOperation - MedianFilter - 1] + ROI3.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI3.Y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                }
              
                // End finding...........................................


                /// Finding Line in ROI2................................
              //  Rect ROIRect = new Rect();
                PointNo = 2;
                m_image = DistanceLineFinder(m_image, ROI4, PointNo);
                if (m_image == null)
                {
                    strProgStatus = "Waiting";
                    ErrorView = ErrorView + ImageID + "_ROI4" + PointNo + " ERROR";
                    //  goto Exit;
                    // MainForm.instance.bResetObject = true;
                }
                else
                {
                    strProgStatus = "";
                    Image.Line(MinMax_X[MedianFilter] + ROI4.X, MinMax_Y[MedianFilter] + ROI4.Y, MinMax_X[intCountOperation - MedianFilter - 1] + ROI4.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI4.Y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                }
                
                // End finding...........................................



                // find the Line in ROI areea............................
                line1 = LineFinder.lines[0][0];
                line2 = LineFinder.lines[0][1];

                line1.Start.X = L1Start.X;
                line1.Start.Y = L1Start.Y;

                line1.End.X = L1End.X;
                line1.End.Y = L1End.Y;

                line2.Start.X = L2Start.X;
                line2.Start.Y = L2Start.Y;

                line2.End.X = L2End.X;
                line2.End.Y = L2End.Y;
                Point intersection2 = line1.IntersectsWith(line2);
                if(intersection2 ==null) goto Exit;
                // End finding................................................
                if(Direction == 1) //  X_Horizoltal axis........................
                {
                    if(CameraID == 1)
                    {
                        MainForm.instance.Cam1Result[ResultData] = Convert.ToInt16(intersection2.X - intersection1.X);
                    }
                    else if(CameraID==2)
                    {
                        MainForm.instance.Cam2Result[ResultData] = Convert.ToInt16(intersection2.X - intersection1.X);
                    }
                    else if(CameraID==3)
                    {
                        MainForm.instance.Cam3Result[ResultData] = Convert.ToInt16(intersection2.X - intersection1.X);
                    }
                    
                }
                else if(Direction == 0)//  X_Horizoltal axis........................
                {
                    if (CameraID == 1)
                    {
                        MainForm.instance.Cam1Result[ResultData] = Convert.ToInt16(intersection2.Y - intersection1.Y);
                    }
                    else if (CameraID == 2)
                    {
                        MainForm.instance.Cam2Result[ResultData] = Convert.ToInt16(intersection2.Y - intersection1.Y);
                    }
                    else if (CameraID == 3)
                    {
                        MainForm.instance.Cam3Result[ResultData] = Convert.ToInt16(intersection2.Y - intersection1.Y);
                    }
                }

                Image.Line(Convert.ToInt16(intersection1.X), Convert.ToInt16(intersection1.Y), Convert.ToInt16(intersection2.X), Convert.ToInt16(intersection2.Y), Scalar.Red, 10);
               // Cv2.Rectangle(Image,ROI1, Scalar.Red);
               // Image.ToBitmap().Save(ImageID , ImageFormat.Bmp);
            }
            //Cv2.ImShow("Final Image", Image);
            //Cv2.WaitKey(0);
            //m_image = Image.Clone();
            Exit:;
            return Image;
        }
        /// </summary>



        private Mat MouseLineFinder(Mat Image, int thresholdLow, int thresholdHigh, int NumberImage)
        {
            Mat m_image = null;
            if (Image.Width == 0 || Image.Height == 0) goto Exit;
            m_image = Image.Clone();
            mCountObject = 0;
            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];


            int[] MinMax = new int[1000];
            int[] MinMax_X = new int[1000];
            int[] MinMax_Y = new int[1000];
            int[] MinMax_X2 = new int[1000];
            int[] MinMax_Y2 = new int[1000];

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);




            Mat iThresholeLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThresholeHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThreshole = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            Mat iCanyLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCanyHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCany = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat ImageTest = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            // ROI clear bolder of image...........................................
            Rect ROITest = new Rect();
            ROITest.X = 1;
            ROITest.Y = 1;
            ROITest.Height = m_image.Height - 2;
            ROITest.Width = m_image.Width - 2;
            //......................................................................

            TestImage = m_image.Clone();
            iThresholeLow = m_image.Clone();
            iThresholeHigh = m_image.Clone();

            // Filting Image.............................................................................................
            Cv2.CvtColor(TestImage, TestImage, ColorConversionCodes.BGRA2GRAY);
            //Cv2.ImShow("Test", TestImage);
            //Cv2.WaitKey(1);

            Cv2.Blur(TestImage, TestImage, new OpenCvSharp.Size(1, 1));
            Cv2.GaussianBlur(TestImage, TestImage, new OpenCvSharp.Size(3, 3), 0);
            Cv2.MedianBlur(TestImage, TestImage, 1);
            Cv2.Threshold(TestImage, iThresholeLow, thresh: thresholdLow, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.Threshold(TestImage, iThresholeHigh, thresh: thresholdHigh, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.BitwiseAnd(iThresholeHigh, ~iThresholeLow, TestImage);
            ViewSet = m_image.Clone();
            //Cv2.ImShow("Test", TestImage);
            //Cv2.WaitKey(0);
            int intCountOperation = 0;
            int intTemp = 0;
            int bDir = 0;
            if (m_image.Width > m_image.Height)
                bDir = 1;
            else
                bDir = 2;

               contours = Cv2.FindContoursAsArray(TestImage, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
              if (contours.Length == 0)
              {
                  return MainForm.instance.m_MouserImage;
            }


              for (int i = 0; i < contours.Length; i++)
              {
                  double area = Cv2.ContourArea(contours[i]);  //  Find the area of contour
                  OpenCvSharp.Rect uRect = Cv2.BoundingRect(contours[i]);


                //  Cv2.DrawContours(MainForm.instance.m_image, contours, Scalar red);
                if (area > 200)
                {
                    for (int j = 0; j < contours[i].Length; j++)
                    {
                        MinMax_X[intCountOperation] = contours[i][j].X;
                        MinMax_Y[intCountOperation] = contours[i][j].Y;
                        intCountOperation++;
                    }

                    Cv2.DrawContours(ViewSet, contours, i, Scalar.Red, 5);
                    Cv2.Rectangle(ViewSet, uRect, Scalar.White, 2);

                }
                    
              //    LineSegmentPolar[] segStd = Cv2.HoughLines(ViewSet, 1, Math.PI / 180, 50, 0, 0);
              }
            int intTemp_X;
            //////////////////////////////////////////////// get Y..................////////////////////////
            if (bDir == 2)
            {
                for (int i = 0; i < intCountOperation; i++)
                {
                    for (int j = i + 1; j < intCountOperation; j++)
                    {
                        if (MinMax_Y[j] < MinMax_Y[i])

                        {
                            intTemp = MinMax_Y[i];
                            intTemp_X = MinMax_X[i];

                            MinMax_Y[i] = MinMax_Y[j];
                            MinMax_X[i] = MinMax_X[j];



                            MinMax_Y[j] = intTemp;
                            MinMax_X[j] = intTemp_X;
                        }
                    }
                }
            }

            ////////////////////////////////////////////// get X..................////////////////////////
            if (bDir == 1)
            {
                for (int i = 0; i < intCountOperation; i++)
                {
                    for (int j = i + 1; j < intCountOperation; j++)
                    {
                        if (MinMax_X[j] < MinMax_X[i])

                        {
                            intTemp = MinMax_Y[i];
                            intTemp_X = MinMax_X[i];

                            MinMax_Y[i] = MinMax_Y[j];
                            MinMax_X[i] = MinMax_X[j];



                            MinMax_Y[j] = intTemp;
                            MinMax_X[j] = intTemp_X;
                        }
                    }
                }
            }

                
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (bDir == 1)
            {
                ViewSet.Line(MinMax_X[0], 0, MinMax_X[0], ViewSet.Height, Scalar.Green, 3, LineTypes.AntiAlias, 0);
                ViewSet.Line(MinMax_X[intCountOperation - 1], 0, MinMax_X[intCountOperation - 1], ViewSet.Height, Scalar.Green, 3, LineTypes.AntiAlias, 0);

                XGap = MinMax_X[intCountOperation - 1] - MinMax_X[0];

                // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMin.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMax.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
            }
            else if (bDir == 2)
            {
                ViewSet.Line(0, MinMax_Y[0], ViewSet.Width, MinMax_Y[0], Scalar.Yellow, 3, LineTypes.AntiAlias, 0);
                ViewSet.Line(0, MinMax_Y[intCountOperation - 1], ViewSet.Width, MinMax_Y[intCountOperation - 1], Scalar.Yellow, 3, LineTypes.AntiAlias, 0);

                YGap = MinMax_Y[intCountOperation - 1] - MinMax_Y[0]; ;

                //MainForm.instance.m_MouserImage.Line(MainForm.instance.m_MouserImage.Width + PXMin.X - ViewSet.Width, y, PXMin.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMax.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
            }

        
              ///////////HoughLine Detection.......................
        /*      LineSegmentPolar[] segStd = Cv2.HoughLines(TestImage, 1, Math.PI / 180, 50, 0, 0);
              int limit = Math.Min(segStd.Length, 50);
              OpenCvSharp.Point PXMin = new OpenCvSharp.Point();
              OpenCvSharp.Point PXMax = new OpenCvSharp.Point();
              OpenCvSharp.Point PYMin = new OpenCvSharp.Point();
              OpenCvSharp.Point PYMax = new OpenCvSharp.Point();

              PXMin.X = 1000;
              PXMin.Y = 1000;

              for (int i = 0; i < limit; i++)
              {
                  if(segStd[i].Theta > 3 && segStd[i].Theta < 3.3) // chieu doc                                                       
                  {
                      float rho = segStd[i].Rho;
                      float theta = segStd[i].Theta;
                      double a = Math.Cos(theta);
                      double b = Math.Sin(theta);
                      double x0 = a * rho;
                      double y0 = b * rho;
                      OpenCvSharp.Point pt1 = new OpenCvSharp.Point(Math.Round(x0 + 1000 * (-b)), Math.Round(y0 + 1000 * (a)));
                      OpenCvSharp.Point pt2 = new OpenCvSharp.Point { X = (int)Math.Round(x0 - 1000 * (-b)), Y = (int)Math.Round(y0 - 1000 * (a)) };
                      //ViewSet.Line(pt1, pt2, Scalar.White, 3, LineTypes.AntiAlias, 0);
                      bDir = 1;
                      if(pt1.X < PXMin.X && pt1.X != 0)
                      {
                          PXMin.X = pt1.X; PXMin.Y = pt1.Y;
                      }
                      if (pt2.X < PXMin.X && pt2.X != 0)
                      {
                          PXMin.X = pt2.X; PXMin.Y = pt2.Y;
                      }
                      if (pt1.X > PXMax.X)
                      {
                          PXMax.X = pt1.X; PXMax.Y = pt1.Y;
                      }
                      if (pt2.X > PXMax.X)
                      {
                          PXMax.X = pt2.X; PXMax.Y = pt2.Y;
                      }

                      MinMax_Y[i] = pt1.Y;
                      MinMax_Y2[i] = pt2.Y;                                                                               
                  }
                  else if (segStd[i].Theta < 1.6 && segStd[i].Theta > 1.55) // chieu ngang
                  {
                      float rho = segStd[i].Rho;
                      float theta = segStd[i].Theta;
                      double a = Math.Cos(theta);
                      double b = Math.Sin(theta);
                      double x0 = a * rho;
                      double y0 = b * rho;
                      OpenCvSharp.Point pt1 = new OpenCvSharp.Point(Math.Round(x0 + ViewSet.Width * (-b)), Math.Round(y0 + ViewSet.Height * (a)));
                      OpenCvSharp.Point pt2 = new OpenCvSharp.Point { X = (int)Math.Round(x0 - ViewSet.Width * (-b)), Y = (int)Math.Round(y0 - ViewSet.Height * (a)) };

                      MinMax_X[i] = pt1.Y;
                      MinMax_X2[i] = pt2.Y;
                      ViewSet.Line(pt1, pt2, Scalar.White, 3, LineTypes.AntiAlias, 0);
                      bDir = 2;

                      if (pt1.Y < PXMin.Y && pt1.Y != 0)                                       
                      {
                          PXMin.Y = pt1.Y; PXMin.X = pt1.X;
                      }
                      if (pt2.Y < PXMin.Y && pt2.Y != 0)
                      {
                          PXMin.Y = pt2.Y; PXMin.X = pt2.X;
                      }
                      if (pt1.Y > PXMax.Y)
                      {
                          PXMax.Y = pt1.Y; PXMax.X = pt1.X;
                      }
                      if (pt2.Y > PXMax.Y)
                      {
                          PXMax.Y = pt2.Y; PXMax.X = pt2.X;
                      }
                  }
                  else
                  {
                      //  bDir = 0;
                      float rho = segStd[i].Rho;
                      float theta = segStd[i].Theta;
                      double a = Math.Cos(theta);
                      double b = Math.Sin(theta);
                      double x0 = a * rho;
                      double y0 = b * rho;
                      OpenCvSharp.Point pt1 = new OpenCvSharp.Point(Math.Round(x0 + ViewSet.Width * (-b)), Math.Round(y0 + ViewSet.Height * (a)));
                      OpenCvSharp.Point pt2 = new OpenCvSharp.Point { X = (int)Math.Round(x0 - ViewSet.Width * (-b)), Y = (int)Math.Round(y0 - ViewSet.Height * (a)) };

                      MinMax_X[i] = pt1.Y;
                      MinMax_X2[i] = pt2.Y;
                      ViewSet.Line(pt1, pt2, Scalar.White, 3, LineTypes.AntiAlias, 0);

                  }

                  // Draws result lines

              }
              if(bDir == 1)
              {
                  ViewSet.Line(PXMin.X, 0, PXMin.X, ViewSet.Height, Scalar.Pink, 3, LineTypes.AntiAlias, 0);
                  ViewSet.Line(PXMax.X, 0, PXMax.X, ViewSet.Height, Scalar.Pink, 3, LineTypes.AntiAlias, 0);

                  XGap = PXMax.X - PXMin.X;

                 // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMin.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                 // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMax.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
              }
              else if(bDir == 2)
              {
                 ViewSet.Line(0, PXMin.Y, ViewSet.Width ,PXMin.Y, Scalar.Blue, 1, LineTypes.AntiAlias, 0);
                  ViewSet.Line(0, PXMax.Y, ViewSet.Width, PXMax.Y, Scalar.Blue, 1, LineTypes.AntiAlias, 0);

                  YGap = PXMax.Y - PXMin.Y;

                 //MainForm.instance.m_MouserImage.Line(MainForm.instance.m_MouserImage.Width + PXMin.X - ViewSet.Width, y, PXMin.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                 // MainForm.instance.m_MouserImage.Line(PXMin.X, y, PXMax.X + X, ViewSet.Height + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
              }


              for (int i = 0; i < limit; i++) // Ghep mang
              {
                  if(bDir == 1)
                  {
                      MinMax_X[limit + i] = MinMax_X2[i];

                  }
                  else if(bDir == 2)
                  {
                      MinMax_Y[limit + i] = MinMax_Y2[i];
                  }
              }

               //(4) Run Probabilistic Hough Transform
              //LineSegmentPoint[] segProb = Cv2.HoughLinesP(TestImage, 1, Math.PI / 180, 50, 10, 5);
              //foreach (LineSegmentPoint s in segProb)
              //{
              //    ViewSet.Line(s.P1, s.P2, Scalar.White, 3, LineTypes.AntiAlias, 0);
              //}
              /////////////End function..................................
              //...............line Finder.........................
            //  int intTemp_X;
              for (int i = 0; i < 2*limit; i++)
              {
                  for (int j = i + 1; j < 2 * limit; j++)
                  {
                      if(bDir == 2)
                      {
                          if (MinMax_Y[j] < MinMax_Y[i])

                          {
                              intTemp = MinMax_Y[i];
                              MinMax_Y[i] = MinMax_Y[j];
                              MinMax_Y[j] = intTemp;
                          }
                      }
                      else if(bDir == 1)
                      {
                          if (MinMax_X[j] < MinMax_X[i])

                          {
                              intTemp = MinMax_X[i];
                              MinMax_X[i] = MinMax_X[j];
                              MinMax_X[j] = intTemp;
                          }
                      }

                  }
              }
              if(bDir == 1)
              {
                //  MainForm.instance.m_image.Line( X, MinMax_X[0] + y, MinMax_X[limit - 1] + X, MinMax_Y[limit - 1] + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
              }

              else if(bDir == 2)
              {
                 // MainForm.instance.m_image.Line(MinMax_X[0] + X, MinMax_Y[0] + y, MinMax_X[limit - 1] + X, MinMax_Y[limit - 1] + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
                 // MainForm.instance.m_image.Line(MinMax_X[0] + X, MinMax_Y[0] + y, MinMax_X[limit - 1] + X, MinMax_Y[limit - 1] + y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
              }

           
        /*  if (intCountOperation == 0)
          {
              return null;
          }
          if (MainForm.instance.MouseClickCount1 == 1 || MainForm.instance.MouseClickCount2 == 1|| MainForm.instance.MouseClickCount3 == 1)
          {

              L1Start = new System.Drawing.Point(MinMax_X[0] + X, MinMax_Y[0] + y);
              L1End = new System.Drawing.Point(MinMax_X[intCountOperation - 1] + X, MinMax_Y[intCountOperation - 1] + y);


          }
          else if (MainForm.instance.MouseClickCount1 == 2 || MainForm.instance.MouseClickCount2 == 2 || MainForm.instance.MouseClickCount3 == 2)
          {

              L2Start = new System.Drawing.Point(MinMax_X[0] + X, MinMax_Y[0] + y);
              L2End = new System.Drawing.Point(MinMax_X[intCountOperation - 1] + X, MinMax_Y[intCountOperation - 1] + y);
          }
          */
        //return m_image;
        Exit:;
            return m_image;
        }

        private Mat DistanceLineFinder(Mat Image, ROI ROI,int Point)
        {

            Rect ROIRect = new Rect();
            ROIRect.X = ROI.X;
            ROIRect.Y = ROI.Y;
            ROIRect.Width = ROI.Width;
            ROIRect.Height = ROI.Height;
            if (Image == null) goto Exit;
            Mat m_image = new Mat(Image, ROIRect);

            //Mat m_image = null;
            //m_image = Image.Clone();
            mCountObject = 0;
            mValue_Width = new int[TotalCharacterStream];
            mValue_Length = new int[TotalCharacterStream];
            mValue_X = new int[TotalCharacterStream];
            mValue_Y = new int[TotalCharacterStream];


           

            Mat TestImage = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);




            Mat iThresholeLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThresholeHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iThreshole = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            Mat iCanyLow = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCanyHigh = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat iCany = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);
            Mat ImageTest = Mat.Zeros(m_image.Size(), MatType.CV_8UC1);

            // ROI clear bolder of image...........................................
            Rect ROITest = new Rect();
            ROITest.X = 1;
            ROITest.Y = 1;
            ROITest.Height = m_image.Height - 2;
            ROITest.Width = m_image.Width - 2;
            //......................................................................

            TestImage = m_image.Clone();
            iThresholeLow = m_image.Clone();
            iThresholeHigh = m_image.Clone();

            // Filting Image.............................................................................................
            Cv2.CvtColor(TestImage, TestImage, ColorConversionCodes.BGRA2GRAY);


            Cv2.Blur(TestImage, TestImage, new OpenCvSharp.Size(1, 1));
            Cv2.GaussianBlur(TestImage, TestImage, new OpenCvSharp.Size(3, 3), 0);
            Cv2.MedianBlur(TestImage, TestImage, 1);
            Cv2.Threshold(TestImage, iThresholeLow, thresh: ROI.ThresLow, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.Threshold(TestImage, iThresholeHigh, thresh: ROI.ThresHigh, maxval: 255, type: ThresholdTypes.Binary);
            Cv2.BitwiseAnd(iThresholeHigh, ~iThresholeLow, TestImage);

            //Cv2.ImShow("Test", TestImage);
            //Cv2.WaitKey(0);
            intCountOperation = 0;
            contours = Cv2.FindContoursAsArray(TestImage, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0)
            {
                return null;
            }
            int intTemp = 0;

            for (int i = 0; i < contours.Length; i++)
            {
                double area = Cv2.ContourArea(contours[i]);  //  Find the area of contour
                OpenCvSharp.Rect uRect = Cv2.BoundingRect(contours[i]);


                for (int j = 0; j < contours[i].Length; j++)
                {
                    MinMax_X[intCountOperation] = contours[i][j].X;
                    MinMax_Y[intCountOperation] = contours[i][j].Y;
                    intCountOperation++;
                }
            }
            //...............line Finder.........................
            int intTemp_X;
            for (int i = 0; i < intCountOperation; i++)
            {
                for (int j = i + 1; j < intCountOperation; j++)
                {
                    if (MinMax_Y[j] < MinMax_Y[i])

                    {
                        intTemp = MinMax_Y[i];
                        intTemp_X = MinMax_X[i];

                        MinMax_Y[i] = MinMax_Y[j];
                        MinMax_X[i] = MinMax_X[j];



                        MinMax_Y[j] = intTemp;
                        MinMax_X[j] = intTemp_X;
                    }
                }
            }

           

            if (intCountOperation == 0)
            {
                return null;
            }
            if (Point ==1)
            {

                //L1Start = new System.Drawing.Point(MinMax_X[0] + ROI.X, MinMax_Y[0] + ROI.Y);
                //L1End = new System.Drawing.Point(MinMax_X[intCountOperation - 1] + ROI.X, MinMax_Y[intCountOperation - 1] + ROI.Y);MedianFilter
                L1Start = new System.Drawing.Point(MinMax_X[MedianFilter] + ROI.X, MinMax_Y[MedianFilter] + ROI.Y);
                L1End = new System.Drawing.Point(MinMax_X[intCountOperation - MedianFilter - 1] + ROI.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI.Y); 

            }
            else if (Point ==2)
            {

                //L2Start = new System.Drawing.Point(MinMax_X[0] + ROI.X, MinMax_Y[0] + ROI.Y);
                //L2End = new System.Drawing.Point(MinMax_X[intCountOperation - 1] + ROI.X, MinMax_Y[intCountOperation - 1] + ROI.Y);
                L2Start = new System.Drawing.Point(MinMax_X[MedianFilter] + ROI.X, MinMax_Y[MedianFilter] + ROI.Y);
                L2End = new System.Drawing.Point(MinMax_X[intCountOperation - MedianFilter - 1] + ROI.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI.Y);
            }

            //return m_image;
            // Image.Line(MinMax_X[0] + ROI.X, MinMax_Y[0] + ROI.Y, MinMax_X[intCountOperation - 1] + ROI.X, MinMax_Y[intCountOperation - 1] + ROI.Y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
            Image.Line(MinMax_X[MedianFilter] + ROI.X, MinMax_Y[MedianFilter] + ROI.Y, MinMax_X[intCountOperation - MedianFilter - 1] + ROI.X, MinMax_Y[intCountOperation - MedianFilter - 1] + ROI.Y, Scalar.Yellow, 10, LineTypes.AntiAlias, 0);
        Exit:;       return Image;
        }

        public Mat CircleDraw(Mat Image, int X, int Y, int Radius)
        {
            Image.Circle(X, Y, Radius, Scalar.Red, 4);
            return Image;
        }

    }
    public class ROI
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ThresLow { get; set; }
        public int ThresHigh { get; set; }
    }
    public class Distance
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ThresLow { get; set; }
        public int ThresHigh { get; set; }
    }

}
