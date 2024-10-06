using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceManager : MonoBehaviour
{
    Dictionary<string, object> resources = new Dictionary<string, object>();

    public void GetTexture(string url, Action<object> onGetSuccess)
    {
        if (resources.TryGetValue(url, out object texture))
        {
            Debug.Log("From resources");
            onGetSuccess?.Invoke(texture);
        }
        else
        {
            Debug.Log("Downloading");
            onGetSuccess += delegate (object tex)
            {
                resources.Add(url, tex);
            };
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            DownloadContent content = new DownloadContent(UnityWebRequestTexture.GetTexture(url), onGetSuccess)
            {
                request = req
            };
            DownloadManager.Instance.StartDownload(content);

        }
    }

    public void GetAsset(string url, Action<object> onGetSuccess)
    {
        if (resources.TryGetValue(url, out object assetBundle))
        {
            onGetSuccess?.Invoke(assetBundle as AssetBundle);
        }
        else
        {
            onGetSuccess += delegate (object tex)
            {
                if (!resources.ContainsKey(url))
                    resources.Add(url, tex);
            };
            UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(url);
            DownloadContent content = new DownloadContent(UnityWebRequestAssetBundle.GetAssetBundle(url), onGetSuccess)
            {
                request = req
            };
            DownloadManager.Instance.StartDownload(content);

        }
    }

}
