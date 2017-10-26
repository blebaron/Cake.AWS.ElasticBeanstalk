using System;
using Amazon;
using Amazon.Runtime;
using Cake.Core;

namespace Cake.AWS.ElasticBeanstalk
{
    public static class CakeEnvironmentExtensions
    {
        public static T SetSettings<T>(this ICakeEnvironment environment, T settings) where T : ElasticBeanstalkSettings
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var credentials = FallbackCredentialsFactory.GetCredentials();
            if (credentials != null)
            {
                settings.Credentials = credentials;
            }

            var region = environment.GetEnvironmentVariable("AWS_REGION");
            if (!String.IsNullOrEmpty(region))
            {
                settings.Region = RegionEndpoint.GetBySystemName(region);
            }

            return settings;
        }

        public static ElasticBeanstalkSettings CreateElasticBeanstalkSettings(this ICakeEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            return environment.SetSettings(new ElasticBeanstalkSettings());
        }
    }
}
