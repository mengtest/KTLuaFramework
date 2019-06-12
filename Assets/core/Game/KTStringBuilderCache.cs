using System.IO;
using System.Text;
using System;

namespace LuaFramework
{
    public static class KTStringBuilderCache
    {
        [ThreadStatic]
        static StringBuilder m_cache = new StringBuilder();
        const int kMaxBuilderSize = 512;

        public static StringBuilder Acquire(int capacity = 256)
        {
            StringBuilder cache = m_cache;
            if (cache == null || cache.Capacity < capacity)
            {
                return new StringBuilder(capacity);
            }
            m_cache = null;
            cache.Length = 0;
            return cache;
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity > kMaxBuilderSize)
            {
                return;
            }
            m_cache = sb;
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            string str = sb.ToString();
            KTStringBuilderCache.Release(sb);
            return str;
        }
    }
}
