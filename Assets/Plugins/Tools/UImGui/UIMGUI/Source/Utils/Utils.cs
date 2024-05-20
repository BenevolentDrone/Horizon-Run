using ImGuiNET;

using System;

using System.Runtime.CompilerServices;

using System.Text;

using UnityEngine;

namespace UImGui
{
	internal static unsafe class Utils
	{
		internal static UnityEngine.Vector2 ToUnityVector2(this System.Numerics.Vector2 numericsVector)
		{
			return new UnityEngine.Vector2(numericsVector.X, numericsVector.Y);
		}

		internal static System.Numerics.Vector2 ToNumericsVector2(this UnityEngine.Vector2 unityVector)
		{
			return new System.Numerics.Vector2(unityVector.x, unityVector.y);
		}
		
		internal static UnityEngine.Color ToUnityColor(this System.Numerics.Vector4 numericsVector4)
		{
			return new UnityEngine.Color(numericsVector4.X, numericsVector4.Y, numericsVector4.Z, numericsVector4.W);
		}
		
		internal static System.Numerics.Vector4 ToNumericsVector4(this UnityEngine.Color unityColor)
		{
			return new System.Numerics.Vector4(unityColor.r, unityColor.g, unityColor.b, unityColor.a);
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 ScreenToImGui(in Vector2 point)
		{
			return new Vector2(point.x, ImGui.GetIO().DisplaySize.Y - point.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vector2 ImGuiToScreen(in Vector2 point)
		{
			return new Vector2(point.x, ImGui.GetIO().DisplaySize.Y - point.y);
		}

		internal static string StringFromPtr(byte* ptr)
		{
			int characters = 0;
			while (ptr[characters] != 0)
			{
				characters++;
			}

			return Encoding.UTF8.GetString(ptr, characters);
		}

		internal static int GetUtf8(string text, byte* utf8Bytes, int utf8ByteCount)
		{
			fixed (char* utf16Ptr = text)
			{
				return Encoding.UTF8.GetBytes(utf16Ptr, text.Length, utf8Bytes, utf8ByteCount);
			}
		}

		internal static int GetUtf8(string text, int start, int length, byte* utf8Bytes, int utf8ByteCount)
		{
			if (start < 0 || length < 0 || start + length > text.Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			fixed (char* utf16Ptr = text)
			{
				return Encoding.UTF8.GetBytes(utf16Ptr + start, length, utf8Bytes, utf8ByteCount);
			}
		}
	}
}