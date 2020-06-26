using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Basler.Pylon;
using HalconDotNet;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;


namespace Auto_Attach
{
    public class BaslerCam
    {
        public Camera camera;
        private PixelDataConverter converter = new PixelDataConverter();

        public string CamID = "";
        public string Name = "";
        public long imageWidth = 0;         // 图像宽
        public long imageHeight = 0;        // 图像高
        public long minExposureTime = 0;    // 最小曝光时间
        public long maxExposureTime = 0;    // 最大曝光时间

        public long ExposureTime = 0; //当前设置的曝光时间
        public long minGain = 0;            // 最小增益
        public long maxGain = 0;            // 最大增益
        public long gain = 0;           //当前设置的增益

        private long grabTime = 0;          // 采集图像时间

        private  HObject hPylonImage=null;
        public HObject himg;
        private IntPtr latestFrameAddress = IntPtr.Zero;
        private Stopwatch stopWatch = new Stopwatch();
        public Bitmap ImageBuffer;
        public Bitmap mhalcon_image4;
        public bool bGrabDone = false;
        public string strPoint;

        
        public string cameraID = "NotAssigned";
    
       // public String Name = "";
        public double CalibrationX;
        public double CalibrationY;
        public double threshold;

        public BaslerCam cameraUSB;



        ///Declaration for Camera.......................
        public int ThreCamLowSet;
    

        public int ThreCamHighSet;


        public int ExpCamTimeSet;





        /// <summary>
        /// 计算采集图像时间自定义委托
        /// </summary>
        /// <param name="time">采集图像时间</param>
        public delegate void delegateComputeGrabTime(long time);
        /// <summary>
        /// 计算采集图像时间委托事件
        /// </summary>
        public event delegateComputeGrabTime eventComputeGrabTime;

        /// <summary>
        /// 图像处理自定义委托
        /// </summary>
        /// <param name="hImage">halcon图像变量</param>
        public delegate void delegateProcessHImage(HObject hImage);


        public delegate void delegateProcessHImagedd(out HObject hDisp, out HObject hScore);
        /// <summary>
        /// 图像处理委托事件
        /// </summary>
        public event delegateProcessHImage eventProcessImage;

        public event delegateProcessHImagedd eventProcessImagedd;

        /// <summary>
        /// if >= Sfnc2_0_0,说明是ＵＳＢ３的相机
        /// </summary>
        static Version Sfnc2_0_0 = new Version(2, 0, 0);


        /******************    实例化相机    ******************/
        /// <summary>
        /// 实例化第一个找到的相机
        /// </summary>
        public BaslerCam(string name)
        {

            Name = name;
          
        }


        /// <summary>
        /// 根据相机UserID实例化相机
        /// </summary>
        /// <param name="UserID"></param>
        
        /*****************************************************/

        /******************    相机操作     ******************/
        /// <summary>
        /// 打开相机
        /// </summary>
        public void OpenCam()
        {
            try
            {


                if (camera.IsOpen)
                {
                    camera.Close();
                }
                if (camera!=null)
                {
                    
                    camera.Open();
                }

                camera.Parameters[PLCamera.Width].SetValue(camera.Parameters[PLCamera.Width].GetMaximum());
                camera.Parameters[PLCamera.Height].SetValue(camera.Parameters[PLCamera.Height].GetMaximum());
                
                camera.Parameters[PLTransportLayer.HeartbeatTimeout].TrySetValue(1000, IntegerValueCorrection.Nearest);  // 1000 ms timeout

                //camera.Parameters[PLCamera.AcquisitionFrameRateEnable].SetValue(true);  // 限制相机帧率
                //camera.Parameters[PLCamera.AcquisitionFrameRateAbs].SetValue(90);
                //camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(10);          // 设置内存中接收图像缓冲区大小

                imageWidth = camera.Parameters[PLCamera.Width].GetValue();               // 获取图像宽 
                imageHeight = camera.Parameters[PLCamera.Height].GetValue();              // 获取图像高
                //GetMinMaxExposureTime();
                //GetExposureTime();
                //GetGain();
                //GetMinMaxGain();
                camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;                      // 注册采集回调函数
                //camera.ConnectionLost += OnConnectionLost;
                //camera.CameraOpened += OnCameraOpened;
                //camera.CameraClosed += OnCameraClosed;
                camera.StreamGrabber.GrabStarted += OnGrabStarted;
              
              //  camera.StreamGrabber.GrabStopped += OnGrabStopped;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //ShowException(e);
            }
        }

        /// <summary>
        /// 关闭相机,释放相关资源
        /// </summary>
        public void CloseCam()
        {
            try
            {
                //camera.Close();
                //camera.Dispose();
                //camera = null;


                if (camera.StreamGrabber.IsGrabbing)
                {
                    camera.StreamGrabber.Stop();
                }
                System.Threading.Thread.Sleep(50);
                //关闭相机;
                if (camera.IsOpen)
                {
                    camera.Close();
                }

                if (hPylonImage != null)
                {
                    hPylonImage.Dispose();
                }

                if (latestFrameAddress != null)
                {
                    Marshal.FreeHGlobal(latestFrameAddress);
                    latestFrameAddress = IntPtr.Zero;
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }


        /// <summary>
        /// 刷新相机
        /// </summary>
        public void refreshCamera()
        {
            // camera.CameraOpened += Configuration.SoftwareTrigger;
            camera.Open(1000, TimeoutHandling.ThrowException);
            // camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
            camera.Parameters[PLTransportLayer.HeartbeatTimeout].TrySetValue(1000, IntegerValueCorrection.Nearest);
            //if (camera.StreamGrabber.IsGrabbing == false)
            //{
            //    camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            //}

        }

        /// <summary>
        /// 单张采集
        /// </summary>
        public bool GrabOne()
        {
            try
            {
                if (camera.StreamGrabber.IsGrabbing)
                {
                    MessageBox.Show("相机当前正处于采集状态！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    camera.Parameters[PLCamera.AcquisitionMode].SetValue("SingleFrame");
                    camera.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                    stopWatch.Restart();    // ****  重启采集时间计时器   ****
                    return true;
                }
            }
            catch (Exception e)
            {
                ShowException(e);
                return false;
            }
        }

        /// <summary>
        /// 开始连续采集
        /// </summary>
        public bool StartGrabbing()
        {
            try
            {
                if (camera.StreamGrabber.IsGrabbing)
                {
                    camera.StreamGrabber.Stop();
                    //MessageBox.Show("相机当前正处于采集状态！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   // return true;
                }
              //  else
              //  {
                    //camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                    camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                    stopWatch.Restart();    // ****  重启采集时间计时器   ****
                    return true;
              //  }
            }
            catch (Exception e)
            {
                ShowException(e);
                return false;
            }
            Thread.Sleep(ExpCamTimeSet / 1000);
        }

        /// <summary>
        /// 停止连续采集
        /// </summary>
        public void StopGrabbing()
        {
            try
            {
                if (camera.StreamGrabber.IsGrabbing)
                {
                    camera.StreamGrabber.Stop();
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }
        /*********************************************************/

        /******************    相机参数设置   ********************/
        /// <summary>
        /// 设置Gige相机心跳时间
        /// </summary>
        /// <param name="value"></param>
        public void SetHeartBeatTime(long value)
        {
            try
            {
                // 判断是否是网口相机
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    camera.Parameters[PLGigECamera.GevHeartbeatTimeout].SetValue(value);
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 设置相机曝光时间
        /// </summary>
        /// <param name="value"></param>
        public void SetExposureTime(long value)
        {
            try
            {
                // Some camera models may have auto functions enabled. To set the ExposureTime value to a specific value,
                // the ExposureAuto function must be disabled first (if ExposureAuto is available).
                camera.Parameters[PLCamera.ExposureAuto].TrySetValue(PLCamera.ExposureAuto.Off); // Set ExposureAuto to Off if it is writable.
                camera.Parameters[PLCamera.ExposureMode].TrySetValue(PLCamera.ExposureMode.Timed); // Set ExposureMode to Timed if it is writable.

                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    // In previous SFNC versions, ExposureTimeRaw is an integer parameter,单位us
                    // integer parameter的数据，设置之前，需要进行有效值整合，否则可能会报错
                    long min = camera.Parameters[PLCamera.ExposureTimeRaw].GetMinimum();
                    long max = camera.Parameters[PLCamera.ExposureTimeRaw].GetMaximum();
                    long incr = camera.Parameters[PLCamera.ExposureTimeRaw].GetIncrement();
                    if (value < min)
                    {
                        value = min;
                        MessageBox.Show("设置值低于此相机的最小曝光时间：" + min + "。已设置为最小曝光时间");

                    }
                    else if (value > max)
                    {
                        value = max;
                        MessageBox.Show("设置值超出此相机的最大曝光时间：" + max + "。已设置为最大曝光时间。");
                    }
                    else
                    {
                        value = min + (((value - min) / incr) * incr);
                    }
                    camera.Parameters[PLCamera.ExposureTimeRaw].SetValue(value);
                    

                    // Or,here, we let pylon correct the value if needed.
                    //camera.Parameters[PLCamera.ExposureTimeRaw].SetValue(value, IntegerValueCorrection.Nearest);
                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    // In SFNC 2.0, ExposureTimeRaw is renamed as ExposureTime,is a float parameter, 单位us.
                    camera.Parameters[PLUsbCamera.ExposureTime].SetValue((double)value);
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 获取最小最大曝光时间
        /// </summary>
        public void GetMinMaxExposureTime()
        {
            try
            {
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    minExposureTime = camera.Parameters[PLCamera.ExposureTimeRaw].GetMinimum();
                    maxExposureTime = camera.Parameters[PLCamera.ExposureTimeRaw].GetMaximum();
                }
                else
                {
                    minExposureTime = (long)camera.Parameters[PLUsbCamera.ExposureTime].GetMinimum();
                    maxExposureTime = (long)camera.Parameters[PLUsbCamera.ExposureTime].GetMaximum();
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 获取当前设置的曝光时间
        /// </summary>
        public void GetExposureTime()
        {
            try
            {
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    ExposureTime = camera.Parameters[PLCamera.ExposureTimeRaw].GetValue();
                }
                else
                {
                    ExposureTime = (long)camera.Parameters[PLUsbCamera.ExposureTime].GetValue();
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="value"></param>
        public void SetGain(long value)
        {
            try
            {
                // Some camera models may have auto functions enabled. To set the gain value to a specific value,
                // the Gain Auto function must be disabled first (if gain auto is available).
                camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off); // Set GainAuto to Off if it is writable.

                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    // Some parameters have restrictions. You can use GetIncrement/GetMinimum/GetMaximum to make sure you set a valid value.                              
                    // In previous SFNC versions, GainRaw is an integer parameter.
                    // integer parameter的数据，设置之前，需要进行有效值整合，否则可能会报错
                    long min = camera.Parameters[PLCamera.GainRaw].GetMinimum();
                    long max = camera.Parameters[PLCamera.GainRaw].GetMaximum();
                    long incr = camera.Parameters[PLCamera.GainRaw].GetIncrement();
                    if (value < min)
                    {
                        value = min;
                        MessageBox.Show("设置值低于此相机的最小增益：" + min + "。已设置为最小增益");

                    }
                    else if (value > max)
                    {
                        value = max;
                        MessageBox.Show("设置值低于此相机的最大增益：" + min + "。已设置为最大增益");

                    }
                    else
                    {
                        value = min + (((value - min) / incr) * incr);
                    }
                    camera.Parameters[PLCamera.GainRaw].SetValue(value);

                    //// Or,here, we let pylon correct the value if needed.
                    //camera.Parameters[PLCamera.GainRaw].SetValue(value, IntegerValueCorrection.Nearest);
                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    // In SFNC 2.0, Gain is a float parameter.
                    camera.Parameters[PLUsbCamera.Gain].SetValue(value);
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }



        public void GetGain()
        {
            try
            {
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    gain = camera.Parameters[PLCamera.GainRaw].GetValue();
                }
                else
                {
                    gain = (long)camera.Parameters[PLUsbCamera.Gain].GetValue();
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 获取最小最大增益
        /// </summary>
        public void GetMinMaxGain()
        {
            try
            {
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    minGain = camera.Parameters[PLCamera.GainRaw].GetMinimum();
                    maxGain = camera.Parameters[PLCamera.GainRaw].GetMaximum();
                }
                else
                {
                    minGain = (long)camera.Parameters[PLUsbCamera.Gain].GetMinimum();
                    maxGain = (long)camera.Parameters[PLUsbCamera.Gain].GetMaximum();
                }
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 设置相机Freerun模式
        /// </summary>
        public void SetFreerun()
        {
            try
            {
                // Set an enum parameter.
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                    }
                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                    }
                }
                stopWatch.Restart();    // ****  重启采集时间计时器   ****
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 设置相机软触发模式
        /// </summary>
        public void SetSoftwareTrigger()
        {
            try
            {
                // Set an enum parameter.
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                    }
                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                    }
                }
                stopWatch.Reset();    // ****  重置采集时间计时器   ****
            }
            catch (Exception e)
            {
                ShowException(e);
            }
        }

        /// <summary>
        /// 发送软触发命令
        /// </summary>
        public void SendSoftwareExecute()
        {
            try
            {
                if (camera.WaitForFrameTriggerReady(1000, TimeoutHandling.ThrowException))
                {
                    camera.ExecuteSoftwareTrigger();
                    stopWatch.Restart();    // ****  重启采集时间计时器   ****
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }

        /// <summary>
        /// 设置相机外触发模式
        /// </summary>
        
        public void SetExternTrigger1()
        {
            IEnumParameter triggerSelector = camera.Parameters[PLCamera.TriggerSelector];
            IEnumParameter triggerMode = camera.Parameters[PLCamera.TriggerMode];
            IEnumParameter triggerSource = camera.Parameters[PLCamera.TriggerSource];

            // Check the available camera trigger mode(s) to select the appropriate one: acquisition start trigger mode
            // (used by older cameras, i.e. for cameras supporting only the legacy image acquisition control mode;
            // do not confuse with acquisition start command) or frame start trigger mode
            // (used by newer cameras, i.e. for cameras using the standard image acquisition control mode;
            // equivalent to the acquisition start trigger mode in the legacy image acquisition control mode).
            string triggerName = "FrameStart";
            if (!triggerSelector.CanSetValue(triggerName))
            {
                triggerName = "AcquisitionStart";
                if (!triggerSelector.CanSetValue(triggerName))
                {
                    throw new NotSupportedException("Could not select trigger. Neither FrameStart nor AcquisitionStart is available.");
                }
            }

            try
            {
                foreach (string trigger in triggerSelector)
                {
                    triggerSelector.SetValue(trigger);

                    if (triggerName == trigger)
                    {
                        // Activate trigger.
                        triggerMode.SetValue(PLCamera.TriggerMode.On);

                        // Set the trigger source to software.
                        triggerSource.SetValue(PLCamera.TriggerSource.Line1);
                    }
                    else
                    {
                        // Turn trigger mode off.
                        triggerMode.SetValue(PLCamera.TriggerMode.Off);
                    }
                }
            }
            finally
            {
                // Set selector for software trigger.
                triggerSelector.SetValue(triggerName);
            }
            // Set acquisition mode to Continuous
            camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
        }
        /****************************************************/


        /****************  图像响应事件函数  ****************/


        // 相机取像回调函数.
        public void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {

           

            try
            {
                // Acquire the image from the camera. Only show the latest image. The camera may acquire images faster than the images can be displayed.

                // Get the grab result.
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.IsValid)
                {
                    // Reduce the number of displayed images to a reasonable amount if the camera is acquiring images very fast.
                    if (!stopWatch.IsRunning || stopWatch.ElapsedMilliseconds > 33)
                    {
                        stopWatch.Restart();

                        Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
                        // Lock the bits of the bitmap.
                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        // Place the pointer to the buffer of the bitmap.
                        converter.OutputPixelFormat = PixelType.BGRA8packed;
                        IntPtr ptrBmp = bmpData.Scan0;
                        converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                        bitmap.UnlockBits(bmpData);

                        // Assign a temporary variable to dispose the bitmap after assigning the new bitmap to the display control.
                        Bitmap bitmapOld = ImageBuffer as Bitmap;
                        // Provide the display control with the new bitmap. This action automatically updates the display.
                        ImageBuffer = bitmap;
                       // ImageBuffer.Save("Buffer", ImageFormat.Bmp);
                       
                        if (bitmapOld != null)
                        {
                            // Dispose the bitmap.
                            bitmapOld.Dispose();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
                bGrabDone = true;
            }
        }


        private void OnGrabStopped(Object sender, GrabStopEventArgs e)
        {
            

            // Reset the stopwatch.
            stopWatch.Reset();
            while (!bGrabDone)
                Thread.Sleep(5);

            // Re-enable the updating of the device list.
            // updateDeviceListTimer.Start();

            // The camera stopped grabbing. Enable the grab buttons. Disable the stop button.
            //EnableButtons(true, false);

            // If the grabbed stop due to an error, display the error message.
            if (e.Reason != GrabStopReason.UserRequest)
            {
                MessageBox.Show("A grab error occured:\n" + e.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OnGrabStarted(Object sender, EventArgs e)
        {


            // Reset the stopwatch used to reduce the amount of displayed images. The camera may acquire images faster than the images can be displayed.

            stopWatch.Reset();

            // Do not update the device list while grabbing to reduce jitter. Jitter may occur because the GUI thread is blocked for a short time when enumerating.

        }
        //unsafe static void OnImageGrabbed2(Object sender, ImageGrabbedEventArgs e)
        //{
        //    // The grab result is automatically disposed when the event call back returns.
        //    // The grab result can be cloned using IGrabResult.Clone if you want to keep a copy of it (not shown in this sample).
        //    try
        //    {
        //        IGrabResult grabResult = e.GrabResult;
        //        // Image grabbed successfully?
        //        if (grabResult.GrabSucceeded)
        //        {
        //            byte[] buffer = grabResult.PixelData as byte[];
        //            fixed (byte* dataGray = buffer)
        //            {
        //                if (hPylonImage != null)
        //                {
        //                    hPylonImage.Dispose();
        //                }

        //                HOperatorSet.GenImage1(out hPylonImage, "byte", (HTuple)grabResult.Width, (HTuple)grabResult.Height, new IntPtr(dataGray));

        //            }
        //            if (grabResult != null)
        //            {
        //                grabResult.Dispose();
        //            }

        //            //camera.StreamGrabber.Stop();
        //            //if (Svision.GetMe().baslerCamera.channelNumber != 1)
        //            //{

        //            //    HOperatorSet.CfaToRgb(hoImage, out hoImage, "bayer_bg", "bilinear");
        //            //}
        //            //isImageOk = true;

        //        }
        //        else
        //        {
        //            throw new Exception(grabResult.ErrorCode.ToString() + grabResult.ErrorDescription);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ShowException(ex)；
        //        //errorImageCode = true;
        //        //errorImageStr = ex.Message;

        //    }
        //}


        public void LoadSettings(string fileName)
        {

            /// Loading Cam Name............................
            string strread = "";
            string headerStr = "Cam_" + Name.Replace(" ", string.Empty);
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

           




            //  Pixel_MM = (CaliValue.X + CaliValue.Y) / 2;
        }
        /// </summary>
        public void AssignBaslerCamera(string camID)
        {
            if (camID == "")
                return;
            cameraID = camID;

            try
            {
                //HOperatorSet.GenEmptyObj(out hPylonImage);
                // 枚举相机列表
                List<ICameraInfo> allCameraInfos = CameraFinder.Enumerate();

                foreach (ICameraInfo cameraInfo in allCameraInfos)
                {
                    if (camID == cameraInfo[CameraInfoKey.SerialNumber])
                    {
                        try
                        {
                            camera = new Camera(cameraInfo);
                            OpenCam();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show("Serial“" + camID + "”Not Found！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //throw new Exception(e.Message);
                        }

                       
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Serial“" + CamID + "”Not Found！！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw new Exception(e.Message);
            }



        }



        /****************************************************/


        // Shows exceptions in a message box.
        private void ShowException(Exception exception)
        {
            MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        ~BaslerCam()
        {
           /* if (camera.StreamGrabber != null)
            {
                if (camera.StreamGrabber.IsGrabbing)
                {
                    camera.StreamGrabber.Stop();
                }
                if (camera.IsOpen)
                {
                    camera.Close();
                }
            }
            */
        }
    }
}

