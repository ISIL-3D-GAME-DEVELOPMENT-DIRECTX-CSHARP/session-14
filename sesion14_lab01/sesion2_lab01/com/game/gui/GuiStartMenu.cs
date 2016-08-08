using Core.Model;
using CrossXDK.com.digitalkancer.modules.moderlLoaders.assimp;
using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.data_type;
using Sesion2_Lab01.com.isil.render.batcher;
using Sesion2_Lab01.com.isil.render.camera;
using Sesion2_Lab01.com.isil.render.components;
using Sesion2_Lab01.com.isil.render.graphics;
using Sesion2_Lab01.com.isil.shader.d2d;
using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01.com.isil.shader.skinnedModel;
using Sesion2_Lab01.com.isil.system.screenManager;
using Sesion2_Lab01.com.isil.system.soundSystem;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sesion2_Lab01.com.game.gui {

    public class GuiStartMenu : Screen {

        private SkinnedModelInstance mSkinnedModel;
        //private NModel mModel3DBetter;
        private Plane3D mPlane3D;

        private NSoundInstance mSoundInstance;

        private NTextField2D mTextField;

        private RenderCamera mRenderCamera;

        private NSpriteBatch mSpriteBatch;

        //private int mTimeCounter;

        public GuiStartMenu() : base() {
            //mTimeCounter = 0;

            mRenderCamera = NativeApplication.instance.RenderCamera;
        }

        public override void Initialize() {
            base.Initialize();

            // cargamos y construimos nuestro Shader
            mPlane3D = new Plane3D("Content/spMario.png", 0, 0, 0, 10f);

            //mModel3DBetter = cl.Load("Content/pavilon/Arch_Bldg_Pavilion01.obj");
            //mModel3DBetter = cl.Load("Content/buston/blustom.x");

            mSkinnedModel = new SkinnedModelInstance("Content/magician/magician.X", "Content/magician/", true);
            mSkinnedModel.GotoAnimation("Run", true);

            //mSprite = new NSprite2D("Content/spMario.png", 0, 0);
            //mSprite.SetShader(mShaderSpriteProgram);

            mTextField = new NTextField2D("Content/font/kronika/kronika_16");
            mTextField.text = "Hola, soy una prueba en 3D =)";

            mSoundInstance = new NSoundInstance(0);
            mSoundInstance.BindDataBuffer("Content/sound/music_play_button.wav");
            mSoundInstance.Play(() => {
                mSoundInstance = null;  
            });

            mSpriteBatch = new NSpriteBatch("Content/spMario.png");
        }

        public override void Update(int dt) {
            base.Update(dt);

            if (mSoundInstance != null) {
                mSoundInstance.Update(dt);
            }

            //mSkinnedModel.X += 0.1f;
            //mSkinnedModel.RotationX = 1.3f;

            //if (mTimeCounter > 1500) {
            //    NativeApplication.instance.ScreenManager.GotoScreen(typeof(GuiWaraScreen));
            //}
            //else {
            //    mTimeCounter += dt;
            //}

            mSpriteBatch.AddSprite(50, 50, 0, 60, 60, NColor.White);
            mSpriteBatch.AddSprite(150, 0, 0, 120, 120, NColor.Blue);
            mSpriteBatch.Update(dt);
        }

        public override void Draw(int dt) {
            base.Draw(dt);

            /////////////////// EMPEZAMOS A DIBUJAR NUESTRO PRIMITIVO ///////////////
            mPlane3D.UpdateAndDraw(mRenderCamera, dt);

            mRenderCamera.ChangeCameraTo(RenderCamera.ORTHOGRAPHIC);
            
            //mSprite.Draw(mRenderCamera, dt);
            //mSprite.X += 1;
            
            mSpriteBatch.Draw(mRenderCamera, dt);


            mTextField.UpdateAndDraw(mRenderCamera, dt);

            mRenderCamera.ChangeCameraTo(RenderCamera.PERSPECTIVE);

            //mModel3DBetter.Draw(mRenderCamera.transformed, dt);

            mSkinnedModel.UpdateDraw(mRenderCamera, dt);
        }
    }
}
