// Guids.cs
// MUST match guids.h

using System;

namespace AV.Cyclone
{
    static class GuidList
    {
        public const string guidCyclonePkgString = "90692408-765a-405e-927a-a8eaebe81bbd";
        public const string guidCycloneCmdSetString = "446a9d89-6688-41bc-b98d-754008cf9883";

        public static readonly Guid guidCycloneCmdSet = new Guid(guidCycloneCmdSetString);
        public static Guid guidCyclonePkgCmdSet = new Guid("{3BB7351B-93D2-4176-8964-59D37C7F9762}");
    };
}