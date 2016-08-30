﻿using System.Collections.Generic;
using UnityEngine;

namespace ProceduralToolkit.Examples.UI
{
    public class ChairGeneratorUI : UIBase
    {
        public MeshFilter meshFilter;
        public RectTransform leftPanel;

        [Space]
        [Range(minLegWidth, maxLegWidth)]
        public float legWidth = 0.07f;
        [Range(minLegHeight, maxLegHeight)]
        public float legHeight = 0.7f;
        [Range(minSeatWidth, maxSeatWidth)]
        public float seatWidth = 0.7f;
        [Range(minSeatDepth, maxSeatDepth)]
        public float seatDepth = 0.7f;
        [Range(minSeatHeight, maxSeatHeight)]
        public float seatHeight = 0.05f;
        [Range(minBackHeight, maxBackHeight)]
        public float backHeight = 0.8f;
        public bool hasStretchers = true;
        public bool hasArmrests = false;

        private const float minLegWidth = 0.05f;
        private const float maxLegWidth = 0.12f;
        private const float minLegHeight = 0.5f;
        private const float maxLegHeight = 1.2f;
        private const float minSeatWidth = 0.5f;
        private const float maxSeatWidth = 1.2f;
        private const float minSeatDepth = 0.3f;
        private const float maxSeatDepth = 1.2f;
        private const float minSeatHeight = 0.03f;
        private const float maxSeatHeight = 0.2f;
        private const float minBackHeight = 0.5f;
        private const float maxBackHeight = 1.3f;

        private List<ColorHSV> targetPalette = new List<ColorHSV>();
        private List<ColorHSV> currentPalette = new List<ColorHSV>();

        private void Awake()
        {
            RenderSettings.skybox = new Material(RenderSettings.skybox);

            Generate();
            currentPalette.AddRange(targetPalette);

            InstantiateControl<SliderControl>(leftPanel)
                .Initialize("Leg width", minLegWidth, maxLegWidth, legWidth, value =>
                {
                    legWidth = value;
                    Generate();
                });
            InstantiateControl<SliderControl>(leftPanel)
                .Initialize("Leg height", minLegHeight, maxLegHeight, legHeight, value =>
                {
                    legHeight = value;
                    Generate();
                });

            InstantiateControl<SliderControl>(leftPanel)
                .Initialize("Seat width", minSeatWidth, maxSeatWidth, seatWidth, value =>
                {
                    seatWidth = value;
                    Generate();
                });
            InstantiateControl<SliderControl>(leftPanel)
                .Initialize("Seat depth", minSeatDepth, maxSeatDepth, seatDepth, value =>
                {
                    seatDepth = value;
                    Generate();
                });
            InstantiateControl<SliderControl>(leftPanel)
                .Initialize("Seat height", minSeatHeight, maxSeatHeight, seatHeight, value =>
                {
                    seatHeight = value;
                    Generate();
                });

            InstantiateControl<SliderControl>(leftPanel)
                .Initialize("Back height", minBackHeight, maxBackHeight, backHeight, value =>
                {
                    backHeight = value;
                    Generate();
                });

            InstantiateControl<ToggleControl>(leftPanel).Initialize("Has stretchers", hasStretchers, value =>
            {
                hasStretchers = value;
                Generate();
            });
            InstantiateControl<ToggleControl>(leftPanel).Initialize("Has armrests", hasArmrests, value =>
            {
                hasArmrests = value;
                Generate();
            });

            InstantiateControl<ButtonControl>(leftPanel).Initialize("Generate", Generate);
        }

        private void Update()
        {
            SkyBoxGenerator.LerpSkybox(RenderSettings.skybox, currentPalette, targetPalette, 1, 2, 3, Time.deltaTime);
        }

        public void Generate()
        {
            targetPalette = new ColorHSV(Random.value, 0.5f, 0.75f).GetTriadicPalette();
            targetPalette.Add(ColorHSV.Lerp(targetPalette[1], targetPalette[2], 0.5f));

            var draft = ChairGenerator.Chair(legWidth, legHeight, seatWidth, seatDepth, seatHeight, backHeight,
                hasStretchers, hasArmrests, targetPalette[0].WithS(0.8f).WithV(0.8f).ToColor());

            var circle = MeshDraft.TriangleFan(PTUtils.PointsOnCircle3XZ(2, 128));
            circle.Paint(new Color(0.8f, 0.8f, 0.8f, 1));
            draft.Add(circle);

            meshFilter.mesh = draft.ToMesh();
        }
    }
}