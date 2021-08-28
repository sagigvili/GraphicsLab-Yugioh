//------------------------------------------------------------------------------------------------------------------
// Global Snow
// Created by Ramiro Oliva (Kronnect)
//------------------------------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GlobalSnowEffect {

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [HelpURL("http://kronnect.com/taptapgo")]
    public class GlobalSnowImageEffect : MonoBehaviour {

        GlobalSnow snow;

        void OnEnable() {
            snow = GetComponent<GlobalSnow>();
        }

        //[ImageEffectOpaque] // uncomment to force camera frost effect to render before transparent objects
        void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (!enabled || snow == null) {
                Graphics.Blit(source, destination);
                return;
            }

            Material snowMat = snow.composeMat;
            Camera cameraEffect = snow.snowCamera;
            if (snowMat == null || cameraEffect == null || (!snow.showSnowInSceneView && Camera.current != cameraEffect)) {
                Graphics.Blit(source, destination);
                return;
            }

            if (snow.distanceOptimization && !snow.deferred && snow.distantSnowMat != null) {
                RenderTexture rtDistantSnow = RenderTexture.GetTemporary(cameraEffect.pixelWidth, cameraEffect.pixelHeight, 24, RenderTextureFormat.ARGB32);
                snow.distantSnowMat.SetMatrix("_ClipToWorld", cameraEffect.cameraToWorldMatrix);
                snow.distantSnowMat.SetMatrix("_CamToWorld", cameraEffect.cameraToWorldMatrix);
                Graphics.Blit(source, rtDistantSnow, snow.distantSnowMat);
                snowMat.SetTexture("_DistantSnow", rtDistantSnow);
                RenderTexture.ReleaseTemporary(rtDistantSnow);
            }
            bool frosted = snow.cameraFrost && snow.snowAmount > 0;
            snowMat.SetVector("_FrostIntensity", new Vector3(frosted ? snow.cameraFrostIntensity * snow.snowAmount * 5f : 0, 5.1f - snow.cameraFrostSpread, snow.cameraFrostDistortion * 0.01f));
            int renderPass = snow.debugSnow ? 1 : 0;
            Graphics.Blit(source, destination, snowMat, renderPass);
        }
    }

}