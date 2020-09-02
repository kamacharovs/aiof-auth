using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace aiof.auth.data
{
    public static partial class Utils
    {
        public static PropertyBuilder HasSnakeCaseColumnName(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            propertyBuilder.Metadata.SetColumnName(
                propertyBuilder
                    .Metadata
                    .Name
                    .ToSnakeCase());

            return propertyBuilder;
        }
    }
}