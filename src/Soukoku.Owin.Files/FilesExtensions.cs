using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soukoku.Owin.Files
{
    using AppFunc = Func<IDictionary<string, object>, Task>;
    using MidFunc = Func<
        Func<IDictionary<string, object>, Task>, // next appfunc
        Func<IDictionary<string, object>, Task>
    >;

    /// <summary>
    /// Extensions for the files middleware.
    /// </summary>
    public static class FilesExtensions
    {
        /// <summary>
        /// Uses the files middleware in aspnet 5 like environment.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">config</exception>
        public static Action<MidFunc> UseFiles(
               this Action<MidFunc> build, FilesConfig config)
        {
            if (build == null) { throw new ArgumentNullException("build"); }
            if (config == null) { throw new ArgumentNullException("config"); }

            build(next => new FilesMiddleware(next, config).Invoke);
            return build;
        }
    }
}
