namespace com.zhusmelb.Util.Path
{
    using System;
    using System.Collections.Generic;
    using IO = System.IO;

    public abstract class PathConvensionAbstract : IPathConvension
    {
        private const string ProductFolderName = "zhusmelb.com";
        protected abstract Dictionary<string, string> GetPathMapper();

        public virtual string AppBase
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }

        }

        public virtual string AppDataBase
        {
            get
            {
                return IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ProductFolderName);
            }
        }

        public virtual string UserDataBase
        {
            get
            {
                return IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ProductFolderName);
            }
        }

        public virtual string Log {
            get { throw new NotImplementedException("Log not implemented"); }
        }

        public virtual string Data {
            get { throw new NotImplementedException("Data not implemented"); }
        }

        public virtual string Cache {
            get { throw new NotImplementedException("Cache not implemented"); }
        }

        public string this[string name] {
            get {
                var mapper = GetPathMapper();
                return (mapper == null || !mapper.ContainsKey(name))
                    ? null
                    : mapper[name];
            }
        }
    }
}
