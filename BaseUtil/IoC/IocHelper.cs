namespace com.zhusmelb.Util.IoC
{
    using Castle.Windsor;

    internal static class IocHelper
    {
        private static readonly IWindsorContainer _iocContainer
            = new WindsorContainer();
        
        public static void BootstrapIoCContainer() {
            _iocContainer.Install(
                new IoCLoggerInstaller()
            );
        }

        public static T GetService<T>() {
            return _iocContainer.Resolve<T>();
        }
    }
}