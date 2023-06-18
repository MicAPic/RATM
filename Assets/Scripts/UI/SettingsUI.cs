using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UI
{
    public class SettingsUI : UI
    {
        [Header("Pipelines")]
        [SerializeField] 
        private UniversalRenderPipelineAsset[] renderPipelineAssets;
        private UniversalRenderPipelineAsset _currentRenderPipelineAsset;

        [Header("Dropdowns")] 
        [SerializeField] 
        private TMP_Dropdown msaaDropdown;
        [SerializeField] 
        private TMP_Dropdown shadowsDropdown;

        private void Awake()
        {
            _currentRenderPipelineAsset = UniversalRenderPipeline.asset;
        }

        void Start()
        {
            SetMSAADropdown();
            SetShadowsDropdown();
        }

        public void SetMSAASetting(int index)
        {
            _currentRenderPipelineAsset.msaaSampleCount = index switch
            {
                0 => 1,
                1 => 2,
                2 => 4,
                3 => 8
            };
        }

        public void SetShadowsSetting(int index)
        {
            var currentMSAAIndex = msaaDropdown.value;
            QualitySettings.SetQualityLevel(index, false);

            _currentRenderPipelineAsset = renderPipelineAssets[index];
            SetMSAASetting(currentMSAAIndex);
        }

        private void SetMSAADropdown()
        {
            msaaDropdown.value = _currentRenderPipelineAsset.msaaSampleCount switch
            {
                1 => 0,
                2 => 1,
                4 => 2,
                8 => 3
            };
        }

        private void SetShadowsDropdown()
        {
            Debug.Log("Setting set: " + QualitySettings.GetQualityLevel());
            shadowsDropdown.value = QualitySettings.GetQualityLevel();
        }
    }
}
