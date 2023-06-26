using System;
using UMA;
using UMA.CharacterSystem;
using UMA.CharacterSystem.Examples;
using UMA.PoseTools;
using UnityEngine;
using UnityEngine.UI;

namespace o3n.UMARaces.Stunner.Scripts
{
    public class SampleExpressionUpdater : MonoBehaviour
    {
        public StTestCustomizerDD stTestCustomizerDD;
        
        private UMAAvatarBase avatar;
        private UMAExpressionPlayer expression;
        private bool available;

        [Range(0,6)]
        public int mood;

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
            SetAvatar();
        }

        public void OnDisable()
        {
            available = false;
            avatar.CharacterCreated.RemoveListener(OnCreated);
        }

        public void SetMood(int value)
        {
            mood = value;
        }

        private void SetAvatar()
        {
            if (stTestCustomizerDD == null)
            {
                avatar = null;
                available = false;
            }

            if (avatar != stTestCustomizerDD.Avatar)
            {
                avatar = stTestCustomizerDD.Avatar;
                if (avatar == null)
                {
                    available = false;
                }
                else
                {
                    Dropdown dropdown = GetComponent<Dropdown>();
                    dropdown.value = 0;
                    OnCreated(null);
                    avatar.CharacterUpdated.AddListener(OnCreated);
                }
            }
        }

        void Update()
        {
            SetAvatar();
            if (available)
            {
                float delta = 10 * Time.deltaTime;
                switch (mood)
                {
                    //case Moods.Neutral:
                    case 0:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 0, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 0, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 0f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 0, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 0, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 0, delta);
                        break;
                    //case Moods.SlightSmile:
                    case 1:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 0.5f, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 0.5f, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 0f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 0f, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0f, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0f, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0f, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0f, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0f, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0f, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0f, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0f, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0f, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, -0.2f, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, -0.2f, delta);
                        break;
                    //case Moods.Happy:
                    case 2:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 1f, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 1f, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 0f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 0f, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0f, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0f, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 1f, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 1f, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0f, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0f, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0f, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0f, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0.1f, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, -0.2f, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, -0.2f, delta);
                        break;
                    //case Moods.Sad:
                    case 3:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, -0.7f, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, -0.7f, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 0f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 0.7f, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0f, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0f, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0f, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0f, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0f, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, -0.3f, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0f, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0f, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 0f, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 0f, delta);
                        break;
                    //case Moods.Angry:
                    case 4:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, -0.3f, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, -0.3f, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 1f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, -1f, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 0.3f, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 0.3f, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 1f, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 1f, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, -0.7f, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, -0.7f, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0f, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, -1f, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 1f, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 0.2f, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 0.2f, delta);
                        break;
                    //case Moods.Surprised:
                    case 5:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 0f, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 0f, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 0f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, 1f, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 1f, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 1f, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 0f, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 0f, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0f, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, 0f, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0f, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0.4f, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 0f, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 1f, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 1f, delta);
                        break;
                    //case Moods.Extra:
                    case 6:
                        expression.leftMouthSmile_Frown = Mathf.Lerp(expression.leftMouthSmile_Frown, 0.5f, delta);
                        expression.rightMouthSmile_Frown = Mathf.Lerp(expression.rightMouthSmile_Frown, 0.5f, delta);
                        expression.browsIn = Mathf.Lerp(expression.browsIn, 1f, delta);
                        expression.midBrowUp_Down = Mathf.Lerp(expression.midBrowUp_Down, -1f, delta);
                        expression.leftBrowUp_Down = Mathf.Lerp(expression.leftBrowUp_Down, 1f, delta);
                        expression.rightBrowUp_Down = Mathf.Lerp(expression.rightBrowUp_Down, 1f, delta);
                        expression.rightUpperLipUp_Down = Mathf.Lerp(expression.rightUpperLipUp_Down, 1f, delta);
                        expression.leftUpperLipUp_Down = Mathf.Lerp(expression.leftUpperLipUp_Down, 1f, delta);
                        expression.rightLowerLipUp_Down = Mathf.Lerp(expression.rightLowerLipUp_Down, 0f, delta);
                        expression.leftLowerLipUp_Down = Mathf.Lerp(expression.leftLowerLipUp_Down, -0.3f, delta);
                        expression.mouthNarrow_Pucker = Mathf.Lerp(expression.mouthNarrow_Pucker, 0f, delta);
                        expression.jawOpen_Close = Mathf.Lerp(expression.jawOpen_Close, 0.25f, delta);
                        expression.noseSneer = Mathf.Lerp(expression.noseSneer, 1f, delta);
                        expression.leftEyeOpen_Close = Mathf.Lerp(expression.leftEyeOpen_Close, 1f, delta);
                        expression.rightEyeOpen_Close = Mathf.Lerp(expression.rightEyeOpen_Close, 1f, delta);
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnCreated(UMAData umaData)
        {
            expression = avatar.GetComponent<UMAExpressionPlayer>();
            if (expression == null)
            {
                return;
            }
            expression.enableBlinking = true;
            expression.enableSaccades = true;
            available = true;
        }
    }
}