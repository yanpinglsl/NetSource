using DotNetCore.CAP.Dashboard.NodeDiscovery;
using DotNetCore.CAP.Messages;
using EFModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string conn = this.Configuration.GetConnectionString("UserServiceConnection");
            string rabbitMQConn = this.Configuration.GetConnectionString("RabbitMQ");

            #region CAP

            services.AddCap(x =>
            {
                x.UseSqlServer(conn);//ʹ��SQLServer������---���ݿ�����
                x.UseRabbitMQ(rabbitMQConn);//ʹ��RabbitMQ---���ӵ�ַ
                x.FailedRetryCount = 10;//���Դ���
                x.FailedRetryInterval = 60;//���Եļ��Ƶ��
                x.FailedThresholdCallback = failed =>
                {
                    var logger = failed.ServiceProvider.GetService<ILogger<Startup>>();
                    logger.LogError($@"MessageType {failed.MessageType} ʧ���ˣ� ������ {x.FailedRetryCount} ��, 
                        ��Ϣ����: {failed.Message.GetName()}");//do anything
                };//ʧ�ܳ���������Ļص�

                //���10��---��һ���ʧ��10��---�ص���֪ͨ---�˹��ָ�������ҵ����ͨ��---Ȼ���޸����ݿ��retries

                //x.TopicNamePrefix = "ZhaoxiDistributedTransaction";

                #region ע��Consul���ӻ�
                x.UseDashboard();
                DiscoveryOptions discoveryOptions = new DiscoveryOptions();
                this.Configuration.Bind(discoveryOptions);
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = discoveryOptions.DiscoveryServerHostName;
                    d.DiscoveryServerPort = discoveryOptions.DiscoveryServerPort;
                    d.CurrentNodeHostName = discoveryOptions.CurrentNodeHostName;
                    d.CurrentNodePort = discoveryOptions.CurrentNodePort;
                    d.NodeId = discoveryOptions.NodeId;
                    d.NodeName = discoveryOptions.NodeName;
                    d.MatchPath = discoveryOptions.MatchPath;
                });
                #endregion
            });
            #endregion

            #region EFCore
            services.AddDbContext<CommonServiceDbContext>(options =>
            {
                options.UseSqlServer(conn);
            });
            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
