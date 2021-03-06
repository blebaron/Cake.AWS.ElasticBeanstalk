﻿using System;
using System.Collections.Generic;
using Amazon.ElasticBeanstalk;
using Amazon.ElasticBeanstalk.Model;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.AWS.ElasticBeanstalk
{
    public class ElasticBeanstalkManager : IElasticBeanstalkManager
    {
        private readonly ICakeEnvironment _Environment;
        private readonly ICakeLog _Log;

        /// <summary>
        /// If the manager should output progrtess events to the cake log
        /// </summary>
        public bool LogProgress { get; set; }

        public ElasticBeanstalkManager(ICakeEnvironment environment, ICakeLog log)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _Environment = environment;
            _Log = log;

            this.LogProgress = true;
        }

        //Request
        private AmazonElasticBeanstalkClient GetClient(ElasticBeanstalkSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (settings.Region == null)
            {
                throw new ArgumentNullException("settings.Region");
            }

            if (settings.Credentials == null)
            {
                if (String.IsNullOrEmpty(settings.AccessKey))
                {
                    throw new ArgumentNullException("settings.AccessKey");
                }
                if (String.IsNullOrEmpty(settings.SecretKey))
                {
                    throw new ArgumentNullException("settings.SecretKey");
                }

                return new AmazonElasticBeanstalkClient(settings.AccessKey, settings.SecretKey, settings.Region);
            }
            else
            {
                return new AmazonElasticBeanstalkClient(settings.Credentials, settings.Region);
            }
        }

        public bool UpdateEnvironmentVersion(string applicationName, string environmentName, string versionLabel, ElasticBeanstalkSettings settings)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException(nameof(environmentName));
            }

            if (string.IsNullOrEmpty(versionLabel))
            {
                throw new ArgumentNullException(nameof(versionLabel));
            }

            try
            {
                var client = GetClient(settings);
                client.UpdateEnvironment(new UpdateEnvironmentRequest {
                    ApplicationName = applicationName,
                    EnvironmentName = environmentName,
                    VersionLabel = versionLabel
                });
            }
            catch (Exception e)
            {
                _Log.Error($"Failed to update environment '{e.Message}'");
                return false;
            }

            return true;
        }

        public bool ApplicationVersionExists(string applicationName, string versionLabel, ElasticBeanstalkSettings settings)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (string.IsNullOrEmpty(versionLabel))
            {
                throw new ArgumentNullException(nameof(versionLabel));
            }

            try
            {
                var client = GetClient(settings);
                var applicationVersionsResponse = client.DescribeApplicationVersions(new DescribeApplicationVersionsRequest
                {
                    ApplicationName = applicationName,
                    VersionLabels = new List<string> { versionLabel }
                });

                return applicationVersionsResponse.ApplicationVersions.Count == 1 && applicationVersionsResponse.ApplicationVersions[0].VersionLabel == versionLabel;
            }
            catch (Exception e)
            {
                _Log.Error($"Failed to describe application version '{e.Message}'");
                return false;
            }
        }

        public bool CreateApplicationVersion(string applicationName, string description, string versionLabel, string s3Bucket, string s3Key, bool autoCreateApplication, ElasticBeanstalkSettings settings)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentNullException("applicationName");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException("description");
            }

            if (string.IsNullOrEmpty(versionLabel))
            {
                throw new ArgumentNullException("versionLabel");
            }

            if (string.IsNullOrEmpty(s3Bucket))
            {
                throw new ArgumentNullException("s3Bucket");
            }

            if (string.IsNullOrEmpty(s3Key))
            {
                throw new ArgumentNullException("s3Key");
            }


            try
            {
                var client = GetClient(settings);
                client.CreateApplicationVersion(new CreateApplicationVersionRequest
                {
                    ApplicationName = applicationName,
                    AutoCreateApplication = autoCreateApplication,
                    Description = description,
                    //Process = true,
                    VersionLabel = versionLabel,
                    SourceBundle = new S3Location
                    {
                        S3Bucket = s3Bucket,
                        S3Key = s3Key
                    }
                });
            }
            catch (Exception ex)
            {
                _Log.Error("Failed to create new application version '{0}'", ex.Message);
                return false;
            }

            _Log.Verbose("Successfully created new application version '{0}' for application '{1}'", versionLabel, applicationName);
            return true;
        }

        public EnvironmentHealth GetApplicationVersionStatus(ElasticBeanstalkSettings settings, string environmentName)
        {
            var client = GetClient(settings);
            var environmentInfoResponse = client.DescribeEnvironmentHealth(new DescribeEnvironmentHealthRequest
            {
                AttributeNames = new List<string> {"Status"},
                EnvironmentName = environmentName
            });
            return environmentInfoResponse.Status;
        }
    }

}
