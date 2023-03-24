# IO.Swagger.Api.DefaultApi

All URIs are relative to */*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateAuthToken**](DefaultApi.md#createauthtoken) | **PUT** /authenticate | 
[**PackageByNameDelete**](DefaultApi.md#packagebynamedelete) | **DELETE** /package/byName/{name} | Delete all versions of this package.
[**PackageByNameGet**](DefaultApi.md#packagebynameget) | **GET** /package/byName/{name} | 
[**PackageCreate**](DefaultApi.md#packagecreate) | **POST** /package | 
[**PackageDelete**](DefaultApi.md#packagedelete) | **DELETE** /package/{id} | Delete this version of the package.
[**PackageRate**](DefaultApi.md#packagerate) | **GET** /package/{id}/rate | 
[**PackageRetrieve**](DefaultApi.md#packageretrieve) | **GET** /package/{id} | 
[**PackageUpdate**](DefaultApi.md#packageupdate) | **PUT** /package/{id} | Update this version of the package.
[**PackagesList**](DefaultApi.md#packageslist) | **POST** /packages | Get packages
[**RegistryReset**](DefaultApi.md#registryreset) | **DELETE** /reset | 

<a name="createauthtoken"></a>
# **CreateAuthToken**
> string CreateAuthToken (AuthenticationRequest body)



### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class CreateAuthTokenExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var body = new AuthenticationRequest(); // AuthenticationRequest | 

            try
            {
                string result = apiInstance.CreateAuthToken(body);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.CreateAuthToken: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**AuthenticationRequest**](AuthenticationRequest.md)|  | 

### Return type

**string**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packagebynamedelete"></a>
# **PackageByNameDelete**
> void PackageByNameDelete (string name, string xAuthorization = null)

Delete all versions of this package.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageByNameDeleteExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var name = new string(); // string | 
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                // Delete all versions of this package.
                apiInstance.PackageByNameDelete(name, xAuthorization);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageByNameDelete: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **name** | [**string**](string.md)|  | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packagebynameget"></a>
# **PackageByNameGet**
> List<PackageHistoryEntry> PackageByNameGet (string name, string xAuthorization = null)



Return the history of this package (all versions).

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageByNameGetExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var name = new string(); // string | 
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                List&lt;PackageHistoryEntry&gt; result = apiInstance.PackageByNameGet(name, xAuthorization);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageByNameGet: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **name** | [**string**](string.md)|  | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

[**List<PackageHistoryEntry>**](PackageHistoryEntry.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packagecreate"></a>
# **PackageCreate**
> PackageMetadata PackageCreate (Package body, string xAuthorization)



### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageCreateExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var body = new Package(); // Package | 
            var xAuthorization = new string(); // string | 

            try
            {
                PackageMetadata result = apiInstance.PackageCreate(body, xAuthorization);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageCreate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**Package**](Package.md)|  | 
 **xAuthorization** | [**string**](string.md)|  | 

### Return type

[**PackageMetadata**](PackageMetadata.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packagedelete"></a>
# **PackageDelete**
> void PackageDelete (string id, string xAuthorization = null)

Delete this version of the package.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageDeleteExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var id = new string(); // string | Package ID
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                // Delete this version of the package.
                apiInstance.PackageDelete(id, xAuthorization);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageDelete: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**](string.md)| Package ID | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packagerate"></a>
# **PackageRate**
> PackageRating PackageRate (string id, string xAuthorization = null)



### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageRateExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var id = new string(); // string | 
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                PackageRating result = apiInstance.PackageRate(id, xAuthorization);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageRate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**](string.md)|  | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

[**PackageRating**](PackageRating.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packageretrieve"></a>
# **PackageRetrieve**
> Package PackageRetrieve (string id, string xAuthorization = null)



Return this package.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageRetrieveExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var id = new string(); // string | ID of package to fetch
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                Package result = apiInstance.PackageRetrieve(id, xAuthorization);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageRetrieve: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **id** | [**string**](string.md)| ID of package to fetch | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

[**Package**](Package.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packageupdate"></a>
# **PackageUpdate**
> void PackageUpdate (Package body, string id, string xAuthorization = null)

Update this version of the package.

The name, version, and ID must match.  The package contents (from PackageData) will replace the previous contents.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackageUpdateExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var body = new Package(); // Package | 
            var id = new string(); // string | 
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                // Update this version of the package.
                apiInstance.PackageUpdate(body, id, xAuthorization);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackageUpdate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**Package**](Package.md)|  | 
 **id** | [**string**](string.md)|  | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="packageslist"></a>
# **PackagesList**
> List<PackageMetadata> PackagesList (List<PackageQuery> body, string xAuthorization = null, string offset = null)

Get packages

Get any packages fitting the query.

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class PackagesListExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var body = new List<PackageQuery>(); // List<PackageQuery> | 
            var xAuthorization = new string(); // string |  (optional) 
            var offset = new string(); // string | Provide this for pagination. If not provided, returns the first page of results. (optional) 

            try
            {
                // Get packages
                List&lt;PackageMetadata&gt; result = apiInstance.PackagesList(body, xAuthorization, offset);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.PackagesList: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**List&lt;PackageQuery&gt;**](PackageQuery.md)|  | 
 **xAuthorization** | [**string**](string.md)|  | [optional] 
 **offset** | [**string**](string.md)| Provide this for pagination. If not provided, returns the first page of results. | [optional] 

### Return type

[**List<PackageMetadata>**](PackageMetadata.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="registryreset"></a>
# **RegistryReset**
> void RegistryReset (string xAuthorization = null)



### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class RegistryResetExample
    {
        public void main()
        {
            var apiInstance = new DefaultApi();
            var xAuthorization = new string(); // string |  (optional) 

            try
            {
                apiInstance.RegistryReset(xAuthorization);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.RegistryReset: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **xAuthorization** | [**string**](string.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
