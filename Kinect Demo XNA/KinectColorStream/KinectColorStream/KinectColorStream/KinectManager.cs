using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Speech;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.IO;

namespace KinectColorStream
{
    class KinectManager
    {
        Game1 mGame;
        KinectSensor mKinect;

        // RGB color array
        Color[] mLatestColorData;

        // Depth data array
        short[] mLatestDepthData;

        // Skeleton points
        Dictionary<JointType, ColorImagePoint> mSkeletonPoints = new Dictionary<JointType, ColorImagePoint>();

        // Contains the colorImage
        Texture2D mColorImage;

        // Contains the depthImage
        Texture2D mDepthImage;

        // Right hand position
        Vector3 mRightHandPos = Vector3.Zero;

        // Right shoulder position
        Vector3 mRightShoulderPos = Vector3.Zero;

        // Offeset between rhp and rsp
        float mDeltaRHPRSP = 0.0f;

        // Speech-Recognition-Engine
        SpeechRecognitionEngine mSpeechEngine;

        public KinectManager(Game1 game)
        {
            mGame = game;
        }

        // Init kinect sensors and handlers
        public string InitKinect()
        {
            // Check for a kinect sensor
            if (KinectSensor.KinectSensors.Count == 0)
                return "Error: No kinect sensors detected!";

            // Get a sensor instance
            mKinect = KinectSensor.KinectSensors[0];

            // Enable all streams
            mKinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            mKinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            mKinect.SkeletonStream.Enable();
            /* mKinect.SkeletonStream.Enable(new TransformSmoothParameters()
             {
                 Smoothing = 0.5f,
                 Correction = 0.5f,
                 Prediction = 0.5f,
                 JitterRadius = 0.05f,
                 MaxDeviationRadius = 0.04f
             });*/

            // Add event handlers
            mKinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(mKinect_ColorFrameReady);
            mKinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(mKinect_DepthFrameReady);
            mKinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(mKinect_SkeletonFrameReady);

            // Start sensor
            mKinect.Start();

            // Init speech Engine
            RecognizerInfo kinectInfo = null;
            foreach (RecognizerInfo rec in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (rec.AdditionalInfo.ContainsKey("Kinect") && rec.Culture.Name == "en-US")
                {
                    if (rec.AdditionalInfo["Kinect"] == "True")
                    {
                        kinectInfo = rec;
                        break;
                    }
                }
            }

            if (kinectInfo == null)
                return "Unable to find Kinect speech recognition information.";
            mSpeechEngine = new SpeechRecognitionEngine(kinectInfo);

            // Set commands
            Choices commands = new Choices();
            commands.Add("one",           // Kinect color stream
                         "two",           // Kinect depth stream
                         "three",         // Kinect depth stream with greyshader
                         "four",          // Kinect joint tracking
                         "five",
                         "six",
                         "color",
                         "depth",
                         "shader",
                         "tracking",
                         "lets fly");        

            // Add commans to engine
            GrammarBuilder builder = new GrammarBuilder();
            builder.Culture = kinectInfo.Culture;
            builder.Append(commands);
            mSpeechEngine.LoadGrammar(new Grammar(builder));

            // Setup kinects microphones
            KinectAudioSource source = mKinect.AudioSource;
            source.BeamAngleMode = BeamAngleMode.Adaptive;
            Stream audioStream = source.Start();
            mSpeechEngine.SetInputToAudioStream(audioStream,
                                                new SpeechAudioFormatInfo(EncodingFormat.Pcm, // Encodingformat
                                                                         16000,               // Samples per second
                                                                         16,                  // Bits per sample
                                                                         1,                   // Channelcount
                                                                         32000,               // Average byts per second
                                                                         2,                   // Blockalign
                                                                         null));              // Format specific data
            // Speech eventHandler
            mSpeechEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(mSpeechEngine_SpeechRecognized);
            mSpeechEngine.RecognizeAsync(RecognizeMode.Multiple);


            return "";
        }

        void mSpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Make sure result is at least 70% confident
            if (e.Result.Confidence < 0.7)
                return;

            switch (e.Result.Text)
            {
                case "one":
                    mGame.mKinectMode = Game1.KinectMode.color;
                    break;
                case "two":
                    mGame.mKinectMode = Game1.KinectMode.depth;
                    break;
                case "three":
                    mGame.mKinectMode = Game1.KinectMode.depthShader;
                    break;
                case "four":
                    mGame.mKinectMode = Game1.KinectMode.tracking;
                    break;
                case "five":
                    mGame.mKinectMode = Game1.KinectMode.colorTracking;
                    break;
                case "six":
                    mGame.mKinectMode = Game1.KinectMode.depthTracking;
                    break;
                case "color":
                    mGame.mKinectMode = Game1.KinectMode.tracking;
                    break;
                case "depth":
                    mGame.mKinectMode = Game1.KinectMode.tracking;
                    break;
                case "shader":
                    mGame.mKinectMode = Game1.KinectMode.tracking;
                    break;
                case "tracking":
                    mGame.mKinectMode = Game1.KinectMode.tracking;
                    break;
                case "lets fly":
                    mGame.mKinectMode = Game1.KinectMode.tracking;
                    break;
            }


        }

        // Set colorFrameData via callback when colorFrame is rdy
        void mKinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            // Store event data
            ColorImageFrame colorImageFrame = e.OpenColorImageFrame();

            // Make sure frame is not empty 
            if (colorImageFrame == null)
                return;

            // Copy pixelData to byte array for colors 
            byte[] pixelData = new byte[colorImageFrame.PixelDataLength]; // 4 byte per pixel
            colorImageFrame.CopyPixelDataTo(pixelData);

            // Set size for latestColorData
            mLatestColorData = new Color[pixelData.Length / 4]; // RGBA
            int offset = 0;

            // Iterate pixels
            for (int i = 0; i < mLatestColorData.Length; i++)
            {
                // Copy BGR (it is inverted and without alpha)
                mLatestColorData[i] = new Color(pixelData[offset + 2], pixelData[offset + 1], pixelData[offset]);
                offset += 4;
            }
            colorImageFrame.Dispose();
        }

        // Draw colorFrame
        public void DrawColorImage(SpriteBatch sp, GraphicsDevice device, Rectangle rect)
        {
            // Make sure color date is available
            if (mLatestColorData == null)
                return;

            mColorImage = new Texture2D(device, 640, 480);
            mColorImage.SetData<Color>(mLatestColorData);

            sp.Draw(mColorImage, rect, Color.White);
        }

        // Set depthFrameData via callback if depthFrame is rdy
        void mKinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            // Store event data
            DepthImageFrame depthImageFrame = e.OpenDepthImageFrame();

            // Make sure frame is not empty
            if (depthImageFrame == null)
                return;

            // Copy depth data
            mLatestDepthData = new short[depthImageFrame.PixelDataLength];
            depthImageFrame.CopyPixelDataTo(mLatestDepthData);

            depthImageFrame.Dispose();

        }

        // Draw depthFrame
        public void DrawDepthImage(SpriteBatch sp, GraphicsDevice device, Rectangle rect)
        {
            // Make sure color data is available
            if (mLatestDepthData == null)
                return;

            mDepthImage = new Texture2D(device, 640, 480, false, SurfaceFormat.Bgra4444);
            mDepthImage.SetData<short>(mLatestDepthData);

            sp.Draw(mDepthImage, rect, Color.White);
        }

        // Draw depthFrame with depthShader
        public void DrawDepthImageWithDepthShader(SpriteBatch sp, GraphicsDevice device, Rectangle rect, Effect depthShader)
        {
            // Make sure color data is available
            if (mLatestDepthData == null)
                return;

            mDepthImage = new Texture2D(device, 640, 480, false, SurfaceFormat.Bgra4444);
            mDepthImage.SetData<short>(mLatestDepthData);

            //sp.Draw(mDepthImage, rect, Color.White);

            sp.End();
            sp.Begin(SpriteSortMode.Deferred, null, null, null, null, depthShader);
            sp.Draw(mDepthImage, rect, Color.White);
            sp.End();
            sp.Begin();

        }

        // Set skeletonFrameData via callback if skeletonFrame is rdy
        void mKinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Store event data
            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();

            // Make sure frame is not empty
            if (skeletonFrame == null)
            {
                try { skeletonFrame.Dispose(); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                return;
            }

            // Individual players which contain skeleton data
            Skeleton[] players = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(players);

            // Make sure that at least one player is being tracked
            Skeleton firstPlayer = null;
            foreach (Skeleton skel in players)
            {
                if (skel.TrackingState == SkeletonTrackingState.Tracked || skel.TrackingState == SkeletonTrackingState.PositionOnly)
                    firstPlayer = skel;
            }

            if (firstPlayer == null)
                return;
            // Remove all previous points
            mSkeletonPoints.Clear();

            // Go through all possible joints an map them
            for (int i = 0; i < 20; i++)
            {
                // Get next joint type
                JointType jointType = ((JointType[])Enum.GetValues(typeof(JointType)))[i];

                ColorImagePoint mappedPoint;
                mappedPoint = mKinect.CoordinateMapper.MapSkeletonPointToColorPoint(firstPlayer.Joints[jointType].Position, ColorImageFormat.RgbResolution640x480Fps30);
                mSkeletonPoints.Add(jointType, mappedPoint);
            }


            // Get right hand
            Joint rightHand = firstPlayer.Joints[JointType.HandRight];
            mRightHandPos = new Vector3(((0.5f * rightHand.Position.X) + 0.5f) * 640,
                                      ((-0.5f * rightHand.Position.Y) + 0.5f) * 480,
                                      0);
            // Get right shoulder
            Joint rightShoulder = firstPlayer.Joints[JointType.ShoulderRight];
            mRightShoulderPos = new Vector3(((0.5f * rightShoulder.Position.X) + 0.5f) * 640,
                          ((-0.5f * rightShoulder.Position.Y) + 0.5f) * 480,
                          0);

            // Get delta right should and right hand
            mDeltaRHPRSP = DeltaRHPRSP(mRightHandPos, mRightShoulderPos);

            skeletonFrame.Dispose();
        }

        public Vector3 GetRightHandPosition()
        {
            if (mRightHandPos != null)
                return mRightHandPos;

            return Vector3.Zero;
        }

        public Vector3 GetRigthShoulderPosition()
        {
            if (mRightShoulderPos != null)
                return mRightShoulderPos;

            return Vector3.Zero;
        }

        public float GetDeltaRHPRSP()
        {
            return mDeltaRHPRSP;
        }

        private float DeltaRHPRSP(Vector3 rhp, Vector3 rsp)
        {
            return rhp.Y - rsp.Y;
        }


        public void DrawSkeletonImage(SpriteBatch sp, Texture2D jointImage)
        {
            // Draw the points
            for (int i = 0; i < mSkeletonPoints.Count; i++)
            {
                // Get next joint type
                JointType jointType = ((JointType[])Enum.GetValues(typeof(JointType)))[i];

                // Check if that joint is mapped
                ColorImagePoint colorImagePoint;
                if (!mSkeletonPoints.TryGetValue(jointType, out colorImagePoint))
                    continue;

                // Draw joint
                sp.Draw(jointImage, new Rectangle(colorImagePoint.X, colorImagePoint.Y, 10, 10), Color.White);
            }
        }
    }
}
