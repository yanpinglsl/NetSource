using Autofac;
using Autofac.Configuration;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using IOCTestInterfaceLib;
using IOCTestServiceLib;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Module = Autofac.Module;

namespace DemoProject.Utility
{
    /// <summary>
    /// Autofac通过模块化进行注册服务
    /// </summary>
    public class CustomAutofacModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            #region AutoFac读取配置文件进行服务注册
            //IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            //configurationBuilder.AddJsonFile("autofac.json");//读取配置文件
            //IConfigurationRoot configurationRoot = configurationBuilder.Build();
            //ConfigurationModule configurationModule = new ConfigurationModule(configurationRoot);
            //containerBuilder.RegisterModule(configurationModule);
            #endregion
            var assembly = this.GetType().GetTypeInfo().Assembly;
            //创建一个容器
            var builder = new ContainerBuilder();
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(assembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);
            builder.RegisterType<ApplicationPartManager>().AsSelf().SingleInstance();
            builder.RegisterTypes(feature.Controllers.Select(ti => ti.AsType()).ToArray()).PropertiesAutowired();

            #region AutoFac作用域
            ////瞬时：每次获取的服务实例都不一样
            //containerBuilder.RegisterType<TestServiceA>().As<ITestServiceA>().InstancePerDependency();
            ////单例：在整个容器中获取的服务实例都是同一个
            //containerBuilder.RegisterType<TestServiceB>().As<ITestServiceB>().SingleInstance();
            ////作用域1：相同作用域下获取到的服务实例相同
            //containerBuilder.RegisterType<TestServiceC>().As<ITestServiceC>().InstancePerLifetimeScope();//作用域
            ////作用域2：可以指定到某一个具体作用域
            //containerBuilder.RegisterType<TestServiceD>().As<ITestServiceD>().InstancePerMatchingLifetimeScope("Show");
            ////请求的生命周期，不同的请求获取的服务实例不一样
            //containerBuilder.RegisterType<TestServiceE>().As<ITestServiceE>().InstancePerRequest();
            #endregion

            containerBuilder.RegisterType<TestServiceA>().As<ITestServiceA>();
            containerBuilder.RegisterType<TestServiceB>().As<ITestServiceB>().SingleInstance();//单例
            containerBuilder.RegisterType<TestServiceC>().As<ITestServiceC>().InstancePerLifetimeScope();//作用域
            containerBuilder.RegisterType<TestServiceD>().As<ITestServiceD>();
            containerBuilder.RegisterType<TestServiceE>().As<ITestServiceE>();

            //属性注入
            containerBuilder.RegisterType<A>().As<IA>().PropertiesAutowired();//必须配置支持属性注入
            //方法注入
            containerBuilder.RegisterType<B>()
                .OnActivated(x => x.Instance.RegisterService(x.Context.Resolve<ITestServiceA>()))
                .As<IB>().InstancePerDependency();//必须配置支持属性注入

            //AOP注册
            containerBuilder.Register(c => new CustomAutofacAop());
            containerBuilder.RegisterType<C>().As<IC>().EnableInterfaceInterceptors();
        }
    }

    public interface IA
    {
        void Show(int id, string name);
    }

    public class A : IA
    {
        //属性注入
        public ITestServiceA TestServiceA { get; set; }
        public void Show(int id, string name)
        {
            TestServiceA.Show();
            Console.WriteLine($"This is {id} _ {name}");
        }
    }

    public interface IB
    {
        void Show(int id, string name);
    }

    public class B : IB
    {
        public ITestServiceA TestServiceA = null;
        public void Show(int id, string name)
        {
            TestServiceA.Show();
            Console.WriteLine($"This is {id} _ {name}");
        }
        public void RegisterService(ITestServiceA testA)
        {
            TestServiceA = testA;
        }
    }
    public class CustomAutofacAop : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"invocation.Methond={invocation.Method}");
            Console.WriteLine($"invocation.Arguments={string.Join(",", invocation.Arguments)}");

            invocation.Proceed(); //继续执行

            Console.WriteLine($"方法{invocation.Method}执行完成了");
        }
    }

    public interface IC
    {
        void Show(int id, string name);
    }

    //Nuget Autofac.Extras.DynamicProxy包
    [Intercept(typeof(CustomAutofacAop))]
    public class C : IC
    {
        public void Show(int id, string name)
        {
            Console.WriteLine($"This is {id} _ {name}");
        }
    }
}
