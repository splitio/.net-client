﻿using Common.Logging;
using Splitio.Services.Client.Interfaces;
using Splitio.Services.InputValidation.Classes;
using Splitio.Services.InputValidation.Interfaces;
using Splitio.Services.Shared.Classes;
using Splitio.Services.Shared.Interfaces;
using System;
using System.Globalization;
using System.Reflection;

namespace Splitio.Services.Client.Classes
{
    public class SplitFactory
    {
        private readonly IApiKeyValidator _apiKeyValidator;
        private readonly ILog _log;
        private readonly IFactoryInstantiationsService _factoryInstantiationsService;
        private readonly string _apiKey;

        private ISplitClient _client;
        private ISplitManager _manager;
        private ConfigurationOptions _options;

        public SplitFactory(string apiKey,
            ConfigurationOptions options = null)
        {
            _apiKey = apiKey;
            _options = options;

            _log = LogManager.GetLogger(typeof(SplitClient));
            _apiKeyValidator = new ApiKeyValidator(_log);
            _factoryInstantiationsService = FactoryInstantiationsService.Instance(_log);
        }

        public ISplitClient Client()
        {
            if (_client == null)
            {
                BuildSplitClient();
            }

            return _client;
        }

        private void BuildSplitClient()
        {
            _options = _options ?? new ConfigurationOptions();

            if (!_options.Ready.HasValue)
            {
                _log.Warn("no ready parameter has been set - incorrect control treatments could be logged if no ready config has been set when building factory");
            }

            _apiKeyValidator.Validate(_apiKey);

            switch (_options.Mode)
            {
                case Mode.Standalone:
                    if (string.IsNullOrEmpty(_apiKey))
                    {
                        throw new Exception("API Key should be set to initialize Split SDK.");
                    }
                    if (_apiKey == "localhost")
                    {
                        _client = new LocalhostClient(_options.LocalhostFilePath, _log);
                    }
                    else
                    {
                        _client = new SelfRefreshingClient(_apiKey, _options, _log);
                    }
                    break;
                case Mode.Consumer:
                    if (_options.CacheAdapterConfig != null && _options.CacheAdapterConfig.Type == AdapterType.Redis)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(_options.CacheAdapterConfig.Host) || string.IsNullOrEmpty(_options.CacheAdapterConfig.Port))
                            {
                                throw new Exception("Redis Host and Port should be set to initialize Split SDK in Redis Mode.");
                            }

                            var handle = Activator.CreateInstance("Splitio.Redis", "Splitio.Redis.Services.Client.Classes.RedisClient", false, default(BindingFlags), default(Binder), new object[] { _options, _log,  _apiKey }, default(CultureInfo), null);
                            _client = (ISplitClient)handle.Unwrap();

                        }
                        catch (Exception e)
                        {
                            throw new Exception("Splitio.Redis package should be added as reference, to build split client in Redis Consumer mode.", e);
                        }
                    }
                    else
                    {
                        throw new Exception("Redis config should be set to build split client in Consumer mode.");
                    }
                    break;
                case Mode.Producer:
                    throw new Exception("Unsupported mode.");
                default:
                    throw new Exception("Mode should be set to build split client.");
            }

            _factoryInstantiationsService.Increase(_apiKey);
        }

        public ISplitManager Manager()
        {
            if (_client == null)
            {
                BuildSplitClient();
            }

            _manager = _client.GetSplitManager();

            return _manager;
        }
    }
}
