﻿// This file is subject to the MIT License as seen in the root of this folder structure (LICENSE)

using UnityEditor;
using UnityEngine;

namespace Crest
{
    [CustomEditor( typeof(WaveSpectrum) )]
    public class WaveSpectrumEditor : Editor
    {
        private static GUIStyle ToggleButtonStyleNormal = null;
        private static GUIStyle ToggleButtonStyleToggled = null;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This component is deprecated and will remove itself at runtime, use OceanWaveSpectrum assets instead.", MessageType.Warning);

            base.OnInspectorGUI();

            // preamble - styles for toggle buttons. this code and the below was based off the useful info provided by user Lasse here:
            // https://gamedev.stackexchange.com/questions/98920/how-do-i-create-a-toggle-button-in-unity-inspector
            if (ToggleButtonStyleNormal == null)
            {
                ToggleButtonStyleNormal = "Button";
                ToggleButtonStyleToggled = new GUIStyle(ToggleButtonStyleNormal);
                ToggleButtonStyleToggled.normal.background = ToggleButtonStyleToggled.active.background;
            }

            EditorGUILayout.Space();

            var spDisabled = serializedObject.FindProperty("_powerDisabled");
            EditorGUILayout.BeginHorizontal();
            bool allEnabled = true;
            for( int i = 0; i < spDisabled.arraySize; i++)
            {
                if (spDisabled.GetArrayElementAtIndex(i).boolValue) allEnabled = false;
            }
            bool toggle = allEnabled;
            if (toggle != EditorGUILayout.Toggle(toggle, GUILayout.Width(13f)))
            {
                for (int i = 0; i < spDisabled.arraySize; i++)
                {
                    spDisabled.GetArrayElementAtIndex(i).boolValue = toggle;
                }
            }
            EditorGUILayout.LabelField("Spectrum", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            var spec = target as WaveSpectrum;

            var spPower = serializedObject.FindProperty("_powerLog");
            
            for( int i = 0; i < spPower.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var spDisabled_i = spDisabled.GetArrayElementAtIndex(i);
                spDisabled_i.boolValue = !EditorGUILayout.Toggle(!spDisabled_i.boolValue, GUILayout.Width(15f));

                float smallWL = WaveSpectrum.SmallWavelength(i);
                EditorGUILayout.LabelField(string.Format("{0}", smallWL), GUILayout.Width(30f));
                var spPower_i = spPower.GetArrayElementAtIndex(i);
                spPower_i.floatValue = GUILayout.HorizontalSlider(spPower_i.floatValue, WaveSpectrum.MIN_POWER_LOG, WaveSpectrum.MAX_POWER_LOG);

                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Empirical Spectra", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            var spWindSpeed = serializedObject.FindProperty("_windSpeed");
            float spd_kmh = spWindSpeed.floatValue * 3.6f;
            EditorGUILayout.LabelField("Wind speed (km/h)", GUILayout.Width(120f));
            spd_kmh = EditorGUILayout.Slider(spd_kmh, 0f, 60f);
            spWindSpeed.floatValue = spd_kmh / 3.6f;
            EditorGUILayout.EndHorizontal();


            // descriptions from this very useful paper: https://hal.archives-ouvertes.fr/file/index/docid/307938/filename/frechot_realistic_simulation_of_ocean_surface_using_wave_spectra.pdf

            if (GUILayout.Button(new GUIContent("Phillips", "Base of modern parametric wave spectra"), spec._applyPhillipsSpectrum ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                spec._applyPhillipsSpectrum = !spec._applyPhillipsSpectrum;
            }
            if (spec._applyPhillipsSpectrum)
            {
                spec._applyJONSWAPSpectrum = spec._applyPiersonMoskowitzSpectrum = false;
                spec.ApplyPhillipsSpectrum(spWindSpeed.floatValue);
            }

            if (GUILayout.Button(new GUIContent("Pierson-Moskowitz", "Fully developed sea with infinite fetch"), spec._applyPiersonMoskowitzSpectrum ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                spec._applyPiersonMoskowitzSpectrum = !spec._applyPiersonMoskowitzSpectrum;
            }
            if (spec._applyPiersonMoskowitzSpectrum)
            {
                spec._applyPhillipsSpectrum = spec._applyJONSWAPSpectrum = false;
                spec.ApplyPiersonMoskowitzSpectrum(spWindSpeed.floatValue);
            }

            EditorGUILayout.BeginHorizontal();
            var spFetch = serializedObject.FindProperty("_fetch");
            spFetch.floatValue = EditorGUILayout.Slider("Fetch", spFetch.floatValue, 0f, 1000000f);
            EditorGUILayout.EndHorizontal();


            if (GUILayout.Button(new GUIContent("JONSWAP", "Fetch limited sea where waves continue to grow"), spec._applyJONSWAPSpectrum ? ToggleButtonStyleToggled : ToggleButtonStyleNormal))
            {
                spec._applyJONSWAPSpectrum = !spec._applyJONSWAPSpectrum;
            }
            if (spec._applyJONSWAPSpectrum)
            {
                spec._applyPhillipsSpectrum = spec._applyPiersonMoskowitzSpectrum = false;
                spec.ApplyJONSWAPSpectrum(spWindSpeed.floatValue);
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
