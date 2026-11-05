using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AgenteMirada
{
    public enum tobii_error_t
    {
        TOBII_ERROR_NO_ERROR = 0,
        TOBII_ERROR_INTERNAL = 1,
        TOBII_ERROR_INSUFFICIENT_LICENSE = 2,
        TOBII_ERROR_NOT_SUPPORTED = 3,
        TOBII_ERROR_NOT_AVAILABLE = 4,
        TOBII_ERROR_CONNECTION_FAILED = 5,
        TOBII_ERROR_TIMED_OUT = 6,
        TOBII_ERROR_ALLOCATION_FAILED = 7,
        TOBII_ERROR_INVALID_PARAMETER = 8,
        TOBII_ERROR_CALIBRATION_ALREADY_STARTED = 9,
        TOBII_ERROR_CALIBRATION_NOT_STARTED = 10,
        TOBII_ERROR_ALREADY_SUBSCRIBED = 11,
        TOBII_ERROR_NOT_SUBSCRIBED = 12,
        TOBII_ERROR_OPERATION_FAILED = 13,
        TOBII_ERROR_CONFLICTING_API_INSTANCES = 14,
        TOBII_ERROR_CALIBRATION_BUSY = 15,
        TOBII_ERROR_CALLBACK_IN_PROGRESS = 16
    }

    public enum tobii_validity_t
    {
        TOBII_VALIDITY_INVALID = 0,
        TOBII_VALIDITY_VALID = 1
    }

    public enum tobii_field_of_use_t
    {
        TOBII_FIELD_OF_USE_STORE_OR_TRANSFER_FALSE = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tobii_gaze_point_t
    {
        public long timestamp_us;
        public tobii_validity_t validity;
        public float x;
        public float y;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void tobii_gaze_point_callback_t(ref tobii_gaze_point_t gaze_point, IntPtr user_data);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void tobii_device_url_receiver_t(IntPtr url, IntPtr user_data);

    public static class Interop
    {
        private const string DLL = "tobii_stream_engine";
        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_api_create(out IntPtr api, IntPtr custom_alloc, IntPtr custom_log);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_api_destroy(IntPtr api);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_enumerate_local_device_urls(IntPtr api, tobii_device_url_receiver_t receptor, IntPtr user_data);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_device_create(IntPtr api, [MarshalAs(UnmanagedType.LPStr)] string url, tobii_field_of_use_t field_of_use, out IntPtr device);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_device_destroy(IntPtr device);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_gaze_point_subscribe(IntPtr device, tobii_gaze_point_callback_t callback, IntPtr user_data);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_gaze_point_unsubscribe(IntPtr device);

        [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern tobii_error_t tobii_device_process_callbacks(IntPtr device);

        public static List<string> ListarDispositivos(IntPtr api)
        {
            var urls = new List<string>();

            tobii_device_url_receiver_t receptor = (url, usuario) => urls.Add(Marshal.PtrToStringAnsi(url));

            tobii_enumerate_local_device_urls(api, receptor, IntPtr.Zero);

            GC.KeepAlive(receptor);

            return urls;
        }
    }
}
