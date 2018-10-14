using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace FWOClientTests
{
    public class CustomAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            
        }
    }
}
