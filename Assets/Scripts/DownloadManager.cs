using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class DownloadManager : MonoBehaviour
{

    public static DownloadManager Instance => instance;
    static DownloadManager instance;

    List<DownloadContent> pendingRequests = new List<DownloadContent>();
    List<DownloadContent> onGoingRequest = new List<DownloadContent>();

    public int OnGoingRequests => onGoingRequest.Count;
    public int PendingRequests => pendingRequests.Count;

    public int queueBufferSize;

    private void Start()
    {
        instance = this;
    }

    public void StartDownload(DownloadContent content)
    {
        if (onGoingRequest.Count < queueBufferSize)
        {
            onGoingRequest.Add(content);
            StartCoroutine(content.StartDownload());
        }
        else
            pendingRequests.Add(content);

        content.OnReqComplete = delegate (DownloadContent downloadContent)
          {
              onGoingRequest.Remove(downloadContent);
              //print("Removed request");
              if (pendingRequests.Count > 0)
              {
                  StartCoroutine(pendingRequests[0].StartDownload());
                  onGoingRequest.Add(pendingRequests[0]);
                  pendingRequests.RemoveAt(0);
                  print("Added from pending");

              }
          };
    }
    private void OnDownloadComplete(object obj)
    {
        Debug.Log(obj);
    }

}

[Serializable]
public class DownloadContent
{
    //AssetBundler handler, Texture Handler etc
    public UnityWebRequest request;
    public Action<object> OnDownloadComplete;
    public Action<DownloadContent> OnReqComplete;

    public DownloadContent()
    {
    }

    public DownloadContent(UnityWebRequest _request, Action<object> _OnDownloadComplete)
    {
        request = _request;
        OnDownloadComplete = _OnDownloadComplete;
    }

    public IEnumerator StartDownload()
    {
        UnityWebRequestAsyncOperation op = request.SendWebRequest();

        while (!op.isDone)
        {
            //Debug.Log(op.progress);
            yield return null;
        }

        //Download completes here and updates the list
        OnReqComplete?.Invoke(this);
        if (request.downloadHandler is DownloadHandlerAssetBundle)
        {
            OnDownloadComplete.Invoke(DownloadHandlerAssetBundle.GetContent(request));
        }
        if (request.downloadHandler is DownloadHandlerTexture)
            OnDownloadComplete.Invoke(DownloadHandlerTexture.GetContent(request));
    }
}

