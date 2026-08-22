// Copyright (c) 2023 Vladimir Popov zor1994@gmail.com https://github.com/ZorPastaman/UtilityAI

using System;
using System.Collections.Concurrent;
using Unity.Profiling;

namespace Zor.UtilityAI.Profiling
{
	internal static class UtilityAIProfiler
	{
#if ENABLE_PROFILER
		private static readonly ConcurrentDictionary<string, ProfilerMarker> Markers = new();
#endif

		internal static Scope Sample(string name)
		{
#if ENABLE_PROFILER
			return new Scope(Markers.GetOrAdd(name, static markerName => new ProfilerMarker(markerName)));
#else
			return default;
#endif
		}

		internal static Scope Sample(string typeName, string methodName)
		{
#if ENABLE_PROFILER
			return Sample($"{typeName}.{methodName}");
#else
			return default;
#endif
		}

		internal static Scope Sample<T>(string typeName, string methodName)
		{
#if ENABLE_PROFILER
			return Sample($"{typeName}.{methodName}.{typeof(T).FullName}");
#else
			return default;
#endif
		}

		internal static Scope Sample(Type type, string typeName, string methodName)
		{
#if ENABLE_PROFILER
			return Sample($"{typeName}.{methodName}.{type.FullName}");
#else
			return default;
#endif
		}

		internal readonly struct Scope : IDisposable
		{
#if ENABLE_PROFILER
			private readonly ProfilerMarker.AutoScope m_scope;

			internal Scope(ProfilerMarker marker)
			{
				m_scope = marker.Auto();
			}
#endif

			public void Dispose()
			{
#if ENABLE_PROFILER
				m_scope.Dispose();
#endif
			}
		}
	}
}
