using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WPFSamples
{
    public class Sample
    {
        public string Category { get; set; }

        public Type WindowType { get; set; }

        public string Description { get; set; }

        public string RelatedUrl { get; set; }

        public string RelatedUrlDescription { get; set; }

        public static Task<IEnumerable<Sample>> LoadFromCurrentAssemblyAsync()
        {
            return Task.Run(() =>
                    from
                        t in typeof(Sample).Assembly.GetTypes()
                    where
                        t.IsSubclassOf(typeof(SampleWindow))
                    let descriptionAttribute = t.GetCustomAttribute<DescriptionAttribute>()
                    let description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty
                    let categoryAttribute = t.GetCustomAttribute<CategoryAttribute>()
                    let category = categoryAttribute != null ? categoryAttribute.Category : "My project"
                    let relatedUrlAttribute = t.GetCustomAttribute<RelatedUrlAttribute>()
                    let relatedUrl = relatedUrlAttribute != null ? relatedUrlAttribute.RelatedUrl : string.Empty
                    let relatedUrlDescription = relatedUrlAttribute != null ? relatedUrlAttribute.Description : string.Empty
                    orderby
                        category == "Highlights" ? 0 : 1, t.Name
                    select
                        new Sample()
                        {
                            WindowType = t,
                            Description = description,
                            Category = category,
                            RelatedUrl = relatedUrl,
                            RelatedUrlDescription = relatedUrlDescription
                        });
        }
    }
}