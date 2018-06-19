namespace com.zhusmelb.Util.IoC
{
    using Castle.Windsor;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;

    using Castle.Core.Logging;
    using Castle.Services.Logging.NLogIntegration;
    
    public class IoCLoggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(Component.For<ILoggerFactory>().ImplementedBy<NLogFactory>());
        }
    }
}