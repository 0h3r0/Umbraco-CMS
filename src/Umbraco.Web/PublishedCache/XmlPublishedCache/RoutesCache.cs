﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Umbraco.Web.PublishedCache.XmlPublishedCache
{
    // Note: RoutesCache closely follows the caching strategy dating from v4, which
    // is obviously broken in many ways (eg it's a global cache but relying to some
    // extend to the content cache, which itself is local to each request...).
    // Not going to fix it anyway.

    class RoutesCache
    {
        private ConcurrentDictionary<int, string> _routes;
        private ConcurrentDictionary<string, int> _nodeIds;

        // NOTE
        // RoutesCache is cleared by
        // - ContentTypeCacheRefresher, whenever anything happens to any content type
        // - DomainCacheRefresher, whenever anything happens to any domain
        // - XmlStore, whenever anything happens to the XML cache

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesCache"/> class.
        /// </summary>
        public RoutesCache()
        {
            Clear();
        }

        /// <summary>
        /// Used ONLY for unit tests
        /// </summary>
        /// <returns></returns>
        internal IDictionary<int, string> GetCachedRoutes()
        {
            return _routes;
        }

        /// <summary>
        /// Used ONLY for unit tests
        /// </summary>
        /// <returns></returns>
        internal IDictionary<string, int> GetCachedIds()
        {
            return _nodeIds;
        }

        #region Public

        /// <summary>
        /// Stores a route for a node.
        /// </summary>
        /// <param name="nodeId">The node identified.</param>
        /// <param name="route">The route.</param>
        public void Store(int nodeId, string route)
        {
            _routes.AddOrUpdate(nodeId, i => route, (i, s) => route);
            _nodeIds.AddOrUpdate(route, i => nodeId, (i, s) => nodeId);
        }

        /// <summary>
        /// Gets a route for a node.
        /// </summary>
        /// <param name="nodeId">The node identifier.</param>
        /// <returns>The route for the node, else null.</returns>
        public string GetRoute(int nodeId)
        {
            string val;
            _routes.TryGetValue(nodeId, out val);
            return val;
        }

        /// <summary>
        /// Gets a node for a route.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns>The node identified for the route, else zero.</returns>
        public int GetNodeId(string route)
        {
            int val;
            _nodeIds.TryGetValue(route, out val);
            return val;
        }

        /// <summary>
        /// Clears the route for a node.
        /// </summary>
        /// <param name="nodeId">The node identifier.</param>
        public void ClearNode(int nodeId)
        {
            if (_routes.ContainsKey(nodeId) == false) return;

            string key;
            if (_routes.TryGetValue(nodeId, out key) == false) return;

            int val;
            _nodeIds.TryRemove(key, out val);
            string val2;
            _routes.TryRemove(nodeId, out val2);
        }

        /// <summary>
        /// Clears all routes.
        /// </summary>
        public void Clear()
        {
            _routes = new ConcurrentDictionary<int, string>();
            _nodeIds = new ConcurrentDictionary<string, int>();
        }
        
        #endregion
    }
}
