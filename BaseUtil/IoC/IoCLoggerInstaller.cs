namespace com.zhusmelb.Util.IoC
{
    using Castle.Windsor;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;

    using Castle.Core.Logging;
    using Castle.Services.Logging.NLogIntegration;
    using Castle.Facilities.Logging;

    public class IoCLoggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing<NLogFactory>().ConfiguredExternally()
            );
        }
    }
}