using HalconDotNet;
using PylonC.NET;
using PylonC.NETSupportLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Auto_Attach
{ 

    public class BaslerCamera
    {
        private ImageProvider m_imageProvider = new ImageProvider();
        PYLON_DEVICE_HANDLE[] hDev = new PYLON_DEVICE_HANDLE[NUM_DEVICES];
        const uint NUM_DEVICES = 3;
        private BaslerCamera BaslerCam;
        object sny_Obj = new object();
        public bool bGrabDone = false;
        public Bitmap m_bitmap = null;
        public HObject mhalcon_image1;
        public HObject mhalcon_image2;
        public Bitmap mhalcon_image3;
        public Bitmap mhalcon_image4;
        public Byte[] imgPtr;
        public delegate void ImageReadyEventHandler();
        public event ImageReadyEventHandler ImageReadyEvent;
        public uint numDevices = 0;
        protected PYLON_DEVICE_INFO_HANDLE hDi;
        protected Thread m_grabThread;
        public bool m_grabThreadRun = false;
       
        public static bool IsInitPylon = false;
        public String Name = "";
        public string cameraID = "Glass MEG 2 (22078271)";

        public delegate void OnImageReady(HObject myImage);
        public event OnImageReady OnImageReadyFunction;
        public Bitmap ImageBuffer;

        //HalconInterface myInterface = null;

        public HObject myImage = null;
        public DPoint CaliValue = new DPoint();
        public int threshold = 30;

        public double Pixel_MM = 0;



        public bool bUIRefreshDone = false;
        public int ImageWidth = 0;
        public int ImageHeight = 0;
        public bool bSaveImage = false;
        List<string> BasCamList = new List<string>();
        public int ThreCamLowSet;
        public int ThreCamHighSet;
        public int ExpCamTimeSet;
        public double CalibrationX;
        public double CalibrationY;


        public  BaslerCamera(String name)
        {
            Name = name;
            Init_Camera_Callback();

            if (!IsInitPylon)
            {
                Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "300000" /*ms*/);
                PylonC.NET.Pylon.Initialize();
                IsInitPylon = true;
            }
            numDevices = PylonC.NET.Pylon.EnumerateDevices();
            mhalcon_image2 = new HObject();
            mhalcon_image1 = new HObject();
        }
        ~BaslerCamera()
        {
            Stop();
            CloseCamera();
        }

        
        public bool ConnectBaslerCam()
        {
            bool res = false;
            if (cameraID == "")
                return res;

            for (int i = 0; i < 3; i++)
            {
                res = OpenCameraByCameraID(cameraID);
                if (res)
                    break;
            }
           // ImageWidth = (int)2592;
           // ImageHeight = (int)1944;
            return res;
        }


        public void AssignBaslerCamera(string camID)
        {
            for (int deviceIndex = 0; deviceIndex < NUM_DEVICES; ++deviceIndex)
            {
                hDev[deviceIndex] = new PYLON_DEVICE_HANDLE();
            }

            if (camID == "")
                return;
            cameraID = camID;

            if (!ConnectBaslerCam())
            {
                MessageBox.Show(Name + "Camera ID:" + camID + " Not Found.");
                
                
            }
        }
       

        public bool IsOpen
        {
            get { return m_imageProvider.IsOpen; }
        }

        public bool StartGrabbing()
        {
            bGrabDone = false;
            OneShot();
            DateTime st_time = DateTime.Now;
            TimeSpan time_span;
            while (!bGrabDone)
            {
                Thread.Sleep(5);
                Application.DoEvents();
                time_span = DateTime.Now - st_time;
                if (time_span.TotalMilliseconds > 1000)
                {
                    return false;
                }
            }
            return true;
        }
        private void Init_Camera_Callback()
        {
            try
            {
                /* Register for the events of the image provider needed for proper operation. */
                m_imageProvider.GrabErrorEvent += new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
                m_imageProvider.DeviceRemovedEvent += new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
                m_imageProvider.DeviceOpenedEvent += new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
                m_imageProvider.DeviceClosedEvent += new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
                m_imageProvider.GrabbingStartedEvent += new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
                m_imageProvider.ImageReadyEvent += new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
                m_imageProvider.GrabbingStoppedEvent += new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void Release_PR_Camera_Callback()
        {
            try
            {
                //lock (sny_Obj)
                {
                    /* Register for the events of the image provider needed for proper operation. */
                    m_imageProvider.GrabErrorEvent -= new ImageProvider.GrabErrorEventHandler(OnGrabErrorEventCallback);
                    m_imageProvider.DeviceRemovedEvent -= new ImageProvider.DeviceRemovedEventHandler(OnDeviceRemovedEventCallback);
                    m_imageProvider.DeviceOpenedEvent -= new ImageProvider.DeviceOpenedEventHandler(OnDeviceOpenedEventCallback);
                    m_imageProvider.DeviceClosedEvent -= new ImageProvider.DeviceClosedEventHandler(OnDeviceClosedEventCallback);
                    m_imageProvider.GrabbingStartedEvent -= new ImageProvider.GrabbingStartedEventHandler(OnGrabbingStartedEventCallback);
                    m_imageProvider.ImageReadyEvent -= new ImageProvider.ImageReadyEventHandler(OnImageReadyEventCallback);
                    m_imageProvider.GrabbingStoppedEvent -= new ImageProvider.GrabbingStoppedEventHandler(OnGrabbingStoppedEventCallback);

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void OnGrabErrorEventCallback(Exception grabException, string additionalErrorMessage)
        {
        }
        private void OnDeviceClosedEventCallback()
        {
        }
        private void OnGrabbingStartedEventCallback()
        {
        }
        private void OnDeviceOpenedEventCallback()
        {

        }
        private void OnDeviceRemovedEventCallback()
        {
        }
        protected void OnImageReadyEvent()
        {
            for (  int i = 0; i < 10; i++)
            {
                if (ImageReadyEvent != null)
                {
                    ImageReadyEvent();
                    Thread.Sleep(5);
                    Application.DoEvents();
                }
                else
                {
                    bGrabDone = true;
                    //Thread.Sleep(5);
                    break;
                }
            }
           
        }

        public int ImgHeight = 0;
        public int ImgWidth = 0;
        /* Handles the event related to an image having been taken and waiting for processing. */
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        public void OnImageReadyEventCallback()
        {
            try
            {
                lock (sny_Obj)
                {
                    //bGrabDone = false;
                    System.Threading.Thread.Sleep(5);
                    /* Acquire the image from the image provider. Only show the latest image. The camera may acquire images faster than images can be displayed*/
                    ImageProvider.Image image = m_imageProvider.GetLatestImage();

                    /* Check if the image has been removed in the meantime. */
                    if (image != null)
                    {
                       // mhalcon_image2.GenEmptyObj();
                       // mhalcon_image1.GenEmptyObj();

                        if (IsCompatible(m_bitmap, image.Width, image.Height, image.Color))
                        {
                            BitmapFactory.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                       
                            ImageBuffer = m_bitmap;
                            // mCGigecamera.GenertateGrayBitmap(mhalcon_image, out mm_bitmap);
                            /* To show the new image, request the display control to update itself. */
                            //pictureBox1.Refresh();

                        }
                        else
                        {
                 
                            BitmapFactory.CreateBitmap(out m_bitmap, image.Width, image.Height, image.Color);
                            BitmapFactory.UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                           
                            ImageBuffer = m_bitmap;
                            // mhalcon_image2 = UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);
                            //  mhalcon_image1 = UpdateBitmap(m_bitmap, image.Buffer, image.Width, image.Height, image.Color);

                        }
                        ImgHeight = image.Height;
                        ImgWidth = image.Width;
                       
                       // ImageBuffer.Save("ImageBuff.bmp", ImageFormat.Bmp);
                        /* The processing of the image is done. Release the image buffer. */
                        m_imageProvider.ReleaseImage();
                        //BitmapData bmpData = m_bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        /* Get the pointer to the bitmap's buffer. */
                        //IntPtr ptrBmp = bmpData.Scan0;
                        //Bitmap bitmapOld = ImageBuffer as Bitmap;
                        if (ImageBuffer != null)
                        {
                           
                        // ImageBuffer.Dispose();
                        }

                        OnImageReadyEvent();
                    }
                    
                    // 20-01 prevent loading old image
                    while (!bGrabDone)
                       Thread.Sleep(10);
                        

                    Thread.Sleep(10);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                ///l
                ///be.DisposeGrabResultIfClone();
                //
                if (ImageBuffer != null)
                {
                    // Dispose the bitmap.
                    //ImageBuffer.Dispose();
                    bGrabDone = true;
                }
                
            }
        }
        /// ////////////////////////////////////////////////////////////////////////////
        /// </summary>

        public int MyImgStride = 0;
 
        #region
        //Bitmap 转halcon函数
        //public HObject HImageConvertFromBitmap8(Bitmap bmp)
        //{
        //    HObject ho_Image;
        //    HOperatorSet.GenEmptyObj(out ho_Image);
        //    unsafe
        //    {
        //        BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
        //        unsafe
        //        {
        //           // HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0, "bgrx", bmp.Width, bmp.Height, -1, "byte", bmp.Width, bmp.Height, 0, 0, -1, 0);
        //           HOperatorSet.GenImage1(out ho_Image, "byte", bmp.Width, bmp.Height, bmpData.Scan0);

        //        }
        //        bmp.UnlockBits(bmpData);
        //        return ho_Image;
        //    }

        //}
        #endregion


        /* Creates a new bitmap object with the supplied properties. */
        public void CreateBitmap(out Bitmap bitmap, int width, int height, bool color)
        {
            bitmap = new Bitmap(width, height, GetFormat(color));

            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                ColorPalette colorPalette = bitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    colorPalette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bitmap.Palette = colorPalette;
            }
        }
        public static bool IsCompatible(Bitmap bitmap, int width, int height, bool color) //比较图像属性函数
        {
            if (bitmap == null
                || bitmap.Height != height
                || bitmap.Width != width
                || bitmap.PixelFormat != GetFormat(color)
             )
            {
                return false;
            }
            return true;
        }

        private static PixelFormat GetFormat(bool color)
        {
            return color ? PixelFormat.Format32bppRgb : PixelFormat.Format8bppIndexed;
        }

        private static int GetStride(int width, bool color)
        {
            return color ? width * 4 : width;
        }
        /* Handles the event related to the image provider having stopped grabbing. */
        private void OnGrabbingStoppedEventCallback()
        {
        }

        /* Handles the event related to a device being open. */
       

        public void CloseCamera()        
        {
            Application.DoEvents();
            Thread.Sleep(50);
            m_imageProvider.Stop();
            Application.DoEvents();
            Thread.Sleep(50);
            m_imageProvider.Close();
        }
        public void CloseCam()
        {
            Application.DoEvents();
            Thread.Sleep(50);
            m_imageProvider.Stop();
            Application.DoEvents();
            Thread.Sleep(50);
            m_imageProvider.Close();
        }

        public bool OpenCameraByCameraID(string CamID)
        {
            bool res = false;
            
                int camIdx = GetCameraID(CamID);
                if (camIdx == -1)
                    return res;

                try
                {
                    if (!m_imageProvider.IsOpen)
                    {
                        m_imageProvider.Open((uint)camIdx);                       
                    }
                    res = true;
                    //m_imageProvider.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
                }
                catch (Exception)
                {
                    //ShowException(e, m_imageProvider.GetLastErrorMessage());
                    res = false;
                }
                //res = true;
            
            return res;
        }

        private int GetCameraID(string CamID)
        {
            for (int i = 0; i < numDevices; i++)           {
                if (GetDevicename((uint)i) == CamID)
                {
                    hDev[i] = PylonC.NET.Pylon.CreateDeviceByIndex((uint)i);
                    bool isAvail = PylonC.NET.Pylon.DeviceFeatureIsAvailable(hDev[i], "EnumEntry_PixelFormat_Mono8");

                    return i;
                }
                    
            }
            return -1;
        }
        public string GetDevicename(uint index) //获取相机名根据找到的设备的ID号
        {
            string devicename = "";

            if (numDevices > 0)
            {
                try
                {
                    hDi = PylonC.NET.Pylon.GetDeviceInfoHandle(index);
                    devicename = PylonC.NET.Pylon.DeviceInfoGetPropertyValueByName(hDi, PylonC.NET.Pylon.cPylonDeviceInfoFriendlyNameKey);
                   
                }
                catch
                {
                    devicename = "Not found the CCD";
                }

            }
            else
            {
                devicename = "Not found the CCD";

            }


            return devicename;
        }

        //public void ContinuousShot()
        //{
        //    try
        //    {
        //        IsLive = true;
        //        m_imageProvider.ContinuousShot(); /* Start the grabbing of images until grabbing is stopped. */
        //    }
        //    catch (Exception)
        //    {
        //        //ShowException(e, m_imageProvider.GetLastErrorMessage());
        //    }
        //}
        public void ContinuousShot()
        {
            if (null == m_grabThread)
            {
                if (m_imageProvider.IsOpen)
                {
                    m_grabThreadRun = true;
                    m_grabThread = new Thread(new ThreadStart(GrabThread));
                    m_grabThread.IsBackground = true;
                    m_grabThread.Start();
                }
            }
            else
            {
                if (m_imageProvider.IsOpen && !m_grabThread.IsAlive)
                {
                    m_grabThreadRun = true;
                    m_grabThread = new Thread(new ThreadStart(GrabThread));
                    m_grabThread.IsBackground = true;
                    m_grabThread.Start();
                }
            }
            IsLive = true;
        }
      
        public void GrabThread()  //采集图像线程函数
        {            
            try
            {
                while (m_grabThreadRun)
                {                    
                    OneShot();
                    
                    //mhalcon_image2.Dispose();
                    Thread.Sleep(15);
                    //Application.DoEvents();
                }                
            }
            catch
            {
                m_grabThreadRun = false;                
            }            
        }

        public bool IsLive = false;
        public void CloseTheImageProvider()
        {
            /* Close the image provider. */
            try
            {
                m_imageProvider.Close();
            }
            catch (Exception e)
            {
                //ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
        }
        public void Stop()
        {
            if (m_grabThread != null)
            {
                if (m_imageProvider.IsOpen) /* Only start when open and grabbing. */
                {
                    if (m_grabThread.IsAlive)
                    {
                        m_grabThreadRun = false; /* Causes the grab thread to stop. */                        
                        m_grabThread.Abort();
                        m_grabThread.Join(); /* Wait for it to stop. */
                    }
                    else
                    {
                        m_grabThread = null;
                    }
                }
            }           
            IsLive = false;            
        }

        public bool OneShot()
        {
            try
            {
                bGrabDone = false;
               
                if (m_imageProvider.IsOpen)
                    m_imageProvider.OneShot();
             

                
                DateTime st_time = DateTime.Now;
                TimeSpan time_span;
                while (!bGrabDone)
                {                    
                    Thread.Sleep(10);
                    Application.DoEvents();
                    time_span = DateTime.Now - st_time;
                    double GetickCount = time_span.TotalMilliseconds;
                    if (time_span.TotalMilliseconds > 1800)
                    {                        
                        bGrabDone = true;
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception)
            {
                //ShowException(e, m_imageProvider.GetLastErrorMessage());
            }
            return true;
        }

      

        public void SetCameraTriggerMode()
        {

            bool bval;
            NODE_HANDLE m_hNode = new NODE_HANDLE();
            m_hNode = m_imageProvider.GetNodeFromDevice("TriggerSelector");
            if (!m_hNode.IsValid)
                return;
            bval = GenApi.NodeIsWritable(m_hNode);
            if (!bval)
                return;
            GenApi.NodeFromString(m_hNode, "FrameStart");

            m_hNode = m_imageProvider.GetNodeFromDevice("TriggerMode");
            if (!m_hNode.IsValid)
                return;
            bval = GenApi.NodeIsWritable(m_hNode);
            if (!bval)
                return;
            GenApi.NodeFromString(m_hNode, "On");

            m_hNode = m_imageProvider.GetNodeFromDevice("TriggerSource");
            if (!m_hNode.IsValid)
                return;
            bval = GenApi.NodeIsWritable(m_hNode);
            if (!bval)
                return;
            GenApi.NodeFromString(m_hNode, "Software");
        }

        public void SetExposureTime(int value)
        {
            bool bval;
            //ExposureTimeRaw
            NODE_HANDLE m_hNode = new NODE_HANDLE();
            m_hNode = m_imageProvider.GetNodeFromDevice("ExposureTimeRaw");
            if (m_hNode.IsValid)
            {
                int inc = checked((int)GenApi.IntegerGetInc(m_hNode));
                int min = checked((int)GenApi.IntegerGetMin(m_hNode));
                int max = checked((int)GenApi.IntegerGetMax(m_hNode));
                int expVal = (value) - (value % inc);
                if (expVal < min)
                    expVal = min;
                if (expVal > max)
                    expVal = max;
                if (!m_hNode.IsValid)
                    return;
                bval = GenApi.NodeIsWritable(m_hNode);
                if (!bval)
                    return;
                GenApi.IntegerSetValue(m_hNode, expVal);
            }
        }

        public void GetGainValue()
        {
            bool bval;
            //ExposureTimeRaw
            NODE_HANDLE m_hNode = new NODE_HANDLE();
            m_hNode = m_imageProvider.GetNodeFromDevice("GainRaw");
            if (m_hNode.IsValid)
            {
              
                if (!m_hNode.IsValid)
                    return;
                bval = GenApi.NodeIsWritable(m_hNode);
                if (!bval)
                    return;
                double result= GenApi.IntegerGetValue(m_hNode);
                Console.WriteLine(result);
            }
        }

        public void LoadSettings(string fileName)
        {
            string strread = "";
            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
            FileOperation.ReadData(fileName, headerStr, "CameraID", ref strread);
            if (strread != "0")
                cameraID = strread;

            FileOperation.ReadData(fileName, headerStr, "CalibrationX", ref strread);
            if (strread != "0")
                CaliValue.X = double.Parse(strread);

            FileOperation.ReadData(fileName, headerStr, "CalibrationY", ref strread);
            if (strread != "0")
                CaliValue.Y = double.Parse(strread);

            FileOperation.ReadData(fileName, headerStr, "Threshold", ref strread);
            if (strread != "0")
                threshold = int.Parse(strread);

            if (cameraID != "NotAssigned")
                AssignBaslerCamera(cameraID);
            else
                MessageBox.Show(Name + " camera Not Assigned.");


            Pixel_MM = (CaliValue.X + CaliValue.Y) / 2;
            /// Loading Cam Name............................
    
            FileOperation.ReadData(fileName, headerStr, "CameraID", ref strread);
            if (strread != "0")
                cameraID = strread;

            FileOperation.ReadData(fileName, headerStr, "CalibrationX", ref strread);
            if (strread != "0")
                CalibrationX = double.Parse(strread);

            FileOperation.ReadData(fileName, headerStr, "CalibrationY", ref strread);
            if (strread != "0")
                CalibrationY = double.Parse(strread);

            FileOperation.ReadData(fileName, headerStr, "Threshold", ref strread);
            if (strread != "0")
                threshold = int.Parse(strread);

            //// Open Cam following Name............................................
            if (cameraID != "NotAssigned")
                AssignBaslerCamera(cameraID);
            else
                MessageBox.Show(Name + " camera Not Assigned.");

            // Loading spectification.............................................
            FileOperation.ReadData(fileName, headerStr, "LowThreshole", ref strread);
            if (strread != "0")
                ThreCamLowSet = int.Parse(strread);

            FileOperation.ReadData(fileName, headerStr, "HighThreholeThreshole", ref strread);
            if (strread != "0")
                ThreCamHighSet = int.Parse(strread);

            FileOperation.ReadData(fileName, headerStr, "Exposure", ref strread);
            if (strread != "0")
                ExpCamTimeSet = int.Parse(strread);

        }


    }
}
