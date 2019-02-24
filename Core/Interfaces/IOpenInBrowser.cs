using System;

namespace ScottLane.SurgeonV2.Core
{
    interface IOpenInBrowser
    {
        /// <summary>
        /// Gets or sets a value that indicates whether the connection supports opening in a browser.
        /// </summary>
        Uri GetBrowserUrl { get; set; }
    }
}
