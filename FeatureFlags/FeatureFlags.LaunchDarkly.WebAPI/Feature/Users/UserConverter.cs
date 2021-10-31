﻿using System;
using System.Collections.Generic;
using LaunchDarkly.Sdk;

namespace FeatureFlags.LaunchDarkly.WebAPI.Feature.Users
{
    public class UserConverter : IUserConverter
    {
        private readonly Dictionary<Type, Func<IFeatureContext, User>> _typeToActionDictionary = new()
        {
            {
                typeof(Feature2Context),
                o =>
                {
                    var context = o as Feature2Context;
                    var user = User.Builder(context.Id)
                        .Name(context.Name)
                        .Build();
                    return user;
                }
            }
        };


        public User Convert(IFeatureContext obj) 
        {
            if (_typeToActionDictionary.TryGetValue(obj.GetType(), out Func<IFeatureContext, User> func))
            {
                return func(obj);
            }

            throw new ArgumentException($"Type {typeof(IFeatureContext)} was not found in the lookup dictionary");
        }
    }
}