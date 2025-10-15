using System;
using System.Collections.Generic;

namespace ARMonopoly_V4___Full_Scale_V1.Core
{
    public static class ServiceLocator
    {
        // A dictionary to hold all our services
        private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
    
        // Method to register a service
        public static void Register<T>(T service)
        {
            services[typeof(T)] = service;
        }
    
        // Method to get a service
        public static T Get<T>()
        {
            return (T)services[typeof(T)];
        }
    }
}

