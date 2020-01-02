﻿using System.Threading.Tasks;
using Lacey.Medusa.Common.Api.Base.Services;
using Lacey.Medusa.Common.Core.Extensions;
using Lacey.Medusa.Common.Core.Serializers;
using Lacey.Medusa.Common.Core.Utils;
using Lacey.Medusa.Common.Email.Services.Email;
using Lacey.Medusa.MakeMoney.Services.Extensions;
using Lacey.Medusa.MakeMoney.Services.Models.ScDevice;
using Lacey.Medusa.MakeMoney.Services.Models.ScNewsAndroid;
using Lacey.Medusa.MakeMoney.Services.Models.ScSaveFirebaseToken;
using Lacey.Medusa.MakeMoney.Services.Providers;
using Microsoft.Extensions.Logging;

namespace Lacey.Medusa.MakeMoney.Services.Services.Concrete
{
    public sealed class MakeMoneyService : IMakeMoneyService
    {
        #region Fields/Constructors

        private readonly ILogger logger;

        private readonly IEmailProvider emailProvider;

        private readonly bool isSendEmails;

        private readonly string userSecretsFile;

        private readonly string commonSecretsFile;

        private readonly IMmStoreService storeService;

        private readonly MakeMoneyProvider makeMoney;

        private ScDeviceResponse scDevice;

        public MakeMoneyService(
            ILogger logger,
            IEmailProvider emailProvider,
            bool isSendEmails, 
            string userSecretsFile, 
            string commonSecretsFile, 
            IMmStoreService storeService)
        {
            this.logger = logger;
            this.emailProvider = emailProvider;
            this.isSendEmails = isSendEmails;
            this.userSecretsFile = userSecretsFile;
            this.commonSecretsFile = commonSecretsFile;
            this.storeService = storeService;

            this.makeMoney = new MakeMoneyProvider(new BaseClientService.Initializer
            {
                Serializer = new NoRootXmlSerializer()
            });
        }

        #endregion

        public async Task Run()
        {
            await this.ScDevice();

            await this.ScSaveFirebaseToken();

            await this.ScNewsAndroid();
        }

        #region private methods

        private bool IsAuthenticated()
        {
            return this.scDevice != null;
        }

        private async Task ScDevice()
        {
            if (this.IsAuthenticated())
            {
                return;
            }

            await ProceedUtils.Proceed<bool?>(this.logger, async () =>
            {
                var session = this.storeService.GetScDevice();
                if (session != null)
                {
                    this.scDevice = session;
                    this.logger.LogTrace(this.scDevice.GetLog());
                    return true;
                }

                var request = this.makeMoney.ScDevice.ScDevice(new ScDeviceRequest
                {
                    AutoName = string.Empty,
                    Model = "Android SDK built for x86",
                    UniqueId = "6ebcffa7b5a59a34",
                    Manufacturer = "Google",
                    AppVesrsion = "4.0",
                    Carrier = "Android",
                    Brand = "google",
                    DeviceLanguage = "en-US",
                    FirebaseToken = GetFirebaseToken(),
                    PushId = string.Empty,
                    ReferalCode = string.Empty,
                    DeploymentType = "1",
                    ScreenHeight = "1794",
                    AutoEmail = string.Empty,
                    OsVersion = "9",
                    ScreenWidth = "1080"
                }).SetDefault();

                var response = await request.ExecuteAsync();
                if (response == null)
                {
                    this.logger.LogError("Authorization failed.");
                    DelayUtils.LargeDelay();
                    return null;
                }

                this.scDevice = response;
                this.logger.LogTrace(this.scDevice.GetLog());
                this.storeService.SaveScDevice(this.scDevice);
                return true;
            });
        }

        private async Task ScSaveFirebaseToken()
        {
            if (!this.IsAuthenticated())
            {
                return;
            }

            await ProceedUtils.Proceed<bool?>(this.logger, async () =>
            {
                var request = this.makeMoney.ScSaveFirebaseToken.ScSaveFirebaseToken(new ScSaveFirebaseTokenRequest
                {
                    FirebaseToken = GetFirebaseToken(),
                    CustomerId = this.scDevice.CustomerId,
                    Version = GetVersion()
                }).SetDefault();

                var response = await request.ExecuteAsync();
                this.logger.LogTrace(response.GetLog());
                return true;
            });
        }

        private async Task ScNewsAndroid()
        {
            if (!this.IsAuthenticated())
            {
                return;
            }

            await ProceedUtils.Proceed<bool?>(this.logger, async () =>
            {
                var request = this.makeMoney.ScNewsAndroid.ScNewsAndroid(new ScNewsAndroidRequest
                {
                    Os = "9",
                    Country = string.Empty,
                    CustomerId = this.scDevice.CustomerId,
                    Version = GetVersion(),
                    AdvertisingId = GetAdvertisingId()
                }).SetDefault();

                var response = await request.ExecuteAsync();
                this.logger.LogTrace(response.GetLog());
                return true;
            });
        }

        private static string GetFirebaseToken()
        {
            return "cdOIKCBIYIo:APA91bGjoz9f0dP63-EtfE_3SF4rg4Ej7ooVWbU3MXD4T-ZUwt3PbTYUFfnkOQ8_P_iMQFyMdotAfZrJaXEzP3s0svPx36VbuQQBJL_AJPYIGNM54pI_59ATjKFgkF29wyTvxT1z_Xq8";
        }

        private static string GetVersion()
        {
            return "6ebcffa7b5a59a34";
        }

        private static string GetAdvertisingId()
        {
            return "d1602ad3-80cd-462e-b00a-859732a25392";
        }

        #endregion
    }
}