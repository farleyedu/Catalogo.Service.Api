using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalogo.Service.Api;

/// <summary>
/// Cache manual.
/// </summary>
/// <remarks>Leon Denis @ByteOn</remarks>
public static class HandCache
{
    /// <summary>
    /// Itens do cache.
    /// </summary>
    private static readonly Dictionary<string, HandCacheItem> CacheItems;

    /// <summary>
    /// Tempo do clique para expiração do cache.
    /// </summary>
    private static int ClickExpirationMinutes { get; set; }

    /// <summary>
    /// Tempo absoluto para expiração do cache.
    /// </summary>
    private static int AbsoluteExpirationMinutes { get; set; }

    /// <summary>
    /// Construtor.
    /// </summary>
    static HandCache()
    {
        // Iniciando item.
        CacheItems = new();
        // Expiração.
        ClickExpirationMinutes = Constants.ClickExpirationTime;
        AbsoluteExpirationMinutes = Constants.AbsoluteExpirationTime;
        // Inicia o serviço de auto remoção.
        if (Constants.IsApplicationCacheEnabled)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (CacheItems.Count > 0)
                    {
                        foreach (KeyValuePair<string, HandCacheItem> cache in CacheItems.Reverse().ToList())
                        {
                            HandCacheItem current = CacheItems[cache.Key];

                            if (DateTime.Now.Subtract(current.InitTime).Minutes >= AbsoluteExpirationMinutes)
                            {
                                lock (CacheItems) CacheItems.Remove(cache.Key);
                                continue;
                            }
                            if (DateTime.Now.Subtract(current.ClickTime).Minutes >= ClickExpirationMinutes)
                            {
                                lock (CacheItems) CacheItems.Remove(cache.Key);
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
        }
    }

    /// <summary>
    /// Seta o item no cache.
    /// </summary>
    /// <param name="key">Chave única.</param>
    /// <param name="item">Item no cache.</param>
    public static void Set(string key, object item)
    {
        if (CacheItems.ContainsKey(key))
        {
            CacheItems[key].Item = item;
            CacheItems[key].ClickTime = DateTime.Now;
            return;
        }
        CacheItems.Add(key, new HandCacheItem(item));
    }

    /// <summary>
    /// Obtem o item no cache tipado.
    /// </summary>
    /// <typeparam name="T">Tipo do item.</typeparam>
    /// <param name="key">Chave única.</param>
    /// <returns>Item Cacheado.</returns>
    public static T Get<T>(string key)
    {
        if (CacheItems.ContainsKey(key))
            return (T)CacheItems[key].Item;

        return default;
    }

    /// <summary>
    /// Obtem o item no cache.
    /// </summary>
    /// <param name="key">Chave única.</param>
    /// <returns>Item Cacheado.</returns>
    public static object Get(string key)
    {
        if (CacheItems.ContainsKey(key))
            return CacheItems[key].Item;

        return default;
    }
}

/// <summary>
/// Item do cache.
/// </summary>
public class HandCacheItem
{
    /// <summary>
    /// Tempo de início.
    /// </summary>
    public DateTime InitTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Tempo do clique.
    /// </summary>
    public DateTime ClickTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Item cacheado.
    /// </summary>
    public object Item { get; set; }

    /// <summary>
    /// Construtor.
    /// </summary>
    public HandCacheItem() { }

    /// <summary>
    /// Construtor.
    /// </summary>
    /// <param name="item">Item no cache.</param>
    public HandCacheItem(object item)
    {
        Item = item;
    }
}
