using UnityEngine;

namespace RootMain
{
    public class ShaderColorChanger : MonoBehaviour
    {
        MeshRenderer[] _meshRenderers;
        SkinnedMeshRenderer[] _skinMeshRenderers;
        public Color _meshColor;

        private void Start() => StartFunc();

        private void StartFunc()
        {
            _meshRenderers = GetComponentsInChildren<MeshRenderer>();
            _skinMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            if(_meshRenderers.Length > 0)
            {
                for(int i = 0; i < _meshRenderers.Length; i++)
                {
                    _meshRenderers[i].material.color = _meshColor;
                }
            }

            if(_skinMeshRenderers.Length > 0)
            {
                for(int i = 0; i < _skinMeshRenderers.Length; i++)
                {
                    _skinMeshRenderers[i].material.color = _meshColor;
                }
            }

        }

        private void Update() => UpdateFunc();

        private void UpdateFunc()
        {
        
        }
    }
}