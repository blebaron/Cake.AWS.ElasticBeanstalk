using Amazon.ElasticBeanstalk;
using Cake.Core;
using Cake.Core.Annotations;


namespace Cake.AWS.ElasticBeanstalk
{    
    /// <summary>
    /// Contains Cake aliases for configuring Amazon Elastic Load Balancers
    /// </summary>
    [CakeAliasCategory("AWS")]
    [CakeNamespaceImport("Amazon")]
    [CakeNamespaceImport("Amazon.ElasticBeanstalk")]
    public static class ElasticBeanstalkAliases
    {
        private static IElasticBeanstalkManager CreateManager(this ICakeContext context)
        {
            return new ElasticBeanstalkManager(context.Environment, context.Log);
        }

        [CakeMethodAlias]
        [CakeAliasCategory("ElasticBeanstalk")]
        public static bool CreateApplicationVersion(this ICakeContext context, string applicationName, string description, string versionLabel, string s3Bucket, string s3Key, bool autoCreateApplication, ElasticBeanstalkSettings settings)
        {
            var manager = context.CreateManager();
            return manager.CreateApplicationVersion(applicationName,                    
                description, 
                versionLabel,
                s3Bucket, 
                s3Key, 
                autoCreateApplication, settings);
        }

        [CakeMethodAlias]
        [CakeAliasCategory("ElasticBeanstalk")]
        public static bool UpdateEnvironmentVersion(this ICakeContext context, string applicationName, string environmentName, string versionLabel, ElasticBeanstalkSettings settings)
        {
            var manager = context.CreateManager();
            return manager.UpdateEnvironmentVersion(applicationName, environmentName, versionLabel, settings);
        }

        [CakeMethodAlias]
        [CakeAliasCategory("ElasticBeanstalk")]
        public static bool ApplicationVersionExists(this ICakeContext context, string applicationName, string versionLabel, ElasticBeanstalkSettings settings)
        {
            var manager = context.CreateManager();
            return manager.ApplicationVersionExists(applicationName, versionLabel, settings);
        }

        [CakeMethodAlias]
        [CakeAliasCategory("ElasticBeanstalk")]
        public static EnvironmentHealth GetApplicationStatus(this ICakeContext context, ElasticBeanstalkSettings settings, string environmentName)
        {
            var manager = context.CreateManager();
            return manager.GetApplicationVersionStatus(settings, environmentName);
        }
    }
}
