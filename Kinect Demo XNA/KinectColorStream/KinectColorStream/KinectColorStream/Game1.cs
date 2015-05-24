using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


// http://www.mediafire.com/view/h2a1j8av2wv5u8h/Integration%20of%20the%20Microsoft%20Kinect%20with%20the%20XNA%20framework.pdf
namespace KinectColorStream
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        KinectManager mKincectManager;

        Effect mDepthShader;

        Texture2D mJointTexture;

        Texture2D mCircleTex;

        SpriteFont mKinectInfo;

        public enum KinectMode { color, depth, depthShader, tracking, colorTracking, depthTracking };
        public KinectMode mKinectMode = KinectMode.depthTracking;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            mKincectManager = new KinectManager(this);

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            String err = mKincectManager.InitKinect();
            if (err != "")
                Console.WriteLine(err + ": Restart programm!");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loat depth shader
            mDepthShader = Content.Load<Effect>("DepthShader");

            // Load jointIdentifier
            mJointTexture = Content.Load<Texture2D>("redSquare");

            // Load circle tex for right hand
            mCircleTex = Content.Load<Texture2D>("circle");

            // Load debug font
            mKinectInfo = Content.Load<SpriteFont>("KinectInfo");

        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            switch (mKinectMode)
            {
                case KinectMode.color:
                    mKincectManager.DrawColorImage(spriteBatch, GraphicsDevice, new Rectangle(0, 0, 640, 480));
                    break;

                case KinectMode.depth:
                    mKincectManager.DrawDepthImage(spriteBatch, GraphicsDevice, new Rectangle(0, 0, 640, 480));
                    break;

                case KinectMode.depthShader:
                    mKincectManager.DrawDepthImageWithDepthShader(spriteBatch, GraphicsDevice, new Rectangle(0, 0, 640, 480), mDepthShader);
                    break;

                case KinectMode.tracking:
                    mKincectManager.DrawSkeletonImage(spriteBatch, mJointTexture);
                    spriteBatch.Draw(mCircleTex, new Rectangle((int)mKincectManager.GetRightHandPosition().X, (int)mKincectManager.GetRightHandPosition().Y, 40, 40), Color.White);
                    spriteBatch.Draw(mCircleTex, new Rectangle((int)mKincectManager.GetRigthShoulderPosition().X, (int)mKincectManager.GetRigthShoulderPosition().Y, 40, 40), Color.White);
                    break;

                case KinectMode.colorTracking:
                    mKincectManager.DrawColorImage(spriteBatch, GraphicsDevice, new Rectangle(0, 0, 640, 480));
                    mKincectManager.DrawSkeletonImage(spriteBatch, mJointTexture);
                    spriteBatch.Draw(mCircleTex, new Rectangle((int)mKincectManager.GetRightHandPosition().X, (int)mKincectManager.GetRightHandPosition().Y, 40, 40), Color.White);
                    spriteBatch.Draw(mCircleTex, new Rectangle((int)mKincectManager.GetRigthShoulderPosition().X, (int)mKincectManager.GetRigthShoulderPosition().Y, 40, 40), Color.White);
                    break;

                case KinectMode.depthTracking:
                    mKincectManager.DrawDepthImageWithDepthShader(spriteBatch, GraphicsDevice, new Rectangle(0, 0, 640, 480), mDepthShader);
                    mKincectManager.DrawSkeletonImage(spriteBatch, mJointTexture);
                    spriteBatch.Draw(mCircleTex, new Rectangle((int)mKincectManager.GetRightHandPosition().X, (int)mKincectManager.GetRightHandPosition().Y, 40, 40), Color.White);
                    spriteBatch.Draw(mCircleTex, new Rectangle((int)mKincectManager.GetRigthShoulderPosition().X, (int)mKincectManager.GetRigthShoulderPosition().Y, 40, 40), Color.White);
                    break;
            }

            String info = "RH: " + "X = " + Math.Round(mKincectManager.GetRightHandPosition().X, 2) +
                       " Y = " + Math.Round(mKincectManager.GetRightHandPosition().Y, 2);
            spriteBatch.DrawString(mKinectInfo, info, new Vector2(0, 0), Color.Black);

            info = "RS: " + "X = " + Math.Round(mKincectManager.GetRigthShoulderPosition().X, 2) +
                                   " Y = " + Math.Round(mKincectManager.GetRigthShoulderPosition().Y, 2);
            spriteBatch.DrawString(mKinectInfo, info, new Vector2(0, 40), Color.Black);

            info = "DHS: " + mKincectManager.GetDeltaRHPRSP();
            spriteBatch.DrawString(mKinectInfo, info, new Vector2(0, 80), Color.Black);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
