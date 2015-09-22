using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace Cycubeat
{
    public class KinectHandler
    {
        public event HandDelegate HandEvent;

        private KinectSensor sensor;
        private Body[] bodies;
        private FrameDescription colorFrameDesc;
        private FrameDescription bodyIndexFrameDescription;
        private WriteableBitmap bodyIndexBitmap;
        private uint[] bodyIndexPixels;
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
        }

        private void trackEngagedPlayersViaHandOverHead()
        {
            /*this.engagementPeopleHaveChanged = false;
            var currentlyEngagedHands = KinectCoreWindow.KinectManualEngagedHands;
            this.handsToEngage.Clear();

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
            }*/
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

                    //trackEngagedPlayersViaHandOverHead();

                    foreach (var item in bodies.Select((value, i) => new { i, value }))
                    {
                        if (item.value.IsTracked)
                        {
                            
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
