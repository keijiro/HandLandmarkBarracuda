using Unity.Barracuda;
using UnityEngine;
using Klak.NNUtils;
using Klak.NNUtils.Extensions;

namespace MediaPipe.HandLandmark {

//
// Implementation of the hand landmark detector class
//
public sealed partial class HandLandmarkDetector : System.IDisposable
{
    #region Private objects

    ResourceSet _resources;
    IWorker _worker;
    ImagePreprocess _preprocess;
    GraphicsBuffer _output;
    BufferReader<Vector4> _readCache;

    void AllocateObjects(ResourceSet resources)
    {
        _resources = resources;

        // NN model
        var model = ModelLoader.Load(_resources.model);

        // GPU worker
        _worker = model.CreateWorker(WorkerFactory.Device.GPU);

        // Preprocess
        _preprocess = new ImagePreprocess(ImageSize, ImageSize, nchwFix: true);

        // Output buffer
        _output = BufferUtil.NewStructured<Vector4>(VertexCount + 1);

        // Landmark data read cache
        _readCache = new BufferReader<Vector4>(_output, VertexCount + 1);
    }

    void DeallocateObjects()
    {
        _worker?.Dispose();
        _worker = null;

        _preprocess?.Dispose();
        _preprocess = null;

        _output?.Dispose();
        _output = null;
    }

    #endregion

    #region Neural network inference function

    void RunModel(Texture source)
    {
        _preprocess.Dispatch(source, _resources.preprocess);
        RunModel();
    }

    void RunModel()
    {
        // NN worker execution
        _worker.Execute(_preprocess.Tensor);

        // Postprocessing
        var post = _resources.postprocess;
        post.SetBuffer(0, "_Landmark", _worker.PeekOutputBuffer("Identity"));
        post.SetBuffer(0, "_Score", _worker.PeekOutputBuffer("Identity_1"));
        post.SetBuffer(0, "_Handedness", _worker.PeekOutputBuffer("Identity_2"));
        post.SetBuffer(0, "_Output", _output);
        post.Dispatch(0, 1, 1, 1);

        // Cache data invalidation
        _readCache.InvalidateCache();
    }

    #endregion
}

} // namespace MediaPipe.HandLandmark
