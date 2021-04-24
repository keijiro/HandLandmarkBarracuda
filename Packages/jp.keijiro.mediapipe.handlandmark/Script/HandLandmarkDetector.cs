using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;

namespace MediaPipe.HandLandmark {

//
// Hand landmark detector class
//
public sealed class HandLandmarkDetector : System.IDisposable
{
    #region Public accessors

    public const int VertexCount = 21;

    public ComputeBuffer VertexBuffer
      => _postBuffer;

    public IEnumerable<Vector4> VertexArray
      => _postRead ? _postReadCache : UpdatePostReadCache();

    #endregion

    #region Public methods

    public HandLandmarkDetector(ResourceSet resources)
    {
        _resources = resources;
        AllocateObjects();
    }

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture image)
      => RunModel(image);

    #endregion

    #region Compile-time constants

    // Input image size (defined by the model)
    const int ImageSize = 224;

    #endregion

    #region Private objects

    ResourceSet _resources;
    ComputeBuffer _preBuffer;
    ComputeBuffer _postBuffer;
    IWorker _worker;

    void AllocateObjects()
    {
        var model = ModelLoader.Load(_resources.model);
        _preBuffer = new ComputeBuffer(ImageSize * ImageSize * 3, sizeof(float));
        _postBuffer = new ComputeBuffer(VertexCount, sizeof(float) * 4);
        _worker = model.CreateWorker();
    }

    void DeallocateObjects()
    {
        _preBuffer?.Dispose();
        _preBuffer = null;

        _postBuffer?.Dispose();
        _postBuffer = null;

        _worker?.Dispose();
        _worker = null;
    }

    #endregion

    #region Neural network inference function

    void RunModel(Texture source)
    {
        // Preprocessing
        var pre = _resources.preprocess;
        pre.SetTexture(0, "_Texture", source);
        pre.SetBuffer(0, "_Tensor", _preBuffer);
        pre.Dispatch(0, ImageSize / 8, ImageSize / 8, 1);

        // Run the BlazeFace model.
        using (var tensor = new Tensor(1, ImageSize, ImageSize, 3, _preBuffer))
            _worker.Execute(tensor);

        // Postprocessing
        var post = _resources.postprocess;
        var landmarkRT = _worker.CopyOutputToTempRT("Identity", 3, VertexCount);
        post.SetTexture(0, "_Landmarks", landmarkRT);
        post.SetBuffer(0, "_Vertices", _postBuffer);
        post.Dispatch(0, 1, 1, 1);
        RenderTexture.ReleaseTemporary(landmarkRT);

        // Read cache invalidation
        _postRead = false;
    }

    #endregion

    #region GPU to CPU readback

    Vector4[] _postReadCache = new Vector4[VertexCount];
    bool _postRead;

    Vector4[] UpdatePostReadCache()
    {
        _postBuffer.GetData(_postReadCache, 0, 0, VertexCount);
        _postRead = true;
        return _postReadCache;
    }

    #endregion
}

} // namespace MediaPipe.HandLandmark
