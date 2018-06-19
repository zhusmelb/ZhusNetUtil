namespace com.zhusmelb.Util.IoC
{
    using Castle.Windsor;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;

    public class IoCLoggerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store) {

        }
    }
}