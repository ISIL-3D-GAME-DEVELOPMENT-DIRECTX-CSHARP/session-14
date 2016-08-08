using System.Collections.Generic;
using System.Linq;
//using Core.FX;

using SharpDX;
using SharpDX.Direct3D11;
using Sesion2_Lab01.com.isil.utils;
using Assimp;
using Sesion2_Lab01.com.isil.shader.skinnedModel;
using Sesion2_Lab01.com.isil.shader.d3d;
using Sesion2_Lab01;
using Sesion2_Lab01.com.isil.content;
using Sesion2_Lab01.com.isil.graphics;
using System;
using Sesion2_Lab01.com.isil.render.camera;

namespace Core.Model {

    public class SkinnedModelInstance {

        public const float DefaultSpeed = 500f;

        private readonly SkinnedModel mSkinnedModel;

        private float mTimePosition;
        private float mAnimationSpeed;
        private string mClipName;
        private bool mIsLoopeable;
        private string[] mClips;

        private float mX;
        private float mY;
        private float mZ;

        private float mScaleX;
        private float mScaleY;
        private float mScaleZ;

        private float mRotationX;
        private float mRotationY;
        private float mRotationZ;
        
        private SkinnedModelProgram mShaderProgram;
        private DeviceContext mDeviceContext;
        private BlendState mBlendState;

        private Matrix mWorld;

        private Action<string> mAnimationEndCallback;

        // these are the available animation clips
        public string[] Clips { get { return mClips; } }

        public float X {
            get { return mX; }
            set { mX = value; }
        }

        public float Y {
            get { return mY; }
            set { mY = value; }
        }

        public float Z {
            get { return mZ; }
            set { mZ = value; }
        }

        public float ScaleX {
            get { return mScaleX; }
            set { mScaleX = value; }
        }

        public float ScaleY {
            get { return mScaleY; }
            set { mScaleY = value; }
        }

        public float ScaleZ {
            get { return mScaleZ; }
            set { mScaleZ = value; }
        }

        public float RotationX {
            get { return mRotationX; }
            set { mRotationX = value; }
        }

        public float RotationY {
            get { return mRotationY; }
            set { mRotationY = value; }
        }

        public float RotationZ {
            get { return mRotationZ; }
            set { mRotationZ = value; }
        }
        
        public SkinnedModelInstance(string model_path, string texture_path, bool flipY) {
            mDeviceContext = NativeApplication.instance.Device.ImmediateContext;

            mSkinnedModel = new SkinnedModel(NativeApplication.instance.Device,
                model_path, texture_path, flipY);

            NBlend opaqueBlend = NBlend.Opaque();
            mBlendState = new BlendState(mDeviceContext.Device, opaqueBlend.BlendStateDescription);

            mShaderProgram = new SkinnedModelProgram(NativeApplication.instance.Device);
            mShaderProgram.Load("Content/Fx_SkinnedModel.fx");

            mClips = mSkinnedModel.Animator.Animations.Select(a => a.Name).ToArray();

            mScaleX = 1f;
            mScaleY = 1f;
            mScaleZ = 1f;
            mIsLoopeable = false;
            mAnimationSpeed = SkinnedModelInstance.DefaultSpeed;
        }

        public void AddEventListener(Action<string> onEndAnimationCallback) {
            mAnimationEndCallback = onEndAnimationCallback;
        }

        public void GotoAnimation(string animation_name, bool isLoop) {
            mIsLoopeable = isLoop;

            mClipName = mSkinnedModel.Animator.Animations.Any(a => a.Name == animation_name) ? animation_name : "Still";
            mSkinnedModel.Animator.SetAnimation(mClipName);
            mTimePosition = 0;
        }

        public void UpdateDraw(RenderCamera renderCamera, float dt) {
            mTimePosition += dt / mAnimationSpeed;

            if (mTimePosition > mSkinnedModel.Animator.Duration) {
                if (mAnimationEndCallback != null) { mAnimationEndCallback(mSkinnedModel.Animator.AnimationName); }

                if (mIsLoopeable) {
                    this.GotoAnimation(mSkinnedModel.Animator.AnimationName, mIsLoopeable);
                }
            }

            // ahora seteamos el tipo de blend
            mDeviceContext.OutputMerger.SetBlendState(mBlendState);

            mWorld = Matrix.Identity;
            mWorld.M41 = mX;
            mWorld.M42 = mY;
            mWorld.M43 = mZ;

            mWorld = Matrix.Scaling(mScaleX, mScaleY, mScaleZ) * 
                Matrix.RotationYawPitchRoll(mRotationX, mRotationY, mRotationZ) * 
                mWorld;

            Matrix wit = NCommon.InverseTranspose(mWorld);

            Matrix transformation = renderCamera.transformed;
            transformation = mWorld * transformation;
            transformation.Transpose();

            SkinnedModelInputParameters inputParameters = SkinnedModelInputParameters.EMPTY;
            inputParameters.gWorld = mWorld;
            inputParameters.gWorldInvTranspose = wit;
            inputParameters.gWorldViewProj = transformation;
            inputParameters.gTexTransform = Matrix.Identity;

            List<Matrix> boneTransformations = mSkinnedModel.Animator.GetTransforms(mTimePosition);
            Matrix[] boneTransforms = boneTransformations.ToArray<Matrix>();

            for (int i = 0; i < mSkinnedModel.SubsetCount; i++) {
                NTexture2D srvDiffuse = null;
                NTexture2D srvNormal = null;

                if (mSkinnedModel.DiffuseMapSRV.Count > 0) { srvDiffuse = mSkinnedModel.DiffuseMapSRV[i]; }
                if (mSkinnedModel.NormalMapSRV.Count > 0) { srvNormal = mSkinnedModel.NormalMapSRV[i]; }
                
                mSkinnedModel.ModelMesh.Draw(mShaderProgram, inputParameters, boneTransforms,
                    srvDiffuse, srvNormal, mDeviceContext, i);
            }
        }
    }
}