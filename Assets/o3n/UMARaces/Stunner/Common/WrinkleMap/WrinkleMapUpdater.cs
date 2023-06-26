using System;
using UMA;
using UMA.CharacterSystem;
using UMA.PoseTools;
using UnityEngine;

namespace o3n.UMARaces.Stunner.Common.WrinkleMap
{
    public class WrinkleMapUpdater : MonoBehaviour
    {
        public float cheekMaxWeight = 0.5f;
        
        private DynamicCharacterAvatar avatar;
        private UMAExpressionPlayer expression;

        private Material wrinkleMapMaterial;
        private float mouthLeftWeight = 0;
        private float mouthRightWeight = 0;
        private float smileLeftWeight = 0;
        private float smileRightWeight = 0;
        private float noseSneerWeight = 0;
        private float midBrowDownWeight = 0;
        private float browsInWeight = 0;
        private float leftBrowUpWeight = 0;
        private float rightBrowUpWeight = 0;
        private float eyeSquintWeight = 0;

        public void OnEnable()
        {
            avatar = GetComponent<DynamicCharacterAvatar>();
            if (avatar != null)
            {
                avatar.CharacterUpdated.AddListener(OnCreated);
            }
        }

        public void OnDisable()
        {
            avatar.CharacterUpdated.RemoveListener(OnCreated);
        }

        private void OnCreated(UMAData umaData)
        {
            expression = GetComponent<UMAExpressionPlayer>();
            if (expression == null)
            {
                return;
            }

            InitializeWrinkleMapMaterial();
            expression.ExpressionChanged.AddListener(HandlePoseChange);
        }

        public void InitializeWrinkleMapMaterial()
        {
            Material[] materials = GetComponentInChildren<Renderer>().materials;
            foreach (var material in materials)
            {
                if (material.name == "st_URP_Skin_WrinkleMap_Material (Instance)")
                {
                    wrinkleMapMaterial = material;
                }
            }

            UpdateWrinkleMapWeights();
        }

        public void HandlePoseChange(UMAData umaData, string poseName, float weight)
        {
            if (wrinkleMapMaterial == null)
            {
                return;
            }

            if ("mouthLeft_Right" == poseName)
            {
                if (weight < 0)
                {
                    mouthRightWeight = -weight;
                    mouthLeftWeight = 0;
                }
                else
                {
                    mouthLeftWeight = weight;
                    mouthRightWeight = 0;
                }
            }
            else if ("leftMouthSmile_Frown" == poseName)
            {
                if (weight < 0)
                {
                    smileLeftWeight = 0;
                }
                else
                {
                    smileLeftWeight = weight;
                }
            }
            else if ("rightMouthSmile_Frown" == poseName)
            {
                if (weight < 0)
                {
                    smileRightWeight = 0;
                }
                else
                {
                    smileRightWeight = weight;
                }
            }
            else if ("noseSneer" == poseName)
            {
                if (weight < 0)
                {
                    noseSneerWeight = 0;
                }
                else
                {
                    noseSneerWeight = weight;
                }
            }
            else if ("midBrowUp_Down" == poseName)
            {
                if (weight < 0)
                {
                    midBrowDownWeight = -weight;
                }
                else
                {
                    midBrowDownWeight = 0;
                }
            }
            else if ("browsIn" == poseName)
            {
                if (weight < 0)
                {
                    browsInWeight = 0;
                }
                else
                {
                    browsInWeight = weight;
                }
            }
            else if ("leftBrowUp_Down" == poseName)
            {
                if (weight < 0)
                {
                    leftBrowUpWeight = 0;
                }
                else
                {
                    leftBrowUpWeight = weight;
                }
            }
            else if ("rightBrowUp_Down" == poseName)
            {
                if (weight < 0)
                {
                    rightBrowUpWeight = 0;
                }
                else
                {
                    rightBrowUpWeight = weight;
                }
            }
            else if ("leftCheekPuff_Squint" == poseName)
            {
                if (weight < 0)
                {
                    eyeSquintWeight = -weight;
                }
                else
                {
                    eyeSquintWeight = 0;
                }
            }

            UpdateWrinkleMapWeights();
        }

        public void UpdateWrinkleMapWeights()
        {
            wrinkleMapMaterial.SetFloat("_ForeheadLeftWeight", leftBrowUpWeight);
            wrinkleMapMaterial.SetFloat("_ForeheadRightWeight", rightBrowUpWeight);
            wrinkleMapMaterial.SetFloat("_CheekLeftWeight", Math.Max(mouthLeftWeight, smileLeftWeight * cheekMaxWeight));
            wrinkleMapMaterial.SetFloat("_CheekRightWeight", Math.Max(mouthRightWeight, smileRightWeight * cheekMaxWeight));
            wrinkleMapMaterial.SetFloat("_EyeLeftWeight", Math.Max(smileLeftWeight, eyeSquintWeight));
            wrinkleMapMaterial.SetFloat("_EyeRightWeight", Math.Max(smileRightWeight, eyeSquintWeight));
            wrinkleMapMaterial.SetFloat("_BrowMiddleWeight",
                Math.Max(Math.Max(noseSneerWeight * .15f, midBrowDownWeight * .5f), browsInWeight));
        }
    }
}