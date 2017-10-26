using System;
using Cake.Core;

namespace Cake.AWS.ElasticBeanstalk
{
    public static class CakeContextExtensions
    {
        public static ElasticBeanstalkSettings CreateElasticBeanstalkSettings(this ICakeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Environment.CreateElasticBeanstalkSettings();
        }
    }
}
