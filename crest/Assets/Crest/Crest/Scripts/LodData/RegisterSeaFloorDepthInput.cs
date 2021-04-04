﻿// Crest Ocean System

// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using UnityEngine;

namespace Crest
{
    /// <summary>
    /// Tags this object as an ocean depth provider. Renders depth every frame and should only be used for dynamic objects.
    /// For static objects, use an Ocean Depth Cache.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu(MENU_PREFIX + "Sea Floor Depth Input")]
    [HelpURL("https://crest.readthedocs.io/en/stable/user/ocean-simulation.html#sea-floor-depth")]
    public class RegisterSeaFloorDepthInput : RegisterLodDataInput<LodDataMgrSeaFloorDepth>
    {
        public override bool Enabled => true;

        public bool _assignOceanDepthMaterial = true;

        public override float Wavelength => 0f;

        protected override Color GizmoColor => new Color(1f, 0f, 0f, 0.5f);

        protected override string ShaderPrefix => "Crest/Inputs/Depth";

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_assignOceanDepthMaterial)
            {
                var rend = GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = new Material(Shader.Find("Crest/Inputs/Depth/Ocean Depth From Geometry"));
                }
            }
        }

#if UNITY_EDITOR
        protected override string FeatureToggleName => "_createSeaFloorDepthData";
        protected override string FeatureToggleLabel => "Create Sea Floor Depth Data";
        protected override bool FeatureEnabled(OceanRenderer ocean) => ocean.CreateSeaFloorDepthData;
#endif // UNITY_EDITOR
    }
}
