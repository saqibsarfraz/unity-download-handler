using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI ongoing;
    [SerializeField] TextMeshProUGUI pending;
    [SerializeField] TMP_InputField totalRequests;
    [SerializeField] TMP_InputField bufferSize;
    [SerializeField] Button Download;


    [SerializeField] ResourceManager resourceManager;
    [SerializeField] DownloadManager downloadManager;

    [SerializeField] List<string> dlcs;

    private void Start()
    {
        Download.onClick.AddListener(DownloadAssetBundle);

    }

    [ContextMenu("download")]
    public void DownloadAssetBundle()
    {
        Int32.TryParse(totalRequests.text, out int result);
        Int32.TryParse(bufferSize.text, out int bufferSizeLength);
        downloadManager.queueBufferSize = bufferSizeLength;

        for (int i = 0; i < result; i++)
        {
            for (int j = 0; j < dlcs.Count; j++)
            {
                resourceManager.GetAsset(dlcs[j], OnGetSuccess);
            }
        }
    }

    private void OnGetSuccess(object obj)
    {
        Debug.Log(obj);
        if (obj is AssetBundle)
            AssetBundle.UnloadAllAssetBundles(true);
        ongoing.text = "On Going: " + downloadManager.OnGoingRequests.ToString();
        pending.text = "Pending: " + downloadManager.PendingRequests.ToString();

    }
}
