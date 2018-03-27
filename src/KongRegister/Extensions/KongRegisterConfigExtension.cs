using System;
using System.Collections.Generic;
using System.Text;

namespace KongRegister.Extensions
{
    public static class KongRegisterConfigExtension
    {
       public static void Validate (this KongRegisterConfig kongRegisterConfig)
        {

            if (kongRegisterConfig.KongApiUrl == null)
            {
                throw new ArgumentNullException(nameof(kongRegisterConfig.KongApiUrl));
            }

            if (kongRegisterConfig.TargetHostDiscovery == null || (!kongRegisterConfig.TargetHostDiscovery.Equals("dynamic", StringComparison.InvariantCultureIgnoreCase) && kongRegisterConfig.TargetHost == null))
            {
                throw new ArgumentNullException(nameof(kongRegisterConfig.TargetHost));
            }

            if (kongRegisterConfig.TargetPortDiscovery == null || (!kongRegisterConfig.TargetPortDiscovery.Equals("dynamic", StringComparison.InvariantCultureIgnoreCase) && kongRegisterConfig.TargetPort == null))
            {
                throw new ArgumentNullException(nameof(kongRegisterConfig.TargetPort));
            }

            if (kongRegisterConfig.UpstreamId == null)
            {
                throw new ArgumentNullException(nameof(kongRegisterConfig.UpstreamId));
            }

            if (kongRegisterConfig.TargetWeight != null && (kongRegisterConfig.TargetWeight < 0 || kongRegisterConfig.TargetWeight > 1000))
            {
                throw new ArgumentOutOfRangeException(nameof(kongRegisterConfig.TargetWeight), "Weight value must be in range 0-1000");
            }

        }

    }
}
