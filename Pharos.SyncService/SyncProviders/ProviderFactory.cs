﻿using Microsoft.Synchronization;
using Pharos.SyncService.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.SyncService.SyncProviders
{
    public static class SyncProviderFactory
    {
        static SyncProviderCache _SyncProviderCache = null;
        static SyncProviderFactory()
        {
            if (_SyncProviderCache == null)
                _SyncProviderCache = new SyncProviderCache();
        }

        private static string KeyFactory(int companyId, string storeId, string syncServiceName)
        {
            return string.Format("{0}_{1}_{2}", companyId, storeId, syncServiceName);
        }
        public static PosDbSyncProvider Factory(int companyId, string storeId, string syncServiceName, bool isNew = false)
        {

            PosDbSyncProvider syncProvider;
            var key = KeyFactory(companyId, storeId, syncServiceName);
            if (isNew)
            {
                if (_SyncProviderCache.ContainsKey(key)) 
                {
                    syncProvider = _SyncProviderCache.Get(key);
                    syncProvider.CloseMetadataStore();
                    _SyncProviderCache.Remove(key);
                }
                goto NewItem;
            }

            if (_SyncProviderCache.ContainsKey(key))
            {
                syncProvider = _SyncProviderCache.Get(key);
                if (syncProvider == null)
                {
                    goto NewItem;
                }
                return syncProvider;
            }
        NewItem:
            syncProvider = new PosDbSyncProvider(companyId, storeId, new RemoteSyncContext().ServiceMappings[syncServiceName]);
            syncProvider.Configuration.ConflictResolutionPolicy = ConflictResolutionPolicy.ApplicationDefined;
            syncProvider.DestinationCallbacks.ItemConflicting += ItemConflicting;
            _SyncProviderCache.Set(key, syncProvider);
            return syncProvider;
        }

        private static void ItemConflicting(object sender, ItemConflictingEventArgs e)
        {
            switch (e.DestinationChange.ChangeKind)
            {
                default:
                    e.SetResolutionAction(ConflictResolutionAction.Merge);
                    break;
            }

        }
    }
}