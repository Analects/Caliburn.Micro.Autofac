//==================================================
// Analects.Caliburn.Micro.Autofac package
//==================================================

#if !WINDOWS_PHONE

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Caliburn.Micro.Autofac
{
    public class AutofacBootstrapper<TRootModel> : Bootstrapper<TRootModel>
    {
        protected IContainer container;

        protected override void Configure()
        {
            var containerBuilder = new ContainerBuilder();

            ConfigureIoC(containerBuilder);

            container = containerBuilder.Build();
        }

        protected virtual void ConfigureIoC(ContainerBuilder builder)
        {
            //  register view models
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              .Where(type => type.Name.EndsWith("ViewModel"))
              .AsSelf()
              .InstancePerDependency();

            //  register views
            builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
              .Where(type => type.Name.EndsWith("View"))
              .AsSelf()
              .InstancePerDependency();

            builder.RegisterType<WindowManager>().As<IWindowManager>().InstancePerLifetimeScope();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().InstancePerLifetimeScope();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.Resolve(typeof(IEnumerable<>).MakeGenericType(new Type[] { service })) as IEnumerable<object>;
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                object result;
                if (container.TryResolve(service, out result))
                {
                    return result;
                }
            }
            else
            {
                object result;
                if (container.TryResolveNamed(key, service, out result))
                {
                    return result;
                }
            }
            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }
    }
}

#endif