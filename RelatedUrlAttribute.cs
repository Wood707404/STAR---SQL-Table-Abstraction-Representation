using System;

namespace WPFSamples
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RelatedUrlAttribute : Attribute
    {
        public string RelatedUrl { get; private set; }

        public string Description { get; private set; }

        public RelatedUrlAttribute(string relatedUrl, string description = null)
        {
            this.RelatedUrl = relatedUrl;
            this.Description = description;
        }
    }
}