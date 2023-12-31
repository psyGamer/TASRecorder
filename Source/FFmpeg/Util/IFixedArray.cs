﻿#pragma warning disable

namespace FFmpeg.Util;

public interface IFixedArray {
    int Length { get; }
}

internal interface IFixedArray<T> : IFixedArray {
    T this[uint index] { get; set; }
    T[] ToArray();
    void UpdateFrom(T[] array);
}
