using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using MediaPipe.HandLandmark;

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] RawImage _previewUI = null;
    [SerializeField] Text _scoreUI = null;
    [SerializeField] Text _handednessUI = null;
    [SerializeField] Shader _shader = null;

    #endregion

    #region Private members

    HandLandmarkDetector _detector;
    Material _material;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _detector = new HandLandmarkDetector(_resources);
        _material = new Material(_shader);
    }

    void OnDestroy()
    {
        _detector.Dispose();
        Destroy(_material);
    }

    void LateUpdate()
    {
        _detector.ProcessImage(_source.Texture);
        _previewUI.texture = _source.Texture;
        _scoreUI.text = $"Score: {_detector.Score:0.00}";
        _handednessUI.text = $"Handedness: {_detector.Handedness:0.00}";
    }

    void OnRenderObject()
    {
        _material.SetBuffer("_Vertices", _detector.OutputBuffer);
        _material.SetPass(0);
        Graphics.DrawProceduralNow
          (MeshTopology.Lines, 4, HandLandmarkDetector.VertexCount);
    }

    #endregion
}
