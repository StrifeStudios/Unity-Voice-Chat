using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void DataChunkFilledEventHandler<T>(T[] buffer, int startingIndex);

/// <summary>
/// Provides a buffer which accumulates data and delivers chunks whenever the threshold amount is reached.
/// Can be used for streams of data which need to be processed in fixed-size chunks but may be collected in variable-sized chunks.
/// </summary>
/// <typeparam name="T">The type of data to store.</typeparam>
public class DataAccumulationBuffer<T> where T : struct
{
    #region Private Fields
    private T[] data;
    private int numLocalBufferElements = 0;
    private int chunkDeliverySize;
    #endregion Private Fields

    /// <summary>
    /// The chunk size that the buffer will broadcast when it is sufficiently filled.
    /// </summary>
    public int ChunkDeliverySize
    {
        get { return chunkDeliverySize; }
    }

    public event DataChunkFilledEventHandler<T> DataChunkFilled;

    public DataAccumulationBuffer(int chunkDeliverySize)
    {
        if (chunkDeliverySize <= 0)
        {
            throw new ArgumentException("chunkDeliverySize must be greater than zero.");
        }
        this.data = new T[chunkDeliverySize];
        this.chunkDeliverySize = chunkDeliverySize;
    }

    /// <summary>
    /// Accumulates all of the data in the given source buffer.
    /// </summary>
    /// <param name="sourceBuffer">The array to copy from.</param>
    public void AccumulateData(T[] sourceBuffer)
    {
        AccumulateData(sourceBuffer, 0, sourceBuffer.Length);
    }

    /// <summary>
    /// Accumulates all of the data in the given source buffer.
    /// </summary>
    /// <param name="sourceBuffer">The array to copy from.</param>
    /// <param name="startIndex">The index in the source buffer to start copying from.</param>
    public void AccumulateData(T[] sourceBuffer, int startIndex)
    {
        AccumulateData(sourceBuffer, startIndex, sourceBuffer.Length - startIndex);
    }

    /// <summary>
    /// Accumulates all of the data in the given source buffer.
    /// </summary>
    /// <param name="sourceBuffer">The array to copy from.</param>
    /// <param name="startIndex">The index in the source buffer to start copying from.</param>
    /// <param name="numElements">The number of elements to copy.</param>
    public void AccumulateData(T[] sourceBuffer, int startIndex, int numElements)
    {
        if (this.numLocalBufferElements + numElements >= chunkDeliverySize)
        {
            int elementsNeeded = chunkDeliverySize - this.numLocalBufferElements;
            Array.Copy(sourceBuffer, 0, this.data, numLocalBufferElements, elementsNeeded);
            DeliverChunkFromLocalBuffer();
            int sourceBufferLastRead = elementsNeeded;
            while (numElements - sourceBufferLastRead >= chunkDeliverySize)
            {
                DeliverChunkFromExternalBuffer(sourceBuffer, sourceBufferLastRead);
                sourceBufferLastRead += this.chunkDeliverySize;
            }
            if (sourceBufferLastRead < numElements)
            {
                int elementsToAdd = numElements - sourceBufferLastRead;
                Array.Copy(sourceBuffer, sourceBufferLastRead, this.data, this.numLocalBufferElements, elementsToAdd);
                this.numLocalBufferElements += elementsToAdd;
            }
        }
        else
        {
            Array.Copy(sourceBuffer, 0, this.data, this.numLocalBufferElements, numElements);
            this.numLocalBufferElements += numElements;
        }
    }

    private void DeliverChunkFromLocalBuffer()
    {
        if (this.DataChunkFilled != null)
        {
            DataChunkFilled(data, 0);
            this.numLocalBufferElements = 0;
        }
        else
        {
            throw new InvalidOperationException("Can't deliver a chunk, no subscribers to the event.");
        }
    }

    private void DeliverChunkFromExternalBuffer(T[] buffer, int startingIndex)
    {
        if (this.DataChunkFilled != null)
        {
            DataChunkFilled(buffer, startingIndex);
        }
        else
        {
            throw new InvalidOperationException("Can't deliver a chunk, no subscribers to the event.");
        }
    }
}