using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvTap.Infrastructure.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace EvTap.Infrastructure.Configurations
{
    public static class QuartzJobConfiguration 
    {
        public static void AddQuartzJobs(this IServiceCollection services) 
        {
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var jobKey = new JobKey("ScraperJob");
                q.AddJob<ScraperJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("ScraperJob-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(100000) // hər 30 saniyədən bir
                        .RepeatForever())
                );
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        }
    }
}
