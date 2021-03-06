﻿using System;
using Lacey.Medusa.Vendor.AdColony.Models.Ads30.Configure;

namespace Lacey.Medusa.Vendor.AdColony.Utils
{
    public static class AdColonyDefault
    {
        public static ConfigureRequestModel GetConfigureRequest(
            string advertiserId,
            string customerId,
            string mediaPath,
            string tempStoragePath,
            string userId,
            string appId,
            string bundleId,
            string[] zones,
            string sid,
            string[] zoneIds,
            string guidKeyPostfix)
        {
            var guid = Guid.NewGuid().ToString();
            var res = new ConfigureRequestModel
            {
                AdvertiserId = advertiserId,
                Carrier = string.Empty,
                CustomId = customerId,
                LimitTracking = false,
                Ln = "en",
                Locale = "US",
                MediaPath = mediaPath,
                TempStoragePath = tempStoragePath,
                DeviceBrand = "Google",
                DeviceModel = "Android SDK built for x86",
                DeviceType = "phone",
                NetworkType = "wifi",
                OsName = "android",
                OsVersion = "9",
                SdkVersion = "3.3.8",
                BatteryLevel = 1,
                SdkType = "android_native",
                CurrentOrientation = 0,
                TimezoneIetf = "GMT",
                TimezoneGmtM = 0,
                TimezoneDstM = 0,
                CellServiceCountryCode = "us",
                DisplayDpi = 420,
                AndroidIdSha1 = string.Empty,
                DeviceApi = 28,
                MemoryUsedMb = 6,
                MemoryClass = 384,
                AvailableStores = new []{ "google" },
                Permissions = new []
                {
                    "android.permission.INTERNET",
                    "android.permission.ACCESS_NETWORK_STATE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.READ_PHONE_STATE",
                    "android.permission.ACCESS_WIFI_STATE",
                    "android.permission.SYSTEM_ALERT_WINDOW",
                    "android.permission.GET_TASKS",
                    "com.google.android.c2dm.permission.RECEIVE",
                    "android.permission.GET_ACCOUNTS",
                    "android.permission.WAKE_LOCK",
                    "android.permission.ACCESS_DOWNLOAD_MANAGER",
                    "android.permission.ACCESS_COARSE_LOCATION",
                    "android.permission.VIBRATE",
                    "com.google.android.finsky.permission.BIND_GET_INSTALL_REFERRER_SERVICE",
                    "proxima.makemoney.android.permission.C2D_MESSAGE",
                    "android.permission.RECEIVE_BOOT_COMPLETED",
                    "com.sec.android.provider.badge.permission.READ",
                    "com.sec.android.provider.badge.permission.WRITE",
                    "com.htc.launcher.permission.READ_SETTINGS",
                    "com.htc.launcher.permission.UPDATE_SHORTCUT",
                    "com.sonyericsson.home.permission.BROADCAST_BADGE",
                    "com.sonymobile.home.permission.PROVIDER_INSERT_BADGE",
                    "com.anddoes.launcher.permission.UPDATE_COUNT",
                    "com.majeur.launcher.permission.UPDATE_BADGE",
                    "com.huawei.android.launcher.permission.CHANGE_BADGE",
                    "com.huawei.android.launcher.permission.READ_SETTINGS",
                    "com.huawei.android.launcher.permission.WRITE_SETTINGS",
                    "android.permission.READ_APP_BADGE",
                    "com.oppo.launcher.permission.READ_SETTINGS",
                    "com.oppo.launcher.permission.WRITE_SETTINGS",
                    "me.everything.badger.permission.BADGE_COUNT_READ",
                    "me.everything.badger.permission.BADGE_COUNT_WRITE",
                    "android.permission.READ_EXTERNAL_STORAGE"
                },
                Immersion = false,
                ScreenHeight = 1794,
                ScreenWidth = 1080,
                CleartextPermitted = true,
                OriginStore = "google",
                UserId = userId,
                AppId = appId,
                BundleId = bundleId,
                Zones = zones,
                Sid = sid,
                SImpCount = 1,
                DeviceTime = 1577431894837,
                ControllerVersion = "2.3.0",
                ZoneIds = zoneIds,
                ForceAdId = string.Empty,
                TestMode = false,
                STime = 82.62299990653992,
                AdRequest = true,
                DeviceAudio = true,
                Guid = guid,
                GuidKey = $"{guid}{guidKeyPostfix}"
            };

            return res;
        }
    }
}