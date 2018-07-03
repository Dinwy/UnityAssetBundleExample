using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Dinwy.AssetBundleExample
{
    public class AssetBundleWWW : MonoBehaviour
    {
        public int MaxParallelDownload { get; private set; }
        public List<string> DownloadList { get; private set; }
        public string URL = "http://localhost:8080/public/";
        public string affix = "StandaloneWindows/cats";

        public GameObject SampleObject;

        void Start()
        {
            StartCoroutine(GetAssetBundle());
        }

        IEnumerator GetAssetBundle()
        {
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(URL + affix, 2, 0))
            {
                var req = uwr.SendWebRequest();
                while (!req.isDone)
                {
                    Debug.Log(uwr.downloadProgress);
                    yield return null;
                };

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);

                    yield break;
                }

                var bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                List<Hash128> listOfCachedVersions = new List<Hash128>();
                Caching.GetCachedVersions(bundle.name, listOfCachedVersions);

                foreach (var ele in bundle.GetAllAssetNames())
                {
                    var rBundle = bundle.LoadAssetAsync(ele);
                    var texture = rBundle.asset as Texture2D;
                    SampleObject.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                    yield return new WaitForSeconds(0.5f);
                }
            };
        }
    }
}
