﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.CloudFoundry.Connector.Services;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Hystrix.Test
{
    public class HystrixProviderConfigurerTest
    {
        [Fact]
        public void UpdateConfiguration_WithNullHystrixRabbitMQServiceInfo_ReturnsInitialConfiguration()
        {
            HystrixProviderConfigurer configurer = new HystrixProviderConfigurer();
            HystrixProviderConnectorOptions config = new HystrixProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                VirtualHost = "vhost"
            };
            configurer.UpdateConfiguration(null, config);

            Assert.Equal("localhost", config.Server);
            Assert.Equal(1234, config.Port);
            Assert.Equal("username", config.Username);
            Assert.Equal("password", config.Password);
            Assert.Equal("vhost", config.VirtualHost);
            Assert.Null(config.Uri);
        }

        [Fact]
        public void UpdateConfiguration_WithHystrixRabbitMQServiceInfo_UpdatesConfigurationFromServiceInfo()
        {
            HystrixProviderConfigurer configurer = new HystrixProviderConfigurer();
            HystrixProviderConnectorOptions config = new HystrixProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                VirtualHost = "vhost"
            };
            HystrixRabbitMQServiceInfo si = new HystrixRabbitMQServiceInfo("MyId", "amqp://si_username:si_password@example.com:5672/si_vhost", false);

            configurer.UpdateConfiguration(si, config);

            Assert.False(config.SslEnabled);
            Assert.Equal("example.com", config.Server);
            Assert.Equal(5672, config.Port);
            Assert.Equal("si_username", config.Username);
            Assert.Equal("si_password", config.Password);
            Assert.Equal("si_vhost", config.VirtualHost);
        }

        [Fact]
        public void UpdateConfiguration_WithHystrixRabbitMQSSLServiceInfo_UpdatesConfigurationFromServiceInfo()
        {
            HystrixProviderConfigurer configurer = new HystrixProviderConfigurer();
            HystrixProviderConnectorOptions config = new HystrixProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                VirtualHost = "vhost"
            };
            HystrixRabbitMQServiceInfo si = new HystrixRabbitMQServiceInfo("MyId", "amqps://si_username:si_password@example.com:5671/si_vhost", false);

            configurer.UpdateConfiguration(si, config);

            Assert.True(config.SslEnabled);
            Assert.Equal("example.com", config.Server);
            Assert.Equal(5671, config.SslPort);
            Assert.Equal("si_username", config.Username);
            Assert.Equal("si_password", config.Password);
            Assert.Equal("si_vhost", config.VirtualHost);
        }

        [Fact]
        public void Configure_NoServiceInfo_ReturnsProvidedConnectorOptions()
        {
            HystrixProviderConnectorOptions config = new HystrixProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                VirtualHost = "vhost"
            };

            HystrixProviderConfigurer configurer = new HystrixProviderConfigurer();
            var opts = configurer.Configure(null, config);
            var uri = new UriInfo(opts);

            Assert.False(config.SslEnabled);
            Assert.Equal("localhost", uri.Host);
            Assert.Equal(1234, uri.Port);
            Assert.Equal("username", uri.UserName);
            Assert.Equal("password", uri.Password);
            Assert.Equal("vhost", uri.Path);
        }

        [Fact]
        public void Configure_ServiceInfoOveridesConfig_ReturnsOverriddenConnectionString()
            {
            HystrixProviderConnectorOptions config = new HystrixProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                VirtualHost = "vhost"
            };

            HystrixProviderConfigurer configurer = new HystrixProviderConfigurer();
            HystrixRabbitMQServiceInfo si = new HystrixRabbitMQServiceInfo("MyId", "amqp://si_username:si_password@example.com:5672/si_vhost", false);

            var opts = configurer.Configure(si, config);
            var uri = new UriInfo(opts);

            Assert.Equal("example.com", uri.Host);
            Assert.Equal(5672, uri.Port);
            Assert.Equal("si_username", uri.UserName);
            Assert.Equal("si_password", uri.Password);
            Assert.Equal("si_vhost", uri.Path);
        }

        [Fact]
        public void Configure_SSLServiceInfoOveridesConfig_ReturnsOverriddenConnectionString()
        {
            HystrixProviderConnectorOptions config = new HystrixProviderConnectorOptions()
            {
                Server = "localhost",
                Port = 1234,
                Username = "username",
                Password = "password",
                VirtualHost = "vhost"
            };

            HystrixProviderConfigurer configurer = new HystrixProviderConfigurer();
            HystrixRabbitMQServiceInfo si = new HystrixRabbitMQServiceInfo("MyId", "amqps://si_username:si_password@example.com/si_vhost", false);

            var opts = configurer.Configure(si, config);
            var uri = new UriInfo(opts);

            Assert.Equal("example.com", uri.Host);
            Assert.Equal("amqps", uri.Scheme);
            Assert.Equal("si_username", uri.UserName);
            Assert.Equal("si_password", uri.Password);
            Assert.Equal("si_vhost", uri.Path);
        }
    }
}
