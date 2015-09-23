using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Input;

namespace Cycubeat
{
    public class KinectHandler
    {
        public event HandDelegate HandEvent;
        public event BodyDelegate BodyEvent;
        
        private KinectSensor sensor;
        private Body[] bodies;
        private FrameDescription colorFrameDesc;
        private FrameDescription bodyIndexFrameDescription;
        private WriteableBitmap bodyIndexBitmap;
        private TimeSpan lastTime;
        private Body[] bodyTrackingArray;
        private uint[] bodyIndexPixels;
        private List<BodyHandPair> handsToEngage;
        private Body trackingBody;
        //private KinectCoreWindow kinectCoreWindow;
        //private bool engagementPeopleHaveChanged = false;
        private MultiSourceFrameReader reader;
        private static readonly uint[] BodyColor =
        {
            0x0000FF00,
            0x00FF0000,
            0xFFFF4000,
            0x40FFFF00,
            0xFF40FF00,
            0xFF808000,
        };

        public KinectHandler()
        {
            sensor = KinectSensor.GetDefault();
            sensor.Open();
            bodyIndexFrameDescription = sensor.BodyIndexFrameSource.FrameDescription;
            bodyIndexPixels = new uint[bodyIndexFrameDescription.Width * bodyIndexFrameDescription.Height];
            bodyIndexBitmap = new WriteableBitmap(bodyIndexFrameDescription.Width, bodyIndexFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body | FrameSourceTypes.Depth | FrameSourceTypes.BodyIndex);
            reader.MultiSourceFrameArrived += reader_MultiSourceFrameArrived;
            bodyTrackingArray = new Body[2];
            handsToEngage = new List<BodyHandPair>();
            colorFrameDesc = sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            //kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            //kinectCoreWindow.PointerMoved += kinectCoreWindow_PointerMoved;
        }

        /*private void kinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs args)
        {
            if (trackingBody == null)
                return;
            if (args.CurrentPoint.Properties.HandType != HandType.RIGHT)
                return;

            KinectPointerPoint point = args.CurrentPoint;
            ulong trackingId = point.Properties.BodyTrackingId;

            if (lastTime == TimeSpan.Zero || lastTime != point.Properties.BodyTimeCounter)
            {
                lastTime = point.Properties.BodyTimeCounter;
            }

            Point screenRelative = new Point(
            point.Position.X * 1366,
            point.Position.Y * 768);
            if (point.Properties.IsEngaged)
            {
                RenderPointer(point.Position, Switcher.pageSwitcher.Canvas_Pointer);
                performLassoClick(screenRelative, trackingId, Button_Cheer);
                performLassoClick(screenRelative, trackingId, Button_Boo);
                performLassoClick(screenRelative, trackingId, Button_Mask);
                performLassoClick(screenRelative, trackingId, Button_Piano);
                performLassoClick(screenRelative, trackingId, Button_Guitar);
                performLassoClick(screenRelative, trackingId, Button_Drum);
                performLassoClick(screenRelative, trackingId, Button_Clear);
            }
        }*/

        /*private void RenderPointer(PointF position, Canvas canvas)
        {
            Double y = position.Y * 768;
            y = (y > 720) ? 720 : y;
            Canvas.SetLeft(Switcher.pageSwitcher.PointerFirst, position.X * 1366 - 25);
            Canvas.SetTop(Switcher.pageSwitcher.PointerFirst, position.Y * 768 - 35);
        }*/

        /*private void performLassoClick(Point relative, ulong trackingId, UIElement relativeTo)
        {
            Button clickedButton = relativeTo as Button;
            if (!clickedButton.IsHitTestVisible)
                return;

            if (clickedButton != null)
            {
                Point relativeToElement = Switcher.pageSwitcher.Canvas_Pointer.TranslatePoint(relative, clickedButton);
                bool insideElement = (relativeToElement.X >= 0 && relativeToElement.X < clickedButton.ActualWidth
                    && relativeToElement.Y >= 0 && relativeToElement.Y < clickedButton.ActualHeight);
                bool isLasso = bodyTrackingArray.Where(x => x.TrackingId == trackingId).First().HandRightState == HandState.Open;
                if (insideElement)
                {
                    VisualStateManager.GoToState(clickedButton, "MouseOver", false);
                    if (isLasso)
                    {
                        clickedButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, trackingId));
                        //VisualStateManager.GoToState(clickedButton, "Pressed", true);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(clickedButton, "Normal", true);
                }
            }
        }*/

        /*private void trackEngagedPlayersViaHandOverHead()
        {
            engagementPeopleHaveChanged = false;
            var currentlyEngagedHands = KinectCoreWindow.KinectManualEngagedHands;
            handsToEngage.Clear();

            // check to see if anybody who is currently engaged should be disengaged
            foreach (var bodyHandPair in currentlyEngagedHands)
            {
                var bodyTrackingId = bodyHandPair.BodyTrackingId;
                foreach (var body in this.bodies)
                {
                    if (body.TrackingId == bodyTrackingId)
                    {
                        // check for disengagement
                        bool toBeDisengaged = (body.Joints[JointType.HandRight].Position.Y < body.Joints[JointType.SpineBase].Position.Y);

                        if (toBeDisengaged)
                        {
                            this.engagementPeopleHaveChanged = true;
                        }
                        else
                        {
                            this.handsToEngage.Add(bodyHandPair);
                        }
                    }
                }
            }
        }*/

        private void reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            using (var frame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(bodies);

                    //trackEngagedPlayersViaHandOverHead();

                    foreach (var item in bodies.Select((value, i) => new { i, value }))
                    {
                        if (item.value.IsTracked)
                        {
                            trackingBody = item.value;
                            var lHand = coordinateMap(item.value.Joints[JointType.HandLeft].Position);

                            var rHand = coordinateMap(item.value.Joints[JointType.HandRight].Position);

                            var lState = item.value.HandLeftState;

                            var rState = item.value.HandRightState;

                            var lHandConf = item.value.HandLeftConfidence;

                            var rHandConf = item.value.HandRightConfidence;

                            var isLOpen = (lHandConf == TrackingConfidence.High && lState == HandState.Open);

                            var isROpen = (rHandConf == TrackingConfidence.High && rState == HandState.Open);

                            HandEvent(lHand, rHand, isLOpen, isROpen);
                        }
                    }
                }
            }

            using (BodyIndexFrame bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
            {
                if (bodyIndexFrame != null)
                {
                    // the fastest way to process the body index data is to directly access 
                    // the underlying buffer
                    using (KinectBuffer bodyIndexBuffer = bodyIndexFrame.LockImageBuffer())
                    {
                        // verify data and write the color data to the display bitmap
                        if (((bodyIndexFrameDescription.Width * bodyIndexFrameDescription.Height) == bodyIndexBuffer.Size) &&
                            (bodyIndexFrameDescription.Width == bodyIndexBitmap.PixelWidth) && (bodyIndexFrameDescription.Height == bodyIndexBitmap.PixelHeight))
                        {
                            ProcessBodyIndexFrameData(bodyIndexBuffer.UnderlyingBuffer, bodyIndexBuffer.Size);
                            RenderBodyIndexPixels();
                        }
                    }
                }
            }
        }

        private Point coordinateMap(CameraSpacePoint position)
        {
            var point = sensor.CoordinateMapper.MapCameraPointToColorSpace(position);
            return new Point(
                float.IsInfinity(point.X) ? 0 : point.X / colorFrameDesc.Width * 1366,
                float.IsInfinity(point.Y) ? 0 : point.Y / colorFrameDesc.Height * 768);
        }

        private void RenderBodyIndexPixels()
        {
            bodyIndexBitmap.WritePixels(
                new Int32Rect(0, 0, bodyIndexBitmap.PixelWidth, bodyIndexBitmap.PixelHeight),
                bodyIndexPixels,
                bodyIndexBitmap.PixelWidth * 4,
                0);
            BodyEvent(bodyIndexBitmap);
        }

        private unsafe void ProcessBodyIndexFrameData(IntPtr bodyIndexFrameData, uint bodyIndexFrameDataSize)
        {
            byte* frameData = (byte*)bodyIndexFrameData;

            // convert body index to a visual representation
            for (int i = 0; i < (int)bodyIndexFrameDataSize; ++i)
            {
                // the BodyColor array has been sized to match
                // BodyFrameSource.BodyCount
                if (frameData[i] < BodyColor.Length)
                {
                    // this pixel is part of a player,
                    // display the appropriate color
                    bodyIndexPixels[i] = BodyColor[frameData[i]];
                }
                else
                {
                    // this pixel is not part of a player
                    // display black
                    bodyIndexPixels[i] = 0x00000000;
                }
            }
        }
    }
}
