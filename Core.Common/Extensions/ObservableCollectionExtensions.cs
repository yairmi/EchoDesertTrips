﻿using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core.Common.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static int RemoveItems<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }
    }
}
