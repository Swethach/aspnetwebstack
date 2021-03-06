﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using Microsoft.Data.Edm;

namespace System.Web.Http.OData.Routing.Conventions
{
    /// <summary>
    /// An implementation of <see cref="IODataRoutingConvention"/> that handles navigation properties.
    /// </summary>
    public class NavigationRoutingConvention : EntitySetRoutingConvention
    {
        /// <summary>
        /// Selects the action.
        /// </summary>
        /// <param name="odataPath">The odata path.</param>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="actionMap">The action map.</param>
        /// <returns></returns>
        public override string SelectAction(ODataPath odataPath, HttpControllerContext controllerContext, ILookup<string, HttpActionDescriptor> actionMap)
        {
            if (odataPath == null)
            {
                throw Error.ArgumentNull("odataPath");
            }

            if (controllerContext == null)
            {
                throw Error.ArgumentNull("controllerContext");
            }

            if (actionMap == null)
            {
                throw Error.ArgumentNull("actionMap");
            }

            if (controllerContext.Request.Method == HttpMethod.Get &&
                (odataPath.PathTemplate == "~/entityset/key/navigation" ||
                 odataPath.PathTemplate == "~/entityset/key/cast/navigation"))
            {
                NavigationPathSegment navigationSegment = odataPath.Segments.Last() as NavigationPathSegment;
                IEdmNavigationProperty navigationProperty = navigationSegment.NavigationProperty;
                IEdmEntityType declaringType = navigationProperty.DeclaringType as IEdmEntityType;

                if (declaringType != null)
                {
                    KeyValuePathSegment keyValueSegment = odataPath.Segments[1] as KeyValuePathSegment;
                    controllerContext.RouteData.Values.Add(ODataRouteConstants.Key, keyValueSegment.Value);

                    // e.g. Try GetNavigationPropertyFromDeclaringType first, then fallback on GetNavigationProperty action name
                    string propertyFromDeclaringTypeActionName = "Get" + navigationProperty.Name + "From" + declaringType.Name;
                    return actionMap.Contains(propertyFromDeclaringTypeActionName) ? propertyFromDeclaringTypeActionName : "Get" + navigationProperty.Name;
                }
            }

            return null;
        }
    }
}