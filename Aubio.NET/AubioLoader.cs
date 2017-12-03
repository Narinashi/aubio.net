﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Aubio.NET.Win32;
using JetBrains.Annotations;

namespace Aubio.NET
{
    /// <summary>
    ///     Loads native dependencies for Aubio.NET.
    /// </summary>
    public static class AubioLoader
    {
        private static IntPtr Cookie { get; set; } = IntPtr.Zero;

        public static void Load([NotNull] string x86 = "x86", [NotNull] string x64 = "x64")
        {
            if (string.IsNullOrWhiteSpace(x86))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(x86));

            if (string.IsNullOrWhiteSpace(x64))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(x64));

            if (Cookie != IntPtr.Zero)
                throw new InvalidOperationException($"You can only call '{nameof(Load)}' once.");

            var directory = GetDependenciesDirectory(x86, x64);

            if (!NativeMethods.SetDefaultDllDirectories(NativeConstants.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS))
                throw new Win32Exception();

            var path = Path.Combine(Environment.CurrentDirectory, directory);

            var cookie = NativeMethods.AddDllDirectory(path);
            if (cookie == IntPtr.Zero)
                throw new Win32Exception();

            Cookie = cookie;
        }

        public static void Free()
        {
            if (Cookie == IntPtr.Zero)
                throw new InvalidOperationException($"You must call '{nameof(Load)}' first.");

            if (!NativeMethods.RemoveDllDirectory(Cookie))
                throw new Win32Exception();

            Cookie = IntPtr.Zero;
        }

        [SuppressMessage("ReSharper", "SwitchStatementMissingSomeCases")]
        private static string GetDependenciesDirectory([NotNull] string x86 = "x86", [NotNull] string x64 = "x64")
        {
            if (string.IsNullOrWhiteSpace(x86))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(x86));

            if (string.IsNullOrWhiteSpace(x64))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(x64));

            var assembly = Assembly.GetEntryAssembly();

            var module = assembly.Modules.Single();
            module.GetPEKind(out var kind, out var _);

            if (!kind.HasFlag(PortableExecutableKinds.ILOnly))
                throw new PlatformNotSupportedException(); // whatever

            if (kind.HasFlag(PortableExecutableKinds.Preferred32Bit) ||
                kind.HasFlag(PortableExecutableKinds.Required32Bit))
                return x86;

            if (kind.HasFlag(PortableExecutableKinds.PE32Plus))
                return x86;

            throw new PlatformNotSupportedException();
        }
    }
}