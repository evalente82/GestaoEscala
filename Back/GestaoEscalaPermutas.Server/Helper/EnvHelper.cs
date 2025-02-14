using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NovosAprovados.Tools
{
    public class EnvHelper
    {
        public static string Get(string key)
        {
            string value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);

            if (String.IsNullOrEmpty(value))
            {
                value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);

                if (String.IsNullOrEmpty(value))
                    value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
            }

            return String.IsNullOrEmpty(value) ? null : value;
        }
    }
}