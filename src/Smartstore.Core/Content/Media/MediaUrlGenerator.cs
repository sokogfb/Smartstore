﻿using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Smartstore.Core.Configuration;
using Smartstore.Core.Content.Media.Imaging;
using Smartstore.Core.Stores;
using Smartstore.Engine;
using Smartstore.Utilities;

namespace Smartstore.Core.Content.Media
{
    public partial class MediaUrlGenerator : IMediaUrlGenerator
    {
        const string _fallbackImagesRootPath = "content/images/";

        private readonly IMediaStorageConfiguration _storageConfig;
        private readonly string _host;
        private readonly string _pathBase;
        private readonly string _fallbackImageFileName;
        private readonly string _processedImagesRootPath;

        public MediaUrlGenerator(
            IApplicationContext appContext,
            IMediaStorageConfiguration storageConfig,
            ISettingService settingService,
            MediaSettings mediaSettings,
            IStoreContext storeContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _storageConfig = storageConfig;
            _processedImagesRootPath = storageConfig.PublicPath;

            var httpContext = httpContextAccessor.HttpContext;
            string pathBase = "/";

            if (httpContext != null)
            {
                var request = httpContext.Request;
                pathBase = request.PathBase;

                var cdn = storeContext.CurrentStore.ContentDeliveryNetwork;
                if (cdn.HasValue() && !CommonHelper.IsDevEnvironment && !httpContext.Connection.IsLocal())
                {
                    _host = cdn;
                }
                else if (mediaSettings.AutoGenerateAbsoluteUrls)
                {
                    _host = "//{0}{1}".FormatInvariant(request.Host, pathBase);
                }
                else
                {
                    _host = pathBase;
                }
            }

            _host = _host.EmptyNull().EnsureEndsWith('/');
            _pathBase = pathBase.EnsureEndsWith('/');
            _fallbackImageFileName = settingService.GetSettingByKeyAsync("Media.DefaultImageName", "default-image.png").Await();
        }

        public static string FallbackImagesRootPath => _fallbackImagesRootPath;

        public virtual string GenerateUrl(
            MediaFileInfo file,
            ProcessImageQuery imageQuery,
            string host = null,
            bool doFallback = true)
        {
            string path;

            // Build virtual path with pattern "media/{id}/{album}/{dir}/{NameWithExt}"
            if (file?.Path != null)
            {
                path = _processedImagesRootPath + file.Id.ToString(CultureInfo.InvariantCulture) + "/" + file.Path;
            }
            else if (doFallback)
            {
                path = _processedImagesRootPath + "0/" + _fallbackImageFileName;
            }
            else
            {
                return null;
            }

            if (host == null)
            {
                host = _host;
            }
            else if (host == string.Empty)
            {
                host = _pathBase;
            }
            else
            {
                host = host.EnsureEndsWith('/');
            }

            var url = host;

            // Strip leading "/", the host/pathBase has this already
            if (path[0] == '/')
            {
                path = path[1..];
            }

            // Append media path
            url += path;

            // Append query
            var query = imageQuery?.ToString();
            if (query != null && query.Length > 0)
            {
                url += query;
            }

            return url;
        }
    }
}