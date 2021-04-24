using Unity.Barracuda;
using UnityEngine;

namespace MediaPipe.HandLandmark {

static class IWorkerExtensions
{
    // Peek a compute buffer of a worker output tensor.
    public static ComputeBuffer PeekOutputBuffer
      (this IWorker worker, string name)
      => ((ComputeTensorData)worker.PeekOutput(name).data).buffer;
}

} // namespace MediaPipe.HandLandmark
