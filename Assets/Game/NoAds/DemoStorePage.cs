using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class DemoStorePage : Singleton<DemoStorePage>, IDetailedStoreListener
{
    public static bool Initialized { get; private set; } = false;
    //[SerializeField] private GameObject noAdsPrefab;
    private IStoreController storeController;
    private IExtensionProvider extensionProvider;
    protected override async void Awake()
    {
        base.Awake();
        if (duplicated) return;
        InitializationOptions options = new();
        options.
#if UNITY_EDITOR || DEBUG
        SetEnvironmentName("test");
#else
        SetEnvironmentName("production");
#endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }
    void HandleIAPCatalogLoaded(AsyncOperation operation)
    {
        ResourceRequest request = operation as ResourceRequest;
        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");
#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay));
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
                    StandardPurchasingModule.Instance(AppStore.NotSpecified));
#endif
        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }
        UnityPurchasing.Initialize(this, builder);
        Initialized = true;
        //Instantiate(noAdsPrefab);
    }

    public void HandlePurchase(string productID)
    {
        storeController.InitiatePurchase(productID);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Error initializing IAP because of {error}." +
            $"\r\nShow a message to the player depending on the error.");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new System.NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        NoAds.Instance.EnableNoAds();
        return PurchaseProcessingResult.Complete;
    }
}
