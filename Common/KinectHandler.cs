using System;
using System.Windows;
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
        public event KinectUpdateEventHandler KinectUpdateEvent;
        public event KinectInputEventHandler KinectInputEvent;

        private KinectSensor sensor;
        private Body[] bodies;
        private FrameDescription colorFrameDesc;
        private FrameDescription bodyIndexFrameDescription;
        private WriteableBitmap bodyIndexBitmap;
        private Body trackingBody;
        private uint[] bodyIndexPixels;
        private KinectCoreWindow kinectCoreWindow;
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
            colorFrameDesc = sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            kinectCoreWindow.PointerMoved += pointerMoved;
        }

        private void pointerMoved(object sender, KinectPointerEventArgs args)
        {
            if (args.CurrentPoint.Properties.HandType != HandType.RIGHT || trackingBody == null || trackingBody.TrackingId != args.CurrentPoint.Properties.BodyTrackingId)
                return;

            KinectPointerPoint pointer = args.CurrentPoint;

            if (pointer.Properties.IsEngaged)
            {
                var pos = new Point(pointer.Position.X * 1366 - 50, pointer.Position.Y * 768 - 50);
                var state = (trackingBody.HandRightState == HandState.Open) ? InputState.Open : InputState.Close;
                var isValid = (trackingBody.Joints[JointType.HandRight].Position.Y > trackingBody.Joints[JointType.SpineMid].Position.Y);
                var e = new KinectInputArgs(pos, state, isValid);
                KinectInputEvent(e);
            }
            else
            {
                trackingBody = null;
            }
        }

        private void trackEngagedPlayersViaHandOverHead()
        {
            foreach (var body in bodies)
            {
                if (trackingBody != null)
                {
                    if (trackingBody.TrackingId == body.TrackingId)
                        trackingBody = body;
                }
                else
                {
                    if (IsHandOverhead(body))
                    {
                        trackingBody = body;
                        KinectCoreWindow.SetKinectOnePersonManualEngagement(new BodyHandPair(body.TrackingId, HandType.RIGHT));
                    }
                }
            }
        }

        private static bool IsHandOverhead(Body body)
        {
            return (body.Joints[JointType.HandRight].Position.Y > body.Joints[JointType.Head].Position.Y);
        }

        private void reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();
            using (var frame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(bodies);

                    trackEngagedPlayersViaHandOverHead();

                    var isTrackingBodyValid = false;
                    if (trackingBody != null)
                    {
                        foreach (var body in bodies)
                        {
                            if (trackingBody.TrackingId == body.TrackingId)
                            {
                                isTrackingBodyValid = true;
                                if (HandEvent != null)
                                {
                                    var lHand = coordinateMap(body.Joints[JointType.HandLeft].Position);
                                    var rHand = coordinateMap(body.Joints[JointType.HandRight].Position);
                                    var lState = body.HandLeftState;
                                    var rState = body.HandRightState;
                                    var lHandConf = body.HandLeftConfidence;
                                    var rHandConf = body.HandRightConfidence;
                                    var isLOpen = (lState == HandState.Open && lHandConf == TrackingConfidence.High);
                                    var isROpen = (rState == HandState.Open && rHandConf == TrackingConfidence.High);
                                    HandEvent(lHand, rHand, isLOpen, isROpen);
                                }
                            }
                        }
                    }
                    KinectUpdateEvent(isTrackingBodyValid);
                }
            }

            if (BodyEvent != null && BodyEvent != null)
            {
                using (BodyIndexFrame bodyIndexFrame = multiSourceFrame.BodyIndexFrameReference.AcquireFrame())
                {
                    if (bodyIndexFrame != null)
                    {
                        using (KinectBuffer bodyIndexBuffer = bodyIndexFrame.LockImageBuffer())
                        {
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
        }

        private Point coordinateMap(CameraSpacePoint position)
        {
            var point = sensor.CoordinateMapper.MapCameraPointToColorSpace(position);
            return new Point(
                float.IsInfinity(point.X) ? 0 : point.X / colorFrameDesc.Width * 1366 - 50,
                float.IsInfinity(point.Y) ? 0 : point.Y / colorFrameDesc.Height * 768 + 50);
        }

        private void RenderBodyIndexPixels()
        {
            bodyIndexBitmap.WritePixels(new Int32Rect(0, 0, bodyIndexBitmap.PixelWidth, bodyIndexBitmap.PixelHeight), bodyIndexPixels, bodyIndexBitmap.PixelWidth * 4, 0);
            BodyEvent(bodyIndexBitmap);
        }

        private unsafe void ProcessBodyIndexFrameData(IntPtr bodyIndexFrameData, uint bodyIndexFrameDataSize)
        {
            byte* frameData = (byte*)bodyIndexFrameData;
            for (int i = 0; i < (int)bodyIndexFrameDataSize; ++i)
            {
                if (frameData[i] < BodyColor.Length)
                    bodyIndexPixels[i] = BodyColor[frameData[i]];
                else
                    bodyIndexPixels[i] = 0x00000000;
            }
        }
    }
}
