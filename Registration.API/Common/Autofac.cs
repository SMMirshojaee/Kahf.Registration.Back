using Autofac;
using System.Reflection;
using Payment;
using SMS;
using Module = Autofac.Module;

namespace Registration.API.Common;

public class DefaultModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(e => e.Name.EndsWith("Business"));

        foreach (Type type in types)
            builder.RegisterType(type).AsSelf();

        builder.RegisterType(typeof(SmsHelper)).AsSelf();
        builder.RegisterType(typeof(Magfa)).AsSelf();
        builder.RegisterType(typeof(SmsDotIr)).AsSelf();
        builder.RegisterType(typeof(PayamResan)).AsSelf();
        builder.RegisterType(typeof(Zarrinpal)).AsSelf();
        //builder.RegisterAssemblyTypes(assembly)
        //    .Where(t => t.Name.EndsWith("Business"))
        //    .AsImplementedInterfaces()
        //    .InstancePerLifetimeScope();
    }
}