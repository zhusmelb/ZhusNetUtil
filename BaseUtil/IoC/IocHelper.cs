namespace com.zhusmelb.Util.IoC
{
    using Castle.Windsor;
    using Castle.Facilities.Logging;
    using Castle.Services.Logging.NLogIntegration;

    public static class IocHelper
    {
        private static readonly IWindsorContainer _iocContainer
            = new WindsorContainer();
        
        public static void BootstrapIoCContainer() {
            _iocContainer.AddFacility<LoggingFacility>(f => f.LogUsing<NLogFactory>().WithConfig("nlog.config"));
        }

        public static T GetService<T>() {
            return _iocContainer.Resolve<T>();
        }
    }
}